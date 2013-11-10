using System;

namespace NedoCms.Common.Paging.Content
{
	[Serializable]
	public sealed class PageContentDescription
	{
		public PageActionDescription Edit { get; set; }
		public PageActionDescription View { get; set; }
		public string Description { get; set; }
	}
}