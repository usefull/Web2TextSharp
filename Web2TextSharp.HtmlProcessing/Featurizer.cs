using Web2TextSharp.HtmlProcessing.Entities;

namespace Web2TextSharp.HtmlProcessing
{
    /// <summary>
    /// Functionality for calculating text element features.
    /// </summary>
    public static class Featurizer
    {
        /// <summary>
        /// Calculates features of a text elements sequence.
        /// </summary>
        /// <param name="textElements">Text elements sequence.</param>
        /// <returns>A collection of a pairwise feature vectors.</returns>
        public static List<CDOMPair> Featurize(this IEnumerable<CDOMElement> textElements)
        {
            List<CDOMPair> result = new();
            CDOMElement? currentLeading = null;

            foreach (var element in textElements)
            {
                element.Featurize(textElements);

                if (currentLeading != null && element.Features != null)
                {
                    var pair = new CDOMPair
                    {
                        Leading = currentLeading,
                        Closing = element
                    };
                    pair.Featurize(textElements);
                    result.Add(pair);
                }

                if (element.Features != null)
                    currentLeading = element;
            }

            return result;
        }

        /// <summary>
        /// Calculates features of a single text element.
        /// </summary>
        /// <param name="element">Text element.</param>
        /// <param name="textElements">Set of text elements.</param>
        private static void Featurize(this CDOMElement element, IEnumerable<CDOMElement> textElements)
        {
            if (string.IsNullOrWhiteSpace(element.Text))
                element.Features = null;
            else
            {
                //todo: Need to be implemented. The code inside the comment block just simulates featurizing.
                element.Features = Array.Empty<float>();
                /////////////////////////////////////////////////////////////////////////////////////////////
            }
        }

        /// <summary>
        /// Calculates features of a neighboring elements pair.
        /// </summary>
        /// <param name="pair">A neighboring elements pair.</param>
        /// <param name="textElements">Set of text elements.</param>
        /// <exception cref="ArgumentNullException">In case of a pair object is null.</exception>
        /// <exception cref="ArgumentException">In case of a pair consisis of invalid elements.</exception>
        private static void Featurize(this CDOMPair pair, IEnumerable<CDOMElement> textElements)
        {
            if (pair == null)
                throw new ArgumentNullException(nameof(pair));

            if (pair.Leading == null || pair.Closing == null)
                throw new ArgumentException(Resources.Errors.NullElementOfPair, nameof(pair));

            if (pair.Leading.Features == null || pair.Closing.Features == null)
                throw new ArgumentException(Resources.Errors.NonSignificantElementOfPair, nameof(pair));

            //todo: Need to be implemented. The code inside the comment block just simulates featurizing.
            pair.Features = Array.Empty<float>();
            /////////////////////////////////////////////////////////////////////////////////////////////
        }
    }
}