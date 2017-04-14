using MrrGitAutomerge.Core.Contracts;
using MrrGitAutomerge.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core
{
    public class MrrGitAutomergeUtil
    {
        protected ILogger Logger;
        protected string UtilScript;

        public MrrGitAutomergeUtil(ILogger logger)
        {
            if (null == logger)
                throw new ArgumentNullException(nameof(logger));

            this.Logger = logger;

            this.Init();
        }

        protected void Init()
        {
            string dir = Path.GetDirectoryName(typeof(MrrGitAutomergeUtil).Assembly.Location);
            string utilScriptPath = Path.Combine(dir, "utils.ps1");
            File.WriteAllBytes(utilScriptPath, CoreResource.utils);
            if (File.Exists(utilScriptPath))
                this.UtilScript = utilScriptPath;
        }

        public List<string> GetLastTenMessages(string workDir)
        {
            ProcessHelper ph = new ProcessHelper(this.Logger, null);
            Process proc = ph.RunPowershellScript(
                this.UtilScript,
                "last10messages",
                workDir
                );

            proc.ExitIn(60);
            string dataOutput = proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
            List<string> response = dataOutput.Split('\n')
                .Select(s => s?.Trim())
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();
            return response;
        }
    }
}
