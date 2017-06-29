using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace carbon.Logic
{
    public class Helpers
    {
        public static double TryDouble(object item)
        {
            try
            {
                return double.Parse(item.ToString());
            }
            catch { return 0; }
        }

        

        public static string JsonSerialize(object serObject)
        {
            StringBuilder sb = new StringBuilder();
            JavaScriptSerializer jsSrlzr = new JavaScriptSerializer();
            jsSrlzr.Serialize(serObject, sb);
            return sb.ToString();
        }

        public static object JsonDesrialize(string serString)
        {
            JavaScriptSerializer jsSrlzr = new JavaScriptSerializer();
            return jsSrlzr.Deserialize<dynamic>(serString);
        }

        public static object JsonDesrialize(string serString, Type serType)
        {
            JavaScriptSerializer jsSrlzr = new JavaScriptSerializer();
            return jsSrlzr.Deserialize(serString, serType);
        }

        
    }
}
