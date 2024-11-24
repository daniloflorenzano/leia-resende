using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.EfCore;

public class EfExpressionTransformer
{
    private class EfMethodCallVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Transforma string.Contains() em EF.Functions.Like()
            if (node.Method.DeclaringType == typeof(string) && node.Method.Name == "Contains")
            {
                if (node.Object != null) // É uma chamada de instância (ex: str.Contains("x"))
                {
                    // Pega o objeto string que está sendo chamado (ex: o 'str' em str.Contains("x"))
                    var stringProperty = Visit(node.Object);
                    
                    // Pega o argumento do Contains (ex: o "x" em str.Contains("x"))
                    var searchTerm = Visit(node.Arguments[0]);

                    // Cria a string de padrão "%valor%"
                    var likePattern = Expression.Call(
                        typeof(string).GetMethod(nameof(string.Concat), new[] { typeof(string[]) })!,
                        Expression.NewArrayInit(
                            typeof(string),
                            Expression.Constant("%"),
                            searchTerm,
                            Expression.Constant("%")
                        )
                    );

                    // Cria a chamada para EF.Functions.Like()
                    return Expression.Call(
                        typeof(DbFunctionsExtensions),
                        nameof(DbFunctionsExtensions.Like),
                        Type.EmptyTypes,
                        Expression.Property(null, typeof(EF), nameof(EF.Functions)),
                        stringProperty,
                        likePattern
                    );
                }
            }
            
            return base.VisitMethodCall(node);
        }
    }

    public static Expression<Func<T, TResult>> Transform<T, TResult>(Expression<Func<T, TResult>> expression)
    {
        var visitor = new EfMethodCallVisitor();
        return (Expression<Func<T, TResult>>)visitor.Visit(expression);
    }
}