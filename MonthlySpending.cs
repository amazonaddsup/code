namespace WebSite
{
	public sealed class MonthlySpending
	{
		public Month Month { get; }

		public double TotalSpent { get; }

		public MonthlySpending(Month month, double totalSpent)
		{
			Month = month;
			TotalSpent = totalSpent;
		}
	}
}
