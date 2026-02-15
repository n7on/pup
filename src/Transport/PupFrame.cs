using System.Threading.Tasks;
using PuppeteerSharp;

namespace Pup.Transport
{
    public class PupFrame
    {
        public IFrame Frame { get; }
        public string Name { get; }
        public string Url { get; }
        public string Id { get; }
        public bool IsMainFrame { get; }
        public bool IsDetached { get; }

        private PupFrame(IFrame frame, string name)
        {
            Frame = frame;
            Name = name ?? "";
            Url = frame.Url ?? "";
            Id = frame.Id;
            IsMainFrame = frame.ParentFrame == null;
            IsDetached = frame.Detached;
        }

        public static async Task<PupFrame> CreateAsync(IFrame frame)
        {
            string name = "";
            if (frame.ParentFrame != null)
            {
                var frameElement = await frame.FrameElementAsync().ConfigureAwait(false);
                if (frameElement != null)
                {
                    name = await frameElement.EvaluateFunctionAsync<string>("el => el.name || el.id || ''").ConfigureAwait(false) ?? "";
                }
            }
            return new PupFrame(frame, name);
        }

        public override string ToString()
        {
            if (IsMainFrame)
                return $"[Main Frame] {Url}";
            return string.IsNullOrEmpty(Name)
                ? $"[Frame {Id}] {Url}"
                : $"[Frame '{Name}'] {Url}";
        }
    }
}
