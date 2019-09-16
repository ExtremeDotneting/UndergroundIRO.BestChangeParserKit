using System.Text;
using System.Threading.Tasks;

namespace UndergroundIRO.BestChangeParserKit
{
    public interface IHttpService
    {
        /// <summary>
        /// Encoding to get html string.
        /// </summary>
        Task<string> GetAsync(string url, Encoding enc = null);
    }
}