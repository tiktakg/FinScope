using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinScope.Models
{
    public class RootNewsBody
    {
        public class Root
        {
            public Content content { get; set; }
        }

        public class Content
        {
            public List<string> columns { get; set; }
            public List<List<object>> data { get; set; }
        }

     

    }

}
