using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Web2TextSharp.HtmlProcessing.Entities;
using Web2TextSharp.HtmlProcessing.Exceptions;
using Web2TextSharp.HtmlProcessing.Resources;

namespace Web2TextSharp.HtmlProcessing
{
    /// <summary>
    /// Functionality for parsing HTML in CDOM.
    /// </summary>
    public static partial class HtmlParser
    {
        /// <summary>
        /// Parses HTML to CDOM.
        /// </summary>
        /// <param name="stream">Stream represents source HTML.</param>
        /// <param name="rootXPath">XPath selector for root HTML element. If null the document root is used.</param>
        /// <returns>Root CDOM element.</returns>
        /// <exception cref="NodeNotFoundException">In case the root element is not found.</exception>
        /// <exception cref="MoreThanOneRootException">In case multiple root elements are found.</exception>
        public static CDOMElement Parse(Stream stream, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(stream);
            return Parse(doc, rootXPath);
        }

        /// <summary>
        /// Parses HTML to CDOM.
        /// </summary>
        /// <param name="stream">Stream represents source HTML.</param>
        /// <param name="encoding">Source document encoding.</param>
        /// <param name="rootXPath">XPath selector for root HTML element. If null the document root is used.</param>
        /// <returns>Root CDOM element.</returns>
        /// <exception cref="NodeNotFoundException">In case the root element is not found.</exception>
        /// <exception cref="MoreThanOneRootException">In case multiple root elements are found.</exception>
        public static CDOMElement Parse(Stream stream, Encoding encoding, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(stream, encoding);
            return Parse(doc, rootXPath);
        }

        /// <summary>
        /// Parses HTML to CDOM.
        /// </summary>
        /// <param name="html">String represents source HTML.</param>
        /// <param name="rootXPath">XPath selector for root HTML element. If null the document root is used.</param>
        /// <returns>Root CDOM element.</returns>
        /// <exception cref="NodeNotFoundException">In case the root element is not found.</exception>
        /// <exception cref="MoreThanOneRootException">In case multiple root elements are found.</exception>
        public static CDOMElement Parse(string html, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            return Parse(doc, rootXPath);
        }

        /// <summary>
        /// Parses HTML to CDOM.
        /// </summary>
        /// <param name="html">Text reader object represents source HTML.</param>
        /// <param name="rootXPath">XPath selector for root HTML element. If null the document root is used.</param>
        /// <returns>Root CDOM element.</returns>
        /// <exception cref="NodeNotFoundException">In case the root element is not found.</exception>
        /// <exception cref="MoreThanOneRootException">In case multiple root elements are found.</exception>
        public static CDOMElement Parse(TextReader html, string? rootXPath = null)
        {
            var doc = new HtmlDocument();
            doc.Load(html);
            return Parse(doc, rootXPath);
        }

        /// <summary>
        /// Parses HTML to CDOM.
        /// </summary>
        /// <param name="doc">HtmlAgilityPack document represents source HTML.</param>
        /// <param name="rootXPath">XPath selector for root HTML element. If null the document root is used.</param>
        /// <returns>Root CDOM element.</returns>
        /// <exception cref="NodeNotFoundException">In case the root element is not found.</exception>
        /// <exception cref="MoreThanOneRootException">In case multiple root elements are found.</exception>
        private static CDOMElement Parse(HtmlDocument doc, string? rootXPath)
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

            return CollapseIfSingleChild(root);
        }

        /// <summary>
        /// Expands all child nodes into a CDOM structure.
        /// </summary>
        /// <param name="element">Initial element.</param>
        private static void ExpandElement(CDOMElement element)
        {
            foreach (var childSrc in element.HtmlSrcNode!.ChildNodes)
            {
                if (_tagsToIgnore.Contains(childSrc.Name))
                    continue;

                if (childSrc.Name == "br")
                    element.Children?.Add(CreateTextElement("\r\n", childSrc, element));
                else if (childSrc.Name == "#text")
                {
                    var nodeText = _regexLikeLinebreak().Replace(HttpUtility.HtmlDecode(childSrc.InnerText), "\r\n");
                    if (_regexSpaces().Replace(nodeText, string.Empty) != string.Empty)
                        element.Children?.Add(CreateTextElement(nodeText, childSrc, element));
                }
                else
                {
                    var nextNode = CreateNodeElement(childSrc, element);
                    ExpandElement(nextNode);
                    if (nextNode.Children?.Any() == true)
                        element.Children?.Add(nextNode);
                }
            }
        }

        /// <summary>
        /// Collapses all child nodes that have only one child.
        /// </summary>
        /// <param name="e">Initial element.</param>
        /// <returns>Result element.</returns>
        private static CDOMElement CollapseIfSingleChild(CDOMElement e)
        {
            var element = e;

            while ((element.Children?.Count ?? 0) == 1)
            {
                var parent = element.Parent;
                var child = element.Children?.FirstOrDefault();

                if (parent != null)
                {
                    parent?.Children?.Remove(element);
                    element.Parent = null;
                }

                element.Children?.Remove(child!);
                parent?.Children?.Add(child!);
                child!.Parent = parent;
                child.Name = $"{element.Name}/{child.Name}";

                element = child;
            }

            element.Children?.ToList()?.ForEach(child => CollapseIfSingleChild(child));

            return element;
        }

        /// <summary>
        /// Creates node element.
        /// </summary>
        /// <param name="htmlSrcNode">Reference to the source HTML-element.</param>
        /// <param name="parent">Reference to the parent element.</param>
        /// <returns>Result element.</returns>
        private static CDOMElement CreateNodeElement(HtmlNode htmlSrcNode, CDOMElement? parent = null) => new()
        {
            Type = CDOMElementType.Node,
            Name = htmlSrcNode.Name,
            HtmlSrcNode = htmlSrcNode,
            Parent = parent,
            Children = new List<CDOMElement>()
        };

        /// <summary>
        /// Creates text element.
        /// </summary>
        /// <param name="text">Element text.</param>
        /// <param name="htmlSrcNode">Reference to the source HTML-element.</param>
        /// <param name="parent">Reference to the parent element.</param>
        /// <returns>Result element.</returns>
        private static CDOMElement CreateTextElement(string text, HtmlNode htmlSrcNode, CDOMElement parent) => new()
        {
            Type = CDOMElementType.Text,
            Name = htmlSrcNode.Name,
            HtmlSrcNode = htmlSrcNode,
            Parent = parent,
            Text = text
        };

        /// <summary>
        /// Tags to be ignored during parsing.
        /// </summary>
        private static readonly string[] _tagsToIgnore = { "area", "base", "col", "colgroup", "embed", "hr", "iframe", "img", "input", "link", "meta", "source", "track", "wbr" };

        /// <summary>
        /// Regexp for various invisible or space symbols.
        /// </summary>
        [GeneratedRegex("[\\p{Z}\\p{Zs}\\p{Cf}\\p{Co}\\p{Cs}\\p{Cn}]", RegexOptions.Compiled)]
        private static partial Regex _regexSpaces();

        /// <summary>
        /// Regexp for replacing various line separators with "\r\n".
        /// </summary>
        [GeneratedRegex("[\\p{Zl}\\p{Zp}]", RegexOptions.Compiled)]
        private static partial Regex _regexLikeLinebreak();
    }
}