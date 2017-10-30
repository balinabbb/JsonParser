using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonParser.Sample
{
    internal class BenchMark
    {
        readonly IJsonParser _parser;
        readonly string _content;

        public BenchMark(string fileName, IJsonParser parser)
        {
            _parser = parser;
            using (var sr = new StreamReader($"../../Sample/{fileName}.json"))
                _content = sr.ReadToEnd();
        }

        public void Launch(Action<string> log)
        {
            var stopWatch = new Stopwatch();
            log("started parsing...");
            try
            {
                stopWatch.Start();
                var result = _parser.Parse(_content);
                stopWatch.Stop();
                if(result == null && _content != "null")
                    throw new ArgumentException();
            }
            catch (ArgumentException)
            {
                log("parsing failed");
                return;
            }
            log($"finished parsing in: {stopWatch.ElapsedMilliseconds} ms with {_parser.GetType().Name}");
        }
    }
}
