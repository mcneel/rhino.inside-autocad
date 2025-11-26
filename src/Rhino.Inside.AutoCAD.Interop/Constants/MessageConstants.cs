namespace Rhino.Inside.AutoCAD.Interop;

/// <summary>
/// A constants class containing message strings, such as warnings, for
/// UI-bound purposes.
/// </summary>
public class MessageConstants
{
    /// <summary>
    /// A constant for empty strings.
    /// </summary>
    public const string EmptyString = "";

    /// <summary>
    /// A Void message.
    /// </summary>
    public const string Void = "VOID";

    /// <summary>
    /// The message displayed if a <see cref="IDatumLineGrid"/> has no warnings.
    /// </summary>
    public const string NoneWarning = "";

    /// <summary>
    /// The message displayed if a <see cref="IDatumLineGrid"/> is not orthogonal.
    /// </summary>
    public const string NonOrthogonalGrid = "Custom gridlines do not form an orthogonal grid";

    /// <summary>
    /// The message displayed if no <see cref="ISketchLine2d"/>s could be found to
    /// generate <see cref="IDatumLine"/>s for a <see cref="IDatumLineGrid"/>. E.g.
    /// the user drew no grouped <see cref="ISketchLine2d"/>s or the <see cref=
    /// "ISketchGrid2d"/> does not intersect any <see cref="ICeilingInstance"/>.
    /// </summary>
    public const string CustomGridLinesNotFound =
        "Custom grid block could not be found. Ensure the Tee Filter Layer is set in the Filters menu and all grid blocks use it.";

    /// <summary>
    /// The message displayed if a <see cref="IBaseLayout"/> which is
    /// <see cref="IBaseLayout.IsCustom"/> false failed to generate any
    /// <see cref="IDatumLine"/>s. This is only possible if the <see cref="ICeilingDomain"/>
    /// is smaller than the panel size, indicating a problem with the CAD file
    /// or source geometry.
    /// </summary>
    public const string BaseLayoutEmptyGrid =
        "Ceiling grid could not be generated. Ensure the RCP is larger than the panel size";

    /// <summary>
    /// The message for a warning dialog when a <see cref="ISpecDesignation"/> is
    /// removed in the RCP solvers.
    /// </summary>
    public const string SpecDesignationRemovedMessage
        = "A Spec Designation which is in use was removed, please select a new spec designation to replace it before proceeding.";

    /// <summary>
    /// The message for an invalid <see cref="ISpecDesignation"/>.
    /// </summary>
    public const string SpecDesignationInvalid = "An Invalid Spec Designation has been used";

    /// <summary>
    /// The message for an invalid <see cref="IClientSpecificationId"/> in a <see cref="ISpecDesignation"/>.
    /// </summary>
    public const string SpecDesignationClientIdIsEmpty = "Client Specification ID cannot be empty";

    /// <summary>
    /// The default display name for an Invalid <see cref="ISpecDesignation"/>.
    /// </summary>
    public const string SpecDesignationInValidDisplayName = "Select a Spec Designation";

    /// <summary>
    /// The message for when the current <see cref="ISpecDesignation"/> is not
    /// unique, e.g. its properties are the same as another <see cref="ISpecDesignation"/>.
    /// </summary>
    public const string SpecDesignationIsIdentical = "Spec Designation attributes match {0}. Enter a unique Spec to proceed.";

    /// <summary>
    /// The message for a warning dialog when a <see cref="IPanelType"/> is
    /// not found.
    /// </summary>
    public const string PanelTypeNotFoundMessage = "Panel Type block style json file could not be found";

    /// <summary>
    /// The message for a when the <see cref="IPerforationToolTolerances"/> fails to import.
    /// </summary>
    public const string FailedToLoadManufacturerMachineTolerances = "Failed to import Manufacturer Machine Tolerances";

    /// <summary>
    /// The name of the default perforation for a panel, which is used when there are no perforations.
    /// </summary>
    public const string NonePerforationName = "M1";

    /// <summary>
    /// The message for when a Perforation Shape is not support by the solver. This string is formatted
    /// with the shape name.
    /// </summary>
    public const string NotSupportedPerforationShape = "Perforation hole shape '{0}' is not supported.";

