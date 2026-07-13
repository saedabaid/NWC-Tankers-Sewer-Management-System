using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Common;

namespace Infrastructure
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private DbContext _context;
        public Repository(DbContext context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            try
            {
                _context.Set<T>().Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public void AddRange(List<T> entities)
        {
            try
            {
                _context.Set<T>().AddRange(entities);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IEnumerable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {

            IEnumerable<T> query = _context.Set<T>().Where(predicate).AsEnumerable();
            return query;
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
            {
                _context.Set<T>().Attach(entity);
            }
            _context.Set<T>().Remove(entity);
        }
        public void DeleteRange(List<T> entities)
        {
            foreach (var entity in entities)
            {
                if (_context.Entry(entity).State == System.Data.Entity.EntityState.Detached)
                {

                    _context.Set<T>().Attach(entity);
                }
            }
            _context.Set<T>().RemoveRange(entities);
        }
        public void Delete(System.Linq.Expressions.Expression<Func<T, bool>> criteria)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            try
            {
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception ex)
            {

            }
        }

        public T FindById(int Id)
        {
            return _context.Set<T>().Find(Id);
        }

        public T FindById(long Id)
        {
            return _context.Set<T>().Find(Id);
        }

        public T FindById(Guid Id)
        {
            return _context.Set<T>().Find(Id);
        }


        #region Addtional
        public IQueryable<T> All { get { return _context.Set<T>(); } }
        public IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return query;
        }
        private string GetEntitySetName(string entityTypeName)
        {
            var adapter = (IObjectContextAdapter)_context;
            var objectContext = adapter.ObjectContext;
            var container = objectContext.MetadataWorkspace.GetEntityContainer
                    (objectContext.DefaultContainerName, DataSpace.CSpace);

            return (from meta in container.BaseEntitySets

                    where meta.ElementType.Name == entityTypeName

                    select meta.Name).FirstOrDefault();

        }

        public string GetEntitySetFullName(ObjectContext ocntxt, EntityObject entity)
        {
            // If the EntityKey exists, simply get the Entity Set name from the key
            if (entity.EntityKey != null)
            {
                return entity.EntityKey.EntitySetName;
            }
            else
            {
                string entityTypeName = entity.GetType().Name;
                var container = ocntxt.MetadataWorkspace.GetEntityContainer(ocntxt.DefaultContainerName, DataSpace.CSpace);
                string entitySetName = (from meta in container.BaseEntitySets
                                        where meta.ElementType.Name == entityTypeName
                                        select meta.Name).First();

                return container.Name + "." + entitySetName;
            }
        }

        public int ExecuteSql(string sql)
        {
            DbConnection conn = _context.Database.Connection;
            ConnectionState initialState = conn.State;
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }
        }
        public IEnumerable<T> ExecWithStoredProcedure(string query, params object[] parameters)
        {
            try
            {
                var resulte = _context.Database.SqlQuery<T>(query, parameters).ToList();
                return resulte;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public void ExecWithStoredProcedureWithNoReturn(string query, params object[] parameters)
        {
            try
            {
                _context.Database.ExecuteSqlCommand(query, parameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        public int ExecStoredProcedureWithRowsAffected(string query, params object[] parameters)
        {
            try
            {
                return _context.Database.ExecuteSqlCommand(query, parameters);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void deAttach(T entity)
        {
            _context.Entry(entity).State = System.Data.Entity.EntityState.Detached;
        }

        /// <summary>
        /// Insert if new otherwise attach data into context
        /// </summary>
        /// <param name="entity"></param>
        public void AddOrAttach(T entity)
        {
            var adapter = (IObjectContextAdapter)_context;
            var objectContext = adapter.ObjectContext;
            // Define an ObjectStateEntry and EntityKey for the current object.
            System.Data.Entity.Core.EntityKey key;
            object originalItem;
            // Get the detached object's entity key.
            //if (((IEntityWithKey)entity).EntityKey == null)
            //{
            //    // Get the entity key of the updated object.
            //    //objectContext.GetEntitySetName(entity.GetType().Name)
            //    key = objectContext.CreateEntityKey(this.GetEntitySetName(entity.GetType().Name), entity);
            //}
            //else
            //{
            key = ((IEntityWithKey)entity).EntityKey;
            //}
            try
            {
                // Get the original item based on the entity key from the context
                // or from the database.

                ObjectStateEntry entry;
                objectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out entry);
                // objectContext.ObjectStateManager.TryGetObjectStateEntry(objectContext.CreateEntityKey(objectContext.GetEntityName<T>(), entity), out entry);

                if (objectContext.TryGetObjectByKey(key, out originalItem))
                {//accept the changed property
                    if (originalItem is EntityObject &&
                        ((EntityObject)originalItem).EntityState != System.Data.Entity.EntityState.Added)
                    {
                        // Call the ApplyCurrentValues method to apply changes
                        // from the updated item to the original version.
                        objectContext.ApplyCurrentValues(key.EntitySetName, entity);
                    }
                }
                else
                {//add the new entity
                    Add(entity);
                }//end else
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Commit()
        {
            _context.SaveChanges();
        }
        #endregion

        #region [ Search ]

        public T Single(ISpecification<T> criteria)
        {
            return GetQuery(criteria).SingleOrDefault();
        }

        public T First(ISpecification<T> criteria)
        {
            return GetQuery(criteria).FirstOrDefault();
        }

        public IEnumerable<T> Get(ISpecification<T> specification)
        {
            var query = GetQuery().Where(specification.Expression);
            return specification.PreExecute(query).AsEnumerable();
        }
        public IEnumerable<T> Get<TOrderBy>(ISpecification<T> specification, Expression<Func<T, TOrderBy>> orderBy, SortDirection sortOrder = SortDirection.Ascending)
        {
            IQueryable<T> query;
            if (sortOrder == SortDirection.Ascending)
                query = GetQuery().Where(specification.Expression).OrderBy(orderBy);
            else
                query = GetQuery().Where(specification.Expression).OrderByDescending(orderBy);
            return specification.PreExecute(query).AsEnumerable();
        }
        public IEnumerable<T> Get(ISpecification<T> specification, string orderby)
        {
            var query = GetQuery().Where(specification.Expression).OrderBy(orderby);
            return specification.PreExecute(query).AsEnumerable();
        }
        public IEnumerable<T> Get<TOrderBy>(ISpecification<T> specification, Expression<Func<T, TOrderBy>> orderBy, int startIndex, int pageSize, SortDirection sortOrder = SortDirection.Ascending)
        {
            IQueryable<T> query;
            if (sortOrder == SortDirection.Ascending)
                query = GetQuery().Where(specification.Expression).OrderBy(orderBy).Skip((startIndex)).Take(pageSize);
            else
                query = GetQuery().Where(specification.Expression).OrderByDescending(orderBy).Skip((startIndex)).Take(pageSize);
            return specification.PreExecute(query).AsEnumerable();

        }
        public IEnumerable<T> Get(ISpecification<T> specification, string orderBy, int startIndex, int pageSize)
        {
            var result = GetQuery().Where(specification.Expression).OrderBy(orderBy).Skip(startIndex).Take(pageSize);
            return specification.PreExecute(result).AsEnumerable();
        }

        public IEnumerable<T> GetAll()
        {
            return GetQuery().AsEnumerable();
        }

        #endregion

        #region [ Query ]

        public IQueryable<T> GetQuery()
        {
            return _context.Set<T>().AsNoTracking();
        }

        public IQueryable<T> GetQuery(ISpecification<T> criteria)
        {
            var query = GetQuery().Where(criteria.Expression);
            return criteria.PreExecute(query);
        }


        #endregion



        public string ExecScalarWithStoredProcedure(string query, params object[] parameters)
        {
            try
            {
                return _context.Database.SqlQuery<string>(query, parameters).FirstOrDefault();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
