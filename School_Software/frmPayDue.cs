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
using Microsoft.Reporting.WinForms;

namespace School_Software
{
    public partial class frmPayDue : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        MySqlDataAdapter da;
        ClassDB db = new ClassDB();

        public string paymentid,admissionno, receivedby;

        string _Name, _Phone, _Email, _Motto, _RegNo, _Address;

        public frmPayDue()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void LoadHeader()
        {
            ReportDataSource rptDS = new ReportDataSource();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblschoolinfo", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                _Name = dr["name"].ToString();
                _Phone = dr["phone"].ToString();
                _Email = dr["email"].ToString();
                _Motto = dr["motto"].ToString();
                _RegNo = dr["regno"].ToString();
                _Address = dr["address"].ToString();

                dr.Close();
                cn.Close();

                reportViewer1.LocalReport.ReportPath = Application.StartupPath + "/Reports/FeeReceipt.rdlc";
                reportViewer1.LocalReport.DataSources.Clear();

                DataSet1 ds1 = new DataSet1();
                da = new MySqlDataAdapter();


                cn.Open();
                da.SelectCommand = new MySqlCommand("SELECT * FROM tblschoolinfo WHERE name = '" + _Name + "'", cn);
                da.Fill(ds1, "dtName");
                cn.Close();

                ReportParameter pName = new ReportParameter("pName", _Name);
                ReportParameter pPhone = new ReportParameter("pPhone", _Phone);
                ReportParameter pEmail = new ReportParameter("pEmail", _Email);
                ReportParameter pMotto = new ReportParameter("pMotto", _Motto);
                ReportParameter pRegNo = new ReportParameter("pRegNo", _RegNo);
                ReportParameter pAddress = new ReportParameter("pAddress", _Address);

                reportViewer1.LocalReport.SetParameters(pName);
                reportViewer1.LocalReport.SetParameters(pPhone);
                reportViewer1.LocalReport.SetParameters(pEmail);
                reportViewer1.LocalReport.SetParameters(pMotto);
                reportViewer1.LocalReport.SetParameters(pRegNo);
                reportViewer1.LocalReport.SetParameters(pAddress);

                rptDS = new ReportDataSource("DataSet1", ("dtName"));
                reportViewer1.LocalReport.DataSources.Add(rptDS);
            }
            else
            {
                _Name = "";
                _Phone = "";
                _Email = "";
                _Motto = "";
                _RegNo = "";
                _Address = "";
            }
            dr.Close();
            cn.Close();
        }


        public void LoadReceipt()
        {
            cn.Open();
            da = new MySqlDataAdapter("SELECT r.paymentid,p.admissionno,p.name,r.class,r.section,r.term,r.feename,r.price,r.amountpaid,r.cchange,r.due,p.price AS totalprice,p.amountpaid AS paidamount,p.cchange AS totalchange,p.due AS totaldue,p.date,p.time,p.receivedby FROM tblfeerecord AS r INNER JOIN tblfeepayment AS p ON r.paymentid = p.paymentid WHERE r.paymentid = '" + lblPaymentID.Text + "'", cn);
            DataSet1 ds = new DataSet1();
            da.Fill(ds, "dtFeeReceipt");

            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(datasource);
            this.reportViewer1.RefreshReport();
            cn.Close();
        }

        public void getFeeTotal()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(price) FROM tblfeerecord WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblFeeTotal.Text = num;
        }

        public void getTotalAmountPaid()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(amountpaid) FROM tblfeerecord WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblAmountPaid.Text = num;
        }

        public void getTotalChange()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(cchange) FROM tblfeerecord WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblChange.Text = num;
        }

        public void getTotalDue()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(due) FROM tblfeerecord WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblDue.Text = num;
        }

        public void LoadRecord()
        {
            int i = 0;
            dataGridView1.Rows.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE paymentid=@paymentid ORDER BY feename ASC", cn);
            cm.Parameters.AddWithValue("@paymentid", paymentid);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        public void getPictureClass()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE admissionno=@admissionno", cn);
            cm.Parameters.AddWithValue("@admissionno", admissionno);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtClass.Text = dr["class"].ToString();

                byte[] data = (byte[])dr["picture"];
                MemoryStream ms = new MemoryStream(data);
                StudentPic.Image = Image.FromStream(ms);
            }
            else
            {
                txtClass.Text = String.Empty;
                StudentPic.Image = StudentPic.InitialImage;
            }
            dr.Close();
            cn.Close();
        }

        public void getPaymentRecord()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeepayment WHERE paymentid = '" + paymentid + "'", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblPaymentID.Text = dr["paymentid"].ToString();
                txtAdmissionNo.Text = dr["admissionno"].ToString();
                txtStudentName.Text = dr["name"].ToString();
                txtSection.Text = dr["section"].ToString();
                txtTerm.Text = dr["term"].ToString();
                lblFeeTotal.Text = dr["price"].ToString();
                lblAmountPaid.Text = dr["amountpaid"].ToString();
                lblChange.Text = dr["cchange"].ToString();
                lblDue.Text = dr["due"].ToString();
                receivedby = dr["receivedby"].ToString();
            }
            else
            {

            }
            dr.Close();
            cn.Close();
        }

        private void frmPayDue_Load(object sender, EventArgs e)
        {
            //this.reportViewer1.RefreshReport();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtAmountToPay.Text == String.Empty || txtAmountToPay.Text == "0.00" || txtAmountToPay.Text == "0")
            {
                txtAmountToPay.Focus();
                MessageBox.Show("Please enter a valid amount to pay!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPrice.Text == String.Empty || txtPrice.Text == "0.00" || txtPrice.Text == "0")
            {
                txtPrice.Focus();
                MessageBox.Show("The price field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtSection.Text == String.Empty)
            {
                MessageBox.Show("Section field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtAdmissionNo.Text == String.Empty)
            {
                MessageBox.Show("Admission No field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtStudentName.Text == String.Empty)
            {
                MessageBox.Show("The student name field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtTerm.Text == String.Empty)
            {
                MessageBox.Show("The term field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }


            if (MessageBox.Show("Add Fee To Cart! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                cn.Open();
                cm = new MySqlCommand("UPDATE tblfeerecord SET price=@price,amountpaid=@amountpaid,cchange=@cchange,due=@due WHERE paymentid=@paymentid AND admissionno=@admissionno AND feename=@feename", cn);
                cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                cm.Parameters.AddWithValue("@feename", txtFeeName.Text);
                cm.Parameters.AddWithValue("@price", txtPrice.Text);
                cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                cm.Parameters.AddWithValue("@cchange", txtChange.Text);
                cm.Parameters.AddWithValue("@due", txtDue.Text);
                cm.ExecuteNonQuery();
                cn.Close();

                cn.Open();
                cm = new MySqlCommand("INSERT INTO tblfeerecord2 (paymentid,admissionno,class,name,section,term,feename,price,amountpaid,cchange,due,date,time,receivedby) VALUES(@paymentid,@admissionno,@class,@name,@section,@term,@feename,@price,@amountpaid,@cchange,@due,@date,@time,@receivedby)", cn);
                cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                cm.Parameters.AddWithValue("@class", txtClass.Text);
                cm.Parameters.AddWithValue("@name", txtStudentName.Text);
                cm.Parameters.AddWithValue("@section", txtSection.Text);
                cm.Parameters.AddWithValue("@term", txtTerm.Text);
                cm.Parameters.AddWithValue("@feename", txtFeeName.Text);
                cm.Parameters.AddWithValue("@price", txtPrice.Text);
                cm.Parameters.AddWithValue("@amountpaid", txtAmountToPay.Text);
                cm.Parameters.AddWithValue("@cchange", txtChange.Text);
                cm.Parameters.AddWithValue("@due", txtDue.Text);
                cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                cm.Parameters.AddWithValue("@receivedby", "AFOLABI TEMIDAYO TIMOTHY");
                cm.ExecuteNonQuery();
                cn.Close();
                LoadRecord();
                getTotalAmountPaid();
                getTotalChange();
                getTotalDue();
                getFeeTotal();
                txtAmountToPay.Text = "0.00";
                txtPrice.Text = "0.00";
                txtAmountPaid.Text = "0.00";
                txtChange.Text = "0.00";
                txtDue.Text = "0.00";
                txtFeeName.Text = "";
                btnAdd.Enabled = false;
            }
        }

        private void txtAmountToPay_TextChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
             string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string feename = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColEdit")
            {
                btnCalculate.Enabled = true;
                txtFeeName.Text = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();
                txtPrice.Text = dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString();
                txtAmountPaid.Text = dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString();
                txtChange.Text = dataGridView1.Rows[e.RowIndex].Cells[4].Value.ToString();
                txtDue.Text = dataGridView1.Rows[e.RowIndex].Cells[5].Value.ToString();
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            var num = (Convert.ToDecimal(txtAmountPaid.Text) + Convert.ToDecimal(txtAmountToPay.Text));
            txtAmountPaid.Text = (num).ToString();
            if (Convert.ToDecimal(txtAmountPaid.Text) >= Convert.ToDecimal(txtPrice.Text))
            {
                var mynum = (Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(txtPrice.Text));
                txtChange.Text = mynum.ToString();
                txtDue.Text = "0.00";
            }
            else if (Convert.ToDecimal(txtAmountPaid.Text) < Convert.ToDecimal(txtPrice.Text))
            {
                var mynum = (Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(txtPrice.Text));
                txtDue.Text = mynum.ToString();
                txtChange.Text = "0.00";    
            }

            btnCalculate.Enabled = false;
            btnAdd.Enabled = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count < 1)
                {
                    MessageBox.Show("Can't save an empty cart!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtClass.Text == String.Empty)
                {
                    MessageBox.Show("Class field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtSection.Text == String.Empty)
                {
                    MessageBox.Show("Section field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAdmissionNo.Text == String.Empty)
                {
                    MessageBox.Show("Admission No field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtStudentName.Text == String.Empty)
                {
                    MessageBox.Show("Student field name is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtTerm.Text == String.Empty)
                {
                    MessageBox.Show("Term field is empty!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Save payment Details! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblfeepayment WHERE paymentid=@paymentid", cn);
                    cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("UPDATE tblfeepayment SET price=@price,amountpaid=@amountpaid,cchange=@cchange,due=@due,date=@date,time=@time,receivedby=@receivedby WHERE paymentid=@paymentid AND admissionno=@admissionno AND name=@name AND section=@section AND term=@term", cn);
                        cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@name", txtStudentName.Text);
                        cm.Parameters.AddWithValue("@section", txtSection.Text);
                        cm.Parameters.AddWithValue("@term", txtTerm.Text);
                        cm.Parameters.AddWithValue("@price", lblFeeTotal.Text);
                        cm.Parameters.AddWithValue("@amountpaid", lblAmountPaid.Text);
                        cm.Parameters.AddWithValue("@cchange", lblChange.Text);
                        cm.Parameters.AddWithValue("@due", lblDue.Text);
                        cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                        cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                        cm.Parameters.AddWithValue("@receivedby", frmLogin.fullname);
                        cm.ExecuteNonQuery();
                        cn.Close();
                        MessageBox.Show("Payment has been saved successfully!", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        txtClass.Text = "";
                        txtTerm.Text = "";
                        txtFeeName.Text = "";
                        txtStudentName.Text = "";
                        txtAdmissionNo.Text = "";
                        lblAmountPaid.Text = "0.00";
                        lblChange.Text = "0.00";
                        lblFeeTotal.Text = "0.00";
                        lblDue.Text = "0.00";                        
                        StudentPic.Image = StudentPic.InitialImage;
                        LoadHeader();
                        LoadReceipt();
                        lblPaymentID.Text = "";
                        LoadRecord();
                    }
                    else
                    {
                        cn.Close();
                        MessageBox.Show("Payment ID does not exist", "INVALID PAYMENT ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
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

        private void txtAmountToPay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
