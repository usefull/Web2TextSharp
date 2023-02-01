using Web2TextSharp.HtmlProcessing.Resources;

namespace Web2TextSharp.HtmlProcessing.Exceptions
{
    public class MoreThanOneRootException : Exception
    {
        public MoreThanOneRootException() : base(Errors.MoreThanOneRootFound)
        {
        }

        public MoreThanOneRootException(string? message) : base(message)
        {
        }
    }
}