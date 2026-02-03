using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Element
{
    [Cmdlet(VerbsCommon.Select, "PupOption")]
    [OutputType(typeof(string[]))]
    [OutputType(typeof(PupSelectOption[]))]
    public class SelectOptionCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The select element to interact with")]
        public PupElement Element { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ByValue",
            HelpMessage = "Select option(s) by value attribute")]
        public string[] Value { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ByText",
            HelpMessage = "Select option(s) by visible text")]
        public string[] Text { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "ByIndex",
            HelpMessage = "Select option(s) by zero-based index")]
        public int[] Index { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "ListOptions",
            HelpMessage = "List all available options in the select element")]
        public SwitchParameter List { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var elementService = ServiceFactory.CreateElementService(Element);

                if (List.IsPresent)
                {
                    var options = elementService.GetSelectOptionsAsync().GetAwaiter().GetResult();
                    WriteObject(options, true);
                    return;
                }

                string[] selectedValues;

                if (Value != null && Value.Length > 0)
                {
                    selectedValues = elementService.SelectOptionByValueAsync(Value).GetAwaiter().GetResult();
                    WriteVerbose($"Selected by value: {string.Join(", ", Value)}");
                }
                else if (Text != null && Text.Length > 0)
                {
                    selectedValues = elementService.SelectOptionByTextAsync(Text).GetAwaiter().GetResult();
                    WriteVerbose($"Selected by text: {string.Join(", ", Text)}");
                }
                else if (Index != null && Index.Length > 0)
                {
                    selectedValues = elementService.SelectOptionByIndexAsync(Index).GetAwaiter().GetResult();
                    WriteVerbose($"Selected by index: {string.Join(", ", Index)}");
                }
                else
                {
                    WriteError(new ErrorRecord(
                        new ArgumentException("Must specify -Value, -Text, -Index, or -List"),
                        "NoSelectionCriteria",
                        ErrorCategory.InvalidArgument,
                        Element));
                    return;
                }

                WriteObject(selectedValues);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SelectOptionError", ErrorCategory.InvalidOperation, Element));
            }
        }
    }
}
