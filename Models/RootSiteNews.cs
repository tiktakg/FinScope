using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.Models
{
    internal class RootSiteNews
    {

        public Sitenews sitenews { get; set; }

        [JsonProperty("sitenews.cursor")]
        public SitenewsCursor sitenewscursor { get; set; }
    }

    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Id
    {
        public string type { get; set; }
    }

    public class INDEX
    {
        public string type { get; set; }
    }

    //public class Metadata
    //{
    //    public Id id { get; set; }
    //    public Tag tag { get; set; }
    //    public Title title { get; set; }
    //    public PublishedAt published_at { get; set; }
    //    public ModifiedAt modified_at { get; set; }
    //    public INDEX INDEX { get; set; }
    //    public TOTAL TOTAL { get; set; }
    //    public PAGESIZE PAGESIZE { get; set; }
    //}

    public class ModifiedAt
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }

    public class PAGESIZE
    {
        public string type { get; set; }
    }

    public class PublishedAt
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }



    public class Sitenews
    {
        public Metadata metadata { get; set; }
        public List<string> columns { get; set; }
        public List<List<object>> data { get; set; }
    }

    public class SitenewsCursor
    {
        public Metadata metadata { get; set; }
        public List<string> columns { get; set; }
        public List<List<int>> data { get; set; }
    }

    public class Tag
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }

    public class Title
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }

    public class TOTAL
    {
        public string type { get; set; }
    }

}
