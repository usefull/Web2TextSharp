using Web2TextSharp.HtmlProcessing.Entities;

namespace Web2TextSharp.HtmlProcessing.Test
{
    [TestClass]
    public class FeaturizerTest
    {
        /// <summary>
        /// Tests the combination of elements in pairs,
        /// taking into account the omission of empty and whitespace elements in the process of calculating features.
        /// </summary>
        [TestMethod]
        public void CombainInPairsTest()
        {
            var textElements = new List<CDOMElement>
            {
                new CDOMElement
                {
                    Name = "#text",
                    Text = "   "
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = " \r\n  "
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "1"
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "\r\n"
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "2"
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "           "
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "   \r\n        "
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "3"
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = "   "
                },
                new CDOMElement
                {
                    Name = "#text",
                    Text = " \r\n  "
                }
            };

            var result = textElements.Featurize();

            Assert.IsTrue(textElements.Where(e => e.Features != null).Select(e => e.Text).SequenceEqual(new[] { "1", "2", "3" }));
            Assert.IsTrue(result.SelectMany(pair => new[] { pair.Leading!.Text, pair.Closing!.Text }).SequenceEqual(new[] { "1", "2", "2", "3" }));
            
        }
    }
}