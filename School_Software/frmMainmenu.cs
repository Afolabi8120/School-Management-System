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
using System.Windows.Forms.DataVisualization.Charting;

namespace School_Software
{
    public partial class frmMainmenu : Form
    {

        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmMainmenu()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        public void LoadChart()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT COUNT(*) AS stu, section FROM tblstudent GROUP BY section", cn);
            DataSet ds = new DataSet();

            da.Fill(ds, "Chart");
            chart1.DataSource = ds.Tables["Chart"];
            Series series1 = chart1.Series[0];
            series1.ChartType = SeriesChartType.Doughnut;

            series1.Name = "STUDENT SUMMARY BY SECTION";

            var chart = chart1;
            chart.Series[series1.Name].XValueMember = "section";
            chart.Series[series1.Name].YValueMembers = "stu";

            chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].LegendText = "#VALX (#PERCENT)";
        }

        public void LoadChart2()
        {
            MySqlDataAdapter da = new MySqlDataAdapter("SELECT SUM(amountpaid) AS paid, term,date FROM tblfeepayment GROUP BY date, term ORDER BY date ASC", cn);
            DataSet ds = new DataSet();

            da.Fill(ds, "Chart");
            chart2.DataSource = ds.Tables["Chart"];
            Series series1 = chart2.Series[0];
            series1.ChartType = SeriesChartType.Doughnut;

            series1.Name = "DAILY INCOME BY DATE";

            var chart = chart2;
            chart.Series[series1.Name].XValueMember = "date";
            chart.Series[series1.Name].YValueMembers = "paid";

            chart.Series[0].IsValueShownAsLabel = true;
            //chart.Series[0].LegendText = "#VALX (#PERCENT)";
        }

        void getTotalStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblTotalStudent.Text = num.ToString();
        }

        void getTotalStaff()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstaff", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblTotalStaff.Text = num.ToString();
        }

        //this will get the current section where status = OPEN
        void getSection()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblsection WHERE status = 'OPEN'", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblSection.Text = dr["name"].ToString();
            }
            else
            {
                lblSection.Text = "0000-0000";
            }
            dr.Close();
            cn.Close();
        }

        //this will get the sum of money paid daily
        void getDailyIncome()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT SUM(amountpaid) FROM tblfeepayment WHERE date =@date", cn);
            cm.Parameters.AddWithValue("@date", DateTime.Now.ToShortDateString());
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            if (num == "")
            {
                lblIncome.Text = "0.00";
            }
            else
            {
                lblIncome.Text = num.ToString();
            }
        }

        //this will get the sum of students that has graduated from the school
        void getGraduatedStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent WHERE class = 'Graduated'", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblGraduated.Text = num.ToString();
        }

        //this will get the number of users that has access to this system
        void getUser()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tbluser", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblUser.Text = num.ToString();
        }

        //this will get the total number of inactive student
        void getInactiveStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent WHERE status = 0", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblInactiveStudent.Text = num.ToString();
        }

        //this will get the total number of active student
        void getActiveStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent WHERE status = 1", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblActiveStudent.Text = num.ToString();
        }

        //this will get the total number of male student
        void getMaleStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent WHERE gender = 'Male'", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblMale.Text = num.ToString();
        }

        //this will get the total number of female student
        void getFemaleStudent()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT COUNT(*) FROM tblstudent WHERE gender = 'Female'", cn);
            string num = cm.ExecuteScalar().ToString();
            cn.Close();

            lblFemale.Text = num.ToString();
        }

        private void btnStudent_Click(object sender, EventArgs e)
        {
            var f1 = new frmStudent();
            f1.btnUpdate.Enabled = false;
            f1.getSection();
            f1.LoadRecord();
            f1.getAdmissionNo();
            f1.getTotalStudent();
            f1.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var time = DateTime.Now;

            lblDate.Text = time.ToLongDateString();
            lblTime.Text = time.ToLongTimeString();

            getTotalStaff();
            getTotalStudent();
            getSection();
            getMaleStudent();
            getFemaleStudent();
            getUser();
            getActiveStudent();
            getInactiveStudent();
            getGraduatedStudent();
            getDailyIncome();
            LoadChart();
            LoadChart2();
        }

        private void btnSection_Click(object sender, EventArgs e)
        {
            var f1 = new frmSection();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnStaff_Click(object sender, EventArgs e)
        {
            var f1 = new frmStaff();
            f1.btnUpdate.Enabled = false;
            f1.LoadRecord();
            f1.getStaffID();
            f1.getTotalStaff();
            f1.ShowDialog();
        }

        private void btnSubject_Click(object sender, EventArgs e)
        {
            var f1 = new frmSubjects();
            f1.LoadRecord();
            f1.getTotalRecord();
            f1.ShowDialog();
        }

        private void btnUsers_Click(object sender, EventArgs e)
        {
            var f1 = new frmUser();
            f1.btnSave.Enabled = true;
            f1.btnUpdate.Enabled = false;
            f1.LoadRecord();
            f1.getTotalUser();
            f1.ShowDialog();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            var f1 = new frmSettings();
            f1.ShowDialog();
        }

        private void btnFeePayment_Click(object sender, EventArgs e)
        {
            var f1 = new frmFeePayment();
            f1.getPaymentID();
            f1.ShowDialog();
        }

        private void btnPaymentHistory_Click(object sender, EventArgs e)
        {
            var f1 = new frmPaymentHistory();
            f1.LoadRecord();
            f1.getAllSection();
            f1.ShowDialog();
        }

        private void btnDuePayment_Click(object sender, EventArgs e)
        {
            var f1 = new frmAllDuePayment();
            f1.getAllSection();
            f1.LoadRecord();
            f1.ShowDialog();
        }

        private void btnExam_Click(object sender, EventArgs e)
        {
            var f1 = new frmCheck();
            f1.ShowDialog();
        }

        private void btnPrintResult_Click(object sender, EventArgs e)
        {
            var f1 = new frmResultManagement();
            f1.getSection();
            f1.ShowDialog();
        }

        private void frmMainmenu_Load(object sender, EventArgs e)
        {
            LoadChart();
            LoadChart2();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure? Click Yes to Proceed!", "LOGGING OUT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Hide();
                var f1 = new frmLogin();
                f1.Show();
            }
        }
    }
}
