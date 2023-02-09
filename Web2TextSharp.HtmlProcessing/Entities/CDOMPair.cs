namespace Web2TextSharp.HtmlProcessing.Entities
{
    /// <summary>
    /// A pair of neighboring elements.
    /// </summary>
    public class CDOMPair
    {
        /// <summary>
        /// The leading element.
        /// </summary>
        public CDOMElement? Leading { get; set; }

        /// <summary>
        /// The closing element.
        /// </summary>
        public CDOMElement? Closing { get; set; }

        /// <summary>
        /// The feature vector of this pair.
        /// </summary>
        public float[]? Features;
    }
}