using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UndergroundIRO.BestChangeParserKit
{
    public class HttpService:IHttpService
    {

        public HttpClient Client { get; }

        public HttpService(HttpClient client = null)
        {
            Client = client ?? new HttpClient();
        }

        public async Task<string> GetAsync(string url, Encoding enc=null)
        {
            enc = enc ?? Encoding.Default;
            var resp = await Client.SendAsync(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        url
                    )
                );
            var responseArr = await resp.Content.ReadAsByteArrayAsync();
            var html = enc.GetString(responseArr, 0, responseArr.Length - 1);
            return html;
        }
    }
}