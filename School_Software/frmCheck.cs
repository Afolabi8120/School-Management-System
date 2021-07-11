using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace School_Software
{
    public partial class frmCheck : Form
    {
        public frmCheck()
        {
            InitializeComponent();
        }

        private void btnFirstTerm_Click(object sender, EventArgs e)
        {
            var f1 = new frmExamScore();
            f1.getAllSection();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnSecondTerm_Click(object sender, EventArgs e)
        {
            var f1 = new frmExamScore();
            f1.cboTerm.Text = "Second Term";
            f1.getAllSection();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnThirdTerm_Click(object sender, EventArgs e)
        {
            var f1 = new frmExamScore2();
            f1.getAllSection();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
