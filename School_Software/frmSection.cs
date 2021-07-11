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
    public partial class frmSection : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public string section;

        public frmSection()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void frmSection_Load(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void txtFrom_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int num1 = Convert.ToInt32(txtFrom.Text);
                int num2 = num1 + 1;
                txtTo.Text = num2.ToString();

                txtSection.Text = num1.ToString() + "-" + num2.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("WARNING: " + ex.Message, "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtFrom.Text == String.Empty)
                {
                    txtFrom.Focus();
                    MessageBox.Show("Please input a valid section!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (MessageBox.Show("Create Section! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblsection WHERE name = @name", cn);
                    cm.Parameters.AddWithValue("@name", txtSection.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("This section already exist in the database!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("UPDATE tblsection SET status=@status", cn);
                        cm.Parameters.AddWithValue("@status", "CLOSE");
                        cm.ExecuteNonQuery();
                        cn.Close();

                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblsection (name,status) VALUES(@name,@status)", cn);
                        cm.Parameters.AddWithValue("@name", txtSection.Text);
                        cm.Parameters.AddWithValue("@status", "OPEN");
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Section has been saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadRecord();
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
            cm = new MySqlCommand("SELECT * FROM tblsection", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["name"].ToString(), dr["status"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string getsection = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Create Section! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblsection WHERE name = @name", cn);
                    cm.Parameters.AddWithValue("@name", getsection);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Section has been deleted successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            txtFrom.Clear();
            txtTo.Clear();
            txtSection.Clear();
            txtFrom.Focus();
        }
    }
}
