using Newtonsoft.Json;

namespace JsonParser
{
    public class NewtonSoftParser : IJsonParser
    {
        public object Parse(string raw)
        {
            return JsonConvert.SerializeObject(raw);
        }
    }
}
