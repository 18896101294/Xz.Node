using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Xz.Node.Framework.Extensions;

namespace Xz.Node.Framework.Common
{
    /// <summary>
    /// json序列化帮助类
    /// </summary>
    public class JsonHelper
    {
        public static JsonHelper Instance { get; } = new JsonHelper();

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss" });
        }

        public string SerializeByConverter(object obj, params JsonConverter[] converters)
        {
            return JsonConvert.SerializeObject(obj, converters);
        }

        public T Deserialize<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }

        public T DeserializeByConverter<T>(string input, params JsonConverter[] converter)
        {
            return JsonConvert.DeserializeObject<T>(input, converter);
        }

        public T DeserializeBySetting<T>(string input, JsonSerializerSettings settings)
        {
            return JsonConvert.DeserializeObject<T>(input, settings);
        }

        private object NullToEmpty(object obj)
        {
            return null;
        }

        #region json转Dictionary键值对
        /// <summary>
        /// json转Dictionary键值对
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public Dictionary<string, string> JsonStringToKeyValuePairs(string jsonStr)
        {
            char jsonBeginToken = '{';
            char jsonEndToken = '}';

            if (string.IsNullOrEmpty(jsonStr))
            {
                return null;
            }
            if (jsonStr[0] != jsonBeginToken || jsonStr[jsonStr.Length - 1] != jsonEndToken)
            {
                throw new InfoException("非法的Json字符串!");
            }
            var resultDic = new Dictionary<string, string>();
            var jobj = JObject.Parse(jsonStr);
            JsonOn(jobj, resultDic);
            return resultDic;
        }
        private Dictionary<string, string> JsonOn(JToken jobT, Dictionary<string, string> Dic)
        {
            if (jobT is JObject jobj && jobj.Properties().Count() > 0)
            {
                foreach (var item in jobj.Properties())
                {
                    JsonProperties(item, Dic);
                }
            }
            else
            {
                Dic.Add(jobT.Path, jobT.ToString());
                return Dic;
            }
            return Dic;
        }

        private Dictionary<string, string> JsonProperties(JProperty jobj, Dictionary<string, string> Dic)
        {
            return JsonOn(jobj.Value, Dic);
        }
        #endregion
    }
}
