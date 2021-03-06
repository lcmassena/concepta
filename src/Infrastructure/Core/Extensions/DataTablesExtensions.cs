﻿using Massena.Infrastructure.Core.Domain.DataTables;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using static Massena.Infrastructure.Core.Domain.DataTables.DataTablesQuery;

namespace Massena.Infrastructure.Core.Extensions
{
    public static class QueryableExtensions
    {

        /// <summary>
        /// Aplica o filtro do DataTables na consulta. 
        /// </summary>
        /// <typeparam name="TQueryDataType">Tipo do objeto que será retornado no DataTablesResponse, oriundo da query</typeparam>
        /// <typeparam name="TResponse">Tipo de response que será enviado, contendo o <paramref name="TQueryDataType"/>. Esse objeto precisa herdar de <seealso cref="DataTablesResponse<>" /></typeparam>
        /// <param name="source">Query não materializada que será utilizada para aplicação do filtro e ordenação</param>
        /// <param name="query">Objeto query do datatables, contendo os filtros e a ordenação</param>
        /// <returns>Esse método irá materializar a query, aplicando os filtros e a ordenação do DataTables</returns>
        public static Task<TResponse> FilterDataTableAsync<TQueryDataType, TResponse>(this IQueryable<TQueryDataType> source, DataTablesQuery query) where TResponse : DataTablesResponse<TQueryDataType>, new()
        {
            return Task.Run(() => FilterDataTable<TQueryDataType, TResponse>(source, query));
        }

        /// <summary>
        /// Aplica o filtro do DataTables na consulta. 
        /// </summary>
        /// <typeparam name="TQueryDataType">Tipo do objeto que será retornado no DataTablesResponse, oriundo da query</typeparam>
        /// <typeparam name="TResponse">Tipo de response que será enviado, contendo o <paramref name="TQueryDataType"/>. Esse objeto precisa herdar de <seealso cref="DataTablesResponse<>" /></typeparam>
        /// <param name="source">Query não materializada que será utilizada para aplicação do filtro e ordenação</param>
        /// <param name="query">Objeto query do datatables, contendo os filtros e a ordenação</param>
        /// <returns>Esse método irá materializar a query, aplicando os filtros e a ordenação do DataTables</returns>
        public static TResponse FilterDataTable<TQueryDataType, TResponse>(this IQueryable<TQueryDataType> source, DataTablesQuery query) where TResponse : DataTablesResponse<TQueryDataType>, new()
        {
            var originalSource = source;
            var totalRecords = source.Count();
            var recordsFiltered = totalRecords;

            if (query == null)
                source = source.Take(10);

            if (query != null)
            {
                source = source.TryApplyOrder(query);
                source = source.TryApplyFilter(query);
                source = source.TryApplySearch(query);

                if (originalSource != source) //Prevent a double count for same result
                    recordsFiltered = source.Count();

                source = source.Skip(query.Start).Take(query.Length);
            }

            return new TResponse()
            {
                RecordsTotal = totalRecords,
                RecordsFiltered = recordsFiltered,
                Data = source.ToList()
            };
        }

        private static IQueryable<T> TryApplyOrder<T>(this IQueryable<T> source, DataTablesQuery query)
        {
            int count = 0;

            if (query.Order != null && query.Order.Any())
            {
                try
                {
                    var properties = typeof(T).GetProperties();
                    Expression expression = source.Expression;
                    query.Order.ForEach(item =>
                    {
                        var column = query.Columns.ElementAt(item.Column);
                        if (column != null && properties.Any(x => x.Name == column.Data))
                        {
                            var parameter = Expression.Parameter(typeof(T), "x");
                            var selector = Expression.PropertyOrField(parameter, column.Data);
                            var method = string.Equals(item.Dir, "desc", StringComparison.OrdinalIgnoreCase) ?
                                (count == 0 ? "OrderByDescending" : "ThenByDescending") :
                                (count == 0 ? "OrderBy" : "ThenBy");
                            expression = Expression.Call(typeof(Queryable), method, new Type[] { source.ElementType, selector.Type }, expression, Expression.Quote(Expression.Lambda(selector, parameter)));
                        }
                        count++;
                    });

                    if (count > 0)
                        source = source.Provider.CreateQuery<T>(expression);
                }
                catch
                {
                    //LogHelper.LogError("Erro ao realizar sort - " + ex.Message);
                }
            }
            return source;
        }

        private static IQueryable<T> TryApplyFilter<T>(this IQueryable<T> source, DataTablesQuery query)
        {
            if (query.Filters != null && query.Filters.Any())
            {
                try
                {
                    var properties = typeof(T).GetProperties();

                    query.Filters.ForEach(filter =>
                    {
                        var property = properties.SingleOrDefault(x => x.Name == filter.Field);
                        if (property != null)
                            source = source.BuildWhereAndQuery(property, filter.Value);
                    });
                }
                catch
                {
                    //LogHelper.LogError("Erro ao realizar ApplyFilters - " + ex.Message);
                }
            }

            return source;
        }

