using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using Uno.Extensions.Specialized;

namespace ViewMyInvoice.Presentation;

public partial record InvoiceViewModel
{
    //private readonly ILogger<InvoiceViewModel> _logger;
    private readonly ILocalizationService _localizationService;
    private readonly IConfigService _configService;
    private readonly IXPathService _xPathService;
    private readonly IDocumentService _documentService;

    public ObservableCollection<InvoiceFieldGroup> GroupedFields { get; init; }
    public IState<CultureInfo> CurrentCulture => State<CultureInfo>
        .Value(this, () => _localizationService.CurrentCulture);

    public InvoiceViewModel(
        //ILogger<InvoiceViewModel> logger,
        ILocalizationService localizationService,
        IConfigService configService,
        IXPathService xPathService,
        IDocumentService documentService)
    {
        //_logger = logger;
        _localizationService = localizationService;
        _configService = configService;
        _xPathService = xPathService;
        _documentService = documentService;

        var mappings = _configService.Config.Mappings
            ?? throw new NullReferenceException("InvoiceViewModel: encountered misconfigured invoice field mappings");
        GroupedFields = [ ..CreateFieldViews(mappings) ];
        RegisterFieldPropertyCallbacks();
    }

    public async Task ToggleLocalization()
    {
        var currentCulture = await CurrentCulture
            ?? throw new NullReferenceException("InvoiceViewModel.ToggleLocalization: could not get current culture");
        var nextCulture = _localizationService.SupportedCultures
            .First(culture => culture.Name != currentCulture.Name);
        await _localizationService.SetCurrentCultureAsync(nextCulture);
    }

    public async Task SaveDocument()
    {
        (var doc, var _) = _xPathService.State;
        if (doc == null)
            throw new NullReferenceException("InvoiceViewModel.SaveDocument: received null document from IXPathService");
        await _documentService.SaveDocument(doc);
    }

    private IEnumerable<InvoiceFieldGroup> CreateFieldViews(IEnumerable<InvoiceField> mappings) => mappings
        .GroupBy(m => m.GroupKey)
        .Select(g => new InvoiceFieldGroup(
            GroupName: g.Key ?? "",
            Fields: [.. g.Select(f => new InvoiceFieldViewModel(f, GetFieldValue(f)))]));

    private void RegisterFieldPropertyCallbacks() => GroupedFields
        .ForEach((i_, g) => g.Fields.ForEach((_, f) => f.PropertyChanged += OnFieldPropertyChanged));

    private string GetFieldValue(InvoiceField mapping)
    {
        var node = _xPathService.Select(mapping);
        return node?.InnerText ?? string.Empty;
    }

    private void OnFieldPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (sender == null || e.PropertyName != nameof(InvoiceFieldViewModel.Value))
            return;

        var viewModel = (InvoiceFieldViewModel)sender;
        var node = _xPathService.Select(viewModel.FieldInfo);
        node?.InnerText = viewModel.Value;
    }
}
