using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.DataClasses
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int PublishYear { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{Title}";
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public Author BookAuthor { get; set; }
    }
}
