using MrrGitAutomerge.Core;
using MrrGitAutomerge.Core.Contracts;
using MrrGitAutomerge.Core.Helpers;
using MrrGitAutomerge.Core.Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MrrGitAutomerge.Runner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //string workDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            string workDir = @"..\..\..\..\..\wt";

            ILogger logger = new ConsoleLogger();
            MrrGitAutomergeUtil mrr = new MrrGitAutomergeUtil(logger);

            //var msgs = mrr.GetLastTenMessages(workDir);
            //var stat = mrr.GetRepoStatus(workDir);
            //bool bCommited = mrr.CommitWorkToGit(workDir, "test msg", "README.md,src/MrrGitAutomerge.Core/Models/,src/MrrGitAutomerge.Runner/Program.cs".Split(',').ToList());
            CommitAndAutomergeDialog caad = new CommitAndAutomergeDialog(mrr, workDir, "master");
            Window wnd = new Window
            {
                Title = "wnd",
                Content = caad,
                Width = 600 + 20,
                Height = 210 + 40
            };
            wnd.ShowDialog();
        }
    }
}
