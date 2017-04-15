using MrrGitAutomerge.Core.Contracts;
using MrrGitAutomerge.Core.Helpers;
using MrrGitAutomerge.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core
{
    public class MrrGitAutomergeUtil
    {
        protected readonly Regex BranchRegex = new Regex("^## ([^.]*).*?$", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Compiled);

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
            string dataOutput = this.RunUtilCommand(workDir, "last10messages");
            if (null == dataOutput)
                return null;

            List<string> response = dataOutput.DataToList();
            return response;
        }

        public StatusResponse GetRepoStatus(string workDir)
        {
            string dataOutput = this.RunUtilCommand(workDir, "repostatus");
            if (null == dataOutput)
                return null;

            List<string> cmdData = dataOutput.DataToList();
            if (null == cmdData || 0 == cmdData.Count)
                return null;

            // ## master...origin/master [ahead 1]
            //  M README.md
            // R  src/MrrGitAutomerge.Core/Helpers/ProcessExtensions.cs -> src/MrrGitAutomerge.Core/Helpers/HelpfulExtensions.cs
            //  D src/MrrGitAutomerge.Core/Helpers/ProcessExtensions.cs
            //  M src/MrrGitAutomerge.Core/MrrGitAutomerge.Core.csproj
            //  M src/MrrGitAutomerge.Runner/Program.cs
            // ?? src/MrrGitAutomerge.Core/Resources/deleteme.txt
            var mtc = BranchRegex.Match(cmdData.FirstOrDefault());
            if (!mtc.Success)
                return null;

            StatusResponse response = new StatusResponse
            {
                Branch = mtc.Groups[1].Value
            };
            foreach (string item in cmdData.Skip(1))
            {
                if (string.IsNullOrEmpty(item) || 4 > item.Length)
                    continue;

                string fileRelPath = item.Substring(2);
                int arrowPos = fileRelPath.IndexOf(" ->");
                if (0 < arrowPos)
                    fileRelPath = fileRelPath.Substring(0, arrowPos);

                FileStatusModel fileModel = new FileStatusModel
                {
                    Status = item.Substring(0, 2),
                    RepositoryRootRelativePath = fileRelPath
                };
                response.UncommitedFiles.Add(fileModel);
            }

            return response;
        }

        public bool CommitWorkToGit(string workDir, string message, List<string> lFiles)
        {
            if (string.IsNullOrEmpty(message) || 0 == lFiles?.Count)
                return false;

            string dataOutput = this.RunUtilCommand(
                workDir, 
                "commitwork", 
                message: message, 
                lFiles: lFiles
                );
            
            return null != dataOutput;
        }

        protected string RunUtilCommand(string workDir, string command, 
            string message = null,
            List<string> lFiles = null)
        {
            string args = $"-option {command}";

            if (!string.IsNullOrEmpty(message))
            {
                args += $" -message \"{message.Replace("\"", string.Empty).Trim()}\"";
            }

            if (null != lFiles && 0 < lFiles.Count)
            {
                string filesArg = string.Join(",",
                    lFiles.Select(s => s.Replace("\"", string.Empty))
                        .Select(s => s.Replace("\\", string.Empty))
                    );
                args += $" -files \"{filesArg}\"";
            }

            this.Logger.Log($"RUN: {Path.GetFileName(this.UtilScript)} {args}");
            ProcessHelper ph = new ProcessHelper(this.Logger, null);
            Process proc = ph.RunPowershellScript(
                this.UtilScript,
                args,
                workDir
                );
            
            int exitCode = proc.ExitIn(60);
            string dataOutput = proc.StandardOutput.ReadToEnd() + proc.StandardError.ReadToEnd();
            this.Logger.Log(dataOutput);

            if (0 != exitCode)
            {
                this.Logger.Log($"ERROR (exitCode:{exitCode}): {dataOutput}");
                return null;
            }

            return dataOutput;
        }
    }
}
