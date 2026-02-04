using System;
using System.Management.Automation;
using Pup.Transport;

namespace Pup.Commands.WebSocket
{
    [Cmdlet(VerbsCommunications.Send, "PupWebSocketMessage")]
    [OutputType(typeof(bool))]
    public class SendWebSocketMessageCommand : PSCmdlet
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page containing the WebSocket connection")]
        public PupPage Page { get; set; }

        [Parameter(
            Position = 1,
            Mandatory = true,
            HelpMessage = "The message to send through the WebSocket")]
        public string Message { get; set; }

        [Parameter(HelpMessage = "WebSocket URL pattern to target (if multiple connections exist)")]
        public string Url { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var urlPattern = Url ?? "";
                var escapedMessage = EscapeJsString(Message);
                var escapedUrl = EscapeJsString(urlPattern);

                var script = $@"
                    (function() {{
                        const urlPattern = '{escapedUrl}';
                        const message = '{escapedMessage}';
                        const websockets = window.__pup_websockets || [];

                        let ws = null;
                        if (urlPattern) {{
                            ws = websockets.find(w => w.__pup_url && w.__pup_url.includes(urlPattern) && w.readyState === WebSocket.OPEN);
                        }} else {{
                            ws = websockets.find(w => w.readyState === WebSocket.OPEN);
                        }}

                        if (ws) {{
                            ws.send(message);
                            return true;
                        }}
                        return false;
                    }})();
                ";

                var result = Page.Page.EvaluateExpressionAsync<bool>(script).GetAwaiter().GetResult();

                if (!result)
                {
                    WriteWarning("No matching open WebSocket connection found.");
                }

                WriteObject(result);
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "SendWebSocketMessageError", ErrorCategory.WriteError, Page));
            }
        }

        private static string EscapeJsString(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            return input
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }
    }
}
