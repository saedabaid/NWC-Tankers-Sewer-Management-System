using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;

namespace Infrastructure
{
    public class Audit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TableName { get; set; }
        public string OldData { get; set; }
        public string NewData { get; set; }
        public string IPAddress { get; set; }
        public string MachineName { get; set; }
        public DateTime ActionDate { get; set; }
    }
    public class AuditLogger : DbContext
    {
        #region data Members
        string _DataSavingConnection;
        #endregion
        public AuditLogger()
            : base("name=AuditConn") // for each client must add connection string with this name
        {

         
        }
        public DbSet<Audit> AuditRecords { get; set; }
        public void LogActions(List<DbEntityEntry> values)
        {
            Audit audit = new Audit();
            audit.ActionDate = DateTime.Now;
            audit.IPAddress = "";
            audit.MachineName = "";
            audit.UserId = 30;
            foreach (var item in values)
            {
                audit.TableName = item.Entity.GetType().ToString();
                if (item.State != EntityState.Deleted)
                audit.NewData = GetDataToString(item.CurrentValues);
                if (item.State != EntityState.Added)
                    audit.OldData = GetDataToString(item.OriginalValues);
                SaveAction(audit);
            }
        }
        private string GetDataToString(DbPropertyValues values)
        {
            StringBuilder data = new StringBuilder();
            if (values != null)
            {
                List<string> props = values.PropertyNames.ToList();
                foreach (string item in props)
                {
                    var dataobject = values.GetValue<object>(item);
                    data.AppendFormat(string.Format("{0}: {1} - ", item, dataobject != null ? dataobject.ToString() : " "));
                }
            }
            return data.ToString();
        }

        private void SaveAction(Audit audit)
        {
          
            this.Database.ExecuteSqlCommand(string.Format("exec Log_Audit @UserId = {0},@TableName = '{1}',@OldData = '{2}',@NewData = '{3}',@IPAddress = '{4}',@MachineName = '{5}',@ActionDate = '{6}' ",
                audit.UserId, audit.TableName, audit.OldData, audit.NewData, audit.IPAddress, audit.MachineName, audit.ActionDate.ToString("yyyy-MM-dd")
                ));
        }
    }
}
