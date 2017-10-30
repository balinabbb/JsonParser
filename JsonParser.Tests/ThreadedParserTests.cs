using System;
using System.Collections.Generic;
using JsonParser.ThreadedParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonParser.Tests
{
    [TestClass]
    public class ThreadedParseTests
    {
        readonly IJsonParser _ = new ThreadedJsonParser();


        [TestMethod]
        public void DOES_NOT_PARSE_NULL_IsParsedOk() => Assert.ThrowsException<ArgumentException>(() => _.Parse(null));
        [TestMethod]
        public void TRUE_IsParsedOk() => Assert.AreEqual(true, _.Parse("true"));
        [TestMethod]
        public void FALSE_IsParsedOk() => Assert.AreEqual(false, _.Parse("false"));
        [TestMethod]
        public void NULL_IsParsedOk() => Assert.AreEqual(null, _.Parse("null"));
        [TestMethod]
        public void INT_IsParsedOk() => Assert.AreEqual(150, _.Parse("150"));
        [TestMethod]
        public void DOUBLE_IsParsedOk() => Assert.AreEqual(150.0005, _.Parse("150.0005"));
        [TestMethod]
        public void STRING_IsParsedOk() => Assert.AreEqual("yolo", _.Parse(@"""yolo"""));
        [TestMethod]
        public void STRING_EMPTY_IsParsedOk() => Assert.AreEqual("", _.Parse(@""""""));
        [TestMethod]
        public void STRING_ESCAPE_IsParsedOk() => Assert.AreEqual("\"", _.Parse("\"\\\"\""));
        [TestMethod]
        public void STRING_UNICODE_IsParsedOk() => Assert.AreEqual("ā", _.Parse("\"\\u0101\""));
        [TestMethod]
        public void STRING_THROWS_FOR_NON_FINISHED_IsParsedOk() => Assert.ThrowsException<ArgumentException>(() => _.Parse("\""));


        [TestMethod]
        public void ParseArray()
        {
            var result = (List<object>)_.Parse("[\"a\",\"b\"]");
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
        }

        [TestMethod]
        public void ParseArrayInArray()
        {
            var result = (List<object>)_.Parse("[\"a\", [\"b\", \"c\"]]");
            Assert.AreEqual(result.Count, 2);
            Assert.AreEqual(result[0], "a");
            var inner = (List<object>)result[1];
            Assert.AreEqual(inner.Count, 2);
            Assert.AreEqual(inner[0], "b");
            Assert.AreEqual(inner[1], "c");
        }

        [TestMethod]
        public void ParseObject()
        {
            var result = (Dictionary<string, object>)_.Parse("{\"a\": 100, \"b\": false}");
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("a"));
            Assert.IsTrue(result.ContainsKey("b"));
            Assert.AreEqual(100, result["a"]);
            Assert.AreEqual(false, result["b"]);
        }

        [TestMethod]
        public void ParseTrickyObject()
        {
            var result = (Dictionary<string, object>)_.Parse("{\"a,:\": 100,    \"b\": [\"asdf\", 0]}");
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("a,:"));
            Assert.IsTrue(result.ContainsKey("b"));
            Assert.AreEqual(100, result["a,:"]);
            var inner = (List<object>) result["b"];
            Assert.AreEqual(2, inner.Count);
            Assert.AreEqual("asdf", inner[0]);
            Assert.AreEqual(0, inner[1]);
        }

        [TestMethod]
        public void ArraySplit()
        {
            var result = ArrayNode.Split("[\"a\", [\"b\", \"c\"]]");
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("\"a\"", result[0]);
            Assert.AreEqual(" [\"b\", \"c\"]", result[1]);
        }

        [TestMethod]
        public void ArraySplit_Complex()
        {
            var result = ArrayNode.Split("[\"a\", [\"b\", \"c\", {\"yolo\": null, \"swag\": \",,,\"}]]");
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual("\"a\"", result[0]);
            Assert.AreEqual(" [\"b\", \"c\", {\"yolo\": null, \"swag\": \",,,\"}]", result[1]);
        }


        [TestMethod]
        public void ObjectSplit()
        {
            var result = ObjectNode.Split("{\"a,:\": 100,    \"b\": [\"asdf\", {\"yolo\": \"swag\"}]}");
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.ContainsKey("a,:"));
            Assert.AreEqual(" 100", result["a,:"]);
            Assert.IsTrue(result.ContainsKey("b"));
            Assert.AreEqual(" [\"asdf\", {\"yolo\": \"swag\"}]", result["b"]);
        }

    }
}
