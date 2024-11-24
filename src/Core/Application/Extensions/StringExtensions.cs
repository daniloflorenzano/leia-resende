namespace Core.Application.Extensions;

public static class StringExtensions
{
    public static string ReturnStringBetween(this string text, string start, string end, bool shouldTrim = false)
    {
        var startText = text.IndexOf(start, StringComparison.Ordinal) + start.Length;

        // texto a partir do inicio informado
        text = text.Substring(startText);

        var endIndex = text.IndexOf(end, StringComparison.Ordinal);
        var length = endIndex - 0;

        var result = text.Substring(0, length);

        if (shouldTrim)
            result = result.Trim();

        return result;
    }
}