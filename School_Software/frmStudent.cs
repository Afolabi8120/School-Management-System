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
    public partial class frmStudent : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string status;

        public frmStudent()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
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

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "image files(*.bmp;*.jpg;*.png;*.gif)|*.bmp*;*.jpg;*.png;*.gif;";

            openFileDialog1.FileName = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                StudentPic.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            StudentPic.Image = StudentPic.InitialImage;
        }

        void Clear()
        {
            txtAdmissionNo.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtName.Clear();
            txtParentName.Clear();
            txtPhoneNo.Clear();
            cboReligion.Text = "";
            cboState.Text = "";
            cboClass.Text = "";
            cboGender.Text = "";
            cboRelationship.Text = "";
            cboMaritalStatus.Text = "";
            StudentPic.Image = StudentPic.InitialImage;
            cbStatus.Checked = false;
            txtName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (cbStatus.Checked == false)
                {
                    MessageBox.Show("Please select student status!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (StudentPic.Image == null || StudentPic.Image == StudentPic.InitialImage)
                {
                    MessageBox.Show("Please select a student image!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboState.Text == String.Empty || cboReligion.Text == String.Empty || cboRelationship.Text == String.Empty || txtAddress.Text == String.Empty || txtPhoneNo.Text == String.Empty || txtParentName.Text == String.Empty || txtEmail.Text == String.Empty || cboClass.Text == String.Empty || txtAdmissionNo.Text == String.Empty || txtName.Text == String.Empty || txtSection.Text == String.Empty || cboGender.Text == String.Empty || cboGender.Text == "-Select Option-")
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Create this Student Account! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(StudentPic.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Open();
                    cm = new MySqlCommand("INSERT INTO tblstudent (admissionno,studentname,gender,religion,dob,mstatus,class,section,email,status,picture,admissiondate,state,pname,phone,relationship,address) VALUES(@admissionno,@studentname,@gender,@religion,@dob,@mstatus,@class,@section,@email,@status,@picture,@admissiondate,@state,@pname,@phone,@relationship,@address)", cn);
                    cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                    cm.Parameters.AddWithValue("@studentname", txtName.Text);
                    cm.Parameters.AddWithValue("@gender", cboGender.Text);
                    cm.Parameters.AddWithValue("@religion", cboReligion.Text);
                    cm.Parameters.AddWithValue("@dob", DOB.Text);
                    cm.Parameters.AddWithValue("@mstatus", cboMaritalStatus.Text);
                    cm.Parameters.AddWithValue("@class", cboClass.Text);
                    cm.Parameters.AddWithValue("@section", txtSection.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@admissiondate", AdmissionDate.Text);
                    cm.Parameters.AddWithValue("@state", cboState.Text);
                    cm.Parameters.AddWithValue("@pname", txtParentName.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                    cm.Parameters.AddWithValue("@relationship", cboRelationship.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Student Account has been saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    //var f1 = new frmStudentSlip();
                    //f1.admissionno = txtAdmissionNo.Text;
                    //f1.date1 = DateTime.Now.ToLongDateString();
                    //f1.LoadHeader();
                    //f1.LoadReceipt();
                    //f1.ShowDialog();

                    Clear();
                    LoadRecord();
                    getTotalStudent();
                    getAdmissionNo();
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
            getAdmissionNo();
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

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent ORDER BY studentname ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["admissionno"].ToString(), dr["studentname"].ToString(), dr["class"].ToString(), dr["gender"].ToString(), dr["dob"].ToString(), dr["section"].ToString(), dr["religion"].ToString(), dr["email"].ToString(), Convert.ToBoolean(dr["status"]));
            }
            dr.Close();
            cn.Close();
        }

        public void getSection()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsection WHERE status =@status", cn);
            cm.Parameters.AddWithValue("@status", "OPEN");
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtSection.Text = dr["name"].ToString();
            }
            else
            {
                txtSection.Text = "";
            }
            dr.Close();
            cn.Close();
        }

        public void getAdmissionNo()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            var rand = new Random();
            int myrand = rand.Next(1, 200);
            txtAdmissionNo.Text = "STU-" + txtSection.Text + "-" + myrand + num;
        }

        public void getTotalStudent()
        {
            var num = dataGridView1.Rows.Count;
            lblTotal.Text = "Total Student Found: " + num;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string admissionno = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
            
                if (frmLogin.usertype == "Administrator")
                {
                    if (ColName == "ColDelete")
                    {
                        if (MessageBox.Show("Remove Student Record! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            cn.Open();
                            cm = new MySqlCommand("DELETE FROM tblstudent WHERE admissionno = @admissionno", cn);
                            cm.Parameters.AddWithValue("@admissionno", admissionno);
                            cm.ExecuteNonQuery();
                            cn.Close();
                            MessageBox.Show("Student Record has been deleted successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Clear();
                            LoadRecord();
                            getTotalStudent();
                        }                    
                    }
                    else if (ColName == "Column2")
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;

                        cn.Open();
                        cm = new MySqlCommand("SELECT * FROM tblstudent WHERE admissionno=@admissionno", cn);
                        cm.Parameters.AddWithValue("@admissionno", admissionno);
                        dr = cm.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            txtAdmissionNo.Text = dr["admissionno"].ToString();
                            txtName.Text = dr["studentname"].ToString();
                            cboGender.Text = dr["gender"].ToString();
                            cboReligion.Text = dr["religion"].ToString();
                            DOB.Text = dr["dob"].ToString();
                            cboMaritalStatus.Text = dr["mstatus"].ToString();
                            cboClass.Text = dr["class"].ToString();
                            txtSection.Text = dr["section"].ToString();
                            txtEmail.Text = dr["email"].ToString();
                            cbStatus.Checked = Convert.ToBoolean(dr["status"]);

                            byte[] data = (byte[])dr["picture"];
                            MemoryStream ms = new MemoryStream(data);
                            StudentPic.Image = Image.FromStream(ms);

                            AdmissionDate.Text = dr["admissiondate"].ToString();
                            cboState.Text = dr["state"].ToString();
                            txtParentName.Text = dr["pname"].ToString();
                            txtPhoneNo.Text = dr["phone"].ToString();
                            cboRelationship.Text = dr["relationship"].ToString();
                            txtAddress.Text = dr["address"].ToString();
                        }
                        else
                        {
                            txtAdmissionNo.Text = String.Empty;
                            txtName.Text = String.Empty;
                            cboGender.Text = String.Empty;
                            cboReligion.Text = String.Empty;
                            DOB.Text = String.Empty;
                            cboMaritalStatus.Text = String.Empty;
                            cboClass.Text = String.Empty;
                            txtSection.Text = String.Empty;
                            txtEmail.Text = String.Empty;
                            cbStatus.Checked = false;

                            StudentPic.Image = StudentPic.InitialImage;

                            AdmissionDate.Text = String.Empty;
                            cboState.Text = String.Empty;
                            txtParentName.Text = String.Empty;
                            txtPhoneNo.Text = String.Empty;
                            cboRelationship.Text = String.Empty;
                            txtAddress.Text = String.Empty;
                        }
                        dr.Close();
                        cn.Close();
                    }
                    else if (ColName == "ColPrint")
                    {
                        var f1 = new frmStudentSlip();
                        f1.admissionno = admissionno;
                        f1.date1 = DateTime.Now.ToLongDateString();
                        f1.LoadHeader();
                        f1.LoadReceipt();
                        f1.ShowDialog();
                    }
                }
                else if (frmLogin.usertype == "Burser" || frmLogin.usertype == "User")
                {              
                    if (ColName == "ColDelete")
                    {
                        MessageBox.Show("Access Denied\nOnly the admin has access to remove a student details\nThank You!!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else if (ColName == "Column2")
                    {
                        btnSave.Enabled = false;
                        btnUpdate.Enabled = true;

                        cn.Open();
                        cm = new MySqlCommand("SELECT * FROM tblstudent WHERE admissionno=@admissionno", cn);
                        cm.Parameters.AddWithValue("@admissionno", admissionno);
                        dr = cm.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            txtAdmissionNo.Text = dr["admissionno"].ToString();
                            txtName.Text = dr["studentname"].ToString();
                            cboGender.Text = dr["gender"].ToString();
                            cboReligion.Text = dr["religion"].ToString();
                            DOB.Text = dr["dob"].ToString();
                            cboMaritalStatus.Text = dr["mstatus"].ToString();
                            cboClass.Text = dr["class"].ToString();
                            txtSection.Text = dr["section"].ToString();
                            txtEmail.Text = dr["email"].ToString();
                            cbStatus.Checked = Convert.ToBoolean(dr["status"]);

                            byte[] data = (byte[])dr["picture"];
                            MemoryStream ms = new MemoryStream(data);
                            StudentPic.Image = Image.FromStream(ms);

                            AdmissionDate.Text = dr["admissiondate"].ToString();
                            cboState.Text = dr["state"].ToString();
                            txtParentName.Text = dr["pname"].ToString();
                            txtPhoneNo.Text = dr["phone"].ToString();
                            cboRelationship.Text = dr["relationship"].ToString();
                            txtAddress.Text = dr["address"].ToString();
                        }
                        else
                        {
                            txtAdmissionNo.Text = String.Empty;
                            txtName.Text = String.Empty;
                            cboGender.Text = String.Empty;
                            cboReligion.Text = String.Empty;
                            DOB.Text = String.Empty;
                            cboMaritalStatus.Text = String.Empty;
                            cboClass.Text = String.Empty;
                            txtSection.Text = String.Empty;
                            txtEmail.Text = String.Empty;
                            cbStatus.Checked = false;

                            StudentPic.Image = StudentPic.InitialImage;

                            AdmissionDate.Text = String.Empty;
                            cboState.Text = String.Empty;
                            txtParentName.Text = String.Empty;
                            txtPhoneNo.Text = String.Empty;
                            cboRelationship.Text = String.Empty;
                            txtAddress.Text = String.Empty;
                        }
                        dr.Close();
                        cn.Close();
                    }
                    else if (ColName == "ColPrint")
                    {
                        var f1 = new frmStudentSlip();
                        f1.admissionno = admissionno;
                        f1.date1 = DateTime.Now.ToLongDateString();
                        f1.LoadHeader();
                        f1.LoadReceipt();
                        f1.ShowDialog();
                    }
                }
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
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE admissionno LIKE '%" + txtSearch.Text + "%' OR studentname LIKE '%" + txtSearch.Text + "%' OR class LIKE '%" + txtSearch.Text + "%'", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["admissionno"].ToString(), dr["studentname"].ToString(), dr["class"].ToString(), dr["gender"].ToString(), dr["dob"].ToString(), dr["section"].ToString(), dr["religion"].ToString(), dr["email"].ToString(), dr["status"]);
            }
            dr.Close();
            cn.Close();
            getTotalStudent();
        }

        private void frmStudent_Load(object sender, EventArgs e)
        {
           
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (StudentPic.Image == null || StudentPic.Image == StudentPic.InitialImage)
                {
                    MessageBox.Show("Please select a student image!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboState.Text == String.Empty || cboReligion.Text == String.Empty || cboRelationship.Text == String.Empty || txtAddress.Text == String.Empty || txtPhoneNo.Text == String.Empty || txtParentName.Text == String.Empty || txtEmail.Text == String.Empty || cboClass.Text == String.Empty || txtAdmissionNo.Text == String.Empty || txtName.Text == String.Empty || txtSection.Text == String.Empty || cboGender.Text == String.Empty || cboGender.Text == "-Select Option-")
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to Create this Student Account! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    byte[] bytImage = new byte[0];
                    MemoryStream ms = new System.IO.MemoryStream();
                    Bitmap bmpImage = new Bitmap(StudentPic.Image);

                    bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    ms.Seek(0, 0);
                    bytImage = ms.ToArray();
                    ms.Close();

                    cn.Open();
                    cm = new MySqlCommand("UPDATE tblstudent SET studentname=@studentname,gender=@gender,religion=@religion,dob=@dob,mstatus=@mstatus,class=@class,section=@section,email=@email,status=@status,picture=@picture,admissiondate=@admissiondate,state=@state,pname=@pname,phone=@phone,relationship=@relationship,address=@address WHERE admissionno=@admissionno", cn);
                    cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                    cm.Parameters.AddWithValue("@studentname", txtName.Text);
                    cm.Parameters.AddWithValue("@gender", cboGender.Text);
                    cm.Parameters.AddWithValue("@religion", cboReligion.Text);
                    cm.Parameters.AddWithValue("@dob", DOB.Text);
                    cm.Parameters.AddWithValue("@mstatus", cboMaritalStatus.Text);
                    cm.Parameters.AddWithValue("@class", cboClass.Text);
                    cm.Parameters.AddWithValue("@section", txtSection.Text);
                    cm.Parameters.AddWithValue("@email", txtEmail.Text);
                    cm.Parameters.AddWithValue("@status", Convert.ToBoolean(cbStatus.Checked.ToString()));
                    cm.Parameters.AddWithValue("@picture", bytImage);
                    cm.Parameters.AddWithValue("@admissiondate", AdmissionDate.Text);
                    cm.Parameters.AddWithValue("@state", cboState.Text);
                    cm.Parameters.AddWithValue("@pname", txtParentName.Text);
                    cm.Parameters.AddWithValue("@phone", txtPhoneNo.Text);
                    cm.Parameters.AddWithValue("@relationship", cboRelationship.Text);
                    cm.Parameters.AddWithValue("@address", txtAddress.Text);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Student Account has been updated successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Clear();
                    LoadRecord();
                    getTotalStudent();
                    getAdmissionNo();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE class =@class", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["admissionno"].ToString(), dr["studentname"].ToString(), dr["class"].ToString(), dr["gender"].ToString(), dr["dob"].ToString(), dr["section"].ToString(), dr["religion"].ToString(), dr["email"].ToString(), dr["status"]);
            }
            dr.Close();
            cn.Close();
        }
    }
}
