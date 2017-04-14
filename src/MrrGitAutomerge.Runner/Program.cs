using MrrGitAutomerge.Core;
using MrrGitAutomerge.Core.Contracts;
using MrrGitAutomerge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            string workDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);

            ILogger logger = new ConsoleLogger();
            MrrGitAutomergeUtil mrr = new MrrGitAutomergeUtil(logger);

            var msgs = mrr.GetLastTenMessages(workDir);
        }
    }
}
