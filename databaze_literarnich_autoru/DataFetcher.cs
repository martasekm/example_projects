using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject
{
    public abstract class DataFetcher
    {
        public DataFetcher()
        {
            authors = null;
        }
        internal List<DataClasses.Author>? authors;
        public abstract Task<List<DataClasses.Author>> GetAuthors();
        internal abstract Task<List<DataClasses.Author>> FetchAuthors();
        public abstract bool EditAuthor(DataClasses.Author oldAuthor, DataClasses.Author editedAuthor);
        public abstract bool RemoveAuthor(DataClasses.Author author);
    }
}
