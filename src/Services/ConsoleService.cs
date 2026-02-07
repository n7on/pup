using System;
using System.Collections.Generic;
using System.Text.Json;
using Pup.Common;
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

                    var formatted = JsonHelper.PrettyPrint(doc.RootElement);
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
    }
}
