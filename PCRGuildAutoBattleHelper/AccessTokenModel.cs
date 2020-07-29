using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRGuildAutoBattleHelper
{
    public class AccessTokenModel
    {
        //log_id不能用int，会炸
        public string log_id { get; set; }
        public int words_result_num { get; set; }
        public JArray words_result { get; set; }
        public int error_code { get; set; }
        public string error_msg { get; set; }
    }
  
}
