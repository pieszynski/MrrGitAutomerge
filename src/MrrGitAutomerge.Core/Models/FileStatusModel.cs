using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrrGitAutomerge.Core.Models
{
    [DebuggerDisplay("{Status} {RepositoryRootRelativePath}")]
    public class FileStatusModel
    {
        public string Status { get; set; }
        public string RepositoryRootRelativePath { get; set; }
    }
}
