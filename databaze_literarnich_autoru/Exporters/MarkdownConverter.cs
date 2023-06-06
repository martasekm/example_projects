using FinalProject.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Exporters
{
    internal static class MarkdownConverter
    {
        private static string GetHeadingSharps(int count)
        {
            return new string('#', count);
        }
        public static string ConvertBookToMarkdown(Book book, bool printAuthorName, int topHeadingLevel = 1)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine($"{GetHeadingSharps(topHeadingLevel)} {book.Title} ({book.PublishYear})");
            if(printAuthorName && book.BookAuthor != null)
            {
                string end = book.BookAuthor.DateOfDeath.HasValue ? $"-{book.BookAuthor.DateOfDeath.Value.Year})" : ")";
                result.AppendLine($"{GetHeadingSharps(topHeadingLevel + 1)} {book.BookAuthor.FullName} ({book.BookAuthor.DateOfBirth.Year}{end}");
            }
            result.AppendLine(book.Description);
            return result.ToString();
        }

        public static string ConvertAuthorToMarkdown(Author author, Func<Book, bool> bookSelectorPredicate, int topHeadingLevel = 1)
        {
            StringBuilder result = new StringBuilder();
            result.AppendLine($"{GetHeadingSharps(topHeadingLevel)} {author.FullName}");
            result.AppendLine($"{GetHeadingSharps(topHeadingLevel + 2)} Základní informace");
            result.AppendLine($"* narozen {author.DateOfBirth.ToShortDateString()}");
            if (author.DateOfDeath != null)
            {
                result.AppendLine($"* zemřel {author.DateOfDeath.Value.ToShortDateString()}");
            }
            result.AppendLine($"\n{GetHeadingSharps(topHeadingLevel + 2)} Život");
            result.AppendLine(author.Description);
            var selectedBooks = author.Books.Where(bookSelectorPredicate).ToList();
            if (selectedBooks.Count == 0)
            {
                return result.ToString();
            }
            result.AppendLine($"\n{GetHeadingSharps(topHeadingLevel + 2)} Dílo");
            foreach (var book in selectedBooks)
            {
                result.AppendLine(ConvertBookToMarkdown(book, false, topHeadingLevel: topHeadingLevel + 4));
            }
            return result.ToString();
        }
    }
}
