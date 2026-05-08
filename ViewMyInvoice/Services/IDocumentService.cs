using System.Xml;

namespace ViewMyInvoice.Services;

public interface IDocumentService
{
    Task<XmlDocument?> OpenDocument();
    Task SaveDocument(XmlDocument document);
}
