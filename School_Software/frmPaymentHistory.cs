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
    public partial class frmPaymentHistory : Form
    {

        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmPaymentHistory()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord2 ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void frmPaymentHistory_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        void getStudentDetails()
        {
            cboName.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE class=@class", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboName.Items.Add(dr["studentname"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void getAllSection()
        {
            cboSection.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsection ORDER BY name ASc", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboSection.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            getStudentDetails();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord2 WHERE class=@class AND name=@name AND section=@section AND term=@term ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            cm.Parameters.AddWithValue("@name", cboName.Text);
            cm.Parameters.AddWithValue("@section", cboSection.Text);
            cm.Parameters.AddWithValue("@term", cboTerm.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
                var f1 = new frmFeePaymentReport();
                f1._class = cboClass.Text;
                f1._name = cboName.Text;
                f1._section = cboSection.Text;
                f1._term = cboTerm.Text;
                f1.LoadHeader();
                f1.LoadReceipt();
                f1.ShowDialog();
        }

       
    }
}
