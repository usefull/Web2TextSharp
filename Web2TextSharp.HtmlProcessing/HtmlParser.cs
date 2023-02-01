using HtmlAgilityPack;
using System.Text;
using Web2TextSharp.HtmlProcessing.Entities;
using Web2TextSharp.HtmlProcessing.Exceptions;
using Web2TextSharp.HtmlProcessing.Resources;

namespace Web2TextSharp.HtmlProcessing
{
    public class HtmlParser
    {
        public CDOMElement Parse(Stream stream, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);
            return Parse(doc, rootXPath);
        }

        public CDOMElement Parse(Stream stream, Encoding encoding, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(stream, encoding);
            return Parse(doc, rootXPath);
        }

        public CDOMElement Parse(string html, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return Parse(doc, rootXPath);
        }

        public CDOMElement Parse(TextReader html, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(html);
            return Parse(doc, rootXPath);
        }

        private CDOMElement Parse(HtmlDocument doc, string? rootXPath)
        {
            HtmlNode rootSrc;
            if (string.IsNullOrWhiteSpace(rootXPath))
                rootSrc = doc.DocumentNode;
            else
            {
                var rootCandidates = doc.DocumentNode.SelectNodes(rootXPath);

                if (rootCandidates == null || !rootCandidates.Any())
                    throw new NodeNotFoundException(Errors.XPathNotFound);

                if (rootCandidates.Count > 1)
                    throw new MoreThanOneRootException();

                rootSrc = rootCandidates[0];
            }

            var root = CreateNodeElement(rootSrc);

            ExpandElement(root);

            return root;
        }

        private void ExpandElement(CDOMElement element)
        {
            foreach (var childSrc in element.HtmlSrcNode!.ChildNodes)
            {
                if (_tagsToIgnore.Contains(childSrc.Name))
                    continue;

                if (childSrc.Name == "br")
                    element.Children!.Add(CreateLeafElement(CDOMElementType.LineBreak, childSrc, element));
                else if (childSrc.Name == "#text")
                {
                    var str = childSrc.InnerText.Replace(" ", string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
                    element.Children!.Add(CreateLeafElement(str == string.Empty ? CDOMElementType.LineBreak : CDOMElementType.Text, childSrc, element));
                }
                else
                {
                    var nextNode = CreateNodeElement(childSrc, element);
                    ExpandElement(nextNode);
                    if (nextNode.Children!.Any())
                        element.Children!.Add(nextNode);
                }
            }
        }

        private static CDOMElement CreateNodeElement(HtmlNode htmlSrcNode, CDOMElement? parent = null) => new()
        {
            Type = CDOMElementType.Node,
            Name = htmlSrcNode.Name,
            HtmlSrcNode = htmlSrcNode,
            Parent = parent,
            Children = new List<CDOMElement>()
        };

        private static CDOMElement CreateLeafElement(CDOMElementType type, HtmlNode htmlSrcNode, CDOMElement parent) => new()
        {
            Type = type,
            Name = htmlSrcNode.Name,
            HtmlSrcNode = htmlSrcNode,
            Parent = parent,
            Text = htmlSrcNode.InnerText
        };

        private static string[] _tagsToIgnore = { "area", "base", "col", "colgroup", "embed", "hr", "iframe", "img", "input", "link", "meta", "source", "track", "wbr" };
    }
}