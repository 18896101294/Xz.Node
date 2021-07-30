using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Xz.Node.Framework.Extensions
{
    public enum ParserAwayEnum
    {
        No,
        Left,
        Right
    }
    /// <summary>
    /// Lambda表达式拓展
    /// </summary>
    public static class LambdaExpressionExtensions
    {
        private static Expression Parser(ParameterExpression parameter, Expression expression, ParserAwayEnum away)
        {
            if (expression == null) return null;
            switch (expression.NodeType)
            {
                //一元运算符
                case ExpressionType.Negate:
                case ExpressionType.NegateChecked:
                case ExpressionType.Not:
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                case ExpressionType.ArrayLength:
                case ExpressionType.Quote:
                case ExpressionType.TypeAs:
                    {
                        var unary = expression as UnaryExpression;
                        var exp = Parser(parameter, unary.Operand, away);
                        return Expression.MakeUnary(expression.NodeType, exp, unary.Type, unary.Method);
                    }
                //二元运算符
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                case ExpressionType.Divide:
                case ExpressionType.Modulo:
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Coalesce:
                case ExpressionType.ArrayIndex:
                case ExpressionType.RightShift:
                case ExpressionType.LeftShift:
                case ExpressionType.ExclusiveOr:
                    {
                        var binary = expression as BinaryExpression;
                        var left = Parser(parameter, binary.Left, ParserAwayEnum.Left);
                        var right = Parser(parameter, binary.Right, ParserAwayEnum.Right);
                        if (left.Type != right.Type)
                        {
                            var method = typeof(LambdaExpressionExtensions).GetMethod("ChangeType");
                            right = Expression.Convert(Expression.Call(method, Expression.Convert(right, typeof(object)), Expression.Constant(left.Type)), left.Type);
                        }
                        var conversion = Parser(parameter, binary.Conversion, away);
                        if (binary.NodeType == ExpressionType.Coalesce && binary.Conversion != null)
                            return Expression.Coalesce(left, right, conversion as LambdaExpression);
                        else
                            return Expression.MakeBinary(expression.NodeType, left, right);
                    }
                //其他
                case ExpressionType.Call:
                    {
                        var call = expression as MethodCallExpression;
                        List<Expression> arguments = new List<Expression>();
                        foreach (var argument in call.Arguments)
                        {
                            arguments.Add(Parser(parameter, argument, away));
                        }
                        var instance = Parser(parameter, call.Object, away);
                        call = Expression.Call(instance, call.Method, arguments);
                        return call;
                    }
                case ExpressionType.Lambda:
                    {
                        var Lambda = expression as LambdaExpression;
                        return Parser(parameter, Lambda.Body, away);
                    }
                case ExpressionType.MemberAccess:
                    {
                        var memberAccess = expression as MemberExpression;
                        if (memberAccess.Expression == null)
                        {
                            memberAccess = Expression.MakeMemberAccess(null, memberAccess.Member);
                        }
                        else
                        {
                            var exp = Parser(parameter, memberAccess.Expression, away);
                            if (away == ParserAwayEnum.Left)
                            {
                                var attrMethod = exp.Type.GetMethod("Attribute");
                                var member = attrMethod.ReturnType.GetMember("Value").FirstOrDefault();

                                var valueMember = Expression.MakeMemberAccess(Expression.Call(exp, attrMethod, Expression.Convert(Expression.Constant(memberAccess.Member.Name), typeof(XName))), member);
                                return valueMember;
                            }
                            //else if (away == ParserAway.Right) {
                            //    var member = exp.Type.GetMember(memberAccess.Member.Name).FirstOrDefault();
                            //    var method = member.ReflectedType.GetMethod("ToString");
                            //    var expressionCall = Expression.Call(Expression.MakeMemberAccess(exp, member), method);
                            //    return expressionCall;
                            //    //var member = exp.Type.GetMember(memberAccess.Member.Name).FirstOrDefault();
                            //    //memberAccess = Expression.MakeMemberAccess(exp, member);
                            //}
                            else
                            {
                                var member = exp.Type.GetMember(memberAccess.Member.Name).FirstOrDefault();
                                memberAccess = Expression.MakeMemberAccess(exp, member);
                            }
                        }
                        return memberAccess;
                    }
                case ExpressionType.Parameter:
                    return parameter;
                case ExpressionType.Constant:
                    return expression;
                case ExpressionType.TypeIs:
                    {
                        var typeis = expression as TypeBinaryExpression;
                        var exp = Parser(parameter, typeis.Expression, away);
                        return Expression.TypeIs(exp, typeis.TypeOperand);
                    }
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", expression.NodeType));
            }
        }
        public static Expression<Func<XElement, bool>> Cast<TToProperty>(this Expression<Func<TToProperty, bool>> expression) where TToProperty : new()
        {
            var p = Expression.Parameter(typeof(XElement), "p");
            var x = Parser(p, expression, ParserAwayEnum.No);
            return Expression.Lambda<Func<XElement, bool>>(x, p);
        }
        public static object ChangeType(this object value, Type type)
        {
            if (value == null && type.IsGenericType) return Activator.CreateInstance(type);
            if (value == null) return null;
            if (type == value.GetType()) return value;
            if (type.IsEnum)
            {
                if (value is string)
                    return System.Enum.Parse(type, value as string);
                else
                    return System.Enum.ToObject(type, value);
            }
            if (!type.IsInterface && type.IsGenericType)
            {
                Type innerType = type.GetGenericArguments()[0];
                object innerValue = ChangeType(value, innerType);
                return Activator.CreateInstance(type, new object[] { innerValue });
            }
            if (value is string && type == typeof(Guid)) return new Guid(value as string);
            if (value is string && type == typeof(Version)) return new Version(value as string);
            if (value is Guid && type == typeof(string)) return value.ToString();
            if (!(value is IConvertible)) return value;

            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                var underlyingType = Nullable.GetUnderlyingType(type);
                type = underlyingType ?? type;
            } // end if
            return Convert.ChangeType(value, type);
        }
    }
}
