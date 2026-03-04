using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

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
        protected CancellationTokenSource Cts;

        protected override void BeginProcessing()
        {
            SynchronizationContext.SetSynchronizationContext(null);
            Cts = new CancellationTokenSource();
        }

        protected override void StopProcessing()
        {
            try { Cts?.Cancel(); } catch { }
        }

        protected T Await<T>(Task<T> task)
        {
            try
            {
                task.Wait(Cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new PipelineStoppedException();
            }
            return task.GetAwaiter().GetResult();
        }

        protected void Await(Task task)
        {
            try
            {
                task.Wait(Cts.Token);
            }
            catch (OperationCanceledException)
            {
                throw new PipelineStoppedException();
            }
            task.GetAwaiter().GetResult();
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
