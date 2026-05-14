using System.Xml;
using Uno.Extensions.Specialized;

namespace ViewMyInvoice.Services;

internal class XPathService : IXPathService
{
    private readonly Dictionary<InvoiceField, XmlNode> _xPathCache = [];

    private readonly Dictionary<string, string> _namespaces = new()
    {
        { "rsm", "urn:un:unece:uncefact:data:standard:CrossIndustryInvoice:100" },
        { "ram", "urn:un:unece:uncefact:data:standard:ReusableAggregateBusinessInformationEntity:100" },
        { "qdt", "urn:un:unece:uncefact:data:standard:QualifiedDataType:100" },
        { "udt", "urn:un:unece:uncefact:data:standard:UnqualifiedDataType:100" },
        { "cbc", "urn:oasis:names:specification:ubl:schema:xsd:CommonBasicComponents-2" },
        { "cac", "urn:oasis:names:specification:ubl:schema:xsd:CommonAggregateComponents-2" },
        { "ext", "urn:oasis:names:specification:ubl:schema:xsd:CommonExtensionComponents-2" },
        { "fx", "urn:factur-x:pdfa:CrossIndustryDocument:invoice:1p0#" },
        { "ubl", "urn:oasis:names:specification:ubl:schema:xsd:Invoice-2" },
        { "xs", "http://www.w3.org/2001/XMLSchema" },
    };
    private XmlNamespaceManager? _namespaceManager;

    private XmlDocument? _document;
    
    public (XmlDocument? document, XmlNamespaceManager? nsManager) State
        => (_document, _namespaceManager);

    public void RegisterDocument(XmlDocument document)
    {
        _xPathCache.Clear();
        _document = document;
        _namespaceManager = GetNamespaceManager(document);
    }

    public XmlNode? Select(InvoiceField mapping)
    {
        if (mapping == null || _document == null || _namespaceManager == null)
            throw new ArgumentNullException(nameof(mapping), "XPathService.Select: arguments or document are null");

        var query = mapping.FieldQuery;
        var root = _document.DocumentElement;
        if (string.IsNullOrEmpty(query) || root == null)
            throw new NullReferenceException("XPathService.Select: query is empty or null, or the document root is null");

        if (_xPathCache.TryGetValue(mapping, out var node))
            return node;
        node = root.SelectSingleNode(query, _namespaceManager);
        if (node != null) _xPathCache.Add(mapping, node);
        return node;
    }

    public XmlNode RegisterElement(InvoiceField mapping)
    {
        if (_document == null || _namespaceManager == null)
            throw new NullReferenceException("XPathService.RegisterElement: no document loaded");

        var node = GetOrCreateNodeAtXPath(_document, mapping.FieldQuery)
            ?? throw new NullReferenceException($"XPathService.RegisterElement: query doesn't match");

        _xPathCache[mapping] = node;
        return node;
    }

    private XmlNode? GetOrCreateNodeAtXPath(XmlDocument doc, string query)
    {
        var node = doc.SelectSingleNode(query, _namespaceManager!);
        if (node != null)
            return node;

        var slashIndex = query.LastIndexOf('/');
        if (slashIndex == -1)
            return null;
        
        var newKey = query[..slashIndex];
        var newNodeName = query[(slashIndex + 1)..];
        var parentNode = GetOrCreateNodeAtXPath(doc, newKey);
        if (parentNode == null)
            return null;

        var childNode = doc.CreateElement(newNodeName, parentNode.NamespaceURI);

        parentNode.AppendChild(childNode);
        return childNode;
    }

    private XmlNamespaceManager GetNamespaceManager(XmlDocument document)
    {
        var ns = new XmlNamespaceManager(document.NameTable);
        _namespaces.ForEach((i, kv) => ns.AddNamespace(kv.Key, kv.Value));
        return ns;
    }
}
