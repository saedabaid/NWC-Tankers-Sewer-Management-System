using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;

namespace Infrastructure
{
    public class UnitofWork : IUnitofWork
    {
        private DbContext _dbContext;
        public UnitofWork(DbContext context)
        {
            _dbContext = context;
        }
        public void Flush()
        {
            try
            {
                if (_dbContext.ChangeTracker.HasChanges())
                {
                    //var entityList = _dbContext.ChangeTracker.Entries().ToList();
                    //if (entityList.Count > 0)
                    //{
                      
                    //    AuditLogger Logger = new AuditLogger();
                    //    Logger.LogActions(entityList);
                    //}

                    _dbContext.SaveChanges();
                }

            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        throw dbEx;
                    }
                }
            }
            catch (Exception ex)
            {
                //todo.. Log ex
                throw ex;
            }
        }

        public void Dispose()
        {
            Flush();
        }
    }
}
