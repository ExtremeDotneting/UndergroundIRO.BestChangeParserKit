using System;
using System.Collections.Generic;
using System.Net.Http;
using IRO.CmdLine;
using IRO.XWebView.CefSharp.OffScreen.Providers;
using IRO.XWebView.Core;
using IRO.XWebView.Core.Consts;
using Newtonsoft.Json;
using UndergroundIRO.BestChangeParserKit;
using UndergroundIRO.ParseKit.Network;

namespace UndergroundIRO.Tests.BestChangeParserKitTest
{
    public class TestCmdLine : CommandLineBase
    {
        int _callNum = 1;

        IXWebView _xwv;

        public TestCmdLine(CmdLineExtension cmdLineExtension = null) : base(cmdLineExtension)
        {
        }

        [CmdInfo]
        public void Test()
        {

            var useXWV = ReadResource<bool>("Use XWebView as http service?");
            IHttpService http;
            if (useXWV)
            {
                 //XWV client used to execute scripts and prevent ban.
                if (_xwv == null)
                {
                    _xwv=new OffScreenCefSharpXWebViewProvider().Resolve(XWebViewVisibility.Hidden).Result;
                }
                http = new XWebViewHttpService(_xwv);
            }
            else
            {
                http = new HttpService(new HttpClient());
            }

            var parser = new BestChangeParser(http);
            var list = parser.Parse("https://www.bestchange.ru/bitcoin-to-bitcoin-cash.html").Result;
            Cmd.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));
            Cmd.WriteLine("Call number: " + _callNum++);
            Cmd.WriteLine("---------------");
        }
    }
}
