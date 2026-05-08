namespace ViewMyInvoice.Models;

public record InvoiceField(
    string FieldKey,
    string FieldQuery,
    string? GroupKey = "")
{
}
