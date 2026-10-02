# ADR 0002: Preview visibility follows the Rhino and Grasshopper windows and documents

## Status

Accepted, 2026-10-01.

## Context

Previews in AutoCAD outlived the things they showed. Closing a Rhino document, or opening a file into it, left its objects drawn in AutoCAD: Rhino raises no `DeleteRhinoObject` event for the objects of a closed document. Switching or closing the document on the Grasshopper canvas left the previous definition's previews behind in the same way.

Previews also stayed on screen when the user put the window that owned them out of sight. Minimising Rhino or the Grasshopper editor, or closing it, left geometry in the AutoCAD view that the user could no longer see or edit at its source. Rhino.Inside's close button does not close the Rhino window; it hides it, so there is no close event to listen for.

## Decision

### Documents

- Rhino previews are tracked per source document, by the document's runtime serial number. When a Rhino document goes away, only the previews of that document are removed.
- A document goes away on `RhinoDoc.CloseDocument`, and on `RhinoDoc.BeginOpenDocument` when the open replaces its contents. Imports and reference (worksession) opens add to the document, so they are ignored.
- Removal names the document it is for, so it does not depend on whether Rhino raises it before or after it starts adding the next document's objects. The next document's objects are previewed as Rhino adds them.
- The temporary headless documents this plugin creates and disposes, such as the one the Brep converter imports into, are ignored.
- When the Grasshopper canvas switches or closes its document, every Grasshopper preview is cleared, and the new document's previews are rebuilt through `IGrasshopperInstance.PreviewExpired`.

### Windows

- The Rhino preview hides while the Rhino main window is minimised or hidden, and the Grasshopper preview while the Grasshopper editor is.
- `RhinoWindowManager` watches the Rhino main window with the CBT hook it already installs on the window's thread. `HCBT_MINMAX` reports minimise and restore requests directly. `HCBT_SYSCOMMAND` with `SC_CLOSE`, and any `HCBT_ACTIVATE`, schedule a re-read of `IsWindowVisible` and `IsIconic` once AutoCAD is next idle, because the hook runs before the window changes and Rhino hides the window after the hook returns. Pending re-reads are merged into one.
- `GrasshopperWindowManager` watches the editor `Form` through its own `VisibleChanged` and `Resize` events. The editor does not exist yet when its canvas is created, so it is attached on the next idle after canvas creation, and a replaced editor is let go in favour of the new one. There is no polling and no window subclassing.
- Hiding is a suppression kept separate from the user's choice: the Rhino preview toggle and the Grasshopper preview mode are left as the user set them. Restoring or showing the window lifts the suppression and returns the preview to the user's choice, rather than to on.
- Two user settings, `HideRhinoPreviewWhenWindowHidden` and `HideGrasshopperPreviewWhenEditorHidden`, both default true, let the user keep either preview on screen regardless of its window.
- In a Grasshopper-only session the Rhino window is never shown, so the Rhino preview stays suppressed for the whole session unless `HideRhinoPreviewWhenWindowHidden` is turned off.

## Consequences

Positive:

- Closing or replacing a Rhino document, or switching the Grasshopper document, leaves nothing stale in AutoCAD.
- What is drawn in AutoCAD matches what the user can see and edit in Rhino and Grasshopper.
- Restoring a window never overrides the user's toggle or preview mode.

Negative:

- The Rhino window state relies on a Win32 CBT hook and an idle re-check, which are easier to break than a managed event.
- A Grasshopper-only session shows no Rhino preview by default, which may surprise users who expect one.
- Two more settings appear on the Support dialog.

Neutral:

- Rhino previews are keyed by document serial as well as object id, so the manager keeps a map from previewed object to source document.

## Alternatives considered

- **Clear every Rhino preview when any document closes.** Simpler, but Rhino may raise the close after the next document's objects have started arriving, so the new previews would be lost.
- **Poll the window state on a timer, or subclass the Rhino window procedure.** Polling costs work on every tick and lags the change; subclassing a window Rhino owns is fragile. The existing hook and the editor's own events cover the cases without either.
- **Turn the preview off through the user's toggle or mode.** Restoring the window could not then tell whether the user had switched the preview off themselves, and would either turn it back on against their choice or leave it off.
