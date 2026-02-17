using System;
using System.Management.Automation;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Permission
{
    [Cmdlet(VerbsCommon.Get, "PupPermission")]
    [OutputType(typeof(PSObject))]
    public class GetPermissionCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            HelpMessage = "The page to get permission for")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            HelpMessage = "The permission to query (if not specified, returns all permissions)")]
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

        [Parameter(HelpMessage = "Origin to query permission for (defaults to current page origin)")]
        public string Origin { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var origin = Origin ?? PermissionHelper.GetOriginFromPage(Page);

                if (!string.IsNullOrEmpty(Permission))
                {
                    // Query single permission
                    var state = QueryPermission(Permission, origin);
                    WriteObject(new PSObject(new
                    {
                        Permission = Permission,
                        State = state,
                        Origin = origin
                    }));
                }
                else
                {
                    // Query all permissions
                    foreach (var perm in PermissionHelper.AllPermissions)
                    {
                        try
                        {
                            var state = QueryPermission(perm, origin);
                            WriteObject(new PSObject(new
                            {
                                Permission = perm,
                                State = state,
                                Origin = origin
                            }));
                        }
                        catch
                        {
                            // Some permissions may not be supported, skip them
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "GetPermissionFailed", ErrorCategory.OperationStopped, Permission));
            }
        }

        private string QueryPermission(string permission, string origin)
        {
            var cdpPermission = PermissionHelper.MapPermissionName(permission.ToLowerInvariant());

            // Use JavaScript Permissions API to query
            var script = $@"
                (async () => {{
                    try {{
                        const result = await navigator.permissions.query({{ name: '{cdpPermission}' }});
                        return result.state;
                    }} catch (e) {{
                        return 'unsupported';
                    }}
                }})()
            ";

            var result = Page.Page.EvaluateExpressionAsync<string>(script).GetAwaiter().GetResult();
            return result ?? "unknown";
        }
    }
}
