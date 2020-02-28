using System.Collections.Generic;
using System.Linq;

namespace WebSite
{
	public static class OrderParser
	{
		public static IReadOnlyList<Item> ParseFile(string fileContents)
		{
			return
				CsvParser.ParseFile(fileContents)
				.Select(Item.FromDictionary)
				.Where(_=>_.Total != 0)
				.ToList();
		}
	}
}
