using System;
using System.Linq.Expressions;

namespace Explore.Linq.EFCore.Scaffolding
{
    public static class PredicateBuilder
    {
        public static Expression<Func<TParam, bool>> True<TParam>()
        {
            return p => true;
        }

        public static Expression<Func<TParam, bool>> False<TParam>()
        {
            return p => false;
        }

        public static Expression<Func<TParam, bool>> Or<TParam>(this Expression<Func<TParam, bool>> left,
                                                                Expression<Func<TParam, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<Func<TParam, bool>>
                (Expression.OrElse(left.Body, invokedExpr), left.Parameters);
        }

        public static Expression<Func<TParam, bool>> And<TParam>(this Expression<Func<TParam, bool>> left,
                                                                 Expression<Func<TParam, bool>> right)
        {
            var invokedExpr = Expression.Invoke(right, left.Parameters);
            return Expression.Lambda<Func<TParam, bool>>
                (Expression.AndAlso(left.Body, invokedExpr), left.Parameters);
        }
    }
}