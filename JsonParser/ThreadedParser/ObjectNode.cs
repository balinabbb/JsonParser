using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JsonParser.ThreadedParser
{
    public class ObjectNode : Node
    {
        public ObjectNode(string raw) : base(raw)
        {
        }

        public static Dictionary<string, string> Split(string raw)
        {
            var result = new Dictionary<string, string>(raw.Split(',').Length);
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
                    var key = new string(current.Skip(1).TakeWhile(c => c != '"').ToArray());
                    if (ValueNode.TryParseString($"\"{key}\"", out key))
                    {
                        result[key] = current.Substring(key.Length + 3);
                        current = "";
                    }
                    else
                    {
                        throw new ArgumentException("Could not parse json");
                    }
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
            if (current != "")
            {
                var key = new string(current.TrimStart().Skip(1).TakeWhile(c => c != '"').ToArray());
                if (ValueNode.TryParseString($"\"{key}\"", out key))
                {
                    result[key] = current.TrimStart().Substring(key.Length + 3);
                }
                else
                {
                    throw new ArgumentException("Could not parse json");
                }
            }
            return result;
        }


        public override Task<NodeResult> Execute()
        {
            if (!CanBeValid())
                return Fail();
            var result = new Dictionary<string, object>();

            var completionSource = new TaskCompletionSource<NodeResult>();
            //TODO cancellation token source for any fail
            Task.WhenAll(Split(Raw).Select(x => new ValueNode(x.Value).Execute().ContinueWith(t =>
            {
                if (t.Result.Success)
                    result[x.Key] = t.Result.Value;
            })).ToArray()).ContinueWith(t => completionSource.SetResult(Success(result).Result));

            return completionSource.Task;
        }

        public override bool CanBeValid() => Raw[0] == '{' && Raw[Raw.Length - 1] == '}';
    }
}
