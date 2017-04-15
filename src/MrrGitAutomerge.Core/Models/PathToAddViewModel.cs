using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MrrGitAutomerge.Core.Models
{
    public class PathToAddViewModel
    {
        public Brush ForeColor { get; set; } = Brushes.Black;
        public bool IsChecked { get; set; }
        public string GitStatus { get; set; }
        public string RelPath { get; set; }
    }
}
