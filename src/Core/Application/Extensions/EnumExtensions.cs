using System.Reflection;
using Core.Application.Attributes;

namespace Core.Application.Extensions;

public static class EnumExtensions
{
    public static string ToPortugueseString(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attribute = field?.GetCustomAttribute<PortugueseDescriptionAttribute>();
        
        return attribute?.Description ?? value.ToString();
    }
}