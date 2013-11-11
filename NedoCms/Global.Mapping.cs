using System.Linq;
using AutoMapper;
using NedoCms;
using NedoCms.Common.Extensions;
using NedoCms.Data.Models;
using NedoCms.Models.Page;
using WebActivatorEx;

[assembly: PreApplicationStartMethod(typeof (MvcApplication), "InitMapping")]

namespace NedoCms
{
	public partial class MvcApplication
	{
		public static void InitMapping()
		{
			Mapper.CreateMap<PageMetadata, PageMetadataModel>();
			Mapper.CreateMap<Page, PageModel>()
			      .ForMember(x => x.HasChildren, exp => exp.MapFrom(x => x.Children.Safe().Any()))
			      .ForMember(x => x.Metadata, exp => exp.MapFrom(x => x.PageMetadatas.Safe().Select(Mapper.Map<PageMetadata, PageMetadataModel>)));
		}
	}
}