namespace Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;
    using System.Data.Entity;

    public class Specification<TEntity> : ISpecification<TEntity>
        where TEntity: class
    {
        #region Fields

        public virtual Expression<Func<TEntity, bool>> Expression { protected set; get; }
        public virtual Expression<Func<TEntity, object>>[] Includes { protected set; get; }

        #endregion Fields

        #region Constructors

        public Specification()
        {
 
        }

        public Specification(Expression<Func<TEntity, bool>> expression)
        {
            this.Expression = expression;
        }

        public Specification(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            this.Expression = expression;
            Includes = includes;
        }

        #endregion Constructors


        public virtual IQueryable<TEntity> PreExecute(IQueryable<TEntity> query)
        {
            if (Includes != null)
            {
                foreach (var include in Includes)
                {
                    query = query.Include(include);
                }
            }
            return query;
        }
    }
}