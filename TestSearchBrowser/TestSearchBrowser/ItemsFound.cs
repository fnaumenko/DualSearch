using System;

namespace TestSearchBrowser
{
    /// <summary>The class ensures the return of the next item found in common search html results.</summary>
    class ItemsFound
    {
        /// <summary>Current search position in responce</summary>
        private int startInd;
        /// <summary>Common search html results</summary>
        private string responce;
        /// <summary>Opening item found tag</summary>
        private string openTag;
        /// <summary>Closing item found tag</summary>
        private string closeTag;

        /// <summary>Constructor by search responce and item tags</summary>
        /// <param name="resp">Common search responce</param>
        /// <param name="itemTags">Item found open/close tags</param>
        public ItemsFound(string resp, string[] itemTags)
        {
            startInd = 0;
            responce = resp;
            openTag = itemTags[0];
            closeTag = itemTags[1];
        }

        /// <summary>Gets the next html item found</summary>
        public string Next()
        {
            string res = string.Empty;
            if (startInd >= 0)
            {
                startInd = responce.IndexOf(openTag, startInd + 1);
                if (startInd > 0)
                {
                    int endInd = responce.IndexOf(openTag, startInd + 1);
                    if (endInd < 0)
                        endInd = responce.IndexOf(closeTag, startInd + 1);
                    res = responce.Substring(startInd, endInd - startInd);
                }
            }
            return res;
        }
    }
}
