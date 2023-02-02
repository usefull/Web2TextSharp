namespace Web2TextSharp.HtmlProcessing.Test
{
    [TestClass]
    public class UnitTest1
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
    }
}