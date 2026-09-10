using Autodesk.AutoCAD.Runtime;

// Tells the AutoCAD managed loader this assembly hosts no extension application and no
// commands, so it must not reflect over the exported types. Without these it calls
// Assembly.GetExportedTypes(), which forces RhinoCommon to resolve before the
// Rhino version has been bound (FileNotFoundException on the AutoCAD command line).
[assembly: ExtensionApplication(null)]
[assembly: CommandClass(null)]
