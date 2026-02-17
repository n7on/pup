using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Download
{
    [Cmdlet(VerbsCommon.Set, "PupDownloadPath")]
    [OutputType(typeof(void))]
    public class SetDownloadPathCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to configure downloads for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            ParameterSetName = "Allow",
            HelpMessage = "Directory path to save downloads to")]
        public string Path { get; set; }

        [Parameter(
            Mandatory = true,
            ParameterSetName = "Deny",
            HelpMessage = "Disable downloads")]
        public SwitchParameter Deny { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                if (Page == null || !Page.Running)
                {
                    throw new InvalidOperationException("Page is not valid or not running.");
                }

                var client = Page.Page.Client;

                if (Deny.IsPresent)
                {
                    client.SendAsync("Browser.setDownloadBehavior", new Dictionary<string, object>
                    {
                        ["behavior"] = "deny"
                    }).GetAwaiter().GetResult();

                    Page.DownloadPath = null;
                    WriteVerbose("Downloads disabled.");
                    return;
                }

                var resolvedPath = GetUnresolvedProviderPathFromPSPath(Path);
                Directory.CreateDirectory(resolvedPath);

                client.SendAsync("Browser.setDownloadBehavior", new Dictionary<string, object>
                {
                    ["behavior"] = "allowAndName",
                    ["downloadPath"] = resolvedPath,
                    ["eventsEnabled"] = true
                }).GetAwaiter().GetResult();

                Page.DownloadPath = resolvedPath;
                WriteVerbose($"Downloads enabled. Path: {resolvedPath}");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetDownloadPathError", ErrorCategory.InvalidOperation, Page));
            }
        }
    }
}
