using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using MrrGitAutomerge.Core.Contracts;
using System;
using System.Diagnostics;

namespace MrrGitAutomerge
{
    public class ExtensionLogger : ILogger
    {
        private static readonly Guid PaneGuid = Guid.Parse("EF175E2B-DE99-4353-92CC-C994318E778C");

        private static IVsOutputWindowPane pane;
        private static IServiceProvider _provider;

        public static void Initialize(IServiceProvider provider)
        {
            _provider = provider;
        }

        public void Log(string text)
        {
            LogStatic(text);
        }

        public static void LogStatic(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            try
            {
                if (CreatePaneIfNone())
                    pane.OutputString($"{DateTime.Now:yyyyMMdd HH:mm:ss}: {message}\r\n");
            }
            catch { }
        }

        static bool CreatePaneIfNone()
        {
            if (null == pane)
            {
                Guid guid = PaneGuid;
                IVsOutputWindow output = (IVsOutputWindow)_provider.GetService(typeof(SVsOutputWindow));
                if (VSConstants.S_OK != output.GetPane(ref guid, out pane))
                {
                    output.CreatePane(ref guid, "Mrr GIT Automerge", 1, 1);
                    output.GetPane(ref guid, out pane);
                }
            }

            return null != pane;
        }
    }
}
