using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace iCareCrmApi.Ado
{
    public class RemoteInfoAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Query
        public DataTable GetRemoteIInfo(string CID)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT * FROM RemoteInfo WHERE RIsEnable = 1 AND CID = @CID  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@CID", SqlDbType.VarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@CID"].Value = CID;

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
        #region Add
        public void AddRemoteIInfo(string CID, string RDevice, string RType, string RNumber, string RPwd)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  INSERT INTO RemoteInfo (CID, RDevice, RType, RNumber, RPwd)
                                                VALUES (@CID, @RDevice, @RType, @RNumber, @RPwd) ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@CID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@RDevice", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@RType", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@RNumber", SqlDbType.VarChar);
            Cmd.Parameters.Add("@RPwd", SqlDbType.VarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@CID"].Value = CID;
            Cmd.Parameters["@RDevice"].Value = RDevice;
            Cmd.Parameters["@RType"].Value = RType;
            Cmd.Parameters["@RNumber"].Value = RNumber;
            Cmd.Parameters["@RPwd"].Value = RPwd;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }
        #endregion
        #region Upd
        public void UpdRemoteIInfo(int RID, string RDevice, string RType, string RNumber, string RPwd)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE RemoteInfo
                                                SET RDevice = @RDevice, 
	                                                RType = @RType,
	                                                RNumber = @RNumber,
	                                                RPwd = @RPwd,
	                                                RModifiedTime = GETDATE()
                                                WHERE RID = @RID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@RID", SqlDbType.Int);
            Cmd.Parameters.Add("@RDevice", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@RType", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@RNumber", SqlDbType.VarChar);
            Cmd.Parameters.Add("@RPwd", SqlDbType.VarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@RID"].Value = RID;
            Cmd.Parameters["@RDevice"].Value = RDevice;
            Cmd.Parameters["@RType"].Value = RType;
            Cmd.Parameters["@RNumber"].Value = RNumber;
            Cmd.Parameters["@RPwd"].Value = RPwd;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }
        public void UpdRemoteIInfo(int RID, bool RIsEnable)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE RemoteInfo
                                                SET RIsEnable = @RIsEnable,
	                                                   RModifiedTime = GETDATE()
                                                WHERE RID = @RID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@RID", SqlDbType.Int);
            Cmd.Parameters.Add("@RIsEnable", SqlDbType.Bit);

            // set values to parameters from textboxes
            Cmd.Parameters["@RID"].Value = RID;
            Cmd.Parameters["@RIsEnable"].Value = RIsEnable;
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