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
    public partial class frmFeeType : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmFeeType()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void getTotalRecord()
        {
            var num = dataGridView1.Rows.Count;
            lblTotal.Text = "Total Subjects Found: " + num;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtName.Text == String.Empty)
                {
                    txtName.Focus();
                    MessageBox.Show("Please enter a valid subject name!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (txtPrice.Text == String.Empty || txtPrice.Text == "0.00" || txtPrice.Text == "0")
                {
                    txtName.Focus();
                    MessageBox.Show("Please enter a valid price!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cboClass.Text == String.Empty)
                {
                    cboClass.Focus();
                    MessageBox.Show("Please select a valid class!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Add Fee Type! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblfeetype WHERE name = @name AND class = @class", cn);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@class", cboClass.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("A fee name has been added for the selected class!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblfeetype (name,class,price) VALUES(@name,@class,@price)", cn);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@class", cboClass.Text);
                        cm.Parameters.AddWithValue("@price", txtPrice.Text);
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Subject has been added successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRecord();
                        getTotalRecord();
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
            cm = new MySqlCommand("SELECT * FROM tblfeetype", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["name"].ToString(), dr["class"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string name = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string type = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Fee! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblfeetype WHERE name = @name AND class=@class", cn);
                    cm.Parameters.AddWithValue("@name", name);
                    cm.Parameters.AddWithValue("@class", type);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Fee has been removed successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                    getTotalRecord();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            cboClass.Text = String.Empty;
            txtName.Focus();
        }

        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}


