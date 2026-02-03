using System;
using System.IO;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsData.Export, "PupPagePdf")]
    [OutputType(typeof(byte[]))]
    public class ExportPagePdfCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "Page to export as PDF")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Path to save the PDF file")]
        public string FilePath { get; set; }

        [Parameter(HelpMessage = "Use landscape orientation")]
        public SwitchParameter Landscape { get; set; }

        [Parameter(HelpMessage = "Print background graphics (default: true)")]
        public SwitchParameter NoPrintBackground { get; set; }

        [Parameter(HelpMessage = "Paper format: A0-A6, Letter, Legal, Tabloid, Ledger (default: A4)")]
        [ValidateSet("A0", "A1", "A2", "A3", "A4", "A5", "A6", "Letter", "Legal", "Tabloid", "Ledger")]
        public string Format { get; set; } = "A4";

        [Parameter(HelpMessage = "Scale of the page rendering (default: 1)")]
        [ValidateRange(0.1, 2.0)]
        public decimal Scale { get; set; } = 1;

        [Parameter(HelpMessage = "Return PDF data as byte array")]
        public SwitchParameter PassThru { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var pageService = ServiceFactory.CreatePageService(Page);

                if (!string.IsNullOrEmpty(FilePath))
                {
                    var directory = Path.GetDirectoryName(Path.GetFullPath(FilePath));
                    if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }

                var pdfData = pageService.ExportPdfAsync(
                    FilePath,
                    Landscape.IsPresent,
                    !NoPrintBackground.IsPresent,
                    Format,
                    Scale
                ).GetAwaiter().GetResult();

                if (PassThru.IsPresent)
                {
                    WriteObject(pdfData);
                }

                if (!string.IsNullOrEmpty(FilePath))
                {
                    WriteVerbose($"PDF saved to: {Path.GetFullPath(FilePath)}");
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ExportPagePdfError", ErrorCategory.WriteError, Page));
            }
        }
    }
}
