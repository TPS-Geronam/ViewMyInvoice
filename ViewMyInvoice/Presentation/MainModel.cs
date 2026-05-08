using System.Xml;

namespace ViewMyInvoice.Presentation;

public partial record MainModel(
    IStringLocalizer Localizer,
    IOptions<AppConfig> AppInfo,
    INavigator Navigator,
    IDocumentService DocumentService,
    IXPathService XPathService)
{
    public string? Title { get; } = $"{Localizer["ApplicationName"]} - {AppInfo?.Value?.Environment}";
    //public IState<string> Name => State<string>.Value(this, () => string.Empty);

    public async Task OpenDocument()
    {
        var xml = await DocumentService.OpenDocument()
            ?? throw new NullReferenceException("MainModel.GoToViewer: received null document from DocumentService");
        await GoToViewer(xml);
    }

    private async Task GoToViewer(XmlDocument xml)
    {
        XPathService.RegisterDocument(xml);
        await Navigator.NavigateViewModelAsync<InvoiceViewModel>(this, data: xml);
    }
}
