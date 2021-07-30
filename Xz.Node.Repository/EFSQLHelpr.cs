using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xz.Node.Framework.Common;
using Xz.Node.Framework.Enums;
using Xz.Node.Framework.Extensions;
using Xz.Node.Framework.Model;

namespace Xz.Node.Repository
{
    /// <summary>
    /// EF帮助类
    /// </summary>
    public class EFSQLHelpr
    {
        #region 查询条件
        /// <summary>
        /// 生成查询表达式，例子：users.Where(lamba.Compile())；lamba是该方法的返回值
        /// </summary>
        /// <typeparam name="T">数据对象</typeparam>
        /// <param name="conditions">查询条件</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> QueryConditionCreate<T>(IList<ConditionInfo> conditions)
        {
            ParameterExpression p = Expression.Parameter(typeof(T), "o");
            var conDef1 = Expression.Constant(1);
            BinaryExpression expression = Expression.Equal(conDef1, conDef1);
            if (conditions != null)
            {
                //分组查询
                var groupNames = conditions.Select(o => o.Group).Distinct().ToList();
                foreach (var groupName in groupNames)
                {
                    BinaryExpression groupExpression = Expression.Equal(conDef1, conDef1);
                    var groupConditions = conditions.Where(o => o.Group == groupName).ToList();
                    if (!groupConditions.Where(o => o.Relation == ConditionRelationEnum.And).Any())//必须有一个and查询，否则因为第一个是1=1查询，都是or，永远返回true
                    {
                        groupConditions.First().Relation = ConditionRelationEnum.And;
                    }
                    foreach (ConditionInfo condition in groupConditions)
                    {
                        if (condition.IsQuery == false)
                            continue;
                        groupExpression = AndOrExpressionGet(groupExpression, p, condition);
                    }
                    var relation = conditions.Where(o => o.Group == groupName).Select(o => o.GroupRelation).First();
                    if (relation == ConditionRelationEnum.And)
                    {
                        expression = Expression.AndAlso(expression, groupExpression);
                    }
                    else
                    {
                        expression = Expression.OrElse(expression, groupExpression);
                    }
                }
            }
            var lambda = Expression.Lambda<Func<T, bool>>(expression, p);
            return lambda;
        }

        /// <summary>
        /// 根据条件关系获取查询表达式
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="p"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static BinaryExpression AndOrExpressionGet(BinaryExpression expression, ParameterExpression p, ConditionInfo condition)
        {
            MemberExpression pi = Expression.Property(p, condition.ColumnName);
            var value = ConstantExpressionGet(condition.ColumnName, condition.Value, pi.Type, condition.Operator);
            var rightExp = ExpressionGet(condition.Operator, pi, value);
            if (rightExp != null)
            {
                if (condition.Relation == ConditionRelationEnum.Or)
                {
                    expression = Expression.OrElse(expression, rightExp);
                }
                else
                {
                    expression = Expression.AndAlso(expression, rightExp);
                }
            }
            return expression;
        }

