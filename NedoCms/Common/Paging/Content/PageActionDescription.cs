using System;

namespace NedoCms.Common.Paging.Content
{
	[Serializable]
	public sealed class PageActionDescription
	{
		public string Action { get; set; }
		public string Controller { get; set; }
	}
}