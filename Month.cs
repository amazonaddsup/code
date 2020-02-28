using System;

namespace WebSite
{
	public sealed class Month: IComparable<Month>
	{
		public int Year { get; }

		public int MonthNumber { get; }

		private DateTime FirstDay => new DateTime(Year, MonthNumber, 1);

		public Month(int year, int month)
		{
			Year = year;
			MonthNumber = month;
		}

		public int CompareTo(Month that)
		{
			return this.FirstDay.CompareTo(that.FirstDay);
		}

		public override bool Equals(object that)
		{
			return this.FirstDay.Equals((that as Month).FirstDay);
		}

		public override int GetHashCode()
		{
			return FirstDay.GetHashCode();
		}

		public override string ToString()
		{
			return FirstDay.ToString($"MMMM").Substring(0, 3) + $" '{Year.ToString().Substring(2)}";
		}
	}
}
