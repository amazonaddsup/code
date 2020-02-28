using System.Collections.Generic;
using System.Linq;

namespace WebSite
{
	public static class IReadOnlyDictionaryExtensions
	{
		public static string GetDebugString(this IReadOnlyDictionary<string, string> dictionary)
		{
			return string.Join(",", dictionary.Select(kvp => $"key: {kvp.Key}, value: {kvp.Value}"));
		}
	}
}
