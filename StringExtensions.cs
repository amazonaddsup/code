using System.Linq;

namespace WebSite
{
	public static class StringExtensions
	{
		public static string FallbackIfEmpty(this string text, string fallback)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return fallback;
			}

			return text;
		}

		public static string GetFirstWord(this string text)
		{
			return text.Split(' ').First();
		}
	}
}
