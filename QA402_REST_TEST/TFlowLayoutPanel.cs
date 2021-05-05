using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QA402_REST_TEST
{
    class TFlowLayoutPanel : FlowLayoutPanel
    {
        public TFlowLayoutPanel() : base()
        {
            Dock = DockStyle.Fill;
            Padding = new Padding(10);
        }
    }
}
