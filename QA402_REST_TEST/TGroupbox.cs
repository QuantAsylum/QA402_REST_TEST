using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QA402_REST_TEST
{
    class TGroupBox : GroupBox
    {
        public TGroupBox(string text) : base()
        {
            Text = text;
            Dock = DockStyle.Fill;
            Margin = new Padding(10);
        }
    }
}
