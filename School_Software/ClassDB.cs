using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace School_Software
{
    class ClassDB
    {
        public string GetConnection()
        {
            string cn = "server = localhost; username=root; password=; database= sms_db;";
            return cn;
        }
    }
}
