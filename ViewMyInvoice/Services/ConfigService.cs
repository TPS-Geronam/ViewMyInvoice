using Microsoft.Extensions.Configuration;

namespace ViewMyInvoice.Services;

public record ConfigService(ILogger<ConfigService> Logger, IConfiguration Configuration) : IConfigService
{
    private const string CONFIG_SECTION = "InvoiceFieldConfig";

    public InvoiceFieldConfig Config => Configuration
        .GetRequiredSection(CONFIG_SECTION)
        .Get<InvoiceFieldConfig>()!;


    //private const string RESOURCES_LOCATION = "ms-appx:///Assets/Resources/";
    //public async Task<InvoiceFieldConfig?> ConfigAsync()
    //    => await LoadConfig($"{RESOURCES_LOCATION}/invoice_field_config.json");

    //private async Task<InvoiceFieldConfig?> LoadConfig(string fileName)
    //{
    //    try
    //    {
    //        (var text, var _) = await LoadLocalFile(fileName);
    //        return JsonConvert.DeserializeObject<InvoiceFieldConfig>(text);
    //    }
    //    catch (Exception ex)
    //    {
    //        Logger.LogError("DocumentService could not load XML");
    //        Logger.LogErrorMessage(ex.Message);
    //        return default;
    //    }
    //}

    //private static async Task<(string text, StorageFile file)> LoadLocalFile(string fileName)
    //{
    //    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"{RESOURCES_LOCATION}{fileName}"));
    //    var text = await FileIO.ReadTextAsync(file);
    //    return (text, file);
    //}
}