    /// <summary>
    /// The name of a <see cref="IPositionNumberSeries"/>.
    /// </summary>
    public const string PositionNumberSeriesName = "Series {0} ({0} to {1})";

    /// <summary>
    /// The message when a <see cref="IPositionNumberSeries"/> is added.
    /// </summary>
    public const string PositionNumberSeriesAdded =
        "A new position number series has been added. Any Existing Blocks which clash with this new position series will have been updated to a new number range.";

    /// <summary>
    /// The message when a <see cref="IPositionNumberSeries"/> is Removed.
    /// </summary>
    public const string PositionNumberSeriesRemoved =
        "A position number series has been removed. Any Existing Blocks above this new position series will have been updated to removed any gaps in the series.";

    /// <summary>
    /// Text which is "OK", this is used in multiple place like for Button Text. For
    /// example when a <see cref="IPositionNumberSeries"/> is added.
    /// </summary>
    public const string OkText = "OK";

    /// <summary>
    /// The Message for an Invalid file path.
    /// </summary>
    public const string InvalidFilePath = "Invalid file path. Select a valid location and file name";

    /// <summary>
    /// The Message for when a saving a <see cref="ICeilingInstancesSnapshot"/> fails.
    /// </summary>
    public const string CeilingInstanceSaveSnapshotFailed = "RCP Save ICeilingInstancesSnapshot failed: {0}";

    /// <summary>
    /// The Message for when a saving a <see cref="ICeilingInstancesSnapshot"/> fails.
    /// </summary>
    public const string CeilingInstanceLoadSnapshotFailed = "RCP Load ICeilingInstancesSnapshot failed: {0}";

    /// <summary>
    /// The Message for when a saving RCP Inputs fails.
    /// </summary>
    public const string SaveFailedError = "An error occurred and the file could not be saved";

    /// <summary>
    /// The Message for when an invalid path when loading RCP Inputs.
    /// </summary>
    public const string InvalidFilePathRCPLoad = "Invalid file path. Select a JSON file containing your RCP inputs";

    /// <summary>
    /// The Message for when a loading RCP Inputs fails.
    /// </summary>
    public const string LoadFailedError = "An error has occured and the RCP inputs could not be restored";

    /// <summary>
    /// The Message for when RCP Inputs Restore fails.
    /// </summary>
    public const string RcpInputsNotRestored =
        "Inputs were not restored. This is typically caused by a deleted backup file. Try clicking the Refresh button to see the most recent backups and try again";

    /// <summary>
    ///The Message for when RCP Inputs Restore ius successful.
    /// </summary>
    public const string RcpInputsRestored = "RCP inputs successfully restored";

    /// <summary>
    /// The Message for when RCP Inputs Save is successful.
    /// </summary>
    public const string RcpSaved = "RCP inputs successfully saved";

    /// <summary>
    /// The Message for when a <see cref="ITrimType"/> already exists with inputs.
    /// </summary>
    public const string TrimAlreadyExistsMessage =
        "A Trim with these properties already exists, Do you want to create a duplicate? Press Cancel to abort.";

    /// <summary>
    /// The Message for the notification button text to add duplicate
    /// <see cref="ITrimType"/>s
    /// </summary>
    public const string AddDuplicate = "Add Duplicate";

    /// <summary>
    /// The message displayed to the user if they modify the document while running the
    /// RCP Solver notifying them to check their RCPs to ensure the changes have not
    /// invalidated them.
    /// </summary>
    public const string DocumentChangedReviewRCPs = "Review your RCP selection to ensure the document changes have not invalidated any RCPs";

    /// <summary>
    /// The message displayed to the user if they change the filter layer for
    /// <see cref="IBaseLayout.IsCustom"/> while running the RCP Solver.
    /// </summary>
    public const string CustomGridFilterLayerChange = "Review your RCP selection to ensure the layer filter change has not invalidated any custom grids";

    /// <summary>
    /// The Symbol used to decorate <see cref="IPanel"/>s <see cref="IPanelTypePositionNumber"/>
    /// in drawings for panels which are Field Cut.
    /// </summary>
    public const string FieldCutSymbol = "*";

    /// <summary>
    /// The Symbol used to decorate <see cref="IPanel"/>s <see cref="IPanelTypePositionNumber"/>
    /// in drawings for panels which are using rotated 180 degrees <see cref="IPanelType"/>s.
    /// This the opening parentheses and should be used in conjunction with the <see
    /// cref="RotatedPanelSymbolClosing"/>.
    /// </summary>
    public const string RotatedPanelSymbolOpening = "(";

