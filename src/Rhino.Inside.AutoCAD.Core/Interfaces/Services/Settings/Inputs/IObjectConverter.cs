using System.ComponentModel;

namespace Rhino.Inside.AutoCAD.Core.Interfaces;

/// <summary>
/// An interface that serves as a contract for managing and validating
/// <see cref="ISettingInput"/>.
/// </summary>
public interface IObjectConverter : INotifyPropertyChanged, IDataErrorInfo
{
    /// <summary>
    /// Event raised when the <see cref="SelectedValue"/> changes.
    /// </summary>
    event EventHandler? SelectedValueChanged;

    /// <summary>
    /// A collection of values which can be used for storing multiple values
    /// input by the user.
    /// </summary>
    object? Values { get; }

    /// <summary>
    /// The data stored in this property.
    /// </summary>
    object? SelectedValue { get; set; }

    /// <summary>
    /// The previous <see cref="SelectedValue"/> stored in this property.
    /// Allows for editable values which can be reverted to the previous value.
    /// </summary>
    object? PreviousValue { get; set; }

    /// <summary>
    /// Returns false if the <see cref="SelectedValue"/> contains an error
    /// otherwise true indicating a valid value.
    /// </summary>
    bool IsValid { get; set; }
    
    /// <summary>
    /// The path name of a property which displays the corresponding value in
    /// the UI view.
    /// </summary>
    string DisplayMemberPath { get; }

    /// <summary>
    /// Validates the <see cref="SelectedValue"/> returning true if the input
    /// is valid or false if its not.
    /// </summary>
    bool ValidateInput();

    /// <summary>
    /// Returns the value stored in this <see cref="IObjectConverter"/>. 
    /// </summary>
    object? GetSelectedValue();

    /// <summary>
    /// Returns the value stored in this <see cref="IObjectConverter"/> as a
    /// value type which can be sent over the web, e.g. as a JSON value. 
    /// </summary>
    object? GetSelectedAsValueType();

    /// <summary>
    /// Method which allows a custom value to be assigned to this
    /// <see cref="IObjectConverter"/> value property for multiple selection
    /// with a display member path string for UI display. Optional value
    /// input can be used to set the <see cref="SelectedValue"/> to a value
    /// from the <paramref name="customValues"/> collection.
    /// </summary>
    void SetSelectionValues(object customValues, string displayMemberPath, object? value = null);
    
    /// <summary>
    /// Cancels any change to the <see cref="SelectedValue"/> property and restores
    /// it to its <see cref="PreviousValue"/>.
    /// </summary>
    void CancelChangedValue();

    /// <summary>
    /// Confirms and applies the change to the <see cref="SelectedValue"/> property
    /// and updates the <see cref="PreviousValue"/>. If the property has not changed
    /// returns false, otherwise returns true indicating the value has been changed.
    /// </summary>
    bool ApplyChangedValue();
}