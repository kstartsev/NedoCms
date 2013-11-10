using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Common.Images;
using NedoCms.Models.CkEditor;

namespace NedoCms.Controllers
{
	/// <summary>
	/// Actions required for ckeditor functionality
	/// </summary>
	public sealed class CkEditorController : Controller
	{
		private const string Folder = "~/Temp/CkEditor";

		/// <summary>
		/// Shows list of already uploaded images
		/// </summary>
		[HttpGet]
		public ActionResult Images(ImagesModel model)
		{
			var path = GetOrAddImagesFolder();

			model.Images = from file in Directory.EnumerateFiles(path)
						   let info = new FileInfo(file)
						   let name = Folder + "/" + info.Name
						   select new ImageItemModel
						   {
							   Url = Url.Content(name),
							   Name = info.Name
						   };

			return View(model);
		}

		/// <summary>
		/// Invoked when user tries to upload new image using CkEditor
		/// </summary>
		[HttpPost]
		public ActionResult ImagesUpload(UploadModel model)
		{
			var result = new UploadResultModel(model);
			if (model.Upload == null)
			{
				result.Error = "Image cannot be found";
				return View(result);
			}
			try
			{
				var path = GetOrAddImagesFolder();
				path = Path.Combine(path, model.Upload.FileName);

				if (System.IO.File.Exists(path))
				{
					result.Error = "Image with same name already exists";
					return View(result);
				}

				model.Upload.SaveAs(path);
				result.Url = Url.Content(Folder + "/" + model.Upload.FileName);
			}
			catch (Exception e)
			{
				result.Url = null;
				result.Error = e.ToString();
			}
			return View(result);
		}

		/// <summary>
		/// Renders image by path
		/// </summary>
		[HttpGet]
		public ActionResult Image(string path)
		{
			var p = Server.MapPath(path);

			return new ImageResult(() => new ResolveImageResult
			{
				MimeType = "image",
				Stream = System.IO.File.OpenRead(p)
			});
		}

		private string GetOrAddImagesFolder()
		{
			var dir = Server.MapPath(Folder);
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}
	}
}