    /// <summary>
    /// The Symbol used to decorate <see cref="IPanel"/>s <see cref="IPanelTypePositionNumber"/>
    /// in drawings for panels which are using rotated 180 degrees <see cref="IPanelType"/>s.
    /// This the closing parentheses and should be used in conjunction with the <see
    /// cref="RotatedPanelSymbolOpening"/>.
    /// </summary>
    public const string RotatedPanelSymbolClosing = ")";

    /// <summary>
    /// A message shown to the user when adding <see cref="IBreakPoint"/> in the <see
    /// cref="ICurveParameterPickerJig"/>.
    /// </summary>
    public const string AddBreakPointMessage = "\nAdd Break Point:";

    /// <summary>
    /// The message to display if filter hatch layer is locked.
    /// </summary>
    public const string LayerLockedMessage = "Locked layer";

    /// <summary>
    /// The message to display if input parameter is empty.
    /// </summary>
    public const string EmptyInputErrorMessage = "Input required";

    /// <summary>
    /// A message shown to the user in the UI informing them which <see cref="IOpeningEdgeLoop"/> a
    /// <see cref="ITemplatedOpening"/> is based on.
    /// </summary>
    public const string TemplatedOpeningBaseOnText = "{0}'s Opening {1}";

    /// <summary>
    /// A message shown to the user in the UI if a <see cref="ITemplatedOpening"/> already exists
    /// which matches the current attributes.
    /// </summary>   
    public const string TemplatedOpeningDuplicationError = "A Matching Template Already Exists: \n{0}";

    /// <summary>
    /// A message for when a <see cref="ITemplatedOpening"/> is successfully created.
    /// </summary>
    public const string TemplatedOpeningCreated = "Templated Opening was successfully created, select it from the drop down";

    /// <summary>
    /// An error message for when a <see cref="ITemplatedOpening"/> name is not unique
    /// </summary>
    public const string TemplatedOpeningMissingName = "Name is required and must be unique";

    /// <summary>
    /// An error message for when a <see cref="ITemplatedOpening"/> Type does not match
    /// the <see cref="IOpeningEdgeLoop"/> Type.
    /// </summary>
    public const string TemplatedOpeningTypeMismatch = "This Template is not compatible with this \nopening type. Select a compatible template.";

    /// <summary>
    /// An error message for when a <see cref="ITemplatedOpening"/> Geometry does not match
    /// the <see cref="IOpeningEdgeLoop"/> Type.
    /// </summary>
    public const string TemplatedOpeningGeometryTypeMismatch = "The Geometry of this template cannot be applied \nto this opening. Select a compatible Template.";

    /// <summary>
    /// An error message for the <see cref="IInteropService"/> for when an unsaved document
    /// is used.
    /// </summary>
    public const string UnsavedNotSupported = "Warning: Unsaved documents are not supported. Save your file to run the application.";

    /// <summary>
    /// An error message for the <see cref="IInteropService"/> for when a readonly document
    /// is used.
    /// </summary>
    public const string ReadOnlyNotSupported = "Warning: Read only documents are not supported. Open your file with Write enabled to run the application.";

    /// <summary>
    /// An error message for the <see cref="IInteropService"/> for when unsupported units
    /// are used.
    /// </summary
    public const string FileUnitsNotSupported = "Warning: unsupported document file units ({0}). Set a valid metric or imperial unit system and try again.";

    /// <summary>
    /// The Suffix for a <see cref="ISectionDetail"/> name.
    /// </summary>
    public const string SectionNameSuffix = " Section";

    /// <summary>
    /// Text used when a panel is field cut in the Section tool.
    /// </summary>
    public const string FieldCutPanelDimensionText = @"VARIES - SEE RCP\PFACE OF STRUCT.TO PANEL JOINT\P(FIELD CUT AS REQ'D BY CONTRACTOR)";

    /// <summary>
    /// Text used for dimensions when a panel is not field cut in the Section tool.
    /// </summary>
    public const string NonFieldCutPanelDimensionText = @"<>\PPANEL JOINT TO PANEL JOINT";
}
