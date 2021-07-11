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
    public partial class frmSubjects : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmSubjects()
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
            cm = new MySqlCommand("SELECT * FROM tblsubject", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["name"].ToString(), dr["type"].ToString());
            }
            dr.Close();
            cn.Close();
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

                if (cboType.Text == String.Empty)
                {
                    cboType.Focus();
                    MessageBox.Show("Please select a valid subject type!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Add Subject! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblsubject WHERE name = @name AND type = @type", cn);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@type", cboType.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("A subject has been added for the selected type!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblsubject (name,type) VALUES(@name,@type)", cn);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@type", cboType.Text);
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            cboType.Text = String.Empty;
            txtName.Focus();
        }

        public void getTotalRecord()
        {
            var num = dataGridView1.Rows.Count;
            lblTotal.Text = "Total Subjects Found: " + num;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string name = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            string type = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Subject! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblsubject WHERE name = @name AND type=@type", cn);
                    cm.Parameters.AddWithValue("@name", name);
                    cm.Parameters.AddWithValue("@type", type);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Subject has been removed successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                    getTotalRecord();
                }
            }
        }
    }
}
