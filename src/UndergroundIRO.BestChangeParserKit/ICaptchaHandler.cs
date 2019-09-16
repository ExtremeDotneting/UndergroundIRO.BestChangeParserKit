using System.Threading.Tasks;

namespace UndergroundIRO.BestChangeParserKit
{
    public interface ICaptchaHandler
    {
        /// <summary>
        /// True if handled.
        /// </summary>
        Task<bool> TryHandleCaptcha(BestChangeParser sender, string url);
    }
}