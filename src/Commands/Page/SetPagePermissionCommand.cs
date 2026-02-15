using System;
using System.Collections.Generic;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Page
{
    [Cmdlet(VerbsCommon.Set, "PupPagePermission")]
    [OutputType(typeof(void))]
    public class SetPagePermissionCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to set permission for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The permission to set")]
        [ValidateSet(
            "geolocation",
            "notifications",
            "camera",
            "microphone",
            "clipboard-read",
            "clipboard-write",
            "midi",
            "midi-sysex",
            "background-sync",
            "accelerometer",
            "gyroscope",
            "magnetometer",
            "accessibility-events",
            "payment-handler",
            "idle-detection",
            "screen-wake-lock",
            "storage-access",
            IgnoreCase = true)]
        public string Permission { get; set; }

        [Parameter(
            Position = 2,
            Mandatory = true,
            HelpMessage = "The permission state")]
        [ValidateSet("Granted", "Denied", "Prompt", IgnoreCase = true)]
        public string State { get; set; }

        [Parameter(HelpMessage = "Origin to set permission for (defaults to current page origin)")]
        public string Origin { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                // Map friendly names to CDP permission names
                var cdpPermission = MapPermissionName(Permission.ToLowerInvariant());
                var cdpState = State.ToLowerInvariant();

                // Get origin from page if not specified
                var origin = Origin;
                if (string.IsNullOrEmpty(origin))
                {
                    var pageUrl = Page.Page.Url;
                    if (!string.IsNullOrEmpty(pageUrl) && Uri.TryCreate(pageUrl, UriKind.Absolute, out var uri))
                    {
                        origin = $"{uri.Scheme}://{uri.Host}";
                        if (!uri.IsDefaultPort)
                        {
                            origin += $":{uri.Port}";
                        }
                    }
                }

                // Use CDP to set permission
                var client = Page.Page.Client;
                var parameters = new Dictionary<string, object>
                {
                    ["permission"] = new Dictionary<string, object>
                    {
                        ["name"] = cdpPermission
                    },
                    ["setting"] = cdpState
                };

                if (!string.IsNullOrEmpty(origin))
                {
                    parameters["origin"] = origin;
                }

                client.SendAsync("Browser.setPermission", parameters).GetAwaiter().GetResult();

                WriteVerbose($"Set permission '{Permission}' to '{State}' for origin '{origin ?? "(all)"}'");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SetPagePermissionFailed", ErrorCategory.OperationStopped, Permission));
            }
        }

        private static string MapPermissionName(string permission)
        {
            // Modern Chrome accepts web permission API names directly
            // Only map names that differ between user-friendly and CDP format
            return permission switch
            {
                "midi-sysex" => "midiSysex",
                "background-sync" => "backgroundSync",
                "accessibility-events" => "accessibilityEvents",
                "payment-handler" => "paymentHandler",
                "idle-detection" => "idleDetection",
                "screen-wake-lock" => "wakeLockScreen",
                "storage-access" => "storageAccess",
                "clipboard-read" => "clipboard-read",
                "clipboard-write" => "clipboard-write",
                _ => permission // Most names pass through directly
            };
        }
    }
}
