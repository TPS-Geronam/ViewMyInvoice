using CommunityToolkit.Mvvm.ComponentModel;

namespace ViewMyInvoice.Models;

public partial class InvoiceFieldViewModel(InvoiceField fieldInfo, string value)
    : ObservableObject
{
    [ObservableProperty]
    private InvoiceField _fieldInfo = fieldInfo;
    [ObservableProperty]
    private string _value = value;
}
