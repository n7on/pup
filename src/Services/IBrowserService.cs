using System.Collections.Generic;
using System.Threading.Tasks;
using Pup.Transport;

public interface IBrowserService
{
    Task<PupPage> CreatePageAsync(string name, int width, int height, string url, bool waitForLoad);
    Task<List<PupPage>> GetPagesAsync();
    bool RemoveBrowser();
    bool StopBrowser();
}