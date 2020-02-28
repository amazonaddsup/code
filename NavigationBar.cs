using Microsoft.AspNetCore.Blazor.Hosting;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace WebSite
{
	public class NavigationBar
	{
		private WebAssemblyHost Host { get; set; }

		private NavigationManager NavigationManager => Host.Services.GetRequiredService<NavigationManager>();

		public void RegisterHost(WebAssemblyHost host)
		{
			Host = host;
		}

		public void NavigateTo(MenuItem item)
		{
			if (!String.IsNullOrWhiteSpace(item.Uri))
			{
				NavigationManager.NavigateTo(item.Uri);
			}
		}

		public static readonly IReadOnlyList<MenuItem> MenuItems = new List<MenuItem>
		{
			MenuItem.ForNavigation(text: "Home", uri: "/"),
			MenuItem.ForNavigation(text: "Donate", uri: "/donate"),
			MenuItem.ForSubMenu(
				text: "More",
				submenu:
			new List<MenuItem>()
			{
				MenuItem.ForNavigation(text: "FAQs", uri: "/faqs"),
				MenuItem.ForNavigation(text: "Contact", uri: "/contact"),
			}),
		};

		public class MenuItem
		{
			public string Text { get; private set; }

			public string Uri { get; private set; }

			public IReadOnlyList<MenuItem> Items { get; private set; }

			private MenuItem(string text)
			{
				Text = text;
			}

			public static MenuItem ForSubMenu(string text, IReadOnlyList<MenuItem> submenu)
			{
				return new MenuItem(text) { Items = submenu };
			}

			public static MenuItem ForNavigation(string text, string uri)
			{
				return new MenuItem(text) { Uri = uri };
			}
		}
	}
}
