using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void AddRange(List<T> entities);
        void Delete(T entity);
        void DeleteRange(List<T> entities);
        void Delete(Expression<Func<T, bool>> criteria);
        void Update(T entity);
        T FindById(int Id);
        T FindById(long Id);
        T FindById(Guid Id);

        #region [ Search ]

        /// <summary>
        /// Gets single entity using specification
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        T Single(ISpecification<T> criteria);

        /// <summary>
        /// Gets first entity with specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        T First(ISpecification<T> criteria);

        /// <summary>
        /// Gets entities which satifies a specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        IEnumerable<T> Get(ISpecification<T> specification);
        /// <summary>
        /// Gets entities which satifies a specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        IEnumerable<T> Get<TOrderBy>(ISpecification<T> specification, Expression<Func<T, TOrderBy>> orderBy, SortDirection sortOrder = SortDirection.Ascending);
        /// <summary>
        /// Gets entities which satifies a specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        IEnumerable<T> Get(ISpecification<T> specification, string orderby);
        /// <summary>
        /// Gets entities which satifies a specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        IEnumerable<T> Get<TOrderBy>(ISpecification<T> specification, Expression<Func<T, TOrderBy>> orderBy, int startIndex, int pageSize, SortDirection sortOrder = SortDirection.Ascending);
        /// <summary>
        /// Gets entities which satifies a specification.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <typeparam name="TOrderBy">The type of the order by.</typeparam>
        /// <param name="specification">The specification.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        IEnumerable<T> Get(ISpecification<T> specification, string orderBy, int startIndex, int pageSize);



        /// <summary>
        /// Gets all.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns></returns>
        IEnumerable<T> GetAll();

        #endregion

        #region [ Query ]
        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <remarks>
        /// Not recommended to be used....
        /// </remarks>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <returns></returns>
        IQueryable<T> GetQuery();

        /// <summary>
        /// Gets the query.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        IQueryable<T> GetQuery(ISpecification<T> criteria);

        #endregion


        #region Additional
        IQueryable<T> All { get; }

        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        void AddOrAttach(T entity);

        IEnumerable<T> ExecWithStoredProcedure(string query, params object[] parameters);
        string ExecScalarWithStoredProcedure(string query, params object[] parameters);

        void ExecWithStoredProcedureWithNoReturn(string query, params object[] parameters);

        int ExecStoredProcedureWithRowsAffected(string query, params object[] parameters);
        int ExecuteSql(string sql);
        void deAttach(T entity);
        #endregion
    }
    // Summary:
    //     Specifies the direction in which to sort a list of items.
    public enum SortDirection
    {
        // Summary:
        //     Sort from smallest to largest. For example, from A to Z.
        Ascending = 0,
        //
        // Summary:
        //     Sort from largest to smallest. For example, from Z to A.
        Descending = 1,
    }
}
