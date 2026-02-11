using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Common;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupPageScript")]
    [OutputType(typeof(object))]
    public class InvokePageScriptCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to run script on")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "JavaScript code to execute")]
        public string Script { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "Arguments to pass to the script")]
        public object[] Arguments { get; set; } = new object[0];

        [Parameter(HelpMessage = "Expect a string return value")]
        public SwitchParameter AsString { get; set; }

        [Parameter(HelpMessage = "Expect a number return value")]
        public SwitchParameter AsNumber { get; set; }

        [Parameter(HelpMessage = "Expect a boolean return value")]
        public SwitchParameter AsBoolean { get; set; }

        [Parameter(HelpMessage = "Don't expect a return value (void execution)")]
        public SwitchParameter AsVoid { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);
                object result = null;

                // Auto-wrap simple expressions in arrow functions if needed
                string scriptToExecute = Script;
                if (!Script.TrimStart().StartsWith("(") && !Script.TrimStart().StartsWith("function") &&
                    !Script.Contains("=>") && !Script.Contains("return") && !Script.Contains(";"))
                {
                    scriptToExecute = $"() => {Script}";
                    WriteVerbose($"Auto-wrapped expression: {scriptToExecute}");
                }

                if (AsVoid.IsPresent)
                {
                    // Execute without return value
                    pageService.ExecuteScriptAsync(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    WriteVerbose("JavaScript executed successfully (void)");
                }
                else if (AsString.IsPresent)
                {
                    result = pageService.ExecuteScriptAsync<string>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                }
                else if (AsNumber.IsPresent)
                {
                    result = pageService.ExecuteScriptAsync<double>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                }
                else if (AsBoolean.IsPresent)
                {
                    result = pageService.ExecuteScriptAsync<bool>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                }
                else
                {
                    // Default: execute and convert JsonElement results to PSObjects
                    result = pageService.ExecuteScriptWithConversionAsync(scriptToExecute, Arguments).GetAwaiter().GetResult();
                }

                if (result != null || !AsVoid.IsPresent)
                {
                    WriteObject(result);
                }

                if (!AsVoid.IsPresent)
                {
                    WriteVerbose($"JavaScript executed successfully, returned: {result}");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "InvokePageScriptError", ErrorCategory.OperationStopped, Script));
            }
        }
    }
}