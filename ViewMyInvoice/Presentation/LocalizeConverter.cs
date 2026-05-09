using Microsoft.UI.Xaml.Data;

namespace ViewMyInvoice.Presentation;

public record LocalizeConverter : IValueConverter
{
    private readonly IStringLocalizer _localizer = App.Instance!.Host!.Services.GetService<IStringLocalizer>()!;

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is not string key || string.IsNullOrEmpty(key))
            return value;
        return _localizer[key];
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
        => throw new NotImplementedException();
}
