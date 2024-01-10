using System.Collections.Generic;

public static class DictionaryExtensions
{
    public static bool Remove<TK, TV>(this IDictionary<TK, TV> dict, TK key, out TV value)
    {
        return dict.TryGetValue(key, out value) && dict.Remove(key);
    }
}
