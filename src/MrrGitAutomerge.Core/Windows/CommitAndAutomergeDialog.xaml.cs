using MrrGitAutomerge.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MrrGitAutomerge.Core.Windows
{
    /// <summary>
    /// Interaction logic for CommitAndAutomergeDialog.xaml
    /// </summary>
    public partial class CommitAndAutomergeDialog : UserControl
    {
        protected enum ThisDialogState
        {
            InvalidState,
            Loading,
            Automerge,
            Commit,
            Logs
        }

        protected readonly string WorkDir;
        protected readonly MrrGitAutomergeUtil Mrr;

        protected CommitAndAutomergeViewModel Model = new CommitAndAutomergeViewModel();

        public CommitAndAutomergeDialog(MrrGitAutomergeUtil automergeUtil, string workDir, string mergeToBranch)
        {
            if (null == automergeUtil)
                throw new ArgumentNullException(nameof(automergeUtil));
            if (string.IsNullOrEmpty(workDir))
                throw new ArgumentNullException(nameof(workDir));

            this.WorkDir = workDir;
            this.Model.MergeBranch = mergeToBranch;
            this.Mrr = automergeUtil;

            InitializeComponent();
            this.DataContext = this.Model;

        }

        private void CommitAndAutomerge_Loaded(object sender, RoutedEventArgs e)
        {
            this.ReloadAsync();
        }

        protected async void ReloadAsync()
        {
            this.Model.LoadingTitle = CommitAndAutomergeViewModel.LOADING;
            this.SetState(ThisDialogState.Loading);
            this.Model.LastMessages.Add("Loading messages...");

            Task<List<string>> tMessages = Task.Run(() => this.Mrr.GetLastTenMessages(this.WorkDir));
            Task<StatusResponse> tStatus = Task.Run(() => this.Mrr.GetRepoStatus(this.WorkDir));
            Task<List<string>> tLocalBranches = Task.Run(() => this.Mrr.ListBranches(this.WorkDir));

            await Task.WhenAll(tMessages, tStatus, tLocalBranches);

            List<string> lastMessages = tMessages.Result;
            StatusResponse status = tStatus.Result;
            List<string> localBranches = tLocalBranches.Result;

            if (null == status || null == localBranches)
            {
                this.SetState(ThisDialogState.InvalidState);
                return;
            }

            this.Model.LastMessages.Clear();
            lastMessages.ForEach(this.Model.LastMessages.Add);

            this.Model.FilesToCommit.Clear();
            status.UncommitedFiles
                .Select(s => new PathToAddViewModel
                {
                    IsChecked = true,
                    GitStatus = $"({s.Status?.Trim()})",
                    ForeColor = this.GetBrushForStatus(s.Status),
                    RelPath = s.RepositoryRootRelativePath
                })
                .ToList()
                .ForEach(this.Model.FilesToCommit.Add);

            string mergeBranch = this.Model.MergeBranch;
            this.Model.LocalBranches.Clear();
            if (localBranches.Contains(mergeBranch))
            {
                this.Model.LocalBranches.Add(mergeBranch);
                localBranches.Remove(mergeBranch);
                this.Model.MergeBranch = mergeBranch;
            }
            localBranches.ForEach(this.Model.LocalBranches.Add);

            this.SetState(status.HasUncommitedData ? ThisDialogState.Commit : ThisDialogState.Automerge);
        }

        protected void SetState(ThisDialogState state)
        {
            this.LoadingPanel.Visibility = Visibility.Hidden;
            this.CommitPanel.Visibility = Visibility.Hidden;
            this.AutomergePanel.Visibility = Visibility.Hidden;
            this.InvalidStatePanel.Visibility = Visibility.Hidden;
            this.LogPanel.Visibility = Visibility.Hidden;

            switch (state)
            {
                case ThisDialogState.Automerge:
                    this.AutomergePanel.Visibility = Visibility.Visible;
                    break;
                case ThisDialogState.Commit:
                    this.CommitPanel.Visibility = Visibility.Visible;
                    break;
                case ThisDialogState.Loading:
                    this.LoadingPanel.Visibility = Visibility.Visible;
                    break;
                case ThisDialogState.Logs:
                    this.LogPanel.Visibility = Visibility.Visible;
                    break;
                case ThisDialogState.InvalidState:
                default:
                    this.InvalidStatePanel.Visibility = Visibility.Visible;
                    break;
            }
        }

        protected Brush GetBrushForStatus(string gitStatus)
        {
            string st = gitStatus?.Trim().ToLowerInvariant();
            switch (st)
            {
                case "??": return Brushes.Blue;
                case "d": return Brushes.Red;
                default: return Brushes.Black;
            }
        }

        protected void WindowlogWriteLine(string text)
        {
            this.Model.LogWindowText += text + "\r\n";
        }

        private async void Commit_Click(object sender, RoutedEventArgs e)
        {
            this.Model.LoadingTitle = CommitAndAutomergeViewModel.COMMITING_CHANGES;
            this.SetState(ThisDialogState.Loading);

            List<string> lFiles = this.Model.FilesToCommit
                .Where(w => w.IsChecked)
                .Select(s => s.RelPath)
                .ToList();
            
            bool bIsOk = await Task.Run(() => this.Mrr.CommitWorkToGit(
                this.WorkDir,
                this.Model.Message,
                lFiles
                ));

            this.ReloadAsync();
        }

        private async void Automerge_Click(object sender, RoutedEventArgs e)
        {
            this.Model.LoadingTitle = CommitAndAutomergeViewModel.MERGING;
            this.SetState(ThisDialogState.Logs);

            this.Model.LogWindowText = string.Empty;
            this.Model.IsSubprocessRunning = true;
            bool bStatus = await Task.Run(() => this.Mrr.RunAutomergeScript(
                this.WorkDir,
                this.Model.MergeBranch,
                row => this.WindowlogWriteLine(row)
                ));

            this.Model.IsSubprocessRunning = false;
            this.WindowlogWriteLine(bStatus ? "Automerge successful." : "Automerge FAILED! Please check log for help.");
            
        }

        private void LogWindow_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ScrollViewer sv = sender as ScrollViewer;
            if (null != sv && 0 < e.ExtentHeightChange)
            {
                sv.ScrollToBottom();
            }
        }
    }
}
