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
    public partial class frmSettings : Form
    {
        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSchoolInfo_Click(object sender, EventArgs e)
        {
            var f1 = new frmSchoolInfo();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnFeeType_Click(object sender, EventArgs e)
        {
            var f1 = new frmFeeType();
            f1.LoadRecord();
            f1.getTotalRecord();
            f1.ShowDialog();
        }

        private void btnReprint_Click(object sender, EventArgs e)
        {
            var f1 = new frmReprintReceipt();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnExpenses_Click(object sender, EventArgs e)
        {
            var f1 = new frmExpenses();
            f1.getTrackingID();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnBackup_Click(object sender, EventArgs e)
        {
            var f1 = new frmBackupRestore();
            f1.ShowDialog();
        }
    }
}
