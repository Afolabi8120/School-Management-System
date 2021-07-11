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
    public partial class frmBackupRestore : Form
    {
        //MySqlCommand cm;
        //MySqlConnection cn;
        //MySqlDataReader dr;
        //MySqlBackup mb;
        ClassDB db = new ClassDB();

        //public static string _server = "localhost";
        //public static string _username = "root";
        //public static string _password = "";

        public frmBackupRestore()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void btnBackUp_Click(object sender, EventArgs e)
        {
            string constring = "server=localhost;username=root;password=;database=sms_db;";
            string file = "C:sms_db.sql";
            using (MySqlConnection cn = new MySqlConnection(constring))
            {
                using (MySqlCommand cm = new MySqlCommand())
                {
                    using (MySqlBackup mb = new MySqlBackup(cm))
                    {
                        cm.Connection = cn;
                        cn.Open();
                        mb.ExportToFile(file);
                        cn.Close();                       
                        MessageBox.Show("Database Backup Completed...", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (frmLogin.usertype == "Administrator")
            {
                string constring = "server=localhost;username=root;password=;database=sms_db;";
                string file = "C:sms_db.sql";
                using (MySqlConnection cn = new MySqlConnection(constring))
                {
                    using (MySqlCommand cm = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cm))
                        {
                            cm.Connection = cn;
                            cn.Open();
                            mb.ImportFromFile(file);
                            cn.Close();
                            MessageBox.Show("Database Restore Completed...", "SUCCESS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Acess Denied\nOnly the Admin has access to restore database!\nPlease contact the admin!", "ACCESS DENIED", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}
