using System;
using System.Collections.Generic;
using System.Linq;

namespace WebSite
{
	public sealed class Item
	{
		public PurchaseDate Date { get; }

		public string Id { get; }

		public string Title { get; }

		public string Category { get; }

		public string ASIN { get; }

		public string InferredBrand { get; }

		public double Total { get; }

		public Item()
		{
		}

		public static Item FromDictionary(IReadOnlyDictionary<string, string> dictionary)
		{
			try
			{
				return
					new Item
					(
						date: DateTime.Parse(dictionary["Order Date"]),
						id: dictionary["Order ID"],
						title: dictionary["Title"].FallbackIfEmpty("UNKNOWN"),
						inferredBrand: GetInferredBrandName(dictionary["Title"]),
						category: dictionary["Category"].FallbackIfEmpty("UNKNOWN"),
						asin: dictionary["ASIN/ISBN"],
						total: double.Parse(string.Concat(dictionary["Item Total"].Skip(1) /* Skip the currency symbol */))
					);
			}
			catch (KeyNotFoundException exception)
			{
				throw new Exception($"Failed to parse item. Dictionary: {dictionary.GetDebugString()}. Exception: {exception}");
			}
		}

		private static string GetInferredBrandName(string title)
		{
			if(string.IsNullOrWhiteSpace(title) || (!char.IsLetterOrDigit(title.First()) && !char.IsLowSurrogate(title.First())))
			{
				return "Unknown";
			}

			var closestMultiWordMatch = 
				InternalDatasets
				.MultiWordBrands
				.Where(brand => title.StartsWith(brand))
				.OrderByDescending(brandName => brandName.Length)
				.FirstOrDefault();

			return closestMultiWordMatch.FallbackIfEmpty(title.GetFirstWord());
		}

		private Item
			(
			DateTime date,
			string id,
			string title,
			string inferredBrand,
			string category,
			string asin,
			double total
			)
		{
			Date =date;
			Id = id;
			Title = title;
			InferredBrand = inferredBrand;
			Category = category;
			ASIN = asin;
			Total = total;
		}
	}
}
