using System.Xml;

namespace ViewMyInvoice.Services;

public interface IDocumentService
{
    Task<(XmlDocument doc, string docName)?> OpenDocument();
    Task SaveDocument(XmlDocument document, string? suggestedFileName);
}
