using System.Collections.Generic;

namespace PutridParrot.Expressions;

internal static class DictionaryExtensions
{
    public static object? GetValue(this IDictionary<string, object> d, string keyName) =>
        d.TryGetValue(keyName, out var value) ? value : default;
}