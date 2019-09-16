using System;
using System.Collections.Generic;
using System.Net.Http;
using IRO.CmdLine;
using IRO.XWebView.CefSharp.OffScreen.Providers;
using Newtonsoft.Json;
using UndergroundIRO.BestChangeParserKit;

namespace UndergroundIRO.Tests.BestChangeParserKitTest
{
    public class TestCmdLine : CommandLineBase
    {
        int _callNum = 1;

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
                http = new XWebViewHttpService(new OffScreenCefSharpXWebViewProvider());
            }
            else
            {
                http = new HttpService(new HttpClient());
            }

            var parser = new BestChangeParser();
            var list = parser.Parse("https://www.bestchange.ru/bitcoin-to-bitcoin-cash.html").Result;
            Cmd.WriteLine(JsonConvert.SerializeObject(list, Formatting.Indented));
            Cmd.WriteLine("Call number: " + _callNum++);
            Cmd.WriteLine("---------------");
        }
    }
}
