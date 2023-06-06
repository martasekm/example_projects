using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalProject.Exporters
{
    public abstract class Exporter
    {
        protected string FilePath { get; private set; }
        protected Exporter(string outFilePath)
        {
            FilePath = outFilePath;
        }
        public abstract Task ExportAuthorsAsync(IList<DataClasses.Author> authors, bool includeBooks = true);
        public abstract Task ExportAuthorsAsync(IList<DataClasses.Author> authors, Func<DataClasses.Author, bool> authorSelectorPredicate, bool includeBooks = true);
        public abstract Task ExportAuthorAsync(DataClasses.Author author, bool includeBooks = true);
        public abstract Task ExportAuthorAsync(DataClasses.Author author, Func<DataClasses.Book, bool> bookSelectorPredicate);
        public abstract Task ExportBookAsync(DataClasses.Book book, bool printAuthorName);
        protected abstract Task ExportAsync(string DataToExport);
    }
}
