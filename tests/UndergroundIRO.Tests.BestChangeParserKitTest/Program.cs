using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.OffScreen;
using IRO.CmdLine;
using IRO.XWebView.CefSharp.OffScreen.Providers;
using IRO.XWebView.CefSharp.OffScreen.Utils;
using IRO.XWebView.CefSharp.Utils;
using IRO.XWebView.Core.Utils;

namespace UndergroundIRO.Tests.BestChangeParserKitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            InitializeCefSharp();
            XWebViewThreadSync.Init(new CefSharpThreadSyncInvoker());
            var cmds = new CmdSwitcher();
            cmds.PushCmdInStack(new TestCmdLine());
            cmds.ExecuteStartup(args);
            cmds.RunDefault();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static void InitializeCefSharp()
        {
            var settings = new CefSettings();
            settings.BrowserSubprocessPath = Path.Combine(
                AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                Environment.Is64BitProcess ? "x64" : "x86",
                "CefSharp.BrowserSubprocess.exe"
            );
            CefHelpers.AddDefaultSettings(settings);
            settings.RemoteDebuggingPort = 9222;
            settings.LogSeverity = LogSeverity.Disable;
            Cef.Initialize(settings, false, browserProcessHandler: null);
        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }
    }
}
