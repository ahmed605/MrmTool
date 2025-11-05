// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;

namespace CommunityToolkit.WinUI.Converters;

/// <summary>
/// Static class used to provide internal tools
/// </summary>
internal static class ConverterTools
{
    /// <summary>
    /// Helper method to safely cast an object to a boolean
    /// </summary>
    /// <param name="parameter">Parameter to cast to a boolean</param>
    /// <returns>Bool value or false if cast failed</returns>
    internal static bool TryParseBool(object parameter)
    {
        var parsed = false;
        if (parameter != null)
        {
            bool.TryParse(parameter.ToString(), out parsed);
        }

        return parsed;
    }

    /// <summary>
    /// Helper method to convert a value from a source type to a target type.
    /// </summary>
    /// <param name="value">The value to convert</param>
    /// <param name="targetType">The target type</param>
    /// <returns>The converted value</returns>
    internal static object Convert(object value, Type targetType)
    {
        if (targetType.IsInstanceOfType(value))
        {
            return value;
        }
        else
        {
            return XamlBindingHelper.ConvertValue(targetType, value);
        }
    }
}

/// <summary>
/// This class converts a boolean value into an other object.
/// Can be used to convert true/false to visibility, a couple of colors, couple of images, etc.
/// </summary>
public partial class BoolToObjectConverter : DependencyObject, IValueConverter
{
    /// <summary>
    /// Identifies the <see cref="TrueValue"/> property.
    /// </summary>
    public static readonly DependencyProperty TrueValueProperty =
        DependencyProperty.Register(nameof(TrueValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

    /// <summary>
    /// Identifies the <see cref="FalseValue"/> property.
    /// </summary>
    public static readonly DependencyProperty FalseValueProperty =
        DependencyProperty.Register(nameof(FalseValue), typeof(object), typeof(BoolToObjectConverter), new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the value to be returned when the boolean is true
    /// </summary>
    public object TrueValue
    {
        get { return GetValue(TrueValueProperty); }
        set { SetValue(TrueValueProperty, value); }
    }

    /// <summary>
    /// Gets or sets the value to be returned when the boolean is false
    /// </summary>
    public object FalseValue
    {
        get { return GetValue(FalseValueProperty); }
        set { SetValue(FalseValueProperty, value); }
    }

    /// <summary>
    /// Convert a boolean value to an other object.
    /// </summary>
    /// <param name="value">The source data being passed to the target.</param>
    /// <param name="targetType">The type of the target property, as a type reference.</param>
    /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>The value to be passed to the target dependency property.</returns>
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        bool boolValue = value is bool && (bool)value;

        // Negate if needed
        if (ConverterTools.TryParseBool(parameter))
        {
            boolValue = !boolValue;
        }

        return ConverterTools.Convert(boolValue ? TrueValue : FalseValue, targetType);
    }

    /// <summary>
    /// Convert back the value to a boolean
    /// </summary>
    /// <remarks>If the <paramref name="value"/> parameter is a reference type, <see cref="TrueValue"/> must match its reference to return true.</remarks>
    /// <param name="value">The target data being passed to the source.</param>
    /// <param name="targetType">The type of the target property, as a type reference (System.Type for Microsoft .NET, a TypeName helper struct for Visual C++ component extensions (C++/CX)).</param>
    /// <param name="parameter">An optional parameter to be used to invert the converter logic.</param>
    /// <param name="language">The language of the conversion.</param>
    /// <returns>The value to be passed to the source object.</returns>
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        bool result = Equals(value, ConverterTools.Convert(TrueValue, value.GetType()));

        if (ConverterTools.TryParseBool(parameter))
        {
            result = !result;
        }

        return result;
    }
}

/// <summary>
/// This class converts a boolean value into a Visibility enumeration.
/// </summary>
public partial class BoolToVisibilityConverter : BoolToObjectConverter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoolToVisibilityConverter"/> class.
    /// </summary>
    public BoolToVisibilityConverter()
    {
        TrueValue = Visibility.Visible;
        FalseValue = Visibility.Collapsed;
    }
}