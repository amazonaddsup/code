using System;

namespace WebSite
{
	public class PurchaseDate: IComparable<PurchaseDate>
	{
		private DateTime Date { get; }

		public Month Month => new Month(year: Date.Year, month: Date.Month);

		private PurchaseDate(DateTime date)
		{
			Date = date;
		}

		public DateTime ToDatetime()
		{
			return Date;
		}

		public static PurchaseDate FromDateTime(DateTime date)
		{
			return new PurchaseDate(date);
		}

		public override string ToString()
		{
			return Date.ToString("dd-MMMM-yyyy");
		}

		public int CompareTo(PurchaseDate that)
		{
			return this.Date.CompareTo(that.Date);
		}

		public bool InBetween(DateTime start, DateTime end)
		{
			return this.ToDatetime() >= start && this.ToDatetime() <= end;
		}

		public static implicit operator PurchaseDate(DateTime dateTime) => FromDateTime(dateTime);
		
	}
}
