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
using System.IO;

namespace School_Software
{
    public partial class frmStaff : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string status;

        public frmStaff()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "image files(*.bmp;*.jpg;*.png;*.gif)|*.bmp*;*.jpg;*.png;*.gif;";

            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StaffPic.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            StaffPic.Image = StaffPic.InitialImage;
        }

        void Clear()
        {
            txtStaffID.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtName.Clear();
            cboQualification.Text = "";
            txtPhoneNo.Clear();
            cboReligion.Text = "";
            cboState.Text = "";
            cboDesignation.Text = "";
            cboGender.Text = "";            
            cboMaritalStatus.Text = "";
            StaffPic.Image = StaffPic.InitialImage;
            cbStatus.Checked = false;
            txtName.Focus();
        }

        public void getStaffID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstaff", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            var rand = new Random();
            int myrand = rand.Next(1, 200);
            txtStaffID.Text = "STF-" + DateTime.Now.Year + "-" + myrand + num;
        }

        public void getTotalStaff()
        {
            var num = dataGridView1.Rows.Count;
            lblTotal.Text = "Total Staff Found: " + num;
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstaff", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["staffid"].ToString(), dr["name"].ToString(), dr["designation"].ToString(), dr["gender"].ToString(), dr["dob"].ToString(), dr["mstatus"].ToString(), dr["religion"].ToString(), dr["email"].ToString(), Convert.ToBoolean(dr["status"]));
            }
            dr.Close();
            cn.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbStatus.Checked == false)
                {
                    MessageBox.Show("Please select staff status!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (StaffPic.Image == null || StaffPic.Image == StaffPic.InitialImage)
                {
                    MessageBox.Show("Please select a student image!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboState.Text == String.Empty || cboReligion.Text == String.Empty || cboDesignation.Text == String.Empty || txtAddress.Text == String.Empty || txtPhoneNo.Text == String.Empty || txtStaffID.Text == String.Empty || txtEmail.Text == String.Empty || cboQualification.Text == String.Empty || txtEmail.Text == String.Empty || txtName.Text == String.Empty || cboGender.Text == String.Empty || cboGender.Text == "-Select Option-")
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Create this Staff Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(StaffPic.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblstaff (staffid,name,gender,religion,dob,mstatus,qualification,state,picture,designation,phone,datejoined,address,email,status) VALUES(@staffid,@name,@gender,@religion,@dob,@mstatus,@qualification,@state,@picture,@designation,@phone,@datejoined,@address,@email,@status)", cn);
                    cm.Parameters.AddWithValue("@staffid", txtStaffID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@gender", cboGender.Text);
                    cm.Parameters.AddWithValue("@religion", cboReligion.Text);
                    cm.Parameters.AddWithValue("@dob", DOB.Text);
                    cm.Parameters.AddWithValue("@mstatus", cboMaritalStatus.Text);
                    cm.Parameters.AddWithValue("@qualification", cboQualification.Text);
                    cm.Parameters.AddWithValue("@state", cboState.Text);
                    cm.Parameters.AddWithValue("@picture", bytImage);                   
                    cm.Parameters.AddWithValue("@designation", cboDesignation.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                    cm.Parameters.AddWithValue("@datejoined", dateJoined.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Staff has been added successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTotalStaff();
                    getStaffID();
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
            btnSave.Enabled = true;
            btnUpdate.Enabled = false;
            Clear();
            txtName.Focus();
            getStaffID();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadRecord();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            try
            {
                Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
                Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
                Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
                app.Visible = true;
                worksheet = workbook.Sheets["Sheet1"];
                worksheet = workbook.ActiveSheet;
                worksheet.Name = "Records";

                try
                {
                    for (int i = 0; i < dataGridView1.Columns.Count; i++)
                    {
                        worksheet.Cells[1, i + 1] = dataGridView1.Columns[i].HeaderText;
                    }
                    for (int i = 0; i < dataGridView1.Rows.Count; i++)
                    {
                        for (int j = 0; j < dataGridView1.Columns.Count; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value != null)
                            {
                                worksheet.Cells[i + 2, j + 1] = dataGridView1.Rows[i].Cells[j].Value.ToString();
                            }
                            else
                            {
                                worksheet.Cells[i + 2, j + 1] = "";
                            }
                        }
                    }

                    //Getting the location and file name of the excel to save from user. 
                    SaveFileDialog saveDialog = new SaveFileDialog();
                    saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
                    saveDialog.FilterIndex = 2;

                    if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        workbook.SaveAs(saveDialog.FileName);
                        MessageBox.Show("Record has been exported Successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                finally
                {
                    app.Quit();
                    workbook = null;
                    worksheet = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstaff WHERE staffid LIKE '%" + txtSearch.Text + "%' OR name LIKE '%" + txtSearch.Text + "%'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["staffid"].ToString(), dr["name"].ToString(), dr["designation"].ToString(), dr["gender"].ToString(), dr["dob"].ToString(), dr["mstatus"].ToString(), dr["religion"].ToString(), dr["email"].ToString(), Convert.ToBoolean(dr["status"]));
            }
            dr.Close();
            cn.Close();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (StaffPic.Image == null || StaffPic.Image == StaffPic.InitialImage)
                {
                    MessageBox.Show("Please select a student image!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboState.Text == String.Empty || cboReligion.Text == String.Empty || cboDesignation.Text == String.Empty || txtAddress.Text == String.Empty || txtPhoneNo.Text == String.Empty || txtStaffID.Text == String.Empty || txtEmail.Text == String.Empty || cboQualification.Text == String.Empty || txtEmail.Text == String.Empty || txtName.Text == String.Empty || cboGender.Text == String.Empty || cboGender.Text == "-Select Option-")
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Update this Staff Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(StaffPic.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblstaff SET name=@name,gender=@gender,religion=@religion,dob=@dob,mstatus=@mstatus,qualification=@qualification,state=@state,picture=@picture,designation=@designation,phone=@phone,datejoined=@datejoined,address=@address,email=@email,status=@status WHERE staffid=@staffid", cn);
                    cm.Parameters.AddWithValue("@staffid", txtStaffID.Text);
                    cm.Parameters.AddWithValue("@name", txtName.Text);
                    cm.Parameters.AddWithValue("@gender", cboGender.Text);
                    cm.Parameters.AddWithValue("@religion", cboReligion.Text);
                    cm.Parameters.AddWithValue("@dob", DOB.Text);
                    cm.Parameters.AddWithValue("@mstatus", cboMaritalStatus.Text);
                    cm.Parameters.AddWithValue("@qualification", cboQualification.Text);
                    cm.Parameters.AddWithValue("@state", cboState.Text);
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@designation", cboDesignation.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                    cm.Parameters.AddWithValue("@datejoined", dateJoined.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Staff record has been updated successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTotalStaff();
                    getStaffID();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string staffid = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            
                if (frmLogin.usertype == "Administrator")
                {
                    if (ColName == "ColDelete")
                    {
                        if (MessageBox.Show("Remove Staff Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            cn.Open();
                            cm = new MySqlCommand("DELETE FROM tblstaff WHERE staffid = @staffid", cn);
                            cm.Parameters.AddWithValue("@staffid", staffid);
                            cm.ExecuteNonQuery();
                            cn.Close();
                            MessageBox.Show("Staff Record has been deleted successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Clear();
                            LoadRecord();
                            getTotalStaff();
                        }                    
                    }
                    else if (ColName == "Column2")
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;

                        cn.Open();
                        cm = new MySqlCommand("SELECT * FROM tblstaff WHERE staffid=@staffid", cn);
                        cm.Parameters.AddWithValue("@staffid", staffid);
                        dr = cm.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            txtStaffID.Text = dr["staffid"].ToString();
                            txtName.Text = dr["name"].ToString();
                            cboGender.Text = dr["gender"].ToString();
                            cboReligion.Text = dr["religion"].ToString();
                            DOB.Text = dr["dob"].ToString();
                            cboMaritalStatus.Text = dr["mstatus"].ToString();
                            cboDesignation.Text = dr["designation"].ToString();
                            cboQualification.Text = dr["qualification"].ToString();
                            txtEmail.Text = dr["email"].ToString();
                            cbStatus.Checked = Convert.ToBoolean(dr["status"]);

                            byte[] data = (byte[])dr["picture"];
                            MemoryStream ms = new MemoryStream(data);
                            StaffPic.Image = Image.FromStream(ms);

                            dateJoined.Text = dr["datejoined"].ToString();
                            cboState.Text = dr["state"].ToString();
                            txtPhoneNo.Text = dr["phone"].ToString();
                            txtAddress.Text = dr["address"].ToString();
                        }
                        else
                        {
                            txtName.Text = String.Empty;
                            cboGender.Text = String.Empty;
                            cboReligion.Text = String.Empty;
                            DOB.Text = String.Empty;
                            cboMaritalStatus.Text = String.Empty;
                            cboDesignation.Text = String.Empty;
                            cboQualification.Text = String.Empty;
                            txtEmail.Text = String.Empty;
                            cbStatus.Checked = false;

                            StaffPic.Image = StaffPic.InitialImage;

                            dateJoined.Text = String.Empty;
                            cboState.Text = String.Empty;
                            txtPhoneNo.Text = String.Empty;
                            txtAddress.Text = String.Empty;
                        }
                        dr.Close();
                        cn.Close();
                    }
                }
                else if (frmLogin.usertype == "Burser" || frmLogin.usertype == "User")
                {
                    if (ColName == "ColDelete")
                    {
                        MessageBox.Show("Access Denied\nOnly the admin has access to remove a staff\nThank You!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (ColName == "Column2")
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;

                        cn.Open();
                        cm = new MySqlCommand("SELECT * FROM tblstaff WHERE staffid=@staffid", cn);
                        cm.Parameters.AddWithValue("@staffid", staffid);
                        dr = cm.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            txtStaffID.Text = dr["staffid"].ToString();
                            txtName.Text = dr["name"].ToString();
                            cboGender.Text = dr["gender"].ToString();
                            cboReligion.Text = dr["religion"].ToString();
                            DOB.Text = dr["dob"].ToString();
                            cboMaritalStatus.Text = dr["mstatus"].ToString();
                            cboDesignation.Text = dr["designation"].ToString();
                            cboQualification.Text = dr["qualification"].ToString();
                            txtEmail.Text = dr["email"].ToString();
                            cbStatus.Checked = Convert.ToBoolean(dr["status"]);

                            byte[] data = (byte[])dr["picture"];
                            MemoryStream ms = new MemoryStream(data);
                            StaffPic.Image = Image.FromStream(ms);

                            dateJoined.Text = dr["datejoined"].ToString();
                            cboState.Text = dr["state"].ToString();
                            txtPhoneNo.Text = dr["phone"].ToString();
                            txtAddress.Text = dr["address"].ToString();
                        }
                        else
                        {
                            txtName.Text = String.Empty;
                            cboGender.Text = String.Empty;
                            cboReligion.Text = String.Empty;
                            DOB.Text = String.Empty;
                            cboMaritalStatus.Text = String.Empty;
                            cboDesignation.Text = String.Empty;
                            cboQualification.Text = String.Empty;
                            txtEmail.Text = String.Empty;
                            cbStatus.Checked = false;

                            StaffPic.Image = StaffPic.InitialImage;

                            dateJoined.Text = String.Empty;
                            cboState.Text = String.Empty;
                            txtPhoneNo.Text = String.Empty;
                            txtAddress.Text = String.Empty;
                        }
                        dr.Close();
                        cn.Close();
                    }
                }
        }

        private void txtPhoneNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
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
    }
}
