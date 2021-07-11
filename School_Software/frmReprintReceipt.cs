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
    public partial class frmReprintReceipt : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmReprintReceipt()
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
            cm = new MySqlCommand("SELECT * FROM tblfeerecord ORDER BY name ASC ", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["term"].ToString(), dr["section"].ToString(), dr["date"].ToString(), dr["time"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord2 WHERE class=@class ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["date"].ToString(), dr["time"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string paymentid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColReprint")
            {
                if (MessageBox.Show("Reprint Receipt?! Click Yes to Confirm\nNOTE: This may take some time.\nThank You!!!", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    var f1 = new frmFeeReceipt();
                    f1.paymentid = paymentid;
                    f1.LoadHeader();
                    f1.LoadReceipt();
                    f1.ShowDialog();
                }
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE admissionno LIKE '%" + txtSearch.Text + "%' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["paymentid"].ToString(), dr["admissionno"].ToString(), dr["name"].ToString(), dr["class"].ToString(), dr["date"].ToString(), dr["time"].ToString());
            }
            dr.Close();
            cn.Close();
        }

    }
}
