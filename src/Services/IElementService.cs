using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Transport;

namespace Pup.Services
{
    public interface IElementService
    {
        Task ClickElementAsync();
        Task SetElementTextAsync(string text);
        Task SetElementValueAsync(string value);
        
        Task<string> GetElementAttributeAsync(string attributeName);
        Task<string> GetElementTextAsync();
        Task<string> GetElementValueAsync();
        
        Task HoverElementAsync();
        Task FocusElementAsync();
        Task<bool> IsElementVisibleAsync();
        Task<bool> IsElementEnabledAsync();
    }
}