using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace JsonParser.ThreadedParser
{
    public class ValueNode : Node
    {
        public ValueNode(string raw) : base(raw)
        {
        }

        static Task Configure(Node node, TaskCompletionSource<NodeResult> completionSource)
            => node.Execute().ContinueWith(result =>
                {
                    if (result.Result.Success)
                        completionSource.SetResult(result.Result);
                });

        public override bool CanBeValid() => true;
        //TODO might want to add cancellation tokensource for these executions, if the result is already found
        public override Task<NodeResult> Execute()
        {
            if (Raw == "null")
                return Success(null);
            if (Raw == "true")
                return Success(true);
            if (Raw == "false")
                return Success(false);
            if (int.TryParse(Raw, out var integer))
                return Success(integer);
            if (double.TryParse(Raw, out var number))
                return Success(number);
            if (TryParseString(Raw, out var str))
                return Success(str);
            var completionSource = new TaskCompletionSource<NodeResult>(TaskCreationOptions.RunContinuationsAsynchronously);
            Task.WhenAll(
                Configure(new ArrayNode(Raw), completionSource),
                Configure(new ObjectNode(Raw), completionSource)
            ).ContinueWith(t =>
            {
                if (!completionSource.Task.IsCompleted)
                    completionSource.SetResult(Fail().Result);
            });

            return completionSource.Task;
        }

        public static bool TryParseString(string raw, out string result)
        {
            result = null;
            raw = raw.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(raw) || raw[0] != '"')
                return false;
            result = "";
            for (var index = 1; index < raw.Length;)
            {
                var current = raw[index];
                if (current == '"')
                    return true;
                if (current == '\\')
                {
                    current = raw[++index];
                    if (current == 'u')
                    {
                        var substring = raw.Substring(index + 1, 4);
                        var unicode = (char)int.Parse(substring, NumberStyles.HexNumber);
                        result += unicode;
                        index += 5;
                    }
                    else
                        result += current;
                }
                else
                {
                    result += current;
                    index++;
                }
            }
            return false;
        }


    }
}