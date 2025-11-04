using System;
using System.Management.Automation;
using PowerBrowser.Transport;

namespace PowerBrowser.Commands.Page
{
    [Cmdlet(VerbsLifecycle.Invoke, "PageScript")]
    [OutputType(typeof(object))]
    public class InvokePageScriptCommand : PageBaseCommand
    {
        [Parameter(
            Position = 0,
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
                var page = ResolvePageOrThrow();
                object result = null;

                if (AsVoid.IsPresent)
                {
                    // Execute without return value
                    PageService.ExecuteScriptAsync(page, Script, Arguments).GetAwaiter().GetResult();
                    WriteVerbose("JavaScript executed successfully (void)");
                }
                else if (AsString.IsPresent)
                {
                    result = PageService.ExecuteScriptAsync<string>(page, Script, Arguments).GetAwaiter().GetResult();
                }
                else if (AsNumber.IsPresent)
                {
                    result = PageService.ExecuteScriptAsync<double>(page, Script, Arguments).GetAwaiter().GetResult();
                }
                else if (AsBoolean.IsPresent)
                {
                    result = PageService.ExecuteScriptAsync<bool>(page, Script, Arguments).GetAwaiter().GetResult();
                }
                else
                {
                    // Default: try to get result as object
                    try
                    {
                        result = PageService.ExecuteScriptAsync<object>(page, Script, Arguments).GetAwaiter().GetResult();
                    }
                    catch
                    {
                        // If that fails, execute as void
                        PageService.ExecuteScriptAsync(page, Script, Arguments).GetAwaiter().GetResult();
                        result = null;
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
                WriteError(new ErrorRecord(ex, "InvokePageScriptError", ErrorCategory.OperationStopped, Script));
            }
        }
    }
}