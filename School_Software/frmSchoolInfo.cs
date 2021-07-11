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
    public partial class frmSchoolInfo : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmSchoolInfo()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void frmSchoolInfo_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadRecord()
        {
            cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblschoolinfo", cn);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtSchoolName.Text = dr["name"].ToString();
                    txtPhoneNo.Text = dr["phone"].ToString();
                    txtEmail.Text = dr["email"].ToString();
                    txtMotto.Text = dr["motto"].ToString();
                    txtRegNo.Text = dr["regno"].ToString();
                    txtAddress.Text = dr["address"].ToString();
                }
                else
                {
                    txtSchoolName.Text = String.Empty;
                    txtPhoneNo.Text = String.Empty;
                    txtEmail.Text = String.Empty;
                    txtMotto.Text = String.Empty;
                    txtRegNo.Text = String.Empty;
                    txtAddress.Text = String.Empty;
                }
                dr.Close();
                cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtSchoolName.Text == String.Empty || txtPhoneNo.Text == String.Empty || txtEmail.Text == String.Empty || txtAddress.Text == String.Empty || txtMotto.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Save Info! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblschoolinfo", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblschoolinfo (name,phone,email,motto,regno,address) VALUES(@name,@phone,@email,@motto,@regno,@address)", cn);
                    cm.Parameters.AddWithValue("@name", txtSchoolName.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@motto", txtMotto.Text);
                    cm.Parameters.AddWithValue("@regno", txtRegNo.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("School Info has been saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtSchoolName.Clear();
            txtRegNo.Clear();
            txtPhoneNo.Clear();
            txtMotto.Clear();
            txtEmail.Clear();
            txtAddress.Clear();
            LoadRecord();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Delete Info! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblschoolinfo", cn);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("School Info has been deleted successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtPhoneNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
