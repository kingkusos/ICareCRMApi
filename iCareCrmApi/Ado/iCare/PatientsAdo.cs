using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;

namespace iCareCrmApi.Ado.iCare
{
    public class PatientsAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionStringiCare"];

        #region Query
        public int GetCountPatient()
        {
            DataTable dt = new DataTable();
            try
            {

                string sqlcmd = @"  SELECT (COUNT(*) + 106500) AS PNum FROM Patients  ";

                using (SqlConnection Con = new SqlConnection(conn))
                {
                    Con.Open();

                    SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

                    #region create your parameters
                    // create your parameters
                    //Cmd.Parameters.Add("@CID", SqlDbType.VarChar);

                    #endregion

                    #region set values to parameters from textboxes
                    // set values to parameters from textboxes
                    //Cmd.Parameters["@CID"].Value = CID;

                    #endregion

                    SqlDataAdapter Adt = new SqlDataAdapter(Cmd);
                    Adt.Fill(dt);

                    Con.Close();
                }
            }
            catch (Exception ex)
            {

            }

            int count = 0;
            if (dt != null && dt.Rows.Count > 0)
            {
                count = int.Parse(dt.Rows[0]["PNum"].ToString());
            }

            return count;
        }
        #endregion
    }
}