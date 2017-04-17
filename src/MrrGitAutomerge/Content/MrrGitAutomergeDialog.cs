using Microsoft.VisualStudio.PlatformUI;
using MrrGitAutomerge.Core;
using MrrGitAutomerge.Core.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Content
{
    public class MrrGitAutomergeDialog : DialogWindow
    {
        CommitAndAutomergeDialog Caad;

        public MrrGitAutomergeDialog(MrrGitAutomergeUtil automergeUtil, string workDir, string mergeToBranch)
        {
            this.Title = "Mrr GIT Automerge";
            this.Caad = new CommitAndAutomergeDialog(automergeUtil, workDir, mergeToBranch);
            this.Content = this.Caad;
            this.Width = 600 + 20;
            this.Height = 210 + 40;
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

            this.HasMaximizeButton = true;
            this.HasMinimizeButton = true;
        }

        public string SelectedMergeToBranch => this.Caad.SelectedMergeToBranch;
    }
}
