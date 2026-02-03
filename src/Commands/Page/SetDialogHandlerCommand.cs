using System;
using System.Management.Automation;
using Pup.Common;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PupDialogHandler")]
    [OutputType(typeof(void))]
    public class SetDialogHandlerCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to configure dialog handling for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "SetHandler",
            HelpMessage = "Action to take when a dialog appears (Accept or Dismiss)")]
        public PupDialogAction Action { get; set; }

        [Parameter(
            ParameterSetName = "SetHandler",
            HelpMessage = "Text to enter when a prompt dialog appears (only used with Accept action)")]
        public string PromptText { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "RemoveHandler",
            HelpMessage = "Remove the dialog handler")]
        public SwitchParameter Remove { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);

                if (Remove.IsPresent)
                {
                    pageService.RemoveDialogHandler();
                    WriteVerbose("Dialog handler removed");
                }
                else
                {
                    pageService.SetDialogHandler(Action, PromptText);
                    WriteVerbose($"Dialog handler set to {Action}" + (PromptText != null ? $" with prompt text: {PromptText}" : ""));
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetDialogHandlerError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
