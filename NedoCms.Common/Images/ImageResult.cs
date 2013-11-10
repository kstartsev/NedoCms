using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web.Mvc;
using NedoCms.Common.Extensions;

namespace NedoCms.Common.Images
{
	/// <summary>
	/// Represents an image action result.
	/// </summary>
	public sealed partial class ImageResult : ActionResult
	{
		private readonly Func<ResolveImageResult> _resolve;

		public static readonly ImageResult Empty = new ImageResult(() => null);

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageResult"/> class.
		/// </summary>
		/// <param name="resolve">The resolve.</param>
		public ImageResult(Func<ResolveImageResult> resolve)
		{
			if (resolve == null) throw new ArgumentNullException("resolve");

			_resolve = resolve;
		}

		/// <summary>
		/// Enables processing of the result of an action method by a custom type that inherits from the <see cref="T:System.Web.Mvc.ActionResult"/> class.
		/// </summary>
		/// <param name="context">The context in which the result is executed. The context information includes the controller, HTTP content, request context, and route data.</param>
		public override void ExecuteResult(ControllerContext context)
		{
			var resolveResult = ResolveImage(context);
			if (resolveResult != null)
			{
				var fileStreamResult = new FileStreamResult(resolveResult.Stream, resolveResult.MimeType);
				fileStreamResult.ExecuteResult(context);
			}
			else
			{
				//TODO: let user specify default image if requested one was not found
				new EmptyResult().ExecuteResult(context);
			}
		}

		private ResolveImageResult ResolveImage(ControllerContext context)
		{
			var imageKey = GetImageKey(context);

			var memoryCache = GetMemoryCache(context);
			var fileCache = GetFileCache(context);

			var cacheResult = TryFromCache(imageKey, memoryCache, fileCache);
			if (cacheResult != null) return cacheResult;

			// if image was not found in cache we have to try to get it from given func
			var result = _resolve();
			if (result == null) return null;

			result = GetOrResizeImage(context, result);
			result.Stream.Seek(0, SeekOrigin.Begin);

			// updating cache after new result was prepared
			try
			{
				UpdateMemoryCache(memoryCache, imageKey, result);
				result.Stream.Seek(0, SeekOrigin.Begin); // moving back to the beginning of the stream each time we read from it

				UpdateFileCache(fileCache, imageKey, result, context.HttpContext.Server.MapPath(CacheDirectory));
				result.Stream.Seek(0, SeekOrigin.Begin);

				return TryFromCache(imageKey, memoryCache, fileCache) ?? result;
			}
			catch (Exception)
			{
				result.Stream.Seek(0, SeekOrigin.Begin);
				return result;
			}
		}

		private static string GetImageKey(ControllerContext context)
        {
            var valueProvider = context.Controller.ValueProvider;

			return new StringBuilder()
				.Append(valueProvider.GetValue<string>("controller")).Append(';')
				.Append(valueProvider.GetValue<string>("action")).Append(';')
				.Append(valueProvider.GetValue<string>("imageId")).Append(';')
				.Append(valueProvider.GetValue<string>("route")).Append(';')
				.Append(valueProvider.GetValue<string>("maxWidth")).Append(';')
				.Append(valueProvider.GetValue<string>("maxHeight")).Append(';')
				.Append(valueProvider.GetValue<string>("crop")).Append(';')
				.Append(valueProvider.GetValue<string>("usebestsize")).Append(';')
				.Append(valueProvider.GetValue<string>("center")).Append(';').ToString();
		}

	    private static ResolveImageResult GetOrResizeImage(ControllerContext context, ResolveImageResult result)
		{
			var valueProvider = context.Controller.ValueProvider;

	        var maxWidth = valueProvider.GetValue<int>("maxWidth");
            var maxHeight = valueProvider.GetValue<int>("maxHeight");

		    using (var image = Image.FromStream(result.Stream))
		    {
		        if (maxWidth == 0) maxWidth = image.Width;
                if (maxHeight == 0) maxHeight = image.Height;

			    if (image.Width > maxWidth || image.Height > maxHeight)
			    {
				    var crop = valueProvider.GetValue<bool?>("crop") ?? false;
				    var bs = valueProvider.GetValue<bool?>("usebestsize") ?? false;

				    var type = crop ? ImageResizeType.Crop : (bs ? ImageResizeType.BestSize : ImageResizeType.Default);

				    result = GetResizer(image, type).Resize(new ResizeContext
				    {
					    Width = maxWidth,
					    Height = maxHeight,
					    Center = valueProvider.GetValue<bool?>("center") ?? false
				    });
			    }
                return result;
            }
		}

		private static ResizerBase GetResizer(Image img, ImageResizeType type)
		{
			switch (type)
			{
				case ImageResizeType.Crop: return new CropResizer(img);
				case ImageResizeType.BestSize:return new BestSizeResizer(img);
				case ImageResizeType.Default: return new DefaultResizer(img);
			}
			throw new ArgumentOutOfRangeException("type");
		}
	}
}