using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Services;
using System.Management.Automation.Language;
using System;

namespace Pup.Completers
{
    public class InstalledBrowserCompleter : IArgumentCompleter
    {
        public IEnumerable<CompletionResult> CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            foreach (var browserType in SupportedBrowserService.GetInstalledBrowserTypes())
            {
                if (browserType.StartsWith(wordToComplete, StringComparison.OrdinalIgnoreCase))
                {
                    yield return new CompletionResult(browserType);
                }
            }
        }
    }
}