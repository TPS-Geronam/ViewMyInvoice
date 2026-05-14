using System.Xml;

namespace ViewMyInvoice.Presentation;

public partial record MainModel(
    IOptions<AppConfig> AppInfo,
    INavigator Navigator,
    IStringLocalizer Localizer,
    IDocumentService DocumentService,
    IXPathService XPathService)
{
    public string? Title { get; } = $"{Localizer["ApplicationName"]} - {AppInfo?.Value?.Environment}";

    public async Task OpenDocument()
    {
        var (doc, docName) = await DocumentService.OpenDocument()
            ?? throw new NullReferenceException("MainModel.GoToViewer: received null document from DocumentService");
        await GoToViewer(doc, docName);
    }

    private async Task GoToViewer(XmlDocument doc, string docName)
    {
        XPathService.RegisterDocument(doc, docName);
        await Navigator.NavigateViewModelAsync<InvoiceViewModel>(this, data: doc);
    }
}
