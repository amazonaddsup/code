using System.Threading.Tasks;
using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Blazor.FileReader;
using WebSite;

namespace WebSite
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebAssemblyHostBuilder.CreateDefault(args);
			builder.RootComponents.Add<App>("app");

			builder.Services.AddTelerikBlazor();

			builder.Services.AddFileReaderService(options =>
			{
				options.UseWasmSharedBuffer = true;
			});

			var primaryPage= new PrimaryPage();
			builder.Services.AddSingleton<PrimaryPage>(primaryPage);
			
			var navigationBar = new NavigationBar();
			builder.Services.AddSingleton<NavigationBar>(navigationBar);

			var host = builder.Build();

			primaryPage.RegisterHost(host);
			navigationBar.RegisterHost(host);

			await InternalDatasets.CacheMultiWordBrandsAsync(host);

			await host.RunAsync();
		}
	}
}
