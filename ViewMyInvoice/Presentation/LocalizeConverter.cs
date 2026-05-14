using Microsoft.UI.Xaml.Data;

namespace ViewMyInvoice.Presentation;

public record LocalizeConverter : IValueConverter
{
    private readonly IStringLocalizer _localizer = App.Instance!.Host!.Services.GetService<IStringLocalizer>()!;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string key || string.IsNullOrEmpty(key))
            return value;

        var localized = _localizer[key].ToString();

        if ((string)parameter == "BTKey")
            localized = $"{localized} ({value})";
        
        return localized;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
