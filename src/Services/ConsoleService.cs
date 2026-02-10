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
        private static readonly string EvalScript = EmbeddedResourceService.LoadScript("console-eval.js");

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
