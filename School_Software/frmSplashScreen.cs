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
    public partial class frmSplashScreen : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public frmSplashScreen()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            LoadRecord();
        }

        public void LoadRecord()
        {
            cn.Open();
            cm = new MySqlCommand("SELECT * FROM tblschoolinfo", cn);
            dr = cm.ExecuteReader();
            dr.Read();
            if (dr.HasRows)
            {
                lblSchoolName.Text = dr["name"].ToString();
            }
            else
            {
                lblSchoolName.Text = "School Management System";
            }
            dr.Close();
            cn.Close();
        }

        void LoadScreen()
        {
            panelLoading.Width += 5;

            if (panelLoading.Width == 120)
            {
                lblLoading.Text = "...";
            }
            else if (panelLoading.Width == 150)
            {
                lblLoading.Text = "Please wait.";
            }
            else if (panelLoading.Width == 350)
            {
                lblLoading.Text = "Please wait..";
            }
            else if (panelLoading.Width == 450)
            {
                lblLoading.Text = "Please wait...";
            }
            else if (panelLoading.Width == 420)
            {
                lblLoading.Text = "Almost Done!!!";
            }
            else if (panelLoading.Width == 650)
            {
                lblLoading.Text = "Setting Up The Software...";            
            }
            if (panelLoading.Width == 670)
            {
                this.Hide();   
                var f1 = new frmLogin();
                f1.ShowDialog();                           
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadScreen();
        }
    }
}
