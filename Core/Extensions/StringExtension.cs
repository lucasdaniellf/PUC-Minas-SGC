using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Extensions
{
    public static class StringExtension
    {
        public static string SerializedObjectString(this object obj)
        {
            if(obj != null)
            {
                return JsonConvert.SerializeObject(obj);
            }
            return string.Empty;
        }
    }
}
