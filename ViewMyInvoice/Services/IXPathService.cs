using System.Xml;

namespace ViewMyInvoice.Services;

public interface IXPathService
{
    (XmlDocument? document, XmlNamespaceManager? nsManager) State { get; }
    void RegisterDocument(XmlDocument document);
    XmlNode? Select(InvoiceField mapping);
    XmlNode RegisterElement(InvoiceField mapping);
}
