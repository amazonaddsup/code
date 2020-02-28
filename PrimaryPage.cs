using Blazor.FileReader;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebSite
{
	public class PrimaryPage
	{
		private WebAssemblyHost Host { get; set; }

		private IFileReaderService FileReaderService =>
			Host.Services.GetRequiredService<IFileReaderService>();

		private IJSRuntime JSRuntime =>
			Host.Services.GetRequiredService<IJSRuntime>();

		public IReadOnlyList<Item> ItemsInFile { get; private set; }

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }

		public ElementReference FileSelector { get; set; }

		public
		(
			IReadOnlyList<Summary> Summary,
			IReadOnlyList<Item> Items,
			IReadOnlyList<CategorySpending> CategorySpendings,
			(
			IReadOnlyList<MonthlySpending> Data,
			string[] Labels
			) MonthlySpendings,
			(
			IReadOnlyList<YearlySpending> Data,
			string[] Labels
			) YearlySpendings,
			IReadOnlyList<BrandSpending> BrandSpendings
		) Datasets;

		public void PopulateDatasets()
		{
			Datasets.Items =
					ItemsInFile
					.Where
					(
						item =>
						StartDate == DateTime.MinValue /* No Filter is Applied */
						||
						item.Date.InBetween(StartDate, EndDate)
					)
					.ToList();

			var allCategories =
				Datasets.Items
				.Select(_ => _.Category)
				.Distinct()
				.OrderBy(_ => _)
				.ToArray();

			Datasets.CategorySpendings = ComputeCategorySpendings(Datasets.Items);

			Datasets.BrandSpendings = ComputeBrandSpendings(Datasets.Items);

			Datasets.MonthlySpendings.Data =
				Datasets.Items
				.GroupBy(_ => _.Date.Month)
				.Select(group => new MonthlySpending(group.Key, group.Sum(item => item.Total)))
				.OrderBy(_ => _.Month)
				.ToList();

			Datasets.MonthlySpendings.Labels =
				Datasets.MonthlySpendings.Data
				.Select(_ => _.Month.ToString())
				.ToArray();

			Datasets.YearlySpendings.Data =
				Datasets.Items
				.GroupBy(_ => _.Date.Month.Year)
				.Select(group => new YearlySpending(group.Key, group.Sum(item => item.Total)))
				.OrderBy(_ => _.Year)
				.ToList();

			Datasets.YearlySpendings.Labels =
				Datasets.YearlySpendings.Data
				.Select(_ => _.Year.ToString())
				.ToArray();

			Datasets.Summary = new[]
			{
				(Name: "Total", Value: Datasets.Items.Sum(item=>item.Total).ToString("#,#$")),
				(Name: "Days Shopped", Value:  GetDaysShopped()),
				(Name: "Average Daily Spending", Value:  Datasets.Items.Select(_=>_.Total).Average().ToString("#,#$")),
				(Name: "Median Item Price", Value:  Datasets.Items.Select(_=>_.Total).Median().ToString("#,#$")),
			}
			.Select(_ => new Summary(_.Name, _.Value))
			.ToList();
		}

		private string GetDaysShopped()
		{
			Console.WriteLine("start=" + StartDate + ",end=" + EndDate);
			var totalDays = (EndDate - StartDate).TotalDays;
			var daysShopped = Datasets.Items.Select(_ => _.Date.ToDatetime()).Distinct().Count();
			return $"{daysShopped} / {totalDays} ({((daysShopped / totalDays) * 100):N2})%";
		}

		private bool ShowResults = false;

		public string UiStyle
		{
			get
			{
				if (ShowResults)
				{
					return VisibleDiv;
				}

				return IndivibleDiv;
			}
		}

		private bool ShowYearlyResults => Datasets.YearlySpendings.Data?.Count > 1;

		public string UiYearlySpendingsStyle
		{
			get
			{
				if (ShowYearlyResults)
				{
					return VisibleDiv;
				}

				return IndivibleDiv;
			}
		}

		const string VisibleDiv = "display:block";

		const string IndivibleDiv = "display:none";

		public void RegisterHost(WebAssemblyHost host)
		{
			Host = host;
		}

		public async Task ReadFile()
		{
			var readOneFile = false;
			foreach (var file in await FileReaderService.CreateReference(FileSelector).EnumerateFilesAsync())
			{
				if (readOneFile)
				{
					throw new Exception("Multiple files");
				}

				readOneFile = true;

				// Read into memory and act
				using (var memoryStream = await file.CreateMemoryStreamAsync())
				{
					// Sync calls are ok once file is in memory
					var fileContents = Encoding.UTF8.GetString(memoryStream.ToArray());

					ItemsInFile = OrderParser.ParseFile(fileContents);

					StartDate = ItemsInFile.Min(_ => _.Date).ToDatetime();
					EndDate = ItemsInFile.Max(_ => _.Date).ToDatetime();

					PopulateDatasets();

					ShowResults = true;
				}
			}
		}

		public void ApplyFilter()
		{
			PopulateDatasets();
		}

		private static IReadOnlyList<CategorySpending> ComputeCategorySpendings(IReadOnlyList<Item> items)
		{
			var all =
				items
				.GroupBy(_ => _.Category)
				.Select
				(
					group => new CategorySpending
					(
						categoryId: group.Key,
						totalSpent: group.Sum(item => item.Total)
					)
				)
				.OrderByDescending(_ => _.TotalSpent)
				.ToList();

			var topN = all.Take(5).ToList();
			if (all.Count <= 5)
			{
				return all;
			}

			var misc = new CategorySpending("MISC", all.Except(topN).Sum(_ => _.TotalSpent));
			return topN.Append(misc).ToList();
		}

		private static IReadOnlyList<BrandSpending> ComputeBrandSpendings(IReadOnlyList<Item> items)
		{
			var all =
				items
				.GroupBy(_ => _.InferredBrand)
				.Select
				(
					group => new BrandSpending
					(
						brandName: group.Key,
						totalSpent: group.Sum(item => item.Total)
					)
				)
				.OrderByDescending(_ => _.TotalSpent)
				.ToList();

			var topN = all.Take(5).ToList();
			if (all.Count <= 5)
			{
				return all;
			}

			var misc = new BrandSpending("MISC", all.Except(topN).Sum(_ => _.TotalSpent));
			return topN.Append(misc).ToList();
		}

		public async Task SaveReport()
		{
			await JSRuntime.InvokeVoidAsync("SaveReport", $"{StartDate:dd-MMMM-yyyy}_to_{EndDate:dd-MMMM-yyyy}.png");
		}
	}
}
