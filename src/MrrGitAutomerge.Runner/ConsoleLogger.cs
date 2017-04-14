using MrrGitAutomerge.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Runner
{
    class ConsoleLogger : ILogger
    {
        public void Log(string text)
        {
            Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.f}: {text}");
        }
    }
}
