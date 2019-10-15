using System;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace TestSearchBrowser
{
    public partial class Form1 : Form
    {
        /// <summary>Search engine: name, open/close tags for the found item</summary>
        private struct SeachEngine
        {
            /// <summary>Search engine name</summary>
            public readonly string Name;
            /// <summary>Query string without the query itself</summary>
            public readonly string Inquiry;
            /// <summary>Opening/closing item tags</summary>
            public readonly string[] ItemTags;
            /// <summary>Method for additional processing of item tags</summary>
            public readonly HtmlAdjustMethodDlg Method;

            public SeachEngine(string name, string openTag, string closeTag, HtmlAdjustMethodDlg method)
            {
                Name = name;
                Inquiry = "http://" + name + ".com/search?q=";
                ItemTags = new string[]{ openTag, closeTag };
                Method = method;
            }
        }

        /// <summary>Partition method delegate</summary>
        /// <param name="htmlItem">count od subsets</param>
        delegate string HtmlAdjustMethodDlg(string htmlItem);

        private static readonly SeachEngine[] Engines = new SeachEngine[] {
            new SeachEngine("BING", "<li class=\"b_algo\">", "</li>", AdjustItemBing),
            new SeachEngine("GOOGLE", DivTagOpen + "<div class=\"ZINbbc",
                //string.Concat(System.Linq.Enumerable.Repeat(DivTagClose, 4 /*8*/)),
                "<footer>",
                AdjustItemGoogle)
        };

        private const string TitleHead = "h2";
        private const string TitleHeadTagOpen = "<" + ItemHead + ">";
        private const string TitleHeadTagClose = "</" + ItemHead + ">";
        private const string ItemHead = "h3";
        private const string ItemHeadTagOpen = "<" + ItemHead + ">";
        private const string ItemHeadTagClose = "</" + ItemHead + ">";
        private const string DivTagOpen = "<div>";
        private const string DivTagClose = "</div>";

        private string SearchResult;

        #region Loading and button reaction
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ActiveControl = txtBoxQuery;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtBoxQuery.Text.Trim();
            if (query == string.Empty) return;
            DisplayHtml(Search(query));
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if( !webBrowser1.GoBack() )
                DisplayHtml(SearchResult);
        }
        #endregion

        /// <summary>Display custom HTML in WebBrowser control</summary>
        /// <param name="html">custom HTML text</param>
        private void DisplayHtml(string html)
        {
            if (html == string.Empty) return;

            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("about:blank");            // first navigate to page existed locally for sure
            if (webBrowser1.Document != null)
                webBrowser1.Document.Write(string.Empty);   // write something
            webBrowser1.DocumentText = html;
        }

        /// <summary>Reformat and combined 2 search engine results, leaving minimal format and items found.</summary>
        /// <param name="query">User search query</param>
        /// <returns>Reformatted search engines results.</returns>
        private string Search(string query)
        {
            string res = string.Empty;
            using (WebClient client = new WebClient())
            {
                string response, htmlItem;
                StringBuilder ress = new StringBuilder(
                    "<!DOCTYPE html><html lang=\"en\" xml:lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\" xmlns:Web=\"http://schemas.live.com/Web/\">",
                    query.Length);
                ress.Append("<ol>");
                for (int i = 0; i < Engines.Length; i++)
                {
                    ress.Append(TitleHeadTagOpen + DivTagOpen + Engines[i].Name + DivTagClose + TitleHeadTagClose);
                    response = client.DownloadString(Engines[i].Inquiry + query);  // main job
                    ItemsFound items = new ItemsFound(response, Engines[i].ItemTags);
                    while ((htmlItem = items.Next()) != string.Empty)
                        ress.Append(Engines[i].Method(htmlItem));
                }
                ress.Append("</ol>");
                ress.Append("</html>");
                SearchResult = res = ress.ToString();
            }
            return res;
        }

        /// <summary>Adjust tags of item found by Google to uniform state</summary>
        /// <param name="htmlItem">html string contained item found</param>
        /// <returns>html string contained item with uniformed tags</returns>
        static private string AdjustItemGoogle(string htmlItem)
        {
            int start, end, tailInd = htmlItem.Length - 2 * DivTagClose.Length;
            int startShift = 2 * DivTagOpen.Length;     // remove one enclosing <div> at all
            StringBuilder res = new StringBuilder("<br><li ", htmlItem.Length);
            
            start = htmlItem.IndexOf("<div class=\"BNeawe");    // item found
            end = htmlItem.IndexOf(DivTagClose, start);
            res.Append(htmlItem.Substring(startShift, start - startShift));
            res.Append(ItemHeadTagOpen);                        // add head tag
            res.Append(htmlItem.Substring(start, end - start)); // add item title
            res.Append(ItemHeadTagClose);                       // close head tag
            res.Append(htmlItem.Substring(end, tailInd - end)); // add item body
            res.Append("</li>");
            return res.ToString();
        }

        /// <summary>Adjust tags of item found by Bing to uniform state</summary>
        /// <param name="htmlItem">html string contained item found</param>
        /// <returns>html string contained item with uniformed tags</returns>
        static private string AdjustItemBing(string htmlItem)
        {
            return htmlItem.Replace(TitleHead, ItemHead);
        }
     }
}
