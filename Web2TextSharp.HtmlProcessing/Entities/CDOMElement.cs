using HtmlAgilityPack;

namespace Web2TextSharp.HtmlProcessing.Entities
{
    /// <summary>
    /// CDOM element.
    /// </summary>
    public class CDOMElement
    {
        /// <summary>
        /// Element type.
        /// </summary>
        public CDOMElementType Type { get; set; }

        /// <summary>
        /// Reference to the parent element.
        /// </summary>
        public CDOMElement? Parent { get; set; }

        /// <summary>
        /// Collection of child elements.
        /// </summary>
        public ICollection<CDOMElement>? Children { get; set; }

        /// <summary>
        /// Element name
        /// </summary>
        /// <remarks>The same as the tag name of the corresponding HTML-element.</remarks>
        public string? Name { get; set; }

        /// <summary>
        /// Element name with classes if presented.
        /// </summary>
        /// <remarks>The same as the tag name of the corresponding HTML-element.</remarks>
        public string? NameWithClasses { get; set; }

        /// <summary>
        /// Reference to the source HTML-element.
        /// </summary>
        public HtmlNode? HtmlSrcNode { get; set; }

        /// <summary>
        /// Element text.
        /// </summary>
        /// <remarks>The same as the HTML-element inner text. Only valid for the text elements (<see cref="CDOMElementType.Text"/>).</remarks>
        public string? Text { get; set; }

        /// <summary>
        /// Element feature vector.
        /// </summary>
        public float[]? Features { get; set; }

        /// <summary>
        /// All child text elements enumeration.
        /// </summary>
        /// <returns>Text elements enumeration.</returns>
        public IEnumerable<CDOMElement> EnumerateTextElements()
        {
            if (Type == CDOMElementType.Text)
                yield return this;

            if (Children != null)
                foreach (var child in Children)
                {
                    var children = child.EnumerateTextElements();
                    foreach (var e in children)
                        yield return e;
                }
        }

        /// <summary>
        /// Gets the element classpath up to the root element.
        /// </summary>
        /// <returns>Element classpath.</returns>
        public string GetClassPath() => Parent == null
            ? NameWithClasses ?? string.Empty
            : $"{Parent.GetClassPath()}>{NameWithClasses}";
    }
}