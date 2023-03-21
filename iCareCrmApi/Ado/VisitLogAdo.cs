using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Security.Cryptography;

namespace iCareCrmApi.Ado
{
    public class VisitLogAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Add
        public void AddVisitLog(string CID, string UserID, DateTime VisitTime, string VisitType, string VisitContent)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  INSERT INTO VisitLog (CID, UserID, VisitTime, VisitType, VisitContent)
                                                VALUES (@CID, @UserID, @VisitTime, @VisitType, @VisitContent) ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@CID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@UserID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@VisitTime", SqlDbType.DateTime);
            Cmd.Parameters.Add("@VisitType", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@VisitContent", SqlDbType.NVarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@CID"].Value = CID;
            Cmd.Parameters["@UserID"].Value = UserID;
            Cmd.Parameters["@VisitTime"].Value = VisitTime;
            Cmd.Parameters["@VisitType"].Value = VisitType;
            Cmd.Parameters["@VisitContent"].Value = VisitContent;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }
        #endregion
        #region Upd
        public void UpdVisitLog(int LogID, DateTime VisitTime, string VisitType, string VisitContent)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE VisitLog
                                                SET VisitTime = @VisitTime,
	                                                VisitType = @VisitType,
	                                                VisitContent = @VisitContent
                                                WHERE LogID = @LogID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@LogID", SqlDbType.Int);
            Cmd.Parameters.Add("@VisitTime", SqlDbType.DateTime);
            Cmd.Parameters.Add("@VisitType", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@VisitContent", SqlDbType.NVarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@LogID"].Value = LogID;
            Cmd.Parameters["@VisitTime"].Value = VisitTime;
            Cmd.Parameters["@VisitType"].Value = VisitType;
            Cmd.Parameters["@VisitContent"].Value = VisitContent;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }
        #endregion
        #region Query
        public DataTable GetVisitLogById(int LogID)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT * FROM VisitLog WHERE IsEnable = 1 AND LogID = @LogID  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@LogID", SqlDbType.Int);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@LogID"].Value = LogID;

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

        public DataTable GetVisitLogByCId(int Pcurrent, int PSize,  string CID, string VisitContent)
        {
            DataTable dt = new DataTable();
            try
            {
                #region 
                string sqlString = string.Empty;
                if (!string.IsNullOrWhiteSpace(VisitContent))
                {
                    sqlString = " AND VisitContent LIKE '%'+ @VisitContent +'%' ";
                }
                #endregion

                string sqlcmd = @"  WITH CTEResults AS (
	                                                    SELECT 
	                                                    ROW_NUMBER() OVER (ORDER BY VL.VisitTime DESC) as Rownumber,
	                                                    UNickName,
                                                        CIN.CType,
	                                                    VL.* 
	                                                    FROM VisitLog VL
	                                                    LEFT JOIN UsersInfo UIN ON VL.UserID = UIN.[UID]
                                                        LEFT JOIN ClinicsInfo CIN ON VL.CID = CIN.CID
	                                                    WHERE VL.IsEnable = 1 
	                                                    AND VL.CID = @CID
                                                        " + sqlString + @"
                                                    ) 
                                                    SELECT
                                                    CAST(CEILING(CAST((SELECT MAX(RowNumber) FROM CTEResults) as float) / @PSize) as Int) as TotalPage,
                                                    (SELECT MAX(RowNumber) FROM CTEResults) as TotalCnt,
                                                    * 
                                                    FROM CTEResults
                                                    ORDER BY CTEResults.VisitTime DESC
                                                    OFFSET (@Pcurrent - 1) * @PSize ROWS FETCH NEXT @PSize ROWS ONLY  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@Pcurrent", SqlDbType.Int);
                    Cmd.Parameters.Add("@PSize", SqlDbType.Int);
                    Cmd.Parameters.Add("@CID", SqlDbType.VarChar);
                    Cmd.Parameters.Add("@VisitContent", SqlDbType.NVarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@Pcurrent"].Value = Pcurrent;
                    Cmd.Parameters["@PSize"].Value = PSize;
                    Cmd.Parameters["@CID"].Value = CID;
                    Cmd.Parameters["@VisitContent"].Value = VisitContent;

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