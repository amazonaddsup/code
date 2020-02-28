using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebSite
{
	public class Summary
	{
		public string Name { get; }

		public string Value { get; }

		public Summary()
		{
		}

		public Summary(string name, string value)
		{
			Name = name;
			Value = value;
		}
	}
}
