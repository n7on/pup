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
    }
}
