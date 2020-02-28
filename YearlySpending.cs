namespace WebSite
{
	public sealed class YearlySpending
	{
		public int Year { get; }

		public double TotalSpent { get; }

		public YearlySpending(int year, double totalSpent)
		{
			Year = year;
			TotalSpent = totalSpent;
		}
	}
}
