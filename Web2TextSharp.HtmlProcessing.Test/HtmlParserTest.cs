namespace Web2TextSharp.HtmlProcessing.Test
{
    [TestClass]
    public class HtmlParserTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var fstream = new FileStream("1.html", FileMode.Open))
            {
                var root = HtmlParser.Parse(fstream, "//body");
                var en = root.EnumerateTextElements().ToList();
                ;
                //parser.Parse();
            }

        }

        [TestMethod]
        public void CollapseTest()
        {
            var root = HtmlParser.Parse(@"
<div>
    <a><span>anchor</span></a>
    some text
</div>
<div>
    <p><span><b>inside b</b>string</span></p>
</div>
            ");
            var en = root.EnumerateTextElements().ToList();
        }
    }
}