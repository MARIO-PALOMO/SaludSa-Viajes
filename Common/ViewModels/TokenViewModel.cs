using RestSharp.Deserializers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.ViewModels
{
    public class TokenViewModel
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        [DeserializeAs(Name = ".issued")]
        public DateTime issued { get; set; }
        [DeserializeAs(Name= ".expires")]
        public DateTime expires { get; set; }

        public string error { get; set; }
        public string error_description { get; set; }
    }
}
