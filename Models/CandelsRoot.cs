using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.Models
{
    // CandelsRoot myDeserializedClass = JsonConvert.DeserializeObject<CandelsRoot>(myJsonResponse);
    public class Begin
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }

    public class Candles
    {
        public Metadata metadata { get; set; }
        public List<string> columns { get; set; }
        public List<List<object>> data { get; set; }
    }

    public class Close
    {
        public string type { get; set; }
    }

    public class End
    {
        public string type { get; set; }
        public int bytes { get; set; }
        public int max_size { get; set; }
    }

    public class High
    {
        public string type { get; set; }
    }

    public class Low
    {
        public string type { get; set; }
    }

    public class Metadata
    {
        public Open open { get; set; }
        public Close close { get; set; }
        public High high { get; set; }
        public Low low { get; set; }
        public Value value { get; set; }
        public Volume volume { get; set; }
        public Begin begin { get; set; }
        public End end { get; set; }
    }

    public class Open
    {
        public string type { get; set; }
    }

    public class CandelsRoot
    {
        public Candles candles { get; set; }
    }

    public class Value
    {
        public string type { get; set; }
    }

    public class Volume
    {
        public string type { get; set; }
    }


}
