using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IO;

namespace Telemedicine.Service.DataConnection
{
    public class ExceptionHandler
    {
        public static void LogErrorToText(Exception ex, string browserinfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("********************" + " Error Log - " + DateTime.Now + "*********************");
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append(" browser info Log => " + browserinfo);
            sb.Append(Environment.NewLine);
            sb.Append(Environment.NewLine);
            sb.Append("Exception Type : " + ex.GetType().Name);
            sb.Append(Environment.NewLine);
            sb.Append("Error Message : " + ex.Message);
            sb.Append(Environment.NewLine);
            sb.Append("Error Source : " + ex.Source);
            sb.Append(Environment.NewLine);
            if (ex.StackTrace != null)
            {
                sb.Append("Error Trace : " + ex.StackTrace);
            }
            Exception innerEx = ex.InnerException;

            while (innerEx != null)
            {
                sb.Length = 0;
                sb.Append("********************" + " Error Log - " + DateTime.Now + "*********************");
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append(" browser info Log => " + browserinfo);
                sb.Append(Environment.NewLine);
                sb.Append(Environment.NewLine);
                sb.Append("Exception Type : " + innerEx.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append("Error Message : " + innerEx.Message);
                sb.Append(Environment.NewLine);
                sb.Append("Error Source : " + innerEx.Source);
                sb.Append(Environment.NewLine);
                if (ex.StackTrace != null)
                {
                    sb.Append("Error Trace : " + innerEx.StackTrace);
                }
                innerEx = innerEx.InnerException;
            }
            string filePath = ConnectionInfo.LogFileDirectory; //; AppDomain.CurrentDomain.BaseDirectory + "\\ErrorLog.log";
            if (File.Exists(filePath))
            {
                StreamWriter writer = new StreamWriter(filePath, true);
                writer.WriteLine(sb.ToString());
                writer.Flush();
                writer.Close();
            }
        }
    }
}
