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
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        value is not null && parameter is not null && value.Equals(parameter);

    /// <inheritdoc />
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool flag && flag && parameter is not null)
        {
            return parameter;
        }

        return Binding.DoNothing;
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