        private static IQueryable<T> TryApplySearch<T>(this IQueryable<T> source, DataTablesQuery query)
        {
            if (query.Columns != null && query.Columns.Any())
            {
                var columns = new List<Column>() { new Column { Name = "", Search = query.Search } };
                columns.AddRange(query.Columns);

                columns.ForEach(column =>
                {
                    if (column.Search != null && !string.IsNullOrEmpty(column.Search.Value))
                    {
                        try
                        {
                            var properties = typeof(T).GetProperties();
                            List<Expression> predicates = new List<Expression>();
                            ParameterExpression parameter = Expression.Parameter(typeof(T), "x");

                            properties.Where(x => !x.PropertyType.IsGenericType).ToList().ForEach(property =>
                            {
                                if (!string.IsNullOrEmpty(column.Data) && !column.Data.Equals(property.Name, StringComparison.OrdinalIgnoreCase)) return;

                                Expression selector = Expression.PropertyOrField(parameter, property.Name);
                                Expression predicate = selector.BuildPredicate(property, column.Search.Value);

                                if (predicate != null)
                                    predicates.Add(predicate);
                            });

                            source = source.BuildWhereOrQuery(predicates, parameter);
                        }
                        catch
                        {
                            //LogHelper.LogError("Erro ao realizar TryApplySearch - " + ex.Message);
                        }
                    }
                });
            }
            return source;
        }


        /// <summary>
        /// Pega a query corrente, e injeta os parametros na clausula Where, usando a engine do Linq
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        private static IQueryable<T> InvokeQueryBuilder<T>(this IQueryable<T> source, Expression predicate, ParameterExpression parameter)
        {
            if (predicate == null || parameter == null)
                return source;

            var whereMethod = typeof(Queryable).GetMethods().Where(x => x.Name == "Where" && x.IsGenericMethodDefinition).First(m =>
            {
                return m.GetParameters().Count() == 2; //Put more restriction here to ensure selecting the right overload the first overload that has 2 parameters
            });

            var whereClause = Expression.Lambda<Func<T, bool>>(predicate, parameter);
            var genericMethod = whereMethod.MakeGenericMethod(typeof(T));
            return (IQueryable<T>)genericMethod.Invoke(genericMethod, new object[] { source, whereClause });
        }

        private static IQueryable<T> BuildWhereOrQuery<T>(this IQueryable<T> source, IEnumerable<Expression> expressions, ParameterExpression parameter)
        {
            try
            {
                if (expressions != null && expressions.Any())
                {
                    Expression predicate = expressions.FirstOrDefault();

                    var list = expressions.ToList();
                    list.Remove(predicate);
                    foreach (var expression in list)
                        predicate = Expression.Or(predicate, expression);

                    source = source.InvokeQueryBuilder(predicate, parameter);
                }
            }
            catch
            {
                //LogHelper.LogError("Erro ao realizar BuildQuery - " + ex.Message);
            }

            return source;
        }


        private static readonly Type[] AvailableTypes = new Type[] { typeof(string), typeof(int), typeof(DateTime), typeof(bool), typeof(Int32), typeof(Int16), typeof(Int64) };
        private static IQueryable<T> BuildWhereAndQuery<T>(this IQueryable<T> source, PropertyInfo property, string value)
        {
            try
            {
                ParameterExpression parameter = Expression.Parameter(typeof(T), "x");
                Expression selector = Expression.PropertyOrField(parameter, property.Name);
                Expression predicate = selector.BuildPredicate(property, value);

                source = source.InvokeQueryBuilder(predicate, parameter);
            }
            catch
            {
            }

            return source;
        }

        private static Expression BuildPredicate(this Expression selector, PropertyInfo property, string queryValue)
        {
            if (property.PropertyType == typeof(string))
            {
                Expression selectorValue = Expression.Constant(queryValue);
                return Expression.Call(selector, typeof(string).GetMethod("Contains"), new Expression[] { selectorValue });
            }
            else
                return TryConvertQueryValue(property, selector, queryValue);
        }


        private static Expression TryConvertQueryValue(PropertyInfo property, Expression selector, string queryValue)
        {

            var converter = TypeDescriptor.GetConverter(property.PropertyType);
            try
            {
                if (converter != null)
                {
                    var convertedValue = converter.ConvertFromString(queryValue);
                    Expression selectorValue = Expression.Constant(convertedValue);
                    return Expression.Equal(selector, Expression.Convert(selectorValue, property.PropertyType));
                }
            }
            catch
            {
            }
            return null;
        }

    }
}
