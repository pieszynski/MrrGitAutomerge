using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MrrGitAutomerge.Core.Models
{
    public class CommitAndAutomergeViewModel : INotifyPropertyChanged
    {
        public const string LOADING = "Loading";
        public const string COMMITING_CHANGES = "Commiting changes";
        public const string MERGING = "Merging";

        private string _logWindowText;
        public string LogWindowText
        {
            get { return this._logWindowText; }
            set { this._logWindowText = value; this.Notify(nameof(LogWindowText)); }
        }
        private bool _isSubprocessRunning;
        public bool IsSubprocessRunning
        {
            get { return this._isSubprocessRunning; }
            set { this._isSubprocessRunning = value; this.Notify(nameof(IsSubprocessRunning)); this.Notify(nameof(SubprocessRunningVisibility)); }
        }
        public Visibility SubprocessRunningVisibility => this.IsSubprocessRunning ? Visibility.Visible : Visibility.Hidden;

        private string _loadingTitle;
        public string LoadingTitle
        {
            get { return this._loadingTitle; }
            set { this._loadingTitle = value; this.Notify(nameof(LoadingTitle)); }
        }
        private string _mergeBranch;
        public string MergeBranch
        {
            get { return this._mergeBranch; }
            set { this._mergeBranch = value; this.Notify(nameof(MergeBranch)); }
        }
        private object _messageObj;
        public object MessageObj
        {
            get { return this._messageObj; }
            set { this._messageObj = value; this.Notify(nameof(MessageObj)); }
        }
        public string Message => this.MessageObj?.ToString();
        public ObservableCollection<string> LastMessages { get; set; } = new ObservableCollection<string>();
        public ObservableCollection<PathToAddViewModel> FilesToCommit { get; set; } = new ObservableCollection<PathToAddViewModel>();
        public ObservableCollection<string> LocalBranches { get; set; } = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public CommitAndAutomergeViewModel()
        {
            this.LoadingTitle = LOADING;
        }

        private void Notify(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
