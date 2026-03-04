using System;
using System.Management.Automation;
using System.Threading;

namespace Pup.Commands.Base
{
    /// <summary>
    /// Base class for all Pup cmdlets. Clears the SynchronizationContext before
    /// pipeline processing so that .GetAwaiter().GetResult() calls on async methods
    /// do not deadlock under Windows PowerShell 5.1 (.NET Framework).
    /// PowerShell Core is unaffected (no SynchronizationContext by default).
    /// </summary>
    public abstract class PupBaseCommand : PSCmdlet
    {
        protected override void BeginProcessing()
        {
            SynchronizationContext.SetSynchronizationContext(null);
        }

        /// <summary>
        /// Returns true when the exception indicates the element's execution
        /// context was destroyed (e.g. the page navigated) or the DOM node
        /// is detached — i.e. the element reference is stale.
        /// </summary>
        protected static bool IsStaleElementException(Exception ex)
        {
            var msg = ex.Message;
            if (msg == null) return false;
            return msg.Contains("context was destroyed")
                || msg.Contains("context with specified id")
                || msg.Contains("detached from document")
                || msg.Contains("Cannot find context")
                || msg.Contains("Execution context")
                || msg.Contains("Target closed");
        }
    }
}
