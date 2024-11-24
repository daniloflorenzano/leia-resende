using System.Linq.Expressions;
using System.Text;
using System.Reflection;

namespace Core.Application;

public sealed class SearchFilter
{
    public int PaginationStart { get; set; }
    public int PaginationTake { get; set; }
    public Expression<Func<News.News, bool>>? Where { get; set; }
    public Expression<Func<News.News, object>>? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }

    public int GetIdentifier() => GenerateIdentifierBasedOnPropertiesValue();

    private class ExpressionStringBuilder : ExpressionVisitor
    {
        private readonly StringBuilder _sb = new();

        public override string ToString() => _sb.ToString();

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sb.Append('(');
            Visit(node.Left);
            _sb.Append(GetOperatorSymbol(node.NodeType));
            Visit(node.Right);
            _sb.Append(')');
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            // Se temos uma closure (variável capturada), precisamos pegar seu valor
            if (node.Expression is ConstantExpression constant)
            {
                var value = GetMemberValue(node.Member, constant.Value);
                _sb.Append(value?.ToString() ?? "null");
                return node;
            }

            if (node.Expression != null)
            {
                Visit(node.Expression);
                _sb.Append('.');
            }
            _sb.Append(node.Member.Name);
            return node;
        }

        private static object? GetMemberValue(MemberInfo member, object? container)
        {
            if (container == null) return null;

            return member switch
            {
                FieldInfo field => field.GetValue(container),
                PropertyInfo prop => prop.GetValue(container),
                _ => null
            };
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == null)
                _sb.Append("null");
            else
                _sb.Append(node.Value);
            return node;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            _sb.Append(node.Name);
            return node;
        }

        private static string GetOperatorSymbol(ExpressionType type) => type switch
        {
            ExpressionType.Equal => "==",
            ExpressionType.NotEqual => "!=",
            ExpressionType.GreaterThan => ">",
            ExpressionType.GreaterThanOrEqual => ">=",
            ExpressionType.LessThan => "<",
            ExpressionType.LessThanOrEqual => "<=",
            ExpressionType.AndAlso => "&&",
            ExpressionType.OrElse => "||",
            ExpressionType.Add => "+",
            ExpressionType.Subtract => "-",
            ExpressionType.Multiply => "*",
            ExpressionType.Divide => "/",
            _ => type.ToString()
        };
    }

    private static string GetExpressionString(Expression? expression)
    {
        if (expression == null) return string.Empty;
        var visitor = new ExpressionStringBuilder();
        visitor.Visit(expression);
        return visitor.ToString();
    }

    private int GenerateIdentifierBasedOnPropertiesValue()
    {
        unchecked
        {
            int hash = 17;
            
            hash = hash * 23 + PaginationStart.GetHashCode();
            hash = hash * 23 + PaginationTake.GetHashCode();
            hash = hash * 23 + OrderByDescending.GetHashCode();
            
            if (Where != null)
            {
                hash = hash * 23 + GetExpressionString(Where.Body).GetHashCode();
            }
            
            if (OrderBy != null)
            {
                hash = hash * 23 + GetExpressionString(OrderBy.Body).GetHashCode();
            }
            
            return hash;
        }
    }

    public override bool Equals(object? obj)
    {
        if (obj is not SearchFilter other) return false;
        
        return PaginationStart == other.PaginationStart &&
               PaginationTake == other.PaginationTake &&
               OrderByDescending == other.OrderByDescending &&
               GetExpressionString(Where?.Body) == GetExpressionString(other.Where?.Body) &&
               GetExpressionString(OrderBy?.Body) == GetExpressionString(other.OrderBy?.Body);
    }

    public override int GetHashCode()
    {
        return GetIdentifier();
    }
}