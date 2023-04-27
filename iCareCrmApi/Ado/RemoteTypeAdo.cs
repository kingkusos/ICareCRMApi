using System;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace iCareCrmApi.Ado
{
    public class RemoteTypeAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Query
        public DataTable QueryRemoteType()
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT * FROM RemoteType  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    //Cmd.Parameters.Add("@UAccount", SqlDbType.VarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    //Cmd.Parameters["@UAccount"].Value = UAccount;

                    #endregion

                    SqlDataAdapter Adt = new SqlDataAdapter(Cmd);
                    Adt.Fill(dt);

                    Con.Close();
                }
            }
            catch (Exception ex)
            {

            }
            return dt;
        }
        
        #endregion
    }
}