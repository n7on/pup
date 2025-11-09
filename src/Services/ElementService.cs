using Pup.Transport;
using System.Threading.Tasks;

namespace Pup.Services
{
    public class ElementService : IElementService
    {

        private readonly PupElement _element;

        public ElementService(PupElement element)
        {
            _element = element;
        }

        public async Task ClickElementAsync()
        {
            await _element.Element.ClickAsync().ConfigureAwait(false);
        }


        public async Task SetElementTextAsync(string text)
        {
            await _element.Element.TypeAsync(text).ConfigureAwait(false);
        }

        public async Task SetElementValueAsync(string value)
        {
            await _element.Element.EvaluateFunctionAsync("el => el.value = arguments[0]", value).ConfigureAwait(false);
        }

        public async Task<string> GetElementAttributeAsync(string attributeName)
        {
            return await _element.Element.EvaluateFunctionAsync<string>("(el, attr) => el.getAttribute(attr)", attributeName).ConfigureAwait(false);
        }

        public async Task<string> GetElementTextAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<string>("el => el.textContent").ConfigureAwait(false);
        }

        public async Task<string> GetElementValueAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<string>("el => el.value").ConfigureAwait(false);
        }

        public async Task HoverElementAsync()
        {
            await _element.Element.HoverAsync().ConfigureAwait(false);
        }


        public async Task FocusElementAsync()
        {
            await _element.Element.FocusAsync().ConfigureAwait(false);
        }


        public async Task<bool> IsElementVisibleAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<bool>("el => !!(el.offsetWidth || el.offsetHeight || el.getClientRects().length)").ConfigureAwait(false);
        }

        public async Task<bool> IsElementEnabledAsync()
        {
            return await _element.Element.EvaluateFunctionAsync<bool>("el => !el.disabled").ConfigureAwait(false);
        }
    }
}