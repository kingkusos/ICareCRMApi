using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace iCareCrmApi.Ado
{
    public class ClinicInfoAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Query
        public DataTable GetClinicInfo(string CID)
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT  * FROM ClinicsInfo WHERE CID = @CID  ";

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

        public DataTable QueryPagingClinicInfoBySearch(int Pcurrent, int PSize, string CCity, string CArea, string CAddr, string CType, string Sort)
        {
            DataTable dt = new DataTable();
            try
            {
                #region 
                string sqlSearch = string.Empty;
                if (!string.IsNullOrWhiteSpace(CCity))
                {
                    if (string.IsNullOrWhiteSpace(sqlSearch))
                    {
                        sqlSearch += "  WHERE CIN.CCity = @CCity    ";
                    }
                    else
                    {
                        sqlSearch += "  AND CIN.CCity = @CCity    ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(CArea))
                {
                    if (string.IsNullOrWhiteSpace(sqlSearch))
                    {
                        sqlSearch += "  WHERE CIN.CArea = @CArea    ";
                    }
                    else
                    {
                        sqlSearch += "  AND CIN.CArea = @CArea   ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(CAddr))
                {
                    if (string.IsNullOrWhiteSpace(sqlSearch))
                    {
                        sqlSearch += "  WHERE (CIN.CAddr LIKE '%'+ @CAddr +'%' OR CIN.CName  LIKE '%'+ @CAddr +'%')     ";
                    }
                    else
                    {
                        sqlSearch += "  AND (CIN.CAddr LIKE '%'+ @CAddr +'%' OR CIN.CName  LIKE '%'+ @CAddr +'%')   ";
                    }
                }
                if (!string.IsNullOrWhiteSpace(CType))
                {
                    if (string.IsNullOrWhiteSpace(sqlSearch))
                    {
                        sqlSearch += "  WHERE CIN.CType = @CType    ";
                    }
                    else
                    {
                        sqlSearch += "  AND CIN.CType = @CType   ";
                    }
                }

                string sqlOrder = " ORDER BY CTEResults.VisitTime DESC  ";
                if(Sort.Trim().ToLower() == "reverse")
                {
                    sqlOrder = " ORDER BY CTEResults.VisitTime ASC  ";
                }
                #endregion


                string sqlcmd = @"  WITH CTEResults AS (
														SELECT
														ROW_NUMBER() OVER (ORDER BY NVL.VisitTime DESC) as Rownumber,
														UIN.[UID],
														UIN.UNickName,
														NVL.VisitTime,
														CIN.* 
														FROM ClinicsInfo CIN
														LEFT JOIN (
															SELECT 
															ROW_NUMBER() OVER (PARTITION BY CID ORDER BY VisitTime DESC) AS TIME_Sort,
															* 
															FROM VisitLog 
															WHERE IsEnable = 1
														) NVL ON CIN.CID = NVL.CID AND TIME_Sort = 1
														LEFT JOIN UsersInfo UIN ON NVL.UserID = UIN.[UID]
                                                        " + sqlSearch + @"
													) 
													SELECT
                                                    CAST(CEILING(CAST((SELECT MAX(RowNumber) FROM CTEResults) as float) / @PSize) as Int) as TotalPage,
                                                    (SELECT MAX(RowNumber) FROM CTEResults) as TotalCnt,
                                                    * 
                                                    FROM CTEResults
                                                    " + sqlOrder + @"
                                                    OFFSET (@Pcurrent - 1) * @PSize ROWS FETCH NEXT @PSize ROWS ONLY    ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    Cmd.Parameters.Add("@Pcurrent", SqlDbType.Int);
                    Cmd.Parameters.Add("@PSize", SqlDbType.Int);
                    Cmd.Parameters.Add("@CCity", SqlDbType.NVarChar);
                    Cmd.Parameters.Add("@CArea", SqlDbType.NVarChar);
                    Cmd.Parameters.Add("@CAddr", SqlDbType.NVarChar);
                    Cmd.Parameters.Add("@CType", SqlDbType.NVarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    Cmd.Parameters["@Pcurrent"].Value = Pcurrent;
                    Cmd.Parameters["@PSize"].Value = PSize;
                    Cmd.Parameters["@CCity"].Value = CCity;
                    Cmd.Parameters["@CArea"].Value = CArea;
                    Cmd.Parameters["@CAddr"].Value = CAddr;
                    Cmd.Parameters["@CType"].Value = CType;

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
        public void UpdStatusByCId(string CID,  string CType)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE ClinicsInfo
                                                    SET CType = @CType
                                                    WHERE CID = @CID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@CID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@CType", SqlDbType.NVarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@CID"].Value = CID;
            Cmd.Parameters["@CType"].Value = CType;
            // open sql connection
            Con.Open();

            // execute the query and return number of rows affected, should be one
            Cmd.ExecuteNonQuery();

            // close connection when done
            Con.Close();
        }

        public void UpdInfoByCId(string CID, string CName, string CPhone, string CCity, string CArea, string CAddr, string HisSys, bool IsVideo, bool IsMaster, int NumberDr, string CallWay, bool IsOrderTime, string OrderInfo, string MedicalGroup, string EvenPracticing, bool IsSharedCare, string SharedCareContent)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  UPDATE ClinicsInfo
                                                SET  CName = @CName,
													    CPhone = @CPhone,
													    CCity = @CCity,
													    CArea = @CArea,
													    CAddr = @CAddr,
													    HisSys = @HisSys,
													    IsVideo = @IsVideo,
													    IsMaster = @IsMaster,
													    NumberDr = @NumberDr,
													    CallWay = @CallWay,
													    IsOrderTime = @IsOrderTime,
													    OrderInfo = @OrderInfo,
													    MedicalGroup = @MedicalGroup,
													    EvenPracticing = @EvenPracticing,
													    IsSharedCare = @IsSharedCare,
													    SharedCareContent = @SharedCareContent
                                                WHERE CID = @CID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@CID", SqlDbType.VarChar);
            Cmd.Parameters.Add("@CName", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@CPhone", SqlDbType.VarChar);
            Cmd.Parameters.Add("@CCity", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@CArea", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@CAddr", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@HisSys", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@IsVideo", SqlDbType.Bit);
            Cmd.Parameters.Add("@IsMaster", SqlDbType.Bit);
            Cmd.Parameters.Add("@NumberDr", SqlDbType.Int);
            Cmd.Parameters.Add("@CallWay", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@IsOrderTime", SqlDbType.Bit);
            Cmd.Parameters.Add("@OrderInfo", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@MedicalGroup", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@EvenPracticing", SqlDbType.NVarChar);
            Cmd.Parameters.Add("@IsSharedCare", SqlDbType.Bit);
            Cmd.Parameters.Add("@SharedCareContent", SqlDbType.NVarChar);

            // set values to parameters from textboxes
            Cmd.Parameters["@CID"].Value = CID;
            Cmd.Parameters["@CName"].Value = CName;
            Cmd.Parameters["@CPhone"].Value = CPhone;
            Cmd.Parameters["@CCity"].Value = CCity;
            Cmd.Parameters["@CArea"].Value = CArea;
            Cmd.Parameters["@CAddr"].Value = CAddr;
            Cmd.Parameters["@HisSys"].Value = HisSys;
            Cmd.Parameters["@IsVideo"].Value = IsVideo;
            Cmd.Parameters["@IsMaster"].Value = IsMaster;
            Cmd.Parameters["@NumberDr"].Value = NumberDr;
            Cmd.Parameters["@CallWay"].Value = CallWay;
            Cmd.Parameters["@IsOrderTime"].Value = IsOrderTime;
            Cmd.Parameters["@OrderInfo"].Value = OrderInfo;
            Cmd.Parameters["@MedicalGroup"].Value = MedicalGroup;
            Cmd.Parameters["@EvenPracticing"].Value = EvenPracticing;
            Cmd.Parameters["@IsSharedCare"].Value = IsSharedCare;
            Cmd.Parameters["@SharedCareContent"].Value = SharedCareContent;
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