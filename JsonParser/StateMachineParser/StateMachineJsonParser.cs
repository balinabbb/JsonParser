using System;
using System.Collections.Generic;
using System.Globalization;

namespace JsonParser.StateMachineParser
{
    public class StateMachineJsonParser : IJsonParser
    {

        public object Parse(string raw)
        {
            if (TryParseValue(raw, out var result))
                return result;
            throw new ArgumentException("Could not parse string");
        }

        static bool TryParseValue(string raw, out object result)
        {
            result = null;
            if (string.IsNullOrEmpty(raw))
                return false;
            raw = raw.TrimStart().TrimEnd();
            if (raw == "null")
            {
                return true;
            }
            if (raw == "true")
            {
                result = true;
                return true;
            }
            if (raw == "false")
            {
                result = false;
                return true;
            }
            if (int.TryParse(raw, out var integer))
            {
                result = integer;
                return true;
            }
            if (double.TryParse(raw, out var number))
            {
                result = number;
                return true;
            }
            if (TryParseString(raw, out var str))
            {
                result = str;
                return true;
            }
            if (TryParseArray(raw, out var array))
            {
                result = array;
                return true;
            }
            if (TryParseObject(raw, out var obj))
            {
                result = obj;
                return true;
            }
            return false;
        }

        static bool TryParseObject(string raw, out Dictionary<string, object> result)
        {
            result = null;
            raw = raw.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(raw) || raw[0] != '{' || raw[raw.Length - 1] != '}')
                return false;

            result = new Dictionary<string, object>();
            var current = "";
            var currentKey = "";
            for (var i = 1; i < raw.Length - 1; i++)
            {
                if (raw[i] == ':' && TryParseString(current, out var key))
                {
                    currentKey = key;
                    current = "";
                }
                else if (raw[i] == ',' && TryParseValue(current, out var value))
                {
                    result.Add(currentKey, value);
                    current = "";
                }
                else
                {
                    current += raw[i];
                }
            }
            if(TryParseValue(current, out var lastValue))
                result.Add(currentKey, lastValue);
            return true;
        }

        static bool TryParseArray(string raw, out List<object> result)
        {
            result = null;
            raw = raw.TrimStart().TrimEnd();
            if (string.IsNullOrEmpty(raw) || raw[0] != '[' || raw[raw.Length - 1] != ']')
                return false;
            result = new List<object>();
            var current = "";
            for (var i = 1; i < raw.Length - 1; i++)
            {
                if (raw[i] != ',')
                    current += raw[i];
                else if (TryParseValue(current, out var value))
                {
                    result.Add(value);
                    current = "";
                }
                else
                {
                    current += ",";
                }
            }
            if (TryParseValue(current, out var lastValue))
                result.Add(lastValue);
            return true;
        }

        static bool TryParseString(string raw, out string result)
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
