﻿using FinalProject.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig;

namespace FinalProject.Exporters
{
    internal class HtmlExporter : Exporter
    {
        const string CssSplendor = "@media print{*,:after,:before{background:0 0!important;color:#000!important;" +
            "box-shadow:none!important;text-shadow:none!important}a,a:visited{text-decoration:underline}a[href]:" +
            "after{content:\" (\" attr(href) \")\"}abbr[title]:after{content:\" (\" attr(title) \")\"}a[href^=\"#\"]:" +
            "after,a[href^=\"javascript:\"]:after{content:\"\"}blockquote,pre{border:1px solid #999;page-break-inside:avoid}" +
            "thead{display:table-header-group}img,tr{page-break-inside:avoid}img{max-width:100%!important}h2,h3," +
            "p{orphans:3;widows:3}h2,h3{page-break-after:avoid}}@media screen and (min-width:32rem) and (max-width:48rem)" +
            "{html{font-size:15px}}@media screen and (min-width:48rem){html{font-size:16px}}body{line-height:1.85}" +
            ".splendor-p,p{font-size:1rem;margin-bottom:1.3rem}.splendor-h1,.splendor-h2,.splendor-h3,.splendor-h4,h1,h2,h3,h4" +
            "{margin:1.414rem 0 .5rem;font-weight:inherit;line-height:1.42}.splendor-h1,h1{margin-top:0;font-size:3.998rem}" +
            ".splendor-h2,h2{font-size:2.827rem}.splendor-h3,h3{font-size:1.999rem}.splendor-h4,h4{font-size:1.414rem}" +
            ".splendor-h5,h5{font-size:1.121rem}.splendor-h6,h6{font-size:.88rem}.splendor-small,small{font-size:.707em}canvas," +
            "iframe,img,select,svg,textarea,video{max-width:100%}@import url(http://fonts.googleapis.com/css?family=Merriweather:300italic,300);" +
            "html{font-size:18px;max-width:100%}body{color:#444;font-family:Merriweather,Georgia,serif;margin:0;max-width:100%}:" +
            "not(div):not(img):not(body):not(html):not(li):not(blockquote):not(p),p{margin:1rem auto;max-width:36rem;padding:.25rem}" +
            "div,div img{width:100%}blockquote p{font-size:1.5rem;font-style:italic;margin:1rem auto;max-width:48rem}li{margin-left:2rem}" +
            "h1{padding:4rem 0!important}p{color:#555;height:auto;line-height:1.45}code,pre{font-family:Menlo," +
            "Monaco,\"Courier New\",monospace}pre{background-color:#fafafa;font-size:.8rem;overflow-x:scroll;padding:1.125em}a," +
            "a:visited{color:#3498db}a:active,a:focus,a:hover{color:#2980b9}";
        public HtmlExporter(string outFilePath) : base(outFilePath)
        {
        }

        private string ConvertMarkdownToHtml(string markdown, string cssStyle = CssSplendor)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<style>");
            sb.AppendLine(CssSplendor);
            sb.AppendLine("</style>");
            sb.AppendLine("<body>");
            sb.Append(Markdown.ToHtml(markdown));
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }

        public async override Task ExportAuthorAsync(Author author, bool includeBooks = true)
        {
            await ExportAuthorAsync(author, x => includeBooks);
        }

        public async override Task ExportAuthorAsync(Author author, Func<Book, bool> bookSelectorPredicate)
        {
            await ExportAsync(MarkdownConverter.ConvertAuthorToMarkdown(author, bookSelectorPredicate));
        }

        public override Task ExportAuthorsAsync(IList<Author> authors, bool includeBooks = true)
        {
            throw new NotImplementedException();
        }

        public override Task ExportAuthorsAsync(IList<Author> authors, Func<Author, bool> authorSelectorPredicate, bool includeBooks = true)
        {
            throw new NotImplementedException();
        }

        public async override Task ExportBookAsync(Book book, bool printAuthorName)
        {
            await ExportAsync(MarkdownConverter.ConvertBookToMarkdown(book, printAuthorName));
        }

        protected async override Task ExportAsync(string markdown)
        {
            var convertedHtml = ConvertMarkdownToHtml(markdown);
            await File.WriteAllTextAsync(FilePath, convertedHtml);
        }
    }
}
