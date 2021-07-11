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
    public partial class frmExpenses : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmExpenses()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        void Clear()
        {
            txtAmount.Clear();
            txtDescription.Clear();
            txtName.Clear();
            txtNote.Clear();
            txtTrackingID.Clear();
            getTrackingID();
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblexpense ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["trackingid"].ToString(), dr["name"].ToString(), dr["description"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["amount"].ToString(), dr["note"].ToString(), dr["approvedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAmount.Text == "0.00" || txtAmount.Text == "0")
                {
                    txtAmount.Focus();
                    MessageBox.Show("Please enter a valid amount!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtTrackingID.Text == String.Empty || txtName.Text == String.Empty || txtDescription.Text == String.Empty || txtAmount.Text == String.Empty || txtNote.Text == String.Empty || txtApprovedBy.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Save Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblexpense (trackingid,name,description,amount,note,date,time,approvedby) VALUES(@trackingid,@name,@description,@amount,@note,@date,@time,@approvedby)", cn);
                    cm.Parameters.AddWithValue("@trackingid", txtTrackingID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@description", txtDescription.Text);
                    cm.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cm.Parameters.AddWithValue("@note", txtNote.Text);
                    cm.Parameters.AddWithValue("@date", dateIssued.Text);
                    cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                    cm.Parameters.AddWithValue("@approvedby", frmLogin.fullname);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Expense Record has been saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTrackingID();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void getTrackingID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblexpense", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            var rand = new Random();
            int myrand = rand.Next(1, 900);
            txtTrackingID.Text = "E-" + DateTime.Now.Year + myrand + num;
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string trackingid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Delete Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblexpense WHERE trackingid = @trackingid", cn);
                    cm.Parameters.AddWithValue("@trackingid", trackingid);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Expense Record has been deleted successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTrackingID();
                }
            }
            else if (ColName == "Column2")
            {
                btnSave.Enabled = false;
                btnUpdate.Enabled = true;

                cn.Open();
                cm = new MySqlCommand("SELECT * FROM tblexpense WHERE trackingid=@trackingid", cn);
                cm.Parameters.AddWithValue("@trackingid", trackingid);
                dr = cm.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    txtTrackingID.Text = dr["trackingid"].ToString();
                    txtName.Text = dr["name"].ToString();
                    txtDescription.Text = dr["description"].ToString();
                    dateIssued.Text = dr["date"].ToString();
                    txtAmount.Text = dr["amount"].ToString();
                    txtNote.Text = dr["note"].ToString();
                }
                else
                {
                    txtTrackingID.Text = "";
                    txtName.Text = "";
                    txtDescription.Text = "";
                    dateIssued.Text = "";
                    txtAmount.Text = "";
                    txtNote.Text = "";
                }
                dr.Close();
                cn.Close();
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtAmount.Text == "0.00" || txtAmount.Text == "0")
                {
                    txtAmount.Focus();
                    MessageBox.Show("Please enter a valid amount!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtTrackingID.Text == String.Empty || txtName.Text == String.Empty || txtDescription.Text == String.Empty || txtAmount.Text == String.Empty || txtNote.Text == String.Empty || txtApprovedBy.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Update Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblexpense SET name=@name,description=@description,amount=@amount,note=@note,date=@date,time=@time,approvedby=@approvedby WHERE trackingid=@trackingid", cn);
                    cm.Parameters.AddWithValue("@trackingid", txtTrackingID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@description", txtDescription.Text);
                    cm.Parameters.AddWithValue("@amount", txtAmount.Text);
                    cm.Parameters.AddWithValue("@note", txtNote.Text);
                    cm.Parameters.AddWithValue("@date", dateIssued.Text);
                    cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                    cm.Parameters.AddWithValue("@approvedby", frmLogin.fullname);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Expense Record has been updated successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTrackingID();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblexpense WHERE date BETWEEN '" + dtFrom.Text + "' AND '" + dtTo.Text + "' ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["trackingid"].ToString(), dr["name"].ToString(), dr["description"].ToString(), dr["date"].ToString(), dr["time"].ToString(), dr["amount"].ToString(), dr["note"].ToString(), dr["approvedby"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnPrintPreview_Click(object sender, EventArgs e)
        {
            var f1 = new frmExpenseReport();
            f1.date1 = dtFrom.Text;
            f1.date2 = dtTo.Text;
            f1.LoadHeader();
            f1.LoadReceipt();
            f1.ShowDialog();
        }

        private void frmExpenses_Load(object sender, EventArgs e)
        {
            txtApprovedBy.Text = frmLogin.fullname;
        }
    }
}
