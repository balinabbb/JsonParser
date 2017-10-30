using System;

namespace JsonParser.ThreadedParser
{
    public class ThreadedJsonParser : IJsonParser
    {
        public object Parse(string raw)
        {
            var node = new ValueNode(raw);
            var result = node.GetResult();
            if (result.Success)
                return result.Value;
            throw new ArgumentException("Could not parse string");
        }
    }
}
