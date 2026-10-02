# ADR 0001: Preview geometry drawn by one custom transient per object

## Status

Accepted, 2026-10-01.

## Context

In Civil 3D with Grasshopper running, orbiting the view became very slow and `acad.exe` memory climbed to 16-20 GB. This only happened with the Grasshopper preview displayed. Rhino-only previews, and Grasshopper with the preview switched off, behaved normally.

Our preview code was not running during the orbit: breakpoints in it were not hit. A memory dump showed a managed heap of only about 345 MB, so the growth was native, on the AutoCAD side.

The cause was how previews were registered. Each preview item became its own AutoCAD entity: a `Line` per line, a `SubDMesh` per mesh, edge curves per Brep, repeated across the shaded and wireframe preview servers. Each entity was added as a separate transient through `TransientManager`. A typical definition produced tens of thousands of transients, and one component alone produced about two million. `TransientManager` is designed for a handful of short-lived graphics such as jigs and highlights, and AutoCAD regenerates every transient on each orbit frame. Building the entities also churned native memory: entities were created on every rebuild, and some intermediate objects were not disposed.

A spike compared the two approaches on 50,000 lines plus a 200 x 200 mesh. With one transient entity per item, orbiting crawled and the process hit a memory limit. With a single custom transient drawing the same geometry, orbiting was smooth.

## Decision

Each previewed object (a Grasshopper component or a Rhino object) is registered as one custom transient per preview server.

- `PreviewDrawable` derives from `Autodesk.AutoCAD.EditorInput.Transient` and draws in `SubWorldDraw` from buffers built beforehand (`IPreviewGeometryBuffer`): polylines, a polypoint, and shells (`IPreviewShell`).
- Colour, transparency and material are set through `SubEntityTraits` and read from the current preview settings at draw time.
- Text, dimensions, leaders and hatches are still converted to entities by the existing `IRhinoConvertible` converters, and are drawn inside the transient with `Geometry.Draw`. These are the fallback entities.
- `PreviewDrawableBuilder` builds the buffers and replaces `PreviewGeometryConverter`.
- A selection or colour change redraws the existing transient with `TransientManager.UpdateTransient` instead of rebuilding it.
- Each server caps the number of preview items (each polyline, point, shell and fallback entity a drawable draws, not AutoCAD entities) with the "Preview item limit" user setting (default 50,000). The Rhino preview and the Grasshopper shaded and wireframe previews each have their own cap. When adding an object would exceed the cap, the oldest registered objects are evicted first, so the newest win. This bounds the cost of pathological definitions. The setting is persisted as `MaxPreviewEntityCount`, a name kept from before items were counted so existing settings files keep their value.

### Implementation notes

These points cost time to discover and are easy to break:

- Derive from `EditorInput.Transient`, not from `Drawable` directly. Calling `base(IntPtr.Zero, false)` on a `Drawable` subclass throws `InvalidOperationException` in the `RXObject` constructor.
- Pass empty `EdgeData`, `FaceData` and `VertexData` instances to `Geometry.Shell`, never null.
- The object register must hold a strong reference to each drawable for as long as it is registered as a transient. Otherwise the garbage collector can collect it while AutoCAD still draws it.
- `Geometry.Polypoint` must be given normals and sub-entity marker arrays of the same length as the points.
- Core stays free of AutoCAD types. The buffer interfaces expose wrapper interfaces (`IPoint3dCollection`, `IIntegerCollection`, `IVector3dCollection`, `IIntPtrCollection`), implemented in Interop on `AutocadWrapperBase`. `SubWorldDraw` calls `Unwrap()` and passes the native collections straight to `Geometry`, so a redraw copies nothing.

## Consequences

Positive:

- The transient count drops to the number of previewed objects.
- Curves, points, meshes, Breps, extrusions, surfaces and SubDs create no AutoCAD entities.
- Selection and colour changes are a redraw, not a rebuild.

Negative:

- Each geometry type needs drawing code that we maintain ourselves.
- Curves are tessellated rather than drawn as true curves. The chord tolerance is relative to the curve size, with a 2 degree angle tolerance.
- Points are drawn without `PDMODE`/`PDSIZE` styling.
- Fallback types still create AutoCAD entities.

Neutral:

- The `IRhinoConvertible` converters stay, because baking uses them as well as the fallback path.

## Alternatives considered

- **Keep one transient per entity and lower the cap.** This limits the damage but leaves the underlying cost in place. It is only acceptable as a stopgap.
- **Merge geometry into fewer entities** (for example one polyline or mesh entity per object). This reduces the transient count but still creates and disposes entities on every rebuild, and `SubDMesh` remains expensive.
- **A `DrawableOverrule` on carrier entities.** This could draw custom geometry through entities AutoCAD already manages, but it involves more moving parts and an overrule applies to every entity of its class, not only to our previews.
