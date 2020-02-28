using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite
{
	public static class IEnumerableExtensions
	{
		public static double Median(this IEnumerable<double> elements)
		{
			var orderedList = elements.OrderBy(_ => _).ToList();
			if (orderedList.Count % 2 == 1)
			{
				return orderedList[orderedList.Count / 2];
			}

			return (orderedList[orderedList.Count / 2] + orderedList[(orderedList.Count / 2) + 1]) / 2.0;
		}
	}
}
