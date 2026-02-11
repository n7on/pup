using System;
using System.Management.Automation;
using System.Management.Automation.Host;
using Pup.Common;
using Pup.Transport;
using Pup.Commands.Base;

namespace Pup.Commands.Console
{
    [Cmdlet(VerbsCommon.Enter, "PupConsole")]
    public class EnterConsoleCommand : PupBaseCommand
    {
        [Parameter(
            Position = 0,
            Mandatory = true,
            ValueFromPipeline = true,
            ValueFromPipelineByPropertyName = true,
            HelpMessage = "The page to connect the console to")]
        public PupPage Page { get; set; }

        [Parameter(HelpMessage = "Custom prompt string")]
        public string Prompt { get; set; } = "js> ";

        protected override void ProcessRecord()
        {
            try
            {
                var consoleService = ServiceFactory.CreateConsoleService(Page);

                WriteObject($"Pup JavaScript Console - Connected to: {Page.Page.Url}");
                WriteObject("Type JavaScript expressions to evaluate. Type 'exit' or 'quit' to leave.");
                WriteObject("");

                while (true)
                {
                    Host.UI.Write(ConsoleColor.Cyan, Host.UI.RawUI.BackgroundColor, Prompt);
                    var input = Host.UI.ReadLine();

                    if (string.IsNullOrWhiteSpace(input))
                        continue;

                    var trimmedInput = input.Trim().ToLowerInvariant();

                    if (trimmedInput == "exit" || trimmedInput == "quit")
                    {
                        WriteObject("Exiting console...");
                        break;
                    }

                    if (HandleBuiltInCommand(trimmedInput))
                        continue;

                    var result = consoleService.Evaluate(input);

                    // Show console output first
                    foreach (var entry in consoleService.GetNewConsoleEntries())
                    {
                        var color = entry.Type switch
                        {
                            "Error" => ConsoleColor.Red,
                            "Warning" => ConsoleColor.Yellow,
                            "Info" => ConsoleColor.Blue,
                            _ => ConsoleColor.Gray
                        };
                        Host.UI.WriteLine(color, Host.UI.RawUI.BackgroundColor, entry.Text);
                    }

                    // Show result
                    if (result.Success)
                    {
                        Host.UI.WriteLine(ConsoleColor.Green, Host.UI.RawUI.BackgroundColor, result.FormattedValue);
                    }
                    else
                    {
                        Host.UI.WriteLine(ConsoleColor.Red, Host.UI.RawUI.BackgroundColor, $"Error: {result.Error}");
                    }
                }
            }
            catch (PipelineStoppedException)
            {
                WriteObject("Console interrupted.");
            }
            catch (Exception ex)
            {
                WriteError(new ErrorRecord(ex, "ConsoleError", ErrorCategory.OperationStopped, Page));
            }
        }

        private bool HandleBuiltInCommand(string command)
        {
            switch (command)
            {
                case "clear":
                case "cls":
                    TryClearScreen();
                    return true;

                case "url":
                    WriteObject(Page.Page.Url);
                    return true;

                case "help":
                    WriteObject("Commands:");
                    WriteObject("  exit, quit  - Exit the console");
                    WriteObject("  clear, cls  - Clear the screen");
                    WriteObject("  url         - Show current page URL");
                    WriteObject("  help        - Show this help");
                    WriteObject("");
                    WriteObject("Examples:");
                    WriteObject("  document.title");
                    WriteObject("  document.querySelectorAll('a').length");
                    WriteObject("  localStorage");
                    WriteObject("  console.log('test')");
                    return true;

                default:
                    return false;
            }
        }

        private void TryClearScreen()
        {
            try
            {
                Host.UI.RawUI.CursorPosition = new Coordinates(0, 0);
                var blank = new string(' ', Host.UI.RawUI.BufferSize.Width * Host.UI.RawUI.BufferSize.Height);
                Host.UI.Write(blank);
                Host.UI.RawUI.CursorPosition = new Coordinates(0, 0);
            }
            catch
            {
                // Clear may not be supported in all hosts
            }
        }
    }
}
