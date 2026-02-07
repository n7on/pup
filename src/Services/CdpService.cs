using System.Collections;
using System.Text.Json;
using System.Threading.Tasks;
using Pup.Common;
using Pup.Transport;

namespace Pup.Services
{
    public interface ICdpService
    {
        Task<object> SendAsync(string method, object parameters = null);
        Task<string> SendRawAsync(string method, object parameters = null);
        Task<T> SendAsync<T>(string method, object parameters = null);
    }

    public class CdpService : ICdpService
    {
        private readonly PupPage _page;

        public CdpService(PupPage page)
        {
            _page = page;
        }

        public async Task<object> SendAsync(string method, object parameters = null)
        {
            var jsonElement = await SendInternalAsync(method, parameters).ConfigureAwait(false);
            return JsonHelper.ConvertJsonElement(jsonElement);
        }

        public async Task<string> SendRawAsync(string method, object parameters = null)
        {
            var jsonElement = await SendInternalAsync(method, parameters).ConfigureAwait(false);
            return jsonElement.GetRawText();
        }

        public async Task<T> SendAsync<T>(string method, object parameters = null)
        {
            var jsonElement = await SendInternalAsync(method, parameters).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
        }

        private async Task<JsonElement> SendInternalAsync(string method, object parameters)
        {
            var client = _page.Page.Client;

            JsonElement? response;
            if (parameters == null)
            {
                response = await client.SendAsync(method).ConfigureAwait(false);
            }
            else
            {
                var converted = ConvertParameters(parameters);
                response = await client.SendAsync(method, converted).ConfigureAwait(false);
            }

            return response ?? default;
        }

        private static object ConvertParameters(object parameters)
        {
            if (parameters == null)
                return null;

            if (parameters is Hashtable ht)
                return PowerShellHelper.ConvertHashtable(ht);

            if (parameters is IDictionary dict)
                return PowerShellHelper.ConvertDictionary(dict);

            return parameters;
        }
    }
}
