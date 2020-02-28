using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebSite
{
	public static class InternalDatasets
	{
		public static IReadOnlyList<string> MultiWordBrands;

		public static async Task CacheMultiWordBrandsAsync(WebAssemblyHost host)
		{
			var httpClient = host.Services.GetRequiredService<HttpClient>();

			if (MultiWordBrands != null)
			{
				return;
			}

			var multiWordBrandsFile = await httpClient
				.GetStringAsync("MultiWordBrands.txt");

			MultiWordBrands =
				multiWordBrandsFile
				.Split('\n')
				.ToList();
		}
	}
}
