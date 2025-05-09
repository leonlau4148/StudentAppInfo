using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentAppInfo
{
    internal class Prompt
    {
        public bool ShowPrompt(string message, string title)
        {
            DialogResult result = MessageBox.Show(message, title, MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }
    }
}
