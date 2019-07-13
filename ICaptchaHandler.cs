using System.Threading.Tasks;

namespace BestChangeParserKit
{
    public interface ICaptchaHandler
    {
        /// <summary>
        /// True if handled.
        /// </summary>
        Task<bool> TryHandleCaptcha(BestChangeParser sender, string url);
    }
}