        /// <summary>
        /// 根据查询类型获取条件表达式
        /// </summary>
        /// <param name="oper"></param>
        /// <param name="pi"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Expression ExpressionGet(ConditionOperEnum oper, MemberExpression pi, Expression value)
        {
            Expression expression = null;
            switch (oper)
            {
                case ConditionOperEnum.Equal://相等
                    expression = Expression.Equal(pi, value);
                    break;
                case ConditionOperEnum.Unequal://不等
                    expression = Expression.NotEqual(pi, value);
                    break;
                case ConditionOperEnum.GreaterThan://大于
                    expression = Expression.GreaterThan(pi, value, false, pi.Type.GetMethod("Equals", new Type[] { pi.Type, pi.Type }));
                    break;
                case ConditionOperEnum.GreaterThanEqual://大于等于
                    expression = Expression.GreaterThanOrEqual(pi, value, false, pi.Type.GetMethod("Equals", new Type[] { pi.Type, pi.Type }));
                    break;
                case ConditionOperEnum.LessThanEqual://小于等于
                    expression = Expression.LessThanOrEqual(pi, value, false, pi.Type.GetMethod("Equals", new Type[] { pi.Type, pi.Type }));
                    break;
                case ConditionOperEnum.LessThan://小于
                    expression = Expression.LessThan(pi, value, false, pi.Type.GetMethod("Equals", new Type[] { pi.Type, pi.Type }));
                    break;
                case ConditionOperEnum.LeftLike://左模糊
                    if (pi.Type == typeof(string))
                    {
                        expression = Expression.Call(pi
                                            , typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) })
                                            , value);
                    }
                    break;
                case ConditionOperEnum.RightLike://右模糊
                    if (pi.Type == typeof(string))
                    {
                        expression = Expression.Call(pi
                                            , typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) })
                                            , value);
                    }
                    break;
                case ConditionOperEnum.AllLike://全模糊
                    if (pi.Type == typeof(string))
                    {
                        expression = Expression.Call(pi
                                            , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                                            , value);
                    }
                    break;
                case ConditionOperEnum.Exclusive://不包含
                    if (pi.Type == typeof(string))
                    {
                        expression = Expression.Equal(Expression.Call(pi
                                            , typeof(string).GetMethod("Contains", new Type[] { typeof(string) })
                                            , value), Expression.Constant(false));

                    }
                    break;
                case ConditionOperEnum.In:
                    expression = Expression.Call(value
                                        , ((typeof(List<>)).MakeGenericType(pi.Type)).GetMethod("Contains", new Type[] { pi.Type })
                                        , pi);
                    break;
                case ConditionOperEnum.NotIn:
                    expression = Expression.Equal(Expression.Call(value
                                        , ((typeof(List<>)).MakeGenericType(pi.Type)).GetMethod("Contains", new Type[] { pi.Type })
                                        , pi), Expression.Constant(false));
                    break;
            }
            return expression;
        }

        /// <summary>
        /// 生成条件表达式
        /// </summary>
        /// <param name="columnId"></param>
        /// <param name="value"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        private static Expression ConstantExpressionGet(string columnId, string value, Type propertyType, ConditionOperEnum oper)
        {
            if ((propertyType.Name == typeof(bool).Name || propertyType.GenericTypeArguments.Where(s => s.Name == typeof(bool).Name).Any()) && value == "1")
                value = "true";
            if ((propertyType.Name == typeof(bool).Name || propertyType.GenericTypeArguments.Where(s => s.Name == typeof(bool).Name).Any()) && value == "0")
                value = "false";
            //if (!string.IsNullOrEmpty(value))//如果用户输入的值不为空
            //{
            if (oper == ConditionOperEnum.In || oper == ConditionOperEnum.NotIn)
            {
                var listType = (typeof(List<>)).MakeGenericType(propertyType);
                var data = Activator.CreateInstance(listType);
                if (value != null)
                {
                    var addMethod = listType.GetMethod("Add");
                    foreach (var v in value.Split("|"))
                    {
                        addMethod.Invoke(data, new[] { v.ChangeType(propertyType) });
                    }
                }
                var dataDyc = new { TempData = data };
                var constant = Expression.Convert(Expression.PropertyOrField(Expression.Constant(dataDyc), nameof(dataDyc.TempData)), listType);
                return constant;
            }
            else
            {
                var data = value.ChangeType(propertyType);
                var dataDyc = new { TempData = data };
                return Expression.Convert(Expression.PropertyOrField(Expression.Constant(dataDyc), nameof(dataDyc.TempData)), propertyType);
            }
            //}
            //var dataDefault = value.ChangeType(propertyType);
            //var dataDefaultDyc = new { TempData = dataDefault };
            //var result = Expression.Convert(Expression.PropertyOrField(Expression.Constant(dataDefaultDyc), nameof(dataDefaultDyc.TempData)), propertyType);
            //return result;
        }
        #endregion

        #region 排序条件
        /// <summary>
        /// 生成排序条件
        /// </summary>
        /// <typeparam name="T">需要排序对象</typeparam>
        /// <param name="source">Linq表达式</param>
        /// <param name="sorts">排序条件</param>
        /// <returns></returns>
        public static IQueryable<T> SortConditionCreate<T>(IQueryable<T> source, IList<SortInfo> sorts)
        {
            if (sorts == null)
                return source;
            for (var i = 0; i < sorts.Count(); i++)
            {
                var condition = sorts[i];
                string methodName = string.Empty;
                if (i == 0)
                {
                    switch (condition.Direction)
                    {
                        case ConditionDirectionEnum.ASC:
                            methodName = "OrderBy"; break;
                        case ConditionDirectionEnum.DESC:
                            methodName = "OrderByDescending"; break;
                        default:
                            methodName = "OrderBy"; break;
                    }
                }
                else
                {
                    switch (condition.Direction)
                    {
                        case ConditionDirectionEnum.ASC:
                            methodName = "ThenBy"; break;
                        case ConditionDirectionEnum.DESC:
                            methodName = "ThenByDescending"; break;
                        default:
                            methodName = "ThenBy"; break;
                    }
                }
                source = ApplyOrder<T>(source, condition.ColumnName, methodName);
            }
            return source;
        }

        /// <summary>
        /// 生成排序条件
        /// </summary>
        /// <typeparam name="T">需要排序的对象类型</typeparam>
        /// <param name="source">linq表达式</param>
        /// <param name="propertyName">排序的字段名称</param>
        /// <param name="methodName">排序方向</param>
        /// <returns></returns>
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string propertyName, string methodName)
        {
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "a");
            //var property = type.GetProperty(propertyName);
            var property = type.GetProperties().FirstOrDefault(item => item.Name.ToLower() == propertyName.ToLower());
            Expression expr = Expression.Property(arg, property);
            type = property.PropertyType;
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);
            object result = typeof(Queryable).GetMethods().Single(
            a => a.Name == methodName
            && a.IsGenericMethodDefinition
            && a.GetGenericArguments().Length == 2
            && a.GetParameters().Length == 2).MakeGenericMethod(typeof(T), type).Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
        #endregion
    }

    //public class EFTemp
    //{
    //    public dynamic TempData { get; set; }
    //}
}
