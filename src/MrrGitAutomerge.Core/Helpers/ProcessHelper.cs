using MrrGitAutomerge.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core.Helpers
{
    public class ProcessHelper
    {
        protected ILogger Logger;
        protected Action<string> OnLogRowCallback;

        public ProcessHelper(ILogger logger, Action<string> onLogRowCallback)
        {
            if (null == logger)
                throw new ArgumentNullException(nameof(logger));

            this.Logger = logger;
            this.OnLogRowCallback = onLogRowCallback;
        }

        public Process RunPowershellScriptToLog(string script, string args, string workDir)
        {
            Process proc = this.RunAndLog(
                "powershell.exe",
                $"-NoProfile -ExecutionPolicy unrestricted -File \"{script}\" {args}",
                workDir
                );

            return proc;
        }

        public Process RunPowershellScript(string script, string args, string workDir)
        {
            Process proc = RunForResult(
                "powershell.exe",
                $"-NoProfile -ExecutionPolicy unrestricted -File \"{script}\" {args}",
                workDir
                );

            return proc;
        }

        public Process RunAndLog(string exe, string args, string workDir)
        {
            Process proc = Prepare(exe, args, workDir);
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += this.Proc_DataReceived;
            proc.ErrorDataReceived += this.Proc_DataReceived;
            proc.Exited += Proc_Exited;

            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();

            return proc;
        }

        private void Proc_Exited(object sender, EventArgs e)
        {
            Process proc = sender as Process;
            int exitCode = 0;
            int pid = proc?.Id ?? -1;
            if (null != proc)
            {
                exitCode = proc.ExitCode;
                proc.OutputDataReceived -= this.Proc_DataReceived;
                proc.ErrorDataReceived -= this.Proc_DataReceived;
                proc.Exited -= this.Proc_Exited;
            }
            this.Logger.Log($"[PID {pid}] Finished with exit code {exitCode}.");
        }

        private void Proc_DataReceived(object sender, DataReceivedEventArgs e)
        {
            Process proc = sender as Process;
            int pid = proc?.Id ?? -1;

            this.Logger.Log($"[PID {pid}] {e.Data}");
            this.OnLogRowCallback?.Invoke(e.Data);
        }

        public static Process RunForResult(string exe, string args, string workDir)
        {
            Process proc = Prepare(exe, args, workDir);
            proc.Start();
            return proc;
        }

        static Process Prepare(string exe, string args, string workDir)
        {
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(exe, args)
            {
                WorkingDirectory = workDir,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            return proc;
        }
    }
}
