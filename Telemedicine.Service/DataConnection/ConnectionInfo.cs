using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace Telemedicine.Service.DataConnection
{
    public class Connection
    {
        public string ConnectionString { get; set; }
    }
    public class ConnectionInfo
    {

        
        public static Connection GetConnection(string PMSFolder)
        {
            //string value = @"Data Source=LAPTOP-56RNU54V;Initial Catalog=TelemedicineClone;Integrated Security=True"; 
         // string value = @"Data Source =localhost\SQLEXPRESS; Initial Catalog = tele; Persist Security Info = True; User ID = roy; Password = Abcd@123456";
        string value = @"Data Source =EC2AMAZ-ERQKSGT; Initial Catalog = Telemedicine; Persist Security Info = True; User ID = user01; Password = Telemed$2022";
             
            var prop = new Connection
            {
                ConnectionString = value
            };
            return prop;
        }

        public static string LogFileDirectory = "D:\\LogFiles\\Telemedicine\\";

    }
}
