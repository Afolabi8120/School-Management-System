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
    public partial class frmExamScore : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string examtotal;

        public frmExamScore()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        //gets student percentage
        public void getPercentage()
        {
            try
            {
                var subject_count = Convert.ToDouble(txtTotalSubject.Text);
                var examscores_count = Convert.ToDouble(txtExamScores.Text);

                var result = (examscores_count / subject_count);

                txtPercentage.Text = result.ToString() + " %";
            }
            catch (Exception ex)
            {

            }
        }

        public void getTotalScores()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(total) FROM tblexamscore WHERE admissionno =@admissionno AND class=@class AND section=@section AND term=@term ORDER BY subject ASC", cn);
            cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
            cm.Parameters.AddWithValue("@class", txtClass.Text);
            cm.Parameters.AddWithValue("@section", cboSection.Text);
            cm.Parameters.AddWithValue("@term", cboTerm.Text);
            examtotal = cm.ExecuteScalar().ToString();
            cn.Close();

            txtExamScores.Text = examtotal;
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblexamscore WHERE admissionno =@admissionno AND class=@class AND section=@section AND term=@term ORDER BY subject ASC", cn);
            cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
            cm.Parameters.AddWithValue("@class", txtClass.Text);
            cm.Parameters.AddWithValue("@section", cboSection.Text);
            cm.Parameters.AddWithValue("@term", cboTerm.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["subject"].ToString(), dr["ca"].ToString(), dr["test"].ToString(), dr["exam"].ToString(), dr["total"].ToString(), dr["grade"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        void getTotalSubject()
        {
            txtTotalSubject.Text = dataGridView1.Rows.Count.ToString();
        }

        private void cboCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboSubject.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsubject WHERE type=@type ORDER BY name ASC", cn);
            cm.Parameters.AddWithValue("@type", cboCategory.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboSubject.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void getAllSection()
        {
            cboSection.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsection ORDER BY name ASC", cn);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboSection.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE admissionno=@admissionno", cn);
            cm.Parameters.AddWithValue("@admissionno", txtSearch.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtAdmissionNo.Text = dr["admissionno"].ToString();
                txtName.Text = dr["studentname"].ToString();
                txtClass.Text = dr["class"].ToString();
                txtDOB.Text = dr["dob"].ToString();
                txtGender.Text = dr["gender"].ToString();

                byte[] data = (byte[])dr["picture"];
                MemoryStream ms = new MemoryStream(data);
                StudentPic.Image = Image.FromStream(ms);
            }
            else
            {
                cn.Close();
                txtAdmissionNo.Text = "";
                txtName.Text = "";
                txtClass.Text = "";
                txtDOB.Text = "";
                txtGender.Text = "";
                StudentPic.Image = StudentPic.InitialImage;
                MessageBox.Show("Student record not found!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            dr.Close();
            cn.Close();
        }

        private void txtSchoolOpen_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtPresent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAbsent_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtCA_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTest_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtExam_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtInClass_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtExamScores_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        void Clear()
        {
            txtAdmissionNo.Clear();
            txtClass.Clear();
            cboSection.SelectedItem = -1;
            cboTerm.SelectedItem = -1;
            txtDOB.Clear();
            txtExamScores.Text = "0";
            txtGender.Clear();
            txtGrade.Text = "0";
            txtInClass.Clear();
            txtPercentage.Text = "0";
            txtPresent.Text = "0";
            txtPrincipalComment.Clear();
            txtRemark.Clear();
            txtTest.Text = "0";
            txtTotal.Text = "0";
            txtTotalSubject.Text = "0";
            txtSchoolOpen.Text = "0";
            txtSearch.Clear();
            txtSearch.Focus();

            StudentPic.Image = StudentPic.InitialImage;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(txtCA.Text) < 0)
                {
                    txtCA.Focus();
                    MessageBox.Show("CA Score cannot be less than 0!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToInt32(txtCA.Text) > 20)
                {
                    txtCA.Focus();
                    MessageBox.Show("CA Score cannot be greater than 20!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToInt32(txtTest.Text) < 0)
                {
                    txtTest.Focus();
                    MessageBox.Show("Test Score cannot be less than 0!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToInt32(txtTest.Text) > 20)
                {
                    txtTest.Focus();
                    MessageBox.Show("Test Score cannot be greater than 20!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToInt32(txtExam.Text) < 0)
                {
                    txtExam.Focus();
                    MessageBox.Show("Exam Score cannot be less than 0!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (Convert.ToInt32(txtExam.Text) > 60)
                {
                    txtExam.Focus();
                    MessageBox.Show("Exam Score cannot be greater than 60!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboSection.Text == String.Empty)
                {
                    cboSection.Focus();
                    MessageBox.Show("Please select a valid section before proceeding!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAdmissionNo.Text == String.Empty)
                {
                    txtSearch.Focus();
                    MessageBox.Show("Please search for a student before proceeding!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboSection.Text == String.Empty)
                {
                    cboSection.Focus();
                    MessageBox.Show("Please select a valid section!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboTerm.Text == String.Empty)
                {
                    cboTerm.Focus();
                    MessageBox.Show("Please select a valid term!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtCA.Text == String.Empty)
                {
                    txtCA.Focus();
                    MessageBox.Show("Please enter student CA Score!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtTest.Text == String.Empty)
                {
                    txtTest.Focus();
                    MessageBox.Show("Please enter student Test Score!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtExam.Text == String.Empty)
                {
                    txtExam.Focus();
                    MessageBox.Show("Please enter student Exam Score!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
                
                if (txtAdmissionNo.Text == String.Empty)
                {
                    MessageBox.Show("Admission No field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboSubject.Text == String.Empty)
                {
                    cboSubject.Focus();
                    MessageBox.Show("Please select a subject!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Add Score(s) To Mark Sheet! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblexamscore WHERE admissionno =@admissionno AND class=@class AND section=@section AND term=@term AND subject=@subject ORDER BY subject ASC", cn);
                    cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                    cm.Parameters.AddWithValue("@class", txtClass.Text);
                    cm.Parameters.AddWithValue("@section", cboSection.Text);
                    cm.Parameters.AddWithValue("@term", cboTerm.Text);
                    cm.Parameters.AddWithValue("@subject", cboSubject.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("You have already added a score for this student for the selected term, class, section and subject!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblexamscore (admissionno,name,class,term,section,subject,ca,test,exam,total,grade,status) VALUES(@admissionno,@name,@class,@term,@section,@subject,@ca,@test,@exam,@total,@grade,@status)", cn);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@class", txtClass.Text);
                        cm.Parameters.AddWithValue("@term", cboTerm.Text);
                        cm.Parameters.AddWithValue("@section", cboSection.Text);
                        cm.Parameters.AddWithValue("@subject", cboSubject.Text);
                        cm.Parameters.AddWithValue("@ca", txtCA.Text);
                        cm.Parameters.AddWithValue("@test", txtTest.Text);
                        cm.Parameters.AddWithValue("@exam", txtExam.Text);
                        cm.Parameters.AddWithValue("@total", txtTotal.Text);
                        cm.Parameters.AddWithValue("@grade", txtGrade.Text);
                        cm.Parameters.AddWithValue("@status", "PENDING");
                        cm.ExecuteNonQuery();
                        cn.Close();
                        LoadRecord();
                        getTotalSubject();
                        getTotalScores();
                        getPercentage();
                        txtCA.Text = "0";
                        txtTest.Text = "0";
                        txtExam.Text = "0";
                        txtTotal.Text = "0";
                        txtGrade.Text = "";
                     }
                     dr.Close();
                     cn.Close();
                }
            }
            catch (Exception ex)
            {
                cn.Close();
                //MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtExam_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int total = Convert.ToInt32(txtCA.Text) + Convert.ToInt32(txtTest.Text) + Convert.ToInt32(txtExam.Text);
                txtTotal.Text = total.ToString();

                if (Convert.ToInt32(txtTotal.Text) >= 70 && Convert.ToInt32(txtTotal.Text) <= 100)
                {
                    txtGrade.Text = "EXCELLENT";
                }
                else if (Convert.ToInt32(txtTotal.Text) >= 50 && Convert.ToInt32(txtTotal.Text) <= 69)
                {
                    txtGrade.Text = "CREDIT";
                }
                else if (Convert.ToInt32(txtTotal.Text) >= 40 && Convert.ToInt32(txtTotal.Text) <= 49)
                {
                    txtGrade.Text = "PASS";
                }
                else if (Convert.ToInt32(txtTotal.Text) >= 0 && Convert.ToInt32(txtTotal.Text) <= 39)
                {
                    txtGrade.Text = "FAIL";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("WARNING: " + ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string subject = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Student Student! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblexamscore WHERE admissionno =@admissionno AND class=@class AND section=@section AND term=@term AND subject=@subject ORDER BY subject ASC", cn);
                    cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                    cm.Parameters.AddWithValue("@class", txtClass.Text);
                    cm.Parameters.AddWithValue("@section", cboSection.Text);
                    cm.Parameters.AddWithValue("@term", cboTerm.Text);
                    cm.Parameters.AddWithValue("@subject", subject);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    MessageBox.Show("Student Score has been removed successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadRecord();
                    getTotalSubject();
                    getTotalScores();
                    getPercentage();
                }
            }
        }

        private void btnSavePrint_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count < 1)
                {
                    MessageBox.Show("Student Mark Sheet is Empty\nPlease input some score(s)!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAdmissionNo.Text == String.Empty)
                {
                    txtSearch.Focus();
                    MessageBox.Show("Student Details is required before proceeding!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboSection.Text == String.Empty)
                {
                    cboSection.Focus();
                    MessageBox.Show("Please select a valid section!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboTerm.Text == String.Empty)
                {
                    cboTerm.Focus();
                    MessageBox.Show("Please select a valid term!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAbsent.Text == String.Empty || txtPresent.Text == string.Empty || txtSchoolOpen.Text == String.Empty || txtPrincipalComment.Text == String.Empty || txtTeacherComment.Text == String.Empty || txtRemark.Text == String.Empty)
                {
                    MessageBox.Show("All fields are required!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAdmissionNo.Text == String.Empty)
                {
                    MessageBox.Show("Admission No field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Save Score(s) To Mark Sheet! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblexamlist WHERE admissionno =@admissionno AND class=@class AND section=@section AND term=@term ORDER BY name, section, term ASC", cn);
                    cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                    cm.Parameters.AddWithValue("@class", txtClass.Text);
                    cm.Parameters.AddWithValue("@section", cboSection.Text);
                    cm.Parameters.AddWithValue("@term", cboTerm.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("You have already added a score for this student for the selected term, class and section!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        byte[] bytImage = new byte[0];
                        MemoryStream ms = new System.IO.MemoryStream();
                        Bitmap bmpImage = new Bitmap(StudentPic.Image);

                        bmpImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                        ms.Seek(0, 0);
                        bytImage = ms.ToArray();
                        ms.Close();

                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblexamlist (admissionno,name,class,gender,dob,picture,term,section,open,present,absent,termbegin,termend,nexttermbegin,principal,teacher,noinclass,totalscores,totalsubject,percentage,remark,date) VALUES(@admissionno,@name,@class,@gender,@dob,@picture,@term,@section,@open,@present,@absent,@termbegin,@termend,@nexttermbegin,@principal,@teacher,@noinclass,@totalscores,@totalsubject,@percentage,@remark,@date)", cn);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@name", txtName.Text);
                        cm.Parameters.AddWithValue("@class", txtClass.Text);
                        cm.Parameters.AddWithValue("@gender", txtGender.Text);
                        cm.Parameters.AddWithValue("@dob", txtDOB.Text);
                        cm.Parameters.AddWithValue("@picture", bytImage);
                        cm.Parameters.AddWithValue("@term", cboTerm.Text);
                        cm.Parameters.AddWithValue("@section", cboSection.Text);
                        cm.Parameters.AddWithValue("@open", txtSchoolOpen.Text);
                        cm.Parameters.AddWithValue("@present", txtPresent.Text);
                        cm.Parameters.AddWithValue("@absent", txtAbsent.Text);
                        cm.Parameters.AddWithValue("@termbegin", dateTermBegin.Text);
                        cm.Parameters.AddWithValue("@termend", dateTermEnd.Text);
                        cm.Parameters.AddWithValue("@nexttermbegin", dateNextTerm.Text);
                        cm.Parameters.AddWithValue("@principal", txtPrincipalComment.Text);
                        cm.Parameters.AddWithValue("@teacher", txtTeacherComment.Text);
                        cm.Parameters.AddWithValue("@noinclass", txtInClass.Text);
                        cm.Parameters.AddWithValue("@totalscores", txtExamScores.Text);
                        cm.Parameters.AddWithValue("@totalsubject", txtTotalSubject.Text);
                        cm.Parameters.AddWithValue("@percentage", txtPercentage.Text);
                        cm.Parameters.AddWithValue("@remark", txtRemark.Text);
                        cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                        cm.ExecuteNonQuery();
                        cn.Close();

                        cn.Open();
                        cm = new MySqlCommand("UPDATE tblexamscore SET status=@status WHERE admissionno=@admissionno AND class=@class AND term=@term AND section=@section", cn);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@class", txtClass.Text);
                        cm.Parameters.AddWithValue("@term", cboTerm.Text);
                        cm.Parameters.AddWithValue("@section", cboSection.Text);
                        cm.Parameters.AddWithValue("@status", "SUBMITTED");
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Student Score(s) has been saved successfully", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        var f1 = new frmPrintResult();
                        f1._admissionno = txtAdmissionNo.Text;
                        f1._class = txtClass.Text;
                        f1._section = cboSection.Text;
                        f1._term = cboTerm.Text;
                        f1.LoadHeader();
                        f1.LoadReceipt();
                        f1.ShowDialog();
                        
                        Clear();
                        LoadRecord();
                        getTotalSubject();
                        StudentPic.Image = StudentPic.InitialImage;
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

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }
    }
}
