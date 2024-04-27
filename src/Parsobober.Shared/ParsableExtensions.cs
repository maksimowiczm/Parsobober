namespace Parsobober.Shared;

public static class ParsableExtensions
{
    public static T? ParseOrNull<T>(this string? value) where T : struct, IParsable<T>
    {
        if (T.TryParse(value, null, out var result))
        {
            return result;
        }

        return null;
    }
}