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
                var parser = new HtmlParser();
                var root = parser.Parse(fstream);
                ;
                //parser.Parse();
            }

        }
    }
}