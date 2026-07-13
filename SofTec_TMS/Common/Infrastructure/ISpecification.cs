namespace Infrastructure
{
    using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Extentions;

    /// <summary>
    /// In simple terms, a specification is a small piece of logic which is independent and give an answer 
    /// to the question “does this match ?”. With Specification, we isolate the logic that do the selection 
    /// into a reusable business component that can be passed around easily from the entity we are selecting.
    /// </summary>
    /// <see cref="http://en.wikipedia.org/wiki/Specification_pattern"/>
    /// <typeparam name="TEntity"></typeparam>
    public interface ISpecification<TEntity>
    {
        #region Methods

        Expression<Func<TEntity, bool>> Expression { get; }

        IQueryable<TEntity> PreExecute(IQueryable<TEntity> query);

        #endregion Methods
    }    
}