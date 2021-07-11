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
    public partial class frmFeePayment : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        MySqlDataAdapter da;
        ClassDB db = new ClassDB();

        string _Name, _Phone, _Email, _Motto, _RegNo, _Address;

        string paymentid;

        public frmFeePayment()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count > 0)
            {
                MessageBox.Show("Please remove all payment from cart!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                this.Dispose();
            }
        }

        void getStudentDetails()
        {
            cboName.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE class=@class", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboName.Items.Add(dr["studentname"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        void getFeeDetails()
        {
            cboFeeName.Items.Clear();
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeetype WHERE class=@class", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                cboFeeName.Items.Add(dr["name"].ToString());
            }
            dr.Close();
            cn.Close();
        }

        private void cboFeeName_SelectedIndexChanged(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblfeetype WHERE name=@name", cn);
            cm.Parameters.AddWithValue("@name", cboFeeName.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtPrice.Text = dr["price"].ToString();
            }
            else
            {
                txtPrice.Text = "";
            }
            dr.Close();
            cn.Close();
        }

        private void cboClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            cboName.Items.Clear();
            getStudentDetails();
            getFeeDetails();
        }

        private void cboName_SelectedIndexChanged(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblstudent WHERE studentname=@studentname AND class=@class", cn);
            cm.Parameters.AddWithValue("@class", cboClass.Text);
            cm.Parameters.AddWithValue("@studentname", cboName.Text);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                txtAdmissionNo.Text = dr["admissionno"].ToString();

                byte[] data = (byte[])dr["picture"];
                MemoryStream ms = new MemoryStream(data);
                StudentPic.Image = Image.FromStream(ms);
            }
            else
            {
                txtAdmissionNo.Text = "";
                StudentPic.Image = StudentPic.InitialImage;
            }
            dr.Close();
            cn.Close();
        }

        public void getPaymentID()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(DISTINCT(paymentid)) FROM tblfeerecord", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            var rand = new Random();
            int myrand = rand.Next(1, 999);
            lblPaymentID.Text = "P-" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + num + "-" + myrand;
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
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                i++;
                dataGridView1.Rows.Add(i, dr["feename"].ToString(), dr["price"].ToString(), dr["amountpaid"].ToString(), dr["cchange"].ToString(), dr["due"].ToString());
            }
            dr.Close();
            cn.Close();
        }


        private void txtPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAmountPaid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtChange_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtDue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAmountPaid_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToDecimal(txtAmountPaid.Text) >= Convert.ToDecimal(txtPrice.Text))
                {
                    var num = (Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(txtPrice.Text));
                    txtChange.Text = num.ToString();
                    txtDue.Text = "0.00";
                }
                else if (Convert.ToDecimal(txtAmountPaid.Text) < Convert.ToDecimal(txtPrice.Text))
                {
                    var num = (Convert.ToDecimal(txtAmountPaid.Text) - Convert.ToDecimal(txtPrice.Text));
                    txtDue.Text = num.ToString();
                    txtChange.Text = "0.00";
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show("WARNING: " + ex.Message);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (cboClass.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid class!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (txtAmountPaid.Text == String.Empty || txtAmountPaid.Text == "0.00" || txtAmountPaid.Text == "0")
                {
                    txtAmountPaid.Focus();
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

                if (cboName.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid student name!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboTerm.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid term!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (MessageBox.Show("Add Fee To Cart! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE paymentid=@paymentid AND feename=@feename", cn);
                    cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                    cm.Parameters.AddWithValue("@feename", cboFeeName.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        cn.Close();
                        MessageBox.Show("You have already added the same payment name!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("SELECT * FROM tblfeerecord WHERE admissionno=@admissionno AND class=@class AND section=@section AND term=@term AND feename=@feename", cn);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@section", txtSection.Text);
                        cm.Parameters.AddWithValue("@class", cboClass.Text);
                        cm.Parameters.AddWithValue("@term", cboTerm.Text);
                        cm.Parameters.AddWithValue("@feename", cboFeeName.Text);
                        dr = cm.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            cn.Close();
                            MessageBox.Show("The selected student has made some payment for the selected term, section and fee name!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                        else
                        {
                            cn.Close();
                            cn.Open();
                            cm = new MySqlCommand("INSERT INTO tblfeerecord (paymentid,admissionno,class,name,section,term,feename,price,amountpaid,cchange,due,date,time,receivedby) VALUES(@paymentid,@admissionno,@class,@name,@section,@term,@feename,@price,@amountpaid,@cchange,@due,@date,@time,@receivedby)", cn);
                            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                            cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                            cm.Parameters.AddWithValue("@class", cboClass.Text);
                            cm.Parameters.AddWithValue("@name", cboName.Text);
                            cm.Parameters.AddWithValue("@section", txtSection.Text);
                            cm.Parameters.AddWithValue("@term", cboTerm.Text);
                            cm.Parameters.AddWithValue("@feename", cboFeeName.Text);
                            cm.Parameters.AddWithValue("@price", txtPrice.Text);
                            cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                            cm.Parameters.AddWithValue("@cchange", txtChange.Text);
                            cm.Parameters.AddWithValue("@due", txtDue.Text);
                            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                            cm.Parameters.AddWithValue("@receivedby", frmLogin.fullname);
                            cm.ExecuteNonQuery();
                            cn.Close();

                            cn.Open();
                            cm = new MySqlCommand("INSERT INTO tblfeerecord2 (paymentid,admissionno,class,name,section,term,feename,price,amountpaid,cchange,due,date,time,receivedby) VALUES(@paymentid,@admissionno,@class,@name,@section,@term,@feename,@price,@amountpaid,@cchange,@due,@date,@time,@receivedby)", cn);
                            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                            cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                            cm.Parameters.AddWithValue("@class", cboClass.Text);
                            cm.Parameters.AddWithValue("@name", cboName.Text);
                            cm.Parameters.AddWithValue("@section", txtSection.Text);
                            cm.Parameters.AddWithValue("@term", cboTerm.Text);
                            cm.Parameters.AddWithValue("@feename", cboFeeName.Text);
                            cm.Parameters.AddWithValue("@price", txtPrice.Text);
                            cm.Parameters.AddWithValue("@amountpaid", txtAmountPaid.Text);
                            cm.Parameters.AddWithValue("@cchange", txtChange.Text);
                            cm.Parameters.AddWithValue("@due", txtDue.Text);
                            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
                            cm.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                            cm.Parameters.AddWithValue("@receivedby", frmLogin.fullname);
                            cm.ExecuteNonQuery();
                            cn.Close();
                            LoadRecord();
                            getFeeTotal();
                            getTotalAmountPaid();
                            getTotalChange();
                            getTotalDue();
                            txtAmountPaid.Text = "0";
                            txtPrice.Text = "0";
                            txtChange.Text = "0";
                            txtDue.Text = "0";
                        }
                        dr.Close();
                        cn.Close();
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

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string ColName = dataGridView1.Columns[e.ColumnIndex].Name;
            string feename = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            if (ColName == "ColDelete")
            {
                if (MessageBox.Show("Remove Fee! Click Yes to Confirm", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblfeerecord WHERE paymentid=@paymentid AND feename = @feename", cn);
                    cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                    cm.Parameters.AddWithValue("@feename", feename);
                    cm.ExecuteNonQuery();
                    cn.Close();

                    cn.Open();
                    cm = new MySqlCommand("DELETE FROM tblfeerecord2 WHERE paymentid=@paymentid AND feename = @feename", cn);
                    cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                    cm.Parameters.AddWithValue("@feename", feename);
                    cm.ExecuteNonQuery();
                    cn.Close();
                    LoadRecord();
                }
            }
        }

        //SELECT r.paymentid,p.admissionno,p.name,r.class,r.section,r.term,r.feename,r.price,r.amountpaid,r.cchange,r.due,p.price AS totalprice,p.amountpaid AS paidamount,p.cchange AS totalchange,p.due AS totaldue,p.date,p.time,p.receivedby FROM tblfeerecord AS r INNER JOIN tblfeepayment AS p ON r.paymentid = p.paymentid 

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count < 1)
                {
                    MessageBox.Show("Can't save an empty cart!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboClass.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid class!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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

                if (cboName.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid student name!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                if (cboTerm.Text == String.Empty)
                {
                    MessageBox.Show("Please select a valid term!", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        MessageBox.Show("Payment ID Already Exist!", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    else
                    {
                        cn.Close();
                        cn.Open();
                        cm = new MySqlCommand("INSERT INTO tblfeepayment (paymentid,admissionno,name,section,term,price,amountpaid,cchange,due,date,time,receivedby) VALUES(@paymentid,@admissionno,@name,@section,@term,@price,@amountpaid,@cchange,@due,@date,@time,@receivedby)", cn);
                        cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
                        cm.Parameters.AddWithValue("@admissionno", txtAdmissionNo.Text);
                        cm.Parameters.AddWithValue("@name", cboName.Text);
                        cm.Parameters.AddWithValue("@section", txtSection.Text);
                        cm.Parameters.AddWithValue("@term", cboTerm.Text);
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
                        cboClass.SelectedItem = -1;
                        cboTerm.SelectedIndex = -1;
                        cboFeeName.SelectedIndex = -1;
                        cboName.SelectedIndex = -1;
                        txtAdmissionNo.Text = "";
                        lblAmountPaid.Text = "0.00";
                        lblChange.Text = "0.00";
                        lblFeeTotal.Text = "0.00";
                        lblDue.Text = "0.00";
                        StudentPic.Image = StudentPic.InitialImage;
                        LoadHeader();
                        LoadReceipt();
                        getPaymentID();
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

        private void frmFeePayment_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            //LoadHeader();
            //LoadReceipt();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            cn.Open();
            cm = new MySqlCommand("DELETE FROM tblfeerecord WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            cm.ExecuteNonQuery();
            cn.Close();

            cn.Open();
            cm = new MySqlCommand("DELETE FROM tblfeerecord2 WHERE paymentid=@paymentid", cn);
            cm.Parameters.AddWithValue("@paymentid", lblPaymentID.Text);
            cm.ExecuteNonQuery();
            cn.Close();
            cboClass.SelectedItem = -1;
            cboTerm.SelectedIndex = -1;
            cboFeeName.SelectedIndex = -1;
            cboName.SelectedIndex = -1;
            txtAdmissionNo.Text = "";
            lblAmountPaid.Text = "0.00";
            lblChange.Text = "0.00";
            lblFeeTotal.Text = "0.00";
            lblDue.Text = "0.00";
            StudentPic.Image = StudentPic.InitialImage;
            getPaymentID();
            LoadRecord();
        }
    }
}
