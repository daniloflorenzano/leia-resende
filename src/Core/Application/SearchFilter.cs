using System.Linq.Expressions;

namespace Core.Application;

public sealed class SearchFilter
{
    public int PaginationStart { get; set; }
    public int PaginationTake { get; set; }
    public Expression<Func<News.News, bool>>? Where { get; set; }
    public Expression<Func<News.News, object>>? OrderBy { get; set; }
    public bool OrderByDescending { get; set; }

    public int GetIdentifier() => GenerateIdentifierBasedOnPropertiesValue();

    private int GenerateIdentifierBasedOnPropertiesValue()
    {
        unchecked 
        {
            int hash = 17; // número primo inicial
            
            // Combina hash com cada propriedade
            hash = hash * 23 + PaginationStart.GetHashCode();
            hash = hash * 23 + PaginationTake.GetHashCode();
            hash = hash * 23 + OrderByDescending.GetHashCode();
            
            // Para Expression, precisamos considerar a expressão em string
            if (Where != null) 
                hash = hash * 23 + Where.ToString().GetHashCode();
            
            if (OrderBy != null) 
                hash = hash * 23 + OrderBy.ToString().GetHashCode();
            
            return hash;
        }
    }

    // Sobrescreve Equals e GetHashCode para garantir consistência
    public override bool Equals(object? obj)
    {
        if (obj is not SearchFilter other) return false;
        
        return PaginationStart == other.PaginationStart &&
               PaginationTake == other.PaginationTake &&
               OrderByDescending == other.OrderByDescending &&
               Where?.ToString() == other.Where?.ToString() &&
               OrderBy?.ToString() == other.OrderBy?.ToString();
    }

    public override int GetHashCode()
    {
        return GetIdentifier();
    }
}