using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core.Helpers
{
    public static class ProcessExtensions
    {
        public static int ExitIn(this Process proc, int secTimeout)
        {
            if (null == proc)
                return -2;

            try
            {
                if (proc.HasExited)
                    return proc.ExitCode;

                if (!proc.WaitForExit(secTimeout * 1000))
                    proc.Kill();

                return proc.ExitCode;
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
