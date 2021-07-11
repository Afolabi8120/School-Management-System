using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace School_Software
{
    public partial class frmResultManagement : Form
    {

        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmResultManagement()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void frmResultManagement_Load(object sender, EventArgs e)
        {

        }

        public void getSection()
        {
            cboSection.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsection", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboSection.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (cboTerm.Text == "Third Term")
            {
                var f1 = new frmPrintResult2();
                f1._admissionno = txtAdmissionNo.Text;
                f1._class = cboClass.Text;
                f1._section = cboSection.Text;
                f1._term = cboTerm.Text;
                f1.LoadHeader();
                f1.LoadReceipt();
                f1.ShowDialog();
            }
            else
            {
                var f1 = new frmPrintResult();
                f1._admissionno = txtAdmissionNo.Text;
                f1._class = cboClass.Text;
                f1._section = cboSection.Text;
                f1._term = cboTerm.Text;
                f1.LoadHeader();
                f1.LoadReceipt();
                f1.ShowDialog();
            }
        }

    }
}
