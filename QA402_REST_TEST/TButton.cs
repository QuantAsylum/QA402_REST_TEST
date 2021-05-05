using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QA402_REST_TEST
{
    public class TButton : Button
    {
        public delegate void UserMethod();
        UserMethod UMethod;

        public TButton(string text, UserMethod method) : base()
        {
            AutoSize = true;
            Text = text;
            UMethod = method;
            Click += TButton_Click;
        }

        private void TButton_Click(object sender, EventArgs e)
        {
           UMethod?.Invoke();
        }
    }
}
