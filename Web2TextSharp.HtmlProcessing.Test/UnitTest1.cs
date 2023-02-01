namespace Web2TextSharp.HtmlProcessing.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var parser = new HtmlParser();
            parser.Parse("<div><p></p></div>");
            //parser.Parse();
        }
    }
}