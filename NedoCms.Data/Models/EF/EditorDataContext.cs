using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace NedoCms.Data.Models
{
	public class EditorDataContext : DbContext
	{
		public DbSet<Page> Pages { get; set; }
		public DbSet<PageContent> PageContents { get; set; }
		public DbSet<PageMetadata> PageMetadatas { get; set; }
		public DbSet<SharedContent> SharedContents { get; set; }
	}

	[Table("Pages", Schema = "dbo")]
	public class Page
	{
		[Key, Required]
		public Guid Id { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(64)]
		public string Title { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(128)]
		public string Master { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(128)]
		public string MenuLabel { get; set; }

		[Required]
		public int MenuOrder { get; set; }

		[Required]
		public bool Visible { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(256)]
		public string Route { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(2056)]
		public string FullRoute { get; set; }

		[Required]
		public short Type { get; set; }

		public Guid? ParentId { get; set; }

		[ForeignKey("ParentId")]
		public virtual Page Parent { get; set; }

		[InverseProperty("Parent")]
		public virtual ICollection<Page> Children { get; set; }

		[InverseProperty("Page")]
		public virtual ICollection<PageContent> PageContents { get; set; }

		[InverseProperty("Page")]
		public virtual ICollection<PageMetadata> PageMetadatas { get; set; }
	}

	[Table("PageContents", Schema = "dbo")]
	public class PageContent
	{
		[Key, Required]
		public Guid Id { get; set; }

		[Required]
		public Guid PageId { get; set; }
		
		[Required(AllowEmptyStrings = false), StringLength(256)]
		public string PlaceHolder { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Content { get; set; }

		[Required, Range(0, int.MaxValue)]
		public int Order { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Settings { get; set; }

		public Guid? SharedContentId { get; set; }

		[ForeignKey("PageId")]
		public virtual Page Page { get; set; }

		[ForeignKey("SharedContentId")]
		public virtual SharedContent SharedContent { get; set; }
	}

	[Table("PageMetadatas", Schema = "dbo")]
	public class PageMetadata
	{
		[Key, Required]
		public Guid Id { get; set; }

		[Required]
		public Guid PageId { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(128)]
		public string Key { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(1024)]
		public string Value { get; set; }

		[ForeignKey("PageId")]
		public virtual Page Page { get; set; }
	}

	[Table("SharedContents", Schema = "dbo")]
	public class SharedContent
	{
		[Key, Required]
		public Guid Id { get; set; }

		[Required(AllowEmptyStrings = false), StringLength(256)]
		public string Name { get; set; }

		public string Content { get; set; }

		[Required]
		public DateTime CreationDate { get; set; }

		[Required]
		public DateTime ModificationDate { get; set; }

		[InverseProperty("SharedContent")]
		public virtual ICollection<PageContent> PageContents { get; set; }
	}
}