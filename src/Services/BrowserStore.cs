using System.Collections.Generic;
using System.Linq;
using Pup.Transport;

namespace Pup.Services
{
    public static class BrowserStore
    {
        private static readonly Dictionary<string, PupBrowser> _browsers = new Dictionary<string, PupBrowser>();
        private static readonly object _lock = new object();

        public static void Save(string name, PupBrowser browser)
        {
            lock (_lock) { _browsers[name] = browser; }
        }

        public static PupBrowser Get(string name)
        {
            lock (_lock) { return _browsers.TryGetValue(name, out var b) ? b : null; }
        }

        public static List<PupBrowser> GetAll()
        {
            lock (_lock) { return _browsers.Values.ToList(); }
        }

        public static bool Remove(string name)
        {
            lock (_lock) { return _browsers.Remove(name); }
        }

        public static void Clear()
        {
            lock (_lock) { _browsers.Clear(); }
        }
    }
}
