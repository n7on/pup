using System.IO;
using System.Runtime.InteropServices;

namespace Pup.Common
{
    internal static class Interop
    {
        /// <summary>
        /// Returns a wrapper script path that launches the target executable in its
        /// own process group.  On Unix, terminal Ctrl+C sends SIGINT to every process
        /// in the foreground process group; by moving Chrome to a new group we prevent
        /// it from being killed when the user interrupts a PowerShell pipeline.
        /// The wrapper encodes the real executable path so all original arguments are
        /// passed through unchanged.
        /// </summary>
        internal static string CreateProcessGroupWrapper(string targetExecutable)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return targetExecutable;

            // Generate a wrapper script that hard-codes the Chrome path.
            // setpgrp(0,0) moves the process into its own process group before
            // exec, so SIGINT from the terminal never reaches Chrome.
            var wrapper = Path.Combine(Path.GetTempPath(), "pup-pgid-wrapper.sh");
            var escaped = targetExecutable.Replace("'", "'\\''");
            File.WriteAllText(wrapper,
                $"#!/bin/sh\nexec perl -e 'setpgrp(0,0); exec @ARGV' -- '{escaped}' \"$@\"\n");
            Chmod(wrapper, 0x1ED); // 0755
            return wrapper;
        }

        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string path, int mode);

        private static void Chmod(string path, int mode)
        {
            chmod(path, mode);
        }
    }
}
