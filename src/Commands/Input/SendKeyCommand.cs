using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Input
{
    [Cmdlet(VerbsCommunications.Send, "PupKey")]
    [OutputType(typeof(void))]
    public class SendKeyCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to send keys to")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "SingleKey",
            HelpMessage = "The key to press (e.g., Enter, Tab, Escape, ArrowUp, Backspace)")]
        public string Key { get; set; }

        [Parameter(
            ParameterSetName = "SingleKey",
            HelpMessage = "Modifier keys to hold while pressing the key (e.g., Control, Shift, Alt, Meta)")]
        public string[] Modifiers { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "TypeText",
            HelpMessage = "Text to type character by character")]
        public string Text { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);

                if (!string.IsNullOrEmpty(Text))
                {
                    pageService.SendKeysAsync(Text).GetAwaiter().GetResult();
                    WriteVerbose($"Typed text: {Text}");
                }
                else
                {
                    pageService.SendKeyAsync(Key, Modifiers).GetAwaiter().GetResult();

                    if (Modifiers != null && Modifiers.Length > 0)
                    {
                        WriteVerbose($"Pressed {string.Join("+", Modifiers)}+{Key}");
                    }
                    else
                    {
                        WriteVerbose($"Pressed {Key}");
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SendKeyError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
