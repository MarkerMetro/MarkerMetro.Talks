using System.Text.RegularExpressions;
using Windows.ApplicationModel.Background;
using Windows.Media.Devices;
using Windows.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Http;

namespace MetroTalks.Win81.PetesEats.Services
{
    public class PetesEatsService
    {
        public async Task<IEnumerable<Eat>> GetRecentEats()
        {
            var uri = CreateUri("posts/");
            return await FetchPostsFeed(uri);
        }

        private async Task<IEnumerable<Eat>> FetchPostsFeed(Uri uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri);
            var responseContent = await response.Content.ReadAsStringAsync();
            var responseObject = JsonConvert.DeserializeObject<GetRecentEatsResponse>(responseContent);
            return responseObject.response.Posts.Select(CreateEat).Where(r => r != null);
        }

        readonly Regex _htmlTags = new Regex(@"<(?<tag>\w*)>(?<text>.*)</\k<tag>>");
        private IEnumerable<Eat> _allEats;

        private Eat CreateEat(Post post)
        {

            try
            {
                var matches = _htmlTags.Matches(post.Caption);

                var name = matches.OfType<Match>().Select(m => StripTags(m.Value)).First();
                var description = matches.OfType<Match>().Select(m => StripTags(m.Value)).ElementAt(1);

                return new Eat
                {
                    Id = post.Id,
                    Name = name,
                    Description = description,
                    OriginalImageUri = post.Image_permalink,
                    Image500 = post.Photos.First().Alt_sizes.Where(p => p.Width <= 500).OrderByDescending(p => p.Width).FirstOrDefault(),
                    ThumbnailImage = post.Photos.First().Alt_sizes.OrderBy(p => p.Width).FirstOrDefault()
                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        private static string StripTags(string value)
        {
            return System.Net.WebUtility.HtmlDecode(
             value
                .Replace("<b>", string.Empty)
                .Replace("</b>", string.Empty)
                .Replace("<p>", string.Empty)
                .Replace("</p>", string.Empty)
                .Replace("<strong>", string.Empty)
                .Replace("</strong>", string.Empty).Trim());
        }


        public Uri CreateUri(string resource, Dictionary<string, string> parameters = null) {
            parameters = parameters ?? new Dictionary<string, string>();
            string parameterString = string.Join("&", parameters.Select(p => p.Key + "=" + p.Value));
            if (!string.IsNullOrEmpty(parameterString))
            {
                parameterString = "&" + parameterString;
            }
            return new Uri(string.Format("http://api.tumblr.com/v2/blog/thisiswhyyourefat.tumblr.com/{0}?api_key=50J7l6DBuClqrxs4lLEzsx91psiy2As1BOzyTxL3SJBkFFaDS2{1}", resource, parameterString));
        }

        public async Task<Eat> GetPickOfTheWeek()
        {
            //await DownloadAllEats();
            // Hard coded pick of the week
            int pick = 370780954;
            var eat = (await FetchPostsFeed(CreateUri("posts/", new Dictionary<string, string> {{"id", pick.ToString()}}))).First();


            eat.ReviewNotes =
                "This weeks awesome sauce award goes to the inventive soul that pulled together two different types of pig meat and shoved them into a chocolate bar for everyone's enjoyment. Thank you sir, humanity is in your debt.";
            return eat;
        }

        public async Task<IEnumerable<Eat>> SearchEats(string searchTerm)
        {
            // HACK: Use a local file as Tumblr API doesn't support search in captions
            if (_allEats == null)
            {
                // Should really handle locking here but...
                var file =
                    await
                        StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Data/allPosts.json",
                            UriKind.Absolute));
                var contents = await FileIO.ReadTextAsync(file);
                _allEats = await JsonConvert.DeserializeObjectAsync<IEnumerable<Eat>>(contents);
            }

            return _allEats.Where(e =>
                e.Name.ToLower().Contains(searchTerm.ToLower())
                || e.Description.ToLower().Contains(searchTerm.ToLower())).ToArray();
        }

        public async Task DownloadAllEats()
        {
            int offset = 0;
            var allEats = new List<Eat>();
            while (true)
            {
                var eats =
                    await FetchPostsFeed(CreateUri("posts/", new Dictionary<string, string>() {{"offset", offset.ToString()}}));

                allEats.AddRange(eats);
                offset = offset + 20;
                if (eats.Count() < 20)
                {
                    break;
                }
            }

            string json = JsonConvert.SerializeObject(allEats);
            
        }

        public async Task<Eat> GetEat(int id)
        {
            return (await FetchPostsFeed(CreateUri("posts/", new Dictionary<string, string>() {{"id", id.ToString()}}))).FirstOrDefault();
        }
    }

    

    public class Eat
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string OriginalImageUri { get; set; }
        public EatPhoto Image500 { get; set; }
        public EatPhoto ThumbnailImage { get; set; }
        public string ReviewNotes { get; set; }
        public int Id { get; set; }
    }

    public class EatPost
    {

    }
    
public class GetRecentEatsResponse
{
public Meta meta { get; set; }
public Response response { get; set; }
}

public class Meta
{
public int Status { get; set; }
public string Msg { get; set; }
}

public class Response
{
public Blog Blog { get; set; }
public Post[] Posts { get; set; }
public int Total_posts { get; set; }
}

public class Blog
{
public string Title { get; set; }
public string Name { get; set; }
public int Posts { get; set; }
public string Url { get; set; }
public int Updated { get; set; }
public string Description { get; set; }
public bool Ask { get; set; }
public bool AskAnon { get; set; }
public bool Is_nsfw { get; set; }
public bool Share_likes { get; set; }
}

public class Post
{
public string Blog_name { get; set; }
public int Id { get; set; }
public string Post_url { get; set; }
public string Slug { get; set; }
public string Type { get; set; }
public string Date { get; set; }
public int Timestamp { get; set; }
public string State { get; set; }
public string Format { get; set; }
public string Reblog_key { get; set; }
public object[] Tags { get; set; }
public string Short_url { get; set; }
public object[] Highlighted { get; set; }
public int Note_count { get; set; }
public string Caption { get; set; }
public string Image_permalink { get; set; }
public Photo[] Photos { get; set; }
public string Link_url { get; set; }
}

public class Photo
{
public string Caption { get; set; }
public EatPhoto[] Alt_sizes { get; set; }
public Original_Size Original_size { get; set; }
public Exif Exif { get; set; }
}

public class Original_Size
{
public int Width { get; set; }
public int Height { get; set; }
public string Url { get; set; }
}

public class Exif
{
public string Camera { get; set; }
public int ISO { get; set; }
public string Aperture { get; set; }
public string Exposure { get; set; }
public string FocalLength { get; set; }
}

public class EatPhoto
{
public int Width { get; set; }
public int Height { get; set; }
public string Url { get; set; }
}

}
