namespace WebSite
{
	public sealed class BrandSpending
	{
		public string BrandName { get; }

		public double TotalSpent { get; }

		public string DonutChartLabel => $"{BrandName} ({TotalSpent:#,#}$)";

		public BrandSpending(string brandName, double totalSpent)
		{
			BrandName = brandName;
			TotalSpent = totalSpent;
		}
	}
}
