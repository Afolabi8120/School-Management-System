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
    public partial class frmLogin : Form
    {
        MySqlCommand cm;
        MySqlConnection cn;
        MySqlDataReader dr;
        ClassDB db = new ClassDB();

        public static string username, password, fullname, usertype;

        bool found, status;

        public frmLogin()
        {
            InitializeComponent();
            cn = new MySqlConnection();
            cn.ConnectionString = db.GetConnection();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == String.Empty)
            {
                txtUsername.Focus();
                MessageBox.Show("Username field is empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (txtPassword.Text == String.Empty)
            {
                txtPassword.Focus();
                MessageBox.Show("Password field is empty!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            else
            {
                try
                {
                    cn.Open();
                    cm = new MySqlCommand("SELECT * FROM tbluser WHERE username=@username AND password=@password", cn);
                    cm.Parameters.AddWithValue("@username", txtUsername.Text);
                    cm.Parameters.AddWithValue("@password", txtPassword.Text);
                    dr = cm.ExecuteReader();
                    dr.Read();
                    if (dr.HasRows)
                    {
                        username = dr["username"].ToString();
                        password = dr["password"].ToString();
                        fullname = dr["fullname"].ToString();
                        usertype = dr["usertype"].ToString();
                        status = Convert.ToBoolean(dr["status"].ToString());

                        found = true;
                    }
                    else
                    {
                        found = false;
                    }
                    dr.Close();
                    cn.Close();

                    if (found == true)
                    {
                        if (usertype == "Administrator" && Convert.ToBoolean(status) == true)
                        {
                            var f1 = new frmMainmenu();
                            f1.lblFullName.Text = fullname;
                            f1.lblUsertype.Text = usertype;
                            f1.Show();
                            this.Hide();
                            txtPassword.Clear();
                            txtUsername.Clear();
                        }
                        else if (usertype == "Burser" && Convert.ToBoolean(status) == true)
                        {
                            var f1 = new frmBurser();
                            f1.lblFullName.Text = fullname;
                            f1.lblUsertype.Text = usertype;
                            f1.Show();
                            this.Hide();
                            txtPassword.Clear();
                            txtUsername.Clear();
                        }
                        else if (usertype == "User" && Convert.ToBoolean(status) == true)
                        {
                            var f1 = new frmUserMenu();
                            f1.lblFullName.Text = fullname;
                            f1.lblUsertype.Text = usertype;
                            f1.Show();
                            this.Hide();
                            txtPassword.Clear();
                            txtUsername.Clear();
                        }
                    }
                    else if (found == false)
                    {
                        MessageBox.Show("Username or Password is Invalid!", "ALERT", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
