using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Pup.Transport;

namespace Pup.Services
{
    public interface IConsoleService
    {
        ConsoleEvalResult Evaluate(string expression);
        List<PupConsoleEntry> GetNewConsoleEntries();
    }

    public class ConsoleEvalResult
    {
        public bool Success { get; set; }
        public string FormattedValue { get; set; }
        public string Error { get; set; }
    }

    public class ConsoleService : IConsoleService
    {
        private readonly PupPage _page;
        private int _lastConsoleIndex;

        private const string EvalScript = @"
(expression) => {
    try {
        const result = eval(expression);

        const serialize = (val, depth) => {
            if (depth === undefined) depth = 0;
            if (depth > 5) return JSON.stringify('[Max depth]');
            if (val === undefined) return JSON.stringify('[undefined]');
            if (val === null) return 'null';
            if (typeof val === 'function') return JSON.stringify('[Function: ' + (val.name || 'anonymous') + ']');
            if (typeof val === 'symbol') return JSON.stringify(val.toString());

            if (val instanceof Element) {
                var desc = '<' + val.tagName.toLowerCase();
                if (val.id) desc += ' id=""' + val.id + '""';
                if (val.className) desc += ' class=""' + val.className + '""';
                desc += '>';
                return JSON.stringify(desc);
            }

            if (val instanceof NodeList || val instanceof HTMLCollection) {
                var items = Array.from(val).slice(0, 5).map(function(el) {
                    var tag = el.tagName ? el.tagName.toLowerCase() : 'node';
                    if (el.id) tag += '#' + el.id;
                    return tag;
                });
                var desc = 'NodeList(' + val.length + ') [' + items.join(', ');
                if (val.length > 5) desc += ', ...+' + (val.length - 5) + ' more';
                desc += ']';
                return JSON.stringify(desc);
            }

            if (val instanceof Window) return JSON.stringify('[Window]');
            if (val instanceof Document) return JSON.stringify('[Document]');
            if (val instanceof Promise) return JSON.stringify('[Promise]');

            if (Array.isArray(val)) {
                if (val.length === 0) return '[]';
                var items = val.slice(0, 20).map(function(v) { return serialize(v, depth + 1); });
                var result = '[' + items.join(',');
                if (val.length > 20) result += ',""...+' + (val.length - 20) + ' more""';
                return result + ']';
            }

            if (typeof val === 'object') {
                try {
                    var keys = Object.keys(val);
                    if (keys.length === 0) return '{}';
                    var pairs = keys.slice(0, 20).map(function(k) {
                        return JSON.stringify(k) + ':' + serialize(val[k], depth + 1);
                    });
                    var result = '{' + pairs.join(',');
                    if (keys.length > 20) result += ',""...+' + (keys.length - 20) + ' more""';
                    return result + '}';
                } catch (e) {
                    return JSON.stringify('[Object]');
                }
            }

            if (typeof val === 'string') {
                if (val.length > 500) return JSON.stringify(val.substring(0, 500) + '...(' + val.length + ' chars)');
                return JSON.stringify(val);
            }

            if (typeof val === 'number' || typeof val === 'boolean') {
                return String(val);
            }

            return JSON.stringify(String(val));
        };

        return serialize(result);
    } catch (e) {
        return JSON.stringify({__error: e.message});
    }
}";

        public ConsoleService(PupPage page)
        {
            _page = page;
            _lastConsoleIndex = page.ConsoleEntries.Count;
        }

        public ConsoleEvalResult Evaluate(string expression)
        {
            try
            {
                _lastConsoleIndex = _page.ConsoleEntries.Count;

                var jsonString = _page.Page.EvaluateFunctionAsync<string>(EvalScript, expression)
                    .GetAwaiter().GetResult();

                if (string.IsNullOrEmpty(jsonString))
                {
                    return new ConsoleEvalResult { Success = true, FormattedValue = "undefined" };
                }

                try
                {
                    using var doc = JsonDocument.Parse(jsonString);
                    if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                        doc.RootElement.TryGetProperty("__error", out var errorProp))
                    {
                        return new ConsoleEvalResult { Success = false, Error = errorProp.GetString() };
                    }

                    var formatted = FormatJsonElement(doc.RootElement, 0);
                    return new ConsoleEvalResult { Success = true, FormattedValue = formatted };
                }
                catch
                {
                    return new ConsoleEvalResult { Success = true, FormattedValue = jsonString };
                }
            }
            catch (Exception ex)
            {
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                if (errorMessage.Contains("Evaluation failed:"))
                {
                    errorMessage = errorMessage.Substring(errorMessage.IndexOf("Evaluation failed:") + 18).Trim();
                }
                return new ConsoleEvalResult { Success = false, Error = errorMessage };
            }
        }

        public List<PupConsoleEntry> GetNewConsoleEntries()
        {
            var entries = new List<PupConsoleEntry>();
            lock (_page.ConsoleLock)
            {
                for (int i = _lastConsoleIndex; i < _page.ConsoleEntries.Count; i++)
                {
                    entries.Add(_page.ConsoleEntries[i]);
                }
                _lastConsoleIndex = _page.ConsoleEntries.Count;
            }
            return entries;
        }

        private static string FormatJsonElement(JsonElement element, int indent)
        {
            var indentStr = new string(' ', indent * 2);

            switch (element.ValueKind)
            {
                case JsonValueKind.Undefined:
                    return "undefined";

                case JsonValueKind.Null:
                    return "null";

                case JsonValueKind.True:
                    return "true";

                case JsonValueKind.False:
                    return "false";

                case JsonValueKind.Number:
                    return element.GetRawText();

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Array:
                    return FormatArray(element, indent, indentStr);

                case JsonValueKind.Object:
                    return FormatObject(element, indent, indentStr);

                default:
                    return element.GetRawText();
            }
        }

        private static string FormatArray(JsonElement element, int indent, string indentStr)
        {
            var items = new List<string>();
            foreach (var item in element.EnumerateArray())
            {
                items.Add(FormatJsonElement(item, indent + 1));
            }

            if (items.Count == 0)
                return "[]";

            if (items.Count <= 5 && string.Join(", ", items).Length < 80)
                return $"[{string.Join(", ", items)}]";

            var sb = new StringBuilder("[\n");
            for (int i = 0; i < Math.Min(items.Count, 20); i++)
            {
                sb.Append($"{indentStr}  {items[i]}");
                if (i < items.Count - 1) sb.Append(',');
                sb.Append('\n');
            }
            if (items.Count > 20)
            {
                sb.Append($"{indentStr}  ... ({items.Count - 20} more items)\n");
            }
            sb.Append($"{indentStr}]");
            return sb.ToString();
        }

        private static string FormatObject(JsonElement element, int indent, string indentStr)
        {
            var props = new List<string>();
            foreach (var prop in element.EnumerateObject())
            {
                props.Add($"{prop.Name}: {FormatJsonElement(prop.Value, indent + 1)}");
            }

            if (props.Count == 0)
                return "{}";

            if (props.Count <= 3 && string.Join(", ", props).Length < 60)
                return $"{{ {string.Join(", ", props)} }}";

            var sb = new StringBuilder("{\n");
            for (int i = 0; i < Math.Min(props.Count, 30); i++)
            {
                sb.Append($"{indentStr}  {props[i]}");
                if (i < props.Count - 1) sb.Append(',');
                sb.Append('\n');
            }
            if (props.Count > 30)
            {
                sb.Append($"{indentStr}  ... ({props.Count - 30} more properties)\n");
            }
            sb.Append($"{indentStr}}}");
            return sb.ToString();
        }
    }
}
