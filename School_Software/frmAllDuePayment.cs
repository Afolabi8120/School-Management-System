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
    public partial class frmAllDuePayment : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmAllDuePayment()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE due < 0 ORDER BY name ASC", cn);
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

        private void txtAdmissionNo_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE admissionno LIKE '%" + txtAdmissionNo.Text + "%' AND due < 0 ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE class=@class AND due < 0 ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE term=@term AND due < 0 ORDER BY name ASC", cn);
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

        private void cboSection_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE section=@section AND due < 0 ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@section", cboSection.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void txtPaymentID_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE paymentid LIKE '%" + txtPaymentID.Text + "%' AND due < 0 ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["section"].ToString(), dr["term"].ToString(), dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["receivedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string paymentid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string admissionno = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

            if (ColName == "ColPay")
            {
                if (MessageBox.Show("Make Payment! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var f1 = new frmPayDue();
                    f1.btnAdd.Enabled = false;
                    f1.paymentid = paymentid;
                    f1.admissionno = admissionno;
                    f1.getPaymentRecord();
                    f1.getPictureClass();
                    f1.LoadRecord();
                    f1.ShowDialog();
                }
            }
        }
    }
}
