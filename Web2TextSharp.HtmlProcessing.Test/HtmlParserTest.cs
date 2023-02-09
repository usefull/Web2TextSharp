namespace Web2TextSharp.HtmlProcessing.Test
{
    [TestClass]
    public class HtmlParserTest
    {
        /// <summary>
        /// Tests the preservation of the order of child elements after parsing.
        /// </summary>
        [TestMethod]
        public void ChildrenOrderPreservationTest()
        {
            var root = HtmlParser.Parse("<div>1<a>2</a>3<span>4</span></div>");

            var expected = Enumerable.Range(1, 4).Select(i => i.ToString());
            var actual = root.Children!.Select(i => i.Text);

            Assert.IsTrue(expected.SequenceEqual(actual));

            Assert.IsFalse(actual.SequenceEqual(Enumerable.Reverse(expected)));
        }

        /// <summary>
        /// Tests the collapsing single child nodes.
        /// </summary>
        [TestMethod]
        public void CollapseTest()
        {
            var root = HtmlParser.Parse("<div><p><span><a>1</a></span><span><a>2</a></span></p><div><div><p><span><a>3</a><a>4</a></span></p></div></div></div>");

            Assert.AreEqual("div/div/p/span", root.Children!.Skip(1).First().Name);

            Assert.AreEqual("span/a/#text", root.EnumerateTextElements().First(n => n.Text == "1").Name);
            Assert.AreEqual("span/a/#text", root.EnumerateTextElements().First(n => n.Text == "2").Name);
            Assert.AreEqual("a/#text", root.EnumerateTextElements().First(n => n.Text == "3").Name);
            Assert.AreEqual("a/#text", root.EnumerateTextElements().First(n => n.Text == "4").Name);
        }

        /// <summary>
        /// Tests the preservation of elements that has linebreaks after parsing.
        /// </summary>
        [TestMethod]
        public void LinebreakElementsPreservationTest()
        {
            var root = HtmlParser.Parse(@"<div>
<span>1</span>     
<span>2</span>   
   <span>3</span></div>");

            Assert.IsTrue(root.Children!.First().Text!.Contains("\r\n"));
            Assert.IsTrue(root.Children!.Skip(2).First().Text!.Contains("\r\n"));
            Assert.IsTrue(root.Children!.Skip(4).First().Text!.Contains("\r\n"));
        }

        /// <summary>
        /// Tests the ignoring empty elements.
        /// </summary>
        [TestMethod]
        public void IgnoringEmptyElementsTest()
        {
            var root = HtmlParser.Parse(@"<div>    <span>1</span><span>   </span><span><a></a></span></div>");
            Assert.IsTrue(root.Children == null || !root.Children.Any());
        }

        /// <summary>
        /// Tests classpath.
        /// </summary>
        [TestMethod]
        public void ClassPathTest()
        {
            var root = HtmlParser.Parse(@"<div class=""c4""><div class=""c1""><p class=""c2""><span class=""c3"">1</span></p></div><div class=""www rrr aaa"">2</div></div>");

            var node = root.EnumerateTextElements().First(i => i.Text == "1");
            Assert.AreEqual("div.c1>p.c2>span.c3>#text", node.NameWithClasses);
            Assert.AreEqual("#document>div.c4>div.c1>p.c2>span.c3>#text", node.GetClassPath());

            node = root.EnumerateTextElements().First(i => i.Text == "2");
            Assert.AreEqual("div.aaa.rrr.www>#text", node.NameWithClasses);
        }
    }
}