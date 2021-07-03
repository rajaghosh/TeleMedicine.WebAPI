using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace Telemedicine.Service.DataConnection
{
    public partial class DataProvider
    {
        public SqlConnection Con = new SqlConnection();
        public SqlCommand Cmd = new SqlCommand();
        public SqlCommandBuilder CB = new SqlCommandBuilder();
        public SqlDataReader DR;
        public SqlDataAdapter DA = new SqlDataAdapter();
        public DataSet DS = new DataSet();
        public DataTable DT = new DataTable();

        public class Parameter
        {
            public string ParameterName;
            public SqlDbType DataType;
            public object Value;
            public bool IsOutput;

            public Parameter(string parameterName, object value = null, SqlDbType dataType = SqlDbType.VarChar, bool isOutput = false)
            {
                ParameterName = parameterName;
                Value = value;
                DataType = dataType;
                IsOutput = isOutput;
            }
        }

        public DataProvider()
        {
            Con = new SqlConnection();
            Cmd = new SqlCommand();
            CB = new SqlCommandBuilder();
            DA = new SqlDataAdapter();
            DS = new DataSet();
            DT = new DataTable();
        }
        private bool Open(String PMSFolder)
        {
            Con = new SqlConnection();
            try
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                    Con.ConnectionString = ConnectionInfo.GetConnection(PMSFolder).ConnectionString;
                    Con.Open();
                }
                else
                {
                    Con.ConnectionString = ConnectionInfo.GetConnection(PMSFolder).ConnectionString;
                    Con.Open();
                }
                return true;
            }
            catch (Exception ex)
            {

                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
        }
        private bool Close()
        {
            try
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
        }
        public async Task<bool> ExecuteSP(string PMSFolder, string SPName, params Parameter[] Parameters)
        {
            try
            {
                return await Task.Run(() =>
                {
                    Cmd = new SqlCommand();
                    Open(PMSFolder);
                    Cmd.CommandText = SPName;
                    Cmd.Connection = Con;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    foreach (Parameter parameter in Parameters)
                    {
                        if (parameter.Value == null && !parameter.IsOutput)
                            continue;

                        SqlParameter param = new SqlParameter();
                        param.ParameterName = parameter.ParameterName;
                        param.SqlDbType = parameter.DataType;
                        param.Value = parameter.Value;
                        param.Direction = parameter.IsOutput ? ParameterDirection.InputOutput : ParameterDirection.Input;
                        Cmd.Parameters.Add(param);
                    }
                    Cmd.ExecuteNonQuery();

                    foreach (Parameter parameter in Parameters)
                    {
                        if (parameter.IsOutput)
                            parameter.Value = Cmd.Parameters[parameter.ParameterName].Value;
                    }

                    return true;
                });


            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Close();
                Cmd = null;
                Con = null;
            }
        }

        public async Task<bool> ExecuteQuery(string PMSFolder, string query)
        {
            try
            {
                return await Task.Run(() =>
                {
                    Cmd = new SqlCommand();
                    Open(PMSFolder);
                    Cmd.CommandText = query;
                    Cmd.Connection = Con;
                    Cmd.CommandType = CommandType.Text;
                    Cmd.ExecuteNonQuery();
                    return true;
                });


            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Close();
                Cmd = null;
                Con = null;
            }
        }

        public async Task<DataTable> GetDataTable(string PMSFolder, string SPName, params Parameter[] Parameters)
        {
            try
            {
                return await Task.Run(() =>
                {

                    DT = new DataTable();
                    Cmd = new SqlCommand();
                    Open(PMSFolder);
                    Cmd.CommandText = SPName;
                    Cmd.Connection = Con;
                    Cmd.CommandType = CommandType.StoredProcedure;
                    foreach (Parameter parameter in Parameters)
                    {
                        SqlParameter param = new SqlParameter();
                        param.ParameterName = parameter.ParameterName;
                        param.SqlDbType = parameter.DataType;
                        param.Value = parameter.Value;
                        param.Direction = parameter.IsOutput ? ParameterDirection.InputOutput : ParameterDirection.Input;
                        Cmd.Parameters.Add(param);
                    }
                    DA = new SqlDataAdapter(Cmd);
                    DA.Fill(DT);
                    return DT;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Close();
                Cmd = null;
                Con = null;
            }
        }

        public async Task<DataTable> GetDataTable(string PMSFolder, string query)
        {
            try
            {
                return await Task.Run(() =>
                {

                    DT = new DataTable();
                    Cmd = new SqlCommand();
                    Open(PMSFolder);
                    Cmd.CommandText = query;
                    Cmd.Connection = Con;
                    DA = new SqlDataAdapter(Cmd);
                    DA.Fill(DT);
                    return DT;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Close();
                Cmd = null;
                Con = null;
            }
        }

        public SqlDataReader GetDataReader(string PMSFolder, string SPName, SqlCommand Cmd)
        {
            try
            {
                Open(PMSFolder);
                Cmd.CommandText = SPName;
                Cmd.Connection = Con;
                Cmd.CommandType = CommandType.StoredProcedure;

                return Cmd.ExecuteReader();
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con = null;
                Cmd = null;
            }
        }
        public int ExeNonQuery(string PMSFolder, string Query)
        {
            try
            {
                int rowsAffected = 0;
                Open(PMSFolder);
                Cmd = new SqlCommand(Query, Con);
                Cmd.CommandType = CommandType.Text;
                rowsAffected = Cmd.ExecuteNonQuery();
                return rowsAffected;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

        public async Task<int> CmdExeNonQuery(string PMSFolder, SqlCommand Cmd)
        {
            try
            {
                return await Task.Run(() =>
                {
                    int rowsAffected = 0;
                    Open(PMSFolder);
                    Cmd.CommandType = CommandType.Text;
                    Cmd.Connection = Con;
                    return rowsAffected = Cmd.ExecuteNonQuery();
                });
                 
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

        public async Task<DataTable> CmdGetDatatable(string PMSFolder, SqlCommand Cmd)
        {
            try
            {
                return await Task.Run(() =>
                {
                    Open(PMSFolder);
                    Cmd.CommandType = CommandType.Text;
                    Cmd.Connection = Con;

                    DataTable dataTable = new DataTable();
                    new SqlDataAdapter(Cmd).Fill(dataTable);
                    return dataTable;
                });

            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }
        public SqlDataReader ReaderReturn(string PMSFolder, SqlCommand Cmd)
        {
            try
            {
                Open(PMSFolder);
                Cmd.CommandType = CommandType.Text;
                Cmd.Connection = Con;
                DR = Cmd.ExecuteReader();
                return DR;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con = null;
                Cmd = null;
            }
        }
        public async Task<bool> CheckIfExists(string PMSFolder, SqlCommand Cmd)
        {

            try
            {
                return await Task.Run(() =>
                {
                    Open(PMSFolder);
                    Cmd.CommandType = CommandType.Text;
                    Cmd.Connection = Con;
                    DR = Cmd.ExecuteReader();
                    if (DR.HasRows)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }
        public int GetMaximumId(string PMSFolder, string Query)
        {
            try
            {
                int maxId = 0;
                Open(PMSFolder);
                Cmd = new SqlCommand(Query, Con);
                Cmd.CommandType = CommandType.Text;
                maxId = Convert.ToInt32(Cmd.ExecuteScalar());
                return maxId;
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;
            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

        public async Task<object> GetScalarval(string PMSFolder, string Query)
        {
            try
            {
                return await Task.Run(() =>
                {
                    object Rtnval = 0;
                    Open(PMSFolder);
                    Cmd = new SqlCommand(Query, Con);
                    Rtnval = Cmd.ExecuteScalar();
                    return Rtnval;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;

            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

        public async Task<object> CmdGetScalarval(string PMSFolder, SqlCommand Command)
        {
            try
            {
                return await Task.Run(() =>
                {
                    object Rtnval = 0;
                    Open(PMSFolder);
                    Command.CommandType = CommandType.Text;
                    Command.Connection = Con;
                    Rtnval = Command.ExecuteScalar();
                    return Rtnval;
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;

            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

        public async Task<string> GetScalarStringval(string PMSFolder, string Query)
        {
            try
            {
                return await Task.Run(() =>
                {
                    object Rtnval = 0;
                    Open(PMSFolder);
                    Cmd = new SqlCommand(Query, Con);
                    Rtnval = Cmd.ExecuteScalar();

                    SqlDataReader reader = Cmd.ExecuteReader(); 
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(); 
                    while (reader.Read()) sb.Append(reader.GetSqlString(0).Value);
                    return sb.ToString();
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.LogErrorToText(ex, "");
                throw;

            }
            finally
            {
                Con.Close();
                Con = null;
                Cmd = null;
            }
        }

    }

}
