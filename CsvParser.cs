using System.Collections.Generic;
using System.Linq;

namespace WebSite
{
	public static class CsvParser
	{
		const char DoubleQuote = '\"';

		const char Comma = ',';

		public static IReadOnlyList<IReadOnlyDictionary<string, string>> ParseFile(string fileContents)
		{
			var lines = fileContents.Split('\n');
			var columnNames = lines.First().Split(',');
			return
				lines
				.Skip(1)
				.Where(_=>_.Any())
				.Select(line => ParseLine(columnNames, line))
				.ToList();
		}

		private static IReadOnlyDictionary<string, string> ParseLine(IReadOnlyList<string> columnNames, string line)
		{
			var columns = new List<string>();
			for (var startIndex = 0; startIndex < line.Length;)
			{
				var parsed = ParseColumn(line, startIndex);
				columns.Add(parsed.Value);
				startIndex += parsed.Size;
			}

			return new Dictionary<string, string>(columnNames.Zip(columns, (key, value) => KeyValuePair.Create(key, value)));
		}

		private static (string Value, int Size) ParseColumn(string line, int startIndex)
		{
			if (line[startIndex] == DoubleQuote)
			{
				var value = ParseDoubleQuotedColumn(line, startIndex);

				return (value, value.Length /* Column */ + 2 /* Two Quotes */ + 1 /* Comma */ + CountDoubleQuotes(value));
			}
			else
			{
				var value = ParseUnQuotedColumn(line, startIndex);

				return (value, value.Length + 1 /* Comma */ + CountDoubleQuotes(value));
			}
		}

		private static int CountDoubleQuotes(string text)
		{
			return text.Count(_ => _ == DoubleQuote);
		}

		private static string ParseUnQuotedColumn(string line, int startIndex)
		{
			var columnLength = 0;
			while (startIndex + columnLength < line.Length && line[startIndex + columnLength] != Comma)
			{
				columnLength++;
			}

			return line.Substring(startIndex, columnLength);
		}

		private static string ParseDoubleQuotedColumn(string line, int startIndex)
		{
			var value = string.Empty;
			for (startIndex++; startIndex < line.Length;)
			{
				if (line[startIndex] != DoubleQuote)
				{
					value += line[startIndex];
					startIndex++;
				}
				else
				{
					// Just a double quote in the middle
					if (startIndex + 1 < line.Length && line[startIndex + 1] == DoubleQuote)
					{
						value += DoubleQuote;
						startIndex += 2;
					}
					else // End of the column
					{
						break;
					}
				}
			}

			return value;
		}
	}

}
