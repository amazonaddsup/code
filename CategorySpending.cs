using System.Collections.Generic;
using System.Linq;

namespace WebSite
{
	public sealed class CategorySpending
	{
		public string CategoryId { get; }

		public string CategoryIdForDisplay =>
			string.Join
			(
				' ',
				CategoryId
				.Split('_')
				.Select(WordMapper)
			);

		private static readonly IReadOnlyDictionary<string, string> KnownWordMaps =
			new Dictionary<string, string>()
			{
				["OR"] = "or",
				["GPS"] = "GPS",
			};

		private static string WordMapper(string word)
		{
			if (KnownWordMaps.TryGetValue(word, out var knownMap))
			{
				return knownMap;
			}

			return word.First() + word.Substring(1).ToLower();
		}

		public double TotalSpent { get; }

		public string DonutChartLabel => $"{CategoryIdForDisplay} ({TotalSpent:#,#}$)";

		public CategorySpending(string categoryId, double totalSpent)
		{
			CategoryId = categoryId;
			TotalSpent = totalSpent;
		}
	}
}
