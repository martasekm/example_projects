using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.DataClasses
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Description { get; set; }
        public List<Book> Books { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }

        public string FormattedDates
        {
            get
            {
                var resultText = $"*{DateOfBirth.Day}. {DateOfBirth.Month}. {DateOfBirth.Year}";
                if(DateOfDeath != null)
                {
                    resultText += $"\n✟{DateOfDeath.Value.Day}. {DateOfDeath.Value.Month}. {DateOfDeath.Value.Year}";
                }
                return resultText;
            }
        }
        public Author()
        {
            Books = new List<Book>();
        }

        public Author(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
            Books = new List<Book>();
        }

        public void SetAuthorAttributeForBooks()
        {
            foreach(var book in Books)
            {
                book.BookAuthor = this;
            }
        }
    }
}
