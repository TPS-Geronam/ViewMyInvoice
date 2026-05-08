using System.Collections.ObjectModel;

namespace ViewMyInvoice.Models;

public record InvoiceFieldGroup(
    string GroupName,
    ObservableCollection<InvoiceFieldViewModel> Fields)
{
}
