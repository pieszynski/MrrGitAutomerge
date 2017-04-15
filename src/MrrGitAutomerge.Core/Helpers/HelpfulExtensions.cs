using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core.Helpers
{
    public static class HelpfulExtensions
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

        public static List<string> DataToList(this string dataOutput)
        {
            if (string.IsNullOrEmpty(dataOutput))
                return null;

            List<string> response = dataOutput.Split('\n')
                .Where(w => null != w)
                .Select(s => s.TrimEnd())
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();

            return response;
        }
    }
}
