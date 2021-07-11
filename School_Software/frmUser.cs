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
    public partial class frmUser : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string status,username;

        public frmUser()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbStatus.Checked == false)
                {
                    MessageBox.Show("Please select the user status!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtUsername.Text == String.Empty || txtFullName.Text == String.Empty || cboUsertype.Text == String.Empty || txtPassword.Text == String.Empty || txtCPassword.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtPassword.Text != txtCPassword.Text)
                {
                    txtPassword.Focus();
                    MessageBox.Show("Password do not match!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Create this Account! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tbluser WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("Username already taken!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tbluser (username,fullname,password,usertype,status) VALUES(@username,@fullname,@password,@usertype,@status)", cn);
                        cm.Parameters.AddWithValue("@username", txtUsername.Text);
                        cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                        cm.Parameters.AddWithValue("@password", txtPassword.Text);
                        cm.Parameters.AddWithValue("@usertype", cboUsertype.Text);
                        cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("User has been added successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Clear();
                        LoadRecord();
                        getTotalUser();
                    }
                    dr.Close();
                    cn.Close();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["fullname"].ToString(), dr["usertype"].ToString(), Convert.ToBoolean(dr["status"]));
            }
            dr.Close();
            cn.Close();
        }

        private void cbStatus_CheckedChanged(object sender, EventArgs e)
        {
            if (cbStatus.Checked == true)
            {
                cbStatus.Text = "Active";
            }
            else if (cbStatus.Checked == false)
            {
                cbStatus.Text = "In-Active";
            }
        }

        public void getTotalUser()
        {
            var num = dataGridView1.Rows.Count;
            lblTotal.Text = "Total User(s) Found: " + num;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            username = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove User! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tbluser WHERE username = @username", cn);
                    cm.Parameters.AddWithValue("@username", username);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("User has been removed successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                    getTotalUser();
                }
            }
            else if (ColName == "Column2")
            {
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;
                txtUsername.Enabled = false;

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tbluser WHERE username=@username", cn);
                cm.Parameters.AddWithValue("@username", dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString());
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtUsername.Text = dr["username"].ToString();
                    txtFullName.Text = dr["fullname"].ToString();
                    txtPassword.Text = dr["password"].ToString();
                    txtCPassword.Text = dr["password"].ToString();
                    cboUsertype.Text = dr["usertype"].ToString();
                    cbStatus.Checked = Convert.ToBoolean(dr["status"]); ;
                }
                else
                {
                    txtUsername.Text = String.Empty;
                    txtFullName.Text = String.Empty;
                    txtPassword.Text = String.Empty;
                    txtCPassword.Text = String.Empty;
                    cboUsertype.Text = String.Empty;
                    cbStatus.Text = String.Empty;
                }
                dr.Close();
                cn.Close();
                
            }
        }

        void Clear()
        {
            txtCPassword.Clear();
            txtPassword.Clear();
            txtFullName.Clear();
            txtUsername.Clear();
            cboUsertype.Text = "";
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            txtUsername.Enabled = true;
            txtUsername.Focus();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tbluser WHERE username LIKE '%" + txtSearch.Text + "' OR fullname LIKE '%" + txtSearch.Text + "%'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["username"].ToString(), dr["fullname"].ToString(), dr["usertype"].ToString(), Convert.ToBoolean(dr["status"]));
            }
            dr.Close();
            cn.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUsername.Text == String.Empty || txtFullName.Text == String.Empty || cboUsertype.Text == String.Empty || txtPassword.Text == String.Empty || txtCPassword.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtPassword.Text != txtCPassword.Text)
                {
                    txtPassword.Focus();
                    MessageBox.Show("Password do not match!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Update this Account! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                        cn.Open();
                        cm = new MySqlCommand("UPDATE tbluser SET fullname=@fullname,password=@password,usertype=@usertype,status=@status WHERE username=@username", cn);
                        cm.Parameters.AddWithValue("@username", txtUsername.Text);
                        cm.Parameters.AddWithValue("@fullname", txtFullName.Text);
                        cm.Parameters.AddWithValue("@password", txtPassword.Text);
                        cm.Parameters.AddWithValue("@usertype", cboUsertype.Text);
                        cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("User has been updated successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        Clear();
                        LoadRecord();
                        getTotalUser();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
