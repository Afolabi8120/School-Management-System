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
using Microsoft.Reporting.WinForms;

namespace School_Software
{
    public partial class frmPrintResult2 : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataAdapter da;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        string _Name, _Phone, _Email, _Motto, _RegNo, _Address;
        public string date1, date2, _admissionno, _term, _section, _class, _class1;

        public frmPrintResult2()
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

                reportViewer1.LocalReport.ReportPath = Application.StartupPath + "/Reports/ReportSlip2.rdlc";
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
                //ReportParameter pRegNo = new ReportParameter("pRegNo", _RegNo);
                ReportParameter pAddress = new ReportParameter("pAddress", _Address);
                //ReportParameter pDate1 = new ReportParameter("pDate1", date1);
                //ReportParameter pDate2 = new ReportParameter("pDate2", date2);

                reportViewer1.LocalReport.SetParameters(pName);
                reportViewer1.LocalReport.SetParameters(pPhone);
                reportViewer1.LocalReport.SetParameters(pEmail);
                reportViewer1.LocalReport.SetParameters(pMotto);
                //reportViewer1.LocalReport.SetParameters(pRegNo);
                reportViewer1.LocalReport.SetParameters(pAddress);
                //reportViewer1.LocalReport.SetParameters(pClassPromoted);
                //reportViewer1.LocalReport.SetParameters(pDate2);

                rptDS = new ReportDataSource("DataSet1", ("dtResultSlip2"));
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
            da = new MySqlDataAdapter("SELECT l.*, s.subject,s.ca,s.test,s.exam,s.total,s.grade,s.status,s.firstterm,s.secondterm,s.termtotal,s.ave FROM tblexamlist AS l INNER JOIN tblexamscore AS s ON s.admissionno = l.admissionno AND s.class = l.class AND s.section = l.section AND s.term = l.term WHERE s.admissionno = '" + _admissionno + "' AND s.class = '" + _class + "' AND s.section = '" + _section + "' AND s.term = '" + _term + "' ORDER BY subject ASC", cn);
            //da = new MySqlDataAdapter("SELECT l.*, s.subject,s.ca,s.test,s.exam,s.total,s.grade,s.status,s.firstterm,s.secondterm,s.termtotal,s.ave FROM tblexamlist AS l INNER JOIN tblexamscore AS s ON s.admissionno = l.admissionno AND s.class = l.class AND s.section = l.section AND s.term = l.term WHERE s.admissionno = 'STU-2020-2021-721' AND s.class = 'SSS 3' AND s.section = '2020-2021' AND s.term = 'Third Term' ORDER BY subject ASC", cn);
            DataSet1 ds = new DataSet1();
            da.Fill(ds, "dtResultSlip2");

            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[6]);

            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(datasource);
            this.reportViewer1.RefreshReport();
            cn.Close();
        }

        private void frmPrintResult2_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
            LoadHeader();
            LoadReceipt();
        }
    }
}
