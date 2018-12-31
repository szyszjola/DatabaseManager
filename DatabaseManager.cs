using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseManager
{

    public class DatabaseManager
    {

        string connectionString;
        SqlConnection connection;
        public static bool IsAvailable;

        public DatabaseManager(string serwer, string baza)
        {
            connectionString = "Data Source=" + serwer + ";Initial Catalog=" + baza + ";Integrated Security = True;Connect Timeout=20";

            connection = new SqlConnection(@connectionString);
            IsSqlAvailable();
        }

        public DatabaseManager(string server, string database, string user, string password)
        {
            connectionString = "Data Source=" + server + ";Initial Catalog=" + database + ";user=" + user + ";password=" + password + ";Connect Timeout=20";

            connection = new SqlConnection(@connectionString);
            IsSqlAvailable();
        }

        public bool ExecuteProcedureSql(SqlCommand pSqlCmd)
        {
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        pSqlCmd.Connection = con;
                        pSqlCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }


        public DataTable ExecuteProcedureSqlTable(SqlCommand pSqlCmd)
        {
            DataTable dt = new DataTable();
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter())
                        {
                            con.Open();
                            pSqlCmd.Connection = con;
                            da.SelectCommand = pSqlCmd;

                            DataSet ds = new DataSet();
                            da.Fill(ds, "wynik");

                            dt = ds.Tables["wynik"];

                            con.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return dt;
        }


        public DataTable ExecuteQueryTSQL(string query)
        {
            DataTable t = new DataTable();
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlDataAdapter DataAdapter = new SqlDataAdapter(query, con))
                        {
                            DataAdapter.Fill(t);
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return t;
                }
            }
            else
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return t;
        }

        public bool IsSqlAvailable()
        {
            try
            {
                var conn = new SqlConnection(connectionString);
                conn.Open();
                conn.Close();
                IsAvailable = true;
                return true;
            }
            catch (Exception)
            {
                IsAvailable = false;
                return false;
            }
        }

        public String ExecuteQuerySkalar(string query)
        {
            String wynik = "";
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(query, con);
                        string tmp = cmd.ExecuteScalar().ToString();
                        if (tmp != null)
                        {
                            wynik = tmp.ToString();
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return "brak";
                }
            }
            else
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            return wynik;
        }

        public bool ExecuteQuery(string query)
        {
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        SqlCommand cmd = new SqlCommand(query, con);
                        cmd.ExecuteNonQuery();
                        con.Close();
                        return true;
                    }
                    
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return false;
            }
        }

        public void ExecuteTransactionSQL(List<SqlCommand> pSqlCmd)
        {
            if (IsAvailable)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlTransaction tran = con.BeginTransaction())
                        {
                            try
                            {
                                for (int i = 0; i < pSqlCmd.Count; i++)
                                {
                                    pSqlCmd[i].Connection = con;
                                    pSqlCmd[i].Transaction = tran;

                                    pSqlCmd[i].ExecuteNonQuery();
                                }
                                tran.Commit();
                            }
                            catch (Exception ex)
                            {
                                tran.Rollback();
                                MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                            }
                        }
                        con.Close();
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message, "Wystąpił błąd", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
                MessageBox.Show("Serwer aplikacji jest nieosiągalny. Jeśli widzisz komunikat po raz pierwszy - ponów próbę. Jeśli komunikat się powtarza sprawdź dostępność sieci firmowej.", "Błąd połączenia", MessageBoxButtons.OK, MessageBoxIcon.Stop);
        }
    }
}


