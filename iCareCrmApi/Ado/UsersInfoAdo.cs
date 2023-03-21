using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace iCareCrmApi.Ado
{
    public class UsersInfoAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Query
        public DataTable GetUsersInfo(string UAccount)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT  * FROM UsersInfo WHERE UAccount = @UAccount  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@UAccount", SqlDbType.VarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@UAccount"].Value = UAccount;

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
        public DataTable GetUsersInfoByToken(string UAccessToken)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT  * FROM UsersInfo WHERE UAccessToken = @UAccessToken  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@UAccessToken", SqlDbType.VarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@UAccessToken"].Value = UAccessToken;

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
        #region Upd
        public void UpdLastLogin(string UID, string UAccessToken, string UExpired)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE UsersInfo
                                                SET ULastLoginDate = GETDATE(),
									                UAccessToken = @UAccessToken,
									                UExpired = @UExpired
                                                WHERE [UID] = @UID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@UID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@UAccessToken", SqlDbType.VarChar);
            Cmd.Parameters.Add("@UExpired", SqlDbType.VarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@UID"].Value = UID;
            Cmd.Parameters["@UAccessToken"].Value = UAccessToken;
            Cmd.Parameters["@UExpired"].Value = UExpired;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }
        #endregion

    }
}