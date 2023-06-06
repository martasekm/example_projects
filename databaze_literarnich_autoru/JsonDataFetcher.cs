using FinalProject.DataClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;
namespace FinalProject
{
    internal class JsonDataFetcher : DataFetcher
    {
        internal string JsonFilePath { get; private set; }
        public JsonDataFetcher(string filePath)
        {
            JsonFilePath = filePath;

        }
        public override bool EditAuthor(Author oldAuthor, Author editedAuthor)
        {
            throw new NotImplementedException();
        }

        internal async override Task<List<DataClasses.Author>> FetchAuthors()
        {
            List<Author>? result;
            var file = await File.ReadAllTextAsync(JsonFilePath);
            result = await Task.Run(() => JsonConvert.DeserializeObject<List<Author>>(file));
            Debug.Assert(result != null);
            return result;
        }

        public override async Task<List<Author>> GetAuthors()
        {
            if (authors == null)
            {
                authors = await FetchAuthors();
            }
            return authors;
        }

        public override bool RemoveAuthor(Author author)
        {
            throw new NotImplementedException();
        }
    }
}
