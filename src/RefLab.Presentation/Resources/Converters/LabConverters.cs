using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace RefLab.Presentation.Resources.Converters;

/// <summary>
/// Converts a boolean into a visibility.
/// </summary>
public sealed class BoolToVisibilityConverter : IValueConverter
{
    /// <summary>When <see langword="true"/>, a <see langword="true"/> value becomes visible.</summary>
    public bool Invert { get; set; }

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var flag = value is bool boolean && boolean;

        if (Invert)
        {
            flag = !flag;
        }

        return flag ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is Visibility visibility && visibility == Visibility.Visible;
}

/// <summary>
/// Converts a value into a visibility: a non-null value becomes visible.
/// </summary>
public sealed class NotNullToVisibilityConverter : IValueConverter
{
    /// <summary>When <see langword="true"/>, a <see langword="null"/> value becomes visible.</summary>
    public bool Invert { get; set; }

    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var present = value is not null && value is not string text || value is string content && !string.IsNullOrWhiteSpace(content);

        if (Invert)
        {
            present = !present;
        }

        return present ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}

/// <summary>
/// Converts an enumeration value into a boolean so that it can drive a radio
/// button or a check box.
/// </summary>
public sealed class EnumEqualsConverter : IValueConverter
{
    /// <inheritdoc />
    /// <remarks>
    /// The parameter is the name of the enumeration member the control stands
    /// for — <c>ConverterParameter=Male</c> — so that the radio group of an
    /// enumeration can be written in XAML without a code-behind.
    /// </remarks>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null || parameter is null)
        {
            return false;
        }

        if (value.Equals(parameter))
        {
            return true;
        }

        return value is Enum
            && string.Equals(value.ToString(), parameter.ToString(), StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool flag || !flag || parameter is null)
        {
            return Binding.DoNothing;
        }

        // The control is bound to the enumeration itself, so the parameter —
        // the name of the member — is parsed back into the member.
        if (parameter is Enum)
        {
            return parameter;
        }

        var name = parameter.ToString();

        if (string.IsNullOrWhiteSpace(name))
        {
            return Binding.DoNothing;
        }

        var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (!enumType.IsEnum)
        {
            return Binding.DoNothing;
        }

        return Enum.Parse(enumType, name, ignoreCase: false);
    }
}

/// <summary>
/// Converts an enumeration value into the display text of a matching entry in
/// a supplied list.
/// </summary>
public sealed class EnumDisplayConverter : IValueConverter
{
    /// <inheritdoc />
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value?.ToString() ?? string.Empty;

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        Binding.DoNothing;
}
