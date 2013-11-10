using NedoCms.Common.Models.Menu;

namespace NedoCms.Models
{
	public static class Menu
	{
		public static readonly Descriptor[] Items = new[]
		{
			new Descriptor
			{
				Controller = "Page", Action = "Index", Route = "Default", Options = new RenderOptions {Text = "Pages"}, Children = new[]
				{
					new Descriptor {Controller = "Page", Action = "Content", Route = "Default", Hidden = true}
				}
			},
			new Descriptor {Controller = "Page", Action = "Shared", Route = "Default", Options = new RenderOptions {Text = "Shared"}}
		};
	}
}