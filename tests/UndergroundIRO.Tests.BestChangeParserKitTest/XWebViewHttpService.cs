using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using IRO.XWebView.Core.Providers;
using IRO.XWebView.Extensions;
using UndergroundIRO.BestChangeParserKit;

namespace UndergroundIRO.Tests.BestChangeParserKitTest
{
    public class XWebViewHttpService:IHttpService
    {
        readonly IXWebView _xwv;

        readonly IXWebViewProvider _xwvProvider;

        public XWebViewHttpService(IXWebView xwv)
        {
            _xwv = xwv ?? throw new ArgumentNullException(nameof(xwv));
        }

        /// <summary>
        /// If you pass provider - xwv will be created and disposed on each request.
        /// </summary>
        /// <param name="xwvProvider"></param>
        public XWebViewHttpService(IXWebViewProvider xwvProvider)
        {
            _xwvProvider = xwvProvider ?? throw new ArgumentNullException(nameof(xwvProvider));
        }

        /// <summary>
        /// Encoding to get html string.
        /// </summary>
        public async Task<string> GetAsync(string url, Encoding enc = null)
        {
            var xwv = await GetXWV();
            await xwv.LoadUrl(url);
            var html=await xwv.GetHtml();
            ReleaseXWV(xwv);
            return html;
        }

        async Task<IXWebView> GetXWV()
        {
            if (_xwv != null)
                return _xwv;
            return await _xwvProvider.Resolve(XWebViewVisibility.Hidden);
        }

        void ReleaseXWV(IXWebView xwv)
        {
            if (_xwv != null)
                return;
            xwv.Dispose();
        }
    }
}
