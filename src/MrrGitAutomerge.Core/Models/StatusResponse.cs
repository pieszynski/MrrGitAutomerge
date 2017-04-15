using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core.Models
{
    [DebuggerDisplay("{Branch}")]
    public class StatusResponse
    {
        public string Branch { get; set; }
        public List<FileStatusModel> UncommitedFiles { get; set; } = new List<FileStatusModel>();

        public bool HasUncommitedData => 0 < this.UncommitedFiles.Count;
    }
}
