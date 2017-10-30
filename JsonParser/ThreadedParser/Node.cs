using System;
using System.Threading.Tasks;

namespace JsonParser.ThreadedParser
{
    public abstract class Node
    {
        public class NodeResult
        {
            public bool Success { get; set; }
            public object Value { get; set; }
        }

        protected string Raw { get; }

        protected Node(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                throw new ArgumentException("Json can not be null or empty string", nameof(raw));
            Raw = raw.TrimStart().TrimEnd();
        }

        public NodeResult GetResult()
        {
            var execution = Execute();
            execution.Wait();
            return execution.Result;
        }

        public abstract Task<NodeResult> Execute();
        public abstract bool CanBeValid();
        protected Task<NodeResult> Success(object value) => Task.FromResult(new NodeResult { Success = true, Value = value });
        protected Task<NodeResult> Fail() => Task.FromResult(new NodeResult());
    }
}
