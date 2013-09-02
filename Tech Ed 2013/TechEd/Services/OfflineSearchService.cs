using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TechEd.Services
{
    public class OfflineSearchService : ISearchService
    {
        private readonly List<SearchResult> cachedResults = new List<SearchResult>();

        public async Task<IList<SearchResult>> SearchAsync(string term)
        {
            if (cachedResults.Any())
                return cachedResults;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///resources/data/searchresults.json"));
            var json = await FileIO.ReadTextAsync(file);

            var results = JsonConvert.DeserializeObject<IList<SearchResult>>(json);

            cachedResults.AddRange(results);

            return cachedResults;
        }

        public SearchResult GetResult(string itemLink)
        {
            return cachedResults.FirstOrDefault(r => r.ItemLink == itemLink);
        }
    }
}
