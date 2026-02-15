using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;
using Pup.Services;

namespace Pup.Commands.Script
{
    [Cmdlet(VerbsLifecycle.Invoke, "PupScript")]
    [OutputType(typeof(object))]
    public class InvokeScriptCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            ParameterSetName = "FromPage",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to run script on")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 0,
            ParameterSetName = "FromFrame",
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The frame to run script on")]
        public PupFrame Frame { get; set; }

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
                object result = null;

                // Auto-wrap simple expressions in arrow functions if needed
                string scriptToExecute = Script;
                if (!Script.TrimStart().StartsWith("(") && !Script.TrimStart().StartsWith("function") &&
                    !Script.Contains("=>") && !Script.Contains("return") && !Script.Contains(";"))
                {
                    scriptToExecute = $"() => {Script}";
                    WriteVerbose($"Auto-wrapped expression: {scriptToExecute}");
                }

                if (ParameterSetName == "FromFrame")
                {
                    var frameService = ServiceFactory.CreateFrameService(Frame);

                    if (AsVoid.IsPresent)
                    {
                        frameService.ExecuteScriptAsync(scriptToExecute, Arguments).GetAwaiter().GetResult();
                        WriteVerbose("JavaScript executed successfully (void)");
                    }
                    else if (AsString.IsPresent)
                    {
                        result = frameService.ExecuteScriptAsync<string>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    }
                    else if (AsNumber.IsPresent)
                    {
                        result = frameService.ExecuteScriptAsync<double>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    }
                    else if (AsBoolean.IsPresent)
                    {
                        result = frameService.ExecuteScriptAsync<bool>(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    }
                    else
                    {
                        result = frameService.ExecuteScriptWithConversionAsync(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    var pageService = ServiceFactory.CreatePageService(Page);

                    if (AsVoid.IsPresent)
                    {
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
                        result = pageService.ExecuteScriptWithConversionAsync(scriptToExecute, Arguments).GetAwaiter().GetResult();
                    }
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
                WriteError(new ErrorRecord(ex, "InvokeScriptError", ErrorCategory.OperationStopped, Script));
            }
        }
    }
}
