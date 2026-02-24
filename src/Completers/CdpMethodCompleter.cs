using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace Pup.Completers
{
    public class CdpMethodCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(
            string commandName,
            string parameterName,
            string wordToComplete,
            CommandAst commandAst,
            IDictionary fakeBoundParameters)
        {
            string[] methods;

            if (fakeBoundParameters.Contains("Browser"))
                methods = CdpProtocol.BrowserMethods;
            else if (fakeBoundParameters.Contains("Page"))
                methods = CdpProtocol.PageMethods;
            else
                methods = CdpProtocol.BrowserMethods
                    .Concat(CdpProtocol.PageMethods)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToArray();

            foreach (var method in methods)
            {
                if (method.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new CompletionResult(method);
                }
            }
        }
    }
}
