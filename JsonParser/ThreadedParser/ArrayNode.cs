using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonParser.ThreadedParser
{
    public class ArrayNode : Node
    {
        public ArrayNode(string raw) : base(raw)
        {
        }

        public static string[] Split(string raw)
        {
            var result = new List<string>(raw.Split(',').Length);
            var levels = new Dictionary<char, int>
            {
                {'"', 0 },
                {'[', 0 },
                {'{', 0 }
            };
            var current = "";
            for (var i = 1; i < raw.Length - 1; i++)
            {
                if (raw[i] == '"' && (i == 1 || raw[i - 1] != '\\'))
                {
                    if (levels['"'] == 0)
                        levels['"'] = 1;
                    else
                        levels['"'] = 0;
                    current += raw[i];
                }
                else if (raw[i] == ',' && levels.All(x => x.Value == 0))
                {
                    result.Add(current);
                    current = "";
                }
                else if (levels['"'] == 0)
                {
                    if (raw[i] == '[')
                        levels['[']++;
                    else if (raw[i] == ']')
                        levels['[']--;
                    else if (raw[i] == '{')
                        levels['{']++;
                    else if (raw[i] == '}')
                        levels['{']--;
                    current += raw[i];
                }
                else
                {
                    current += raw[i];
                }
            }
            if(current != "")
                result.Add(current);
            return result.ToArray();
        }

        public override Task<NodeResult> Execute()
        {
            if (!CanBeValid())
                return Fail();
            var result = new List<object>();
            
            var completionSource = new TaskCompletionSource<NodeResult>();

            Task.WhenAll(Split(Raw).Select(x => new ValueNode(x).Execute().ContinueWith(t =>
            {
                if(t.Result.Success)
                    result.Add(t.Result.Value);
            })).ToArray()).ContinueWith(t => completionSource.SetResult(Success(result).Result));
            
            return completionSource.Task;
        }

        public override bool CanBeValid() => Raw[0] == '[' && Raw[Raw.Length - 1] == ']';
    }
}
