using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BestChangeParserKit.Exceptions;
using BestChangeParserKit.Models;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace BestChangeParserKit
{
    public class BestChangeParser
    {
        readonly HttpClient _httpClient;

        readonly ICaptchaHandler _captchaHandler;

        public BestChangeParser(HttpClient httpClient, ICaptchaHandler captchaHandler = null)
        {
            _httpClient = httpClient;
            _captchaHandler = captchaHandler;
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public async Task<ICollection<BestChangePairInfo>> Parse(string bestChangePageUrl)
        {
            try
            {
                string html = await LoadHtml(bestChangePageUrl);

                var hw = new HtmlDocument();
                hw.LoadHtml(html);
                hw.OptionUseIdAttribute = true;
                var tableRecords = hw
                    .GetElementbyId("content_table")
                    .ChildNodes
                    .First(x => x.OriginalName == "tbody")
                    .ChildNodes
                    .Where(x => x.OriginalName == "tr");
                var list = new List<BestChangePairInfo>();
                foreach (var tr in tableRecords)
                {
                    var trDescedants = tr.Descendants().ToArray();
                    var exchangerTitle = ResolveExchangerTitle(trDescedants);
                    var fromCount = ResolveFromCount(trDescedants);
                    var fromCoinName = ResolveFromCoinName(trDescedants);
                    var toCount = ResolveToCount(trDescedants);
                    var toCoinName = ResolveToCoinName(trDescedants);
                    var rate = toCount / fromCount;
                    var isManual = ResolveIsManual(trDescedants);
                    var isFloating = ResolveIsFloatingRate(trDescedants);
                    var isVerifying = ResolveNeedVerification(trDescedants);
                    var exchangerUrl = ResolveExchangerUrl(trDescedants);
                    var min = ResolveMin(trDescedants);
                    var fund = ResolveFund(trDescedants);

                    var newInfo = new BestChangePairInfo()
                    {
                        ExchangerTitle = exchangerTitle,
                        CoinNameFrom = fromCoinName,
                        CoinNameTo = toCoinName,
                        Rate = rate,
                        IsManual = isManual,
                        IsFloatingRate = isFloating,
                        NeedVerification = isVerifying,
                        ExchangerUrl = exchangerUrl,
                        Min_FromCoin = min,
                        Fund = fund
                    };
                    list.Add(newInfo);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new BestChangeParserException("Exception while parsing.", ex);
            }
        }

        async Task<string> LoadHtml(string url, int tryesLeft = 3)
        {
            var resp = await _httpClient.SendAsync(
                    new HttpRequestMessage(
                        HttpMethod.Get,
                        url
                    )
                );
            var responseArr = await resp.Content.ReadAsByteArrayAsync();
            Encoding windows1251 = Encoding.GetEncoding(1251);
            var html = windows1251.GetString(responseArr, 0, responseArr.Length - 1);
            if (html.Contains("Parsing is prohibited"))
            {
                var captchaHandled = false;
                if (_captchaHandler != null)
                    captchaHandled = await _captchaHandler.TryHandleCaptcha(this, url);
                if (captchaHandled == true && tryesLeft > 0)
                {
                    return await LoadHtml(url, tryesLeft - 1);
                }
                else
                {
                    throw new BestChangeParserException("Probably unhandled captcha.");
                }
            }
            return html;
        }

        string ResolveExchangerTitle(HtmlNode[] trDescedants)
        {
            try
            {
                var exchangerTitle = trDescedants
                    .First(x => x.HasClass("ca"))
                    .InnerText.Trim();
                return exchangerTitle;
            }
            catch
            {
                return null;
            }
        }

        decimal? ResolveFromCount(HtmlNode[] trDescedants)
        {
            try
            {
                var str = trDescedants
                   .First(x => x.HasClass("fs"))
                   .InnerText.Trim();
                var index = str.IndexOf(" ");
                str = str
                   .Remove(index)
                   .Replace(" ", "");
                var fromCount = JsonConvert.DeserializeObject<decimal>(str);
                return fromCount;
            }
            catch
            {
                return null;
            }
        }

        string ResolveFromCoinName(HtmlNode[] trDescedants)
        {
            try
            {
                var fromCoinName = trDescedants
                   .First(x => x.HasClass("fs"))
                   .ChildNodes
                   .First(x => x.OriginalName == "small")
                   .InnerText.Trim();
                return fromCoinName;
            }
            catch
            {
                return null;
            }
        }

        decimal? ResolveToCount(HtmlNode[] trDescedants)
        {
            try
            {
                var str = trDescedants
                   .First(x => x.HasClass("bi") && x.ChildNodes.Any(r => r.OriginalName == "small"))
                   .InnerText.Trim();
                var index = str.IndexOf(" ");
                str = str
                   .Remove(index)
                   .Replace(" ", "");
                var fromCount = JsonConvert.DeserializeObject<decimal>(str);
                return fromCount;
            }
            catch
            {
                return null;
            }
        }

        decimal? ResolveMin(HtmlNode[] trDescedants)
        {
            try
            {
                var str = trDescedants
                   .First(x => x.HasClass("fm1"))
                   .InnerText.Trim();
                var index = str.IndexOf(" ");
                str = str
                    .Substring(index)
                    .Replace(" ", "");
                var fromCount = JsonConvert.DeserializeObject<decimal>(str);
                return fromCount;
            }
            catch
            {
                return null;
            }
        }

        decimal? ResolveFund(HtmlNode[] trDescedants)
        {
            try
            {
                var str = trDescedants
                   .First(x => x.HasClass("ar") && x.HasClass("arp"))
                   .InnerText.Trim();
                str = str.Replace(" ", "");
                var fromCount = JsonConvert.DeserializeObject<decimal>(str);
                return fromCount;
            }
            catch
            {
                return null;
            }
        }

        string ResolveToCoinName(HtmlNode[] trDescedants)
        {
            try
            {
                var fromCoinName = trDescedants
                    .First(x => x.HasClass("bi") && x.ChildNodes.Any(r => r.OriginalName == "small"))
                    .ChildNodes
                    .First(x => x.OriginalName == "small")
                    .InnerText.Trim();
                return fromCoinName;
            }
            catch
            {
                return null;
            }
        }

        bool? ResolveIsFloatingRate(HtmlNode[] trDescedants)
        {
            try
            {
                var res = trDescedants
                    .Any(x => x.HasClass("floating"));
                return res;
            }
            catch
            {
                return null;
            }
        }

        bool? ResolveIsManual(HtmlNode[] trDescedants)
        {
            try
            {
                var res = trDescedants
                    .Any(x => x.HasClass("manual"));
                return res;
            }
            catch
            {
                return null;
            }
        }

        bool? ResolveNeedVerification(HtmlNode[] trDescedants)
        {
            try
            {
                var res = trDescedants
                    .Any(x => x.HasClass("verifying"));
                return res;
            }
            catch
            {
                return null;
            }
        }

        string ResolveExchangerUrl(HtmlNode[] trDescedants)
        {
            try
            {
                var aNode = trDescedants
                    .First((x) =>
                    {
                        if (x.OriginalName != "a")
                            return false;
                        if (x.GetAttributeValue("rel", null) != "nofollow")
                            return false;
                        return true;
                    });
                var url = aNode.GetAttributeValue("href", null);
                return url;
            }
            catch
            {
                return null;
            }
        }
    }
}
