//using NWC.DTO;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data;
//using System.Data.SqlClient;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NWC.Service.SignalR
//{
//    public class DBDependency
//    {
//        public SqlDependency dependency;
//        private string IsNotifiedTable;

//        public long StartID { get; private set; }

//        public DBDependency(string isNotifiedTable)
//        {
//            this.IsNotifiedTable = isNotifiedTable;
//            SetStartID();
//        }

//        private void SetStartID()
//        {
//            //notification listening will start from this ID and ignore previous ids
//            //this important to avoid sending old notifications
//            //do not supress exceptions comming from this method

//            SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["ICCCDB_DEV"].ConnectionString);

//            if(con.State != ConnectionState.Open)
//            con.Open();

//            SqlCommand tcmd = new SqlCommand("select max(id) from notification", con);
//            tcmd.CommandType = CommandType.Text;
//            var result = tcmd.ExecuteScalar();

//            long startid = (long)result;

//            con.Close();

//            StartID = startid;
//        }

//        //public void HookNotification()
//        //{
//        //    try
//        //    {
//        //        var connectionstring = ConfigurationManager.ConnectionStrings["ICCCDB_DEV"].ConnectionString;

//        //        using (SqlConnection con = new SqlConnection(connectionstring))
//        //        {
//        //            DataTable dt = new DataTable();


//        //            SqlCommand cmd = new SqlCommand("Select ID from [dbo].[Notification] where ID > " + StartID, con);

//        //            cmd.CommandType = CommandType.Text;
//        //            cmd.Notification = null;


//        //            dependency = new SqlDependency(cmd);
//        //            dependency.OnChange += new OnChangeEventHandler(UpdateClients);



//        //            if (con.State == ConnectionState.Closed)
//        //                con.Open();

//        //            cmd.ExecuteNonQuery();
//        //        }
//        //    }
//        //    catch (Exception e)
//        //    {
//        //        //ExceptionManager.GetExceptionLogger().LogException(e);
//        //    }
//        //}

//        //public void UpdateClients(object sender, SqlNotificationEventArgs e)
//        //{
//        //    try
//        //    {
//        //        if (e.Type == SqlNotificationType.Change)
//        //        {
//        //            dependency = sender as SqlDependency;
//        //            dependency.OnChange -= UpdateClients;
//        //            HookNotification();
//        //        }
//        //        var lst_IDs = NotifyEnteredData();
//        //        UpdateNotificationAsSend(lst_IDs);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //ExceptionManager.GetExceptionLogger().LogException(ex);
//        //    }
//        //}

//        //public List<long> NotifyEnteredData()
//        //{
//        //    try
//        //    {
//        //        List<long> lst_IDs = new List<long>();
//        //        var connectionstring = ConfigurationManager.ConnectionStrings["ICCCDB_DEV"].ConnectionString;

//        //        using (SqlConnection con = new SqlConnection(connectionstring))
//        //        {
//        //            DataTable dt = new DataTable();

//        //            SqlCommand cmd = new SqlCommand(
//        //                " Select * from [dbo].[Notification] " +
//        //                " where ID > " + StartID +
//        //                " and not exists (select 1 from " + IsNotifiedTable + " op where op.ID = Notification.ID) ", con);


//        //            cmd.CommandType = CommandType.Text;

//        //            if (con.State == ConnectionState.Closed)
//        //                con.Open();

//        //            dt.Load(cmd.ExecuteReader(CommandBehavior.CloseConnection));

//        //            //insertToTemp(dt.Rows.Count.ToString());

//        //            PersonQuery personBL = new PersonQuery();

//        //            foreach (DataRow dr in dt.Rows)
//        //            {
//        //                lst_IDs.Add(long.Parse(dr[0].ToString()));
//        //                var dto = FillDTOWithDataRow(dr, personBL);
//        //                Send(dto, string.Empty, "1");//dr[1].ToString().ToString()
//        //            }
//        //            return lst_IDs;
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //ExceptionManager.GetExceptionLogger().LogException(ex);
//        //        throw;
//        //    }
//        //}

//        //public NotificationDTO FillDTOWithDataRow(DataRow dr, PersonQuery personBL)
//        //{
//        //    try
//        //    {
//        //        NotificationDTO notificationDTO = new NotificationDTO();
//        //        notificationDTO.ID = long.Parse(dr[0].ToString());
//        //        notificationDTO.Description = dr[1].ToString();
//        //        notificationDTO.NotifyingRecordId = long.Parse(dr[2].ToString());
//        //        notificationDTO.NotificationType = dr[3].ToString();
//        //        notificationDTO.CreatedBy = dr[4].ToString() != string.Empty ? long.Parse(dr[4].ToString()) : 0;
//        //        notificationDTO.IsNotified = bool.Parse(dr[5].ToString());
//        //        notificationDTO.Criticality = int.Parse(dr[6].ToString());
//        //        //notificationDTO.NotificationDateTime = !string.IsNullOrEmpty(dr[7].ToString()) && dr[7] != null ? DateTime.Parse(dr[7].ToString()) : (DateTime?)null;                
//        //        notificationDTO.NotificationDateTime = dr.Field<DateTime?>("NotificationDateTime");

//        //        notificationDTO.IncidentId = !string.IsNullOrEmpty(dr[8].ToString()) && dr[8] != null ? long.Parse(dr[8].ToString()) : (long?)null;
//        //        notificationDTO.NotificationConditions = dr[9].ToString();
//        //        //notificationDTO.CreatedName = personBL.GetPersonAspnetUserName(dr[4].ToString());
//        //        return notificationDTO;
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //ExceptionManager.GetExceptionLogger().LogException(ex);
//        //        throw;
//        //    }
//        //}

//        public void UpdateNotificationAsSend(List<long> lst_IDs)
//        {
//            try
//            {
//                if (lst_IDs.Count == 0)
//                    return;

//                var connectionstring = ConfigurationManager.ConnectionStrings["ICCCDB_DEV"].ConnectionString;

//                using (SqlConnection con = new SqlConnection(connectionstring))
//                {
//                    string query = "Insert into " + IsNotifiedTable + " (ID) ";

//                    string rows = "";
//                    foreach (var item in lst_IDs)
//                    {
//                        rows += (!string.IsNullOrEmpty(rows) ? " union " : "") + " select " + item.ToString();
//                    }

//                    query = query + rows;


//                    SqlCommand cmd = new SqlCommand(query, con);
//                    cmd.CommandType = CommandType.Text;

//                    if (con.State == ConnectionState.Closed)
//                        con.Open();

//                    cmd.ExecuteNonQuery();
//                }
//            }
//            catch (Exception ex)
//            {
//                //ExceptionManager.GetExceptionLogger().LogException(ex);
//            }
//        }

//        //public void Send(NotificationDTO notificationDTO, string message, string groupname)
//        //{
//        //    try
//        //    {
//        //        var context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
//        //        context.Clients.All.IncidentNotificationBroadCastMessage(notificationDTO, message);
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        //ExceptionManager.GetExceptionLogger().LogException(ex);
//        //    }
//        //}
//    }
//}
