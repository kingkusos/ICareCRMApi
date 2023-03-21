using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Configuration;

namespace iCareCrmApi.Ado
{
    public class VisitEditLogAdo
    {
        string conn = ConfigurationSettings.AppSettings["ConnectionString"];
        #region Add
        public void AddVisitEditLog(int LogID)
        {
            SqlConnection Con = new SqlConnection(conn);

            string sqlcmd = @"  INSERT INTO VisitEditLog(LogID, VisitTime, VisitType, VisitContent)
                                                SELECT LogID, VisitTime, VisitType, VisitContent FROM VisitLog WHERE LogID = @LogID ";

            SqlCommand Cmd = new SqlCommand(sqlcmd, Con);

            // create your parameters
            Cmd.Parameters.Add("@LogID", SqlDbType.Int);

            // set values to parameters from textboxes
            Cmd.Parameters["@LogID"].Value = LogID;

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