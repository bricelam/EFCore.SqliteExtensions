﻿using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
using Microsoft.EntityFrameworkCore.Sqlite.Query.ExpressionVisitors.Internal;

namespace Bricelam.EntityFrameworkCore.Sqlite
{
    class SqliteExSqlTranslatingExpressionVisitor : SqliteSqlTranslatingExpressionVisitor
    {
        public SqliteExSqlTranslatingExpressionVisitor(
            SqlTranslatingExpressionVisitorDependencies dependencies,
            RelationalQueryModelVisitor queryModelVisitor,
            SelectExpression targetSelectExpression,
            Expression topLevelPredicate,
            bool inProjection)
            : base(dependencies, queryModelVisitor, targetSelectExpression, topLevelPredicate, inProjection)
        {
        }

        protected override Expression VisitUnary(UnaryExpression expression)
        {
            if (expression.NodeType == ExpressionType.Negate
                && expression.Operand.Type == typeof(TimeSpan))
            {
                return new SqlFunctionExpression(
                    "timespan",
                    typeof(TimeSpan),
                    new[]
                    {
                        Expression.Negate(
                            new SqlFunctionExpression(
                                "days",
                                typeof(double),
                                new[] { Visit(expression.Operand) }))
                    });
            }

            return base.VisitUnary(expression);
        }

        protected override Expression VisitBinary(BinaryExpression binaryExpression)
        {
            if (binaryExpression.NodeType == ExpressionType.Add
                && binaryExpression.Right.Type == typeof(TimeSpan)
                && binaryExpression.Left.Type == typeof(TimeSpan))
            {
                return new SqlFunctionExpression(
                    "timespan",
                    typeof(TimeSpan),
                    new[]
                    {
                        Expression.Add(
                            new SqlFunctionExpression(
                                "days",
                                typeof(double),
                                new[] { Visit(binaryExpression.Left) }),
                            new SqlFunctionExpression(
                                "days",
                                typeof(double),
                                new[] { Visit(binaryExpression.Right) }))
                    });
            }

            return base.VisitBinary(binaryExpression);
        }
    }
}
