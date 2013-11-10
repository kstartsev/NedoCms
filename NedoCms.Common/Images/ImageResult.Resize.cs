using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace NedoCms.Common.Images
{
	public sealed partial class ImageResult
	{
		#region ResizerBase

		private abstract class ResizerBase
		{
			protected ResizerBase(Image image)
			{
				Image = image;
			}

			protected Image Image { get; private set; }

			public ResolveImageResult Resize(ResizeContext context)
			{
				var halfWidth = 0.5f * Image.Width;
				var halfHeight = 0.5f * Image.Height;

				var points = new[]
				{
					new PointF(-halfWidth, -halfHeight), new PointF(+halfWidth, -halfHeight),
					new PointF(+halfWidth, +halfHeight), new PointF(-halfWidth, +halfHeight)
				};

				var matrix = new Matrix();
				matrix.TransformPoints(points);

				var min = new PointF(+float.MaxValue, +float.MaxValue);
				var max = new PointF(-float.MaxValue, -float.MaxValue);

				foreach (var point in points)
				{
					if (min.X > point.X) min.X = point.X;
					if (min.Y > point.Y) min.Y = point.Y;
					if (max.X < point.X) max.X = point.X;
					if (max.Y < point.Y) max.Y = point.Y;
				}

				var width = (float)Math.Ceiling(max.X - min.X);
				var height = (float)Math.Ceiling(max.Y - min.Y);

				float scale;
				var size = CalculateSize(new SizeF(width, height), new SizeF(context.Width, context.Height), out scale);

				var dimensions = new DimensionsContext {Size = size, Scale = scale, Points = points, Max = max, Min = min};

				return ResizeImpl(context, dimensions);
			}

			protected abstract SizeF CalculateSize(SizeF image, SizeF requested, out float scale);

			protected abstract ResolveImageResult ResizeImpl(ResizeContext context, DimensionsContext dimensions);
		}

		#endregion

		#region BestSizeResizer

		private sealed class BestSizeResizer : ResizerBase
		{
			public BestSizeResizer(Image image) : base(image) {}

			protected override SizeF CalculateSize(SizeF image, SizeF requested, out float scale)
			{
				var size = (float) Math.Floor(requested.Width*image.Height/image.Width);
				if (size <= requested.Height)
				{
					scale = requested.Width/image.Width;
					return new SizeF(requested.Width, size);
				}
				size = (float) Math.Floor(requested.Height*image.Width/image.Height);
				{
					scale = requested.Height/image.Height;
					return new SizeF(size, requested.Height);
				}
			}

			protected override ResolveImageResult ResizeImpl(ResizeContext context, DimensionsContext dimensions)
			{
				var size = dimensions.Size;
				var s = new SizeF(size.Width + 0.5f, size.Height + 0.5f).ToSize();
				using (var bitmap = new Bitmap(s.Width, s.Height, PixelFormat.Format32bppArgb))
				using (var graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(Color.Transparent);

					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(Image, new Rectangle(0, 0, s.Width, s.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);

					var result = new MemoryStream();
					bitmap.Save(result, ImageFormat.Png);

					return new ResolveImageResult {Stream = result, MimeType = "image/png"};
				}
			}
		}

		#endregion

		#region CropResizer

		private sealed class CropResizer : ResizerBase
		{
			public CropResizer(Image image) : base(image) {}

			protected override SizeF CalculateSize(SizeF image, SizeF requested, out float scale)
			{
				var scaleH = requested.Height/image.Height;
				var scaleW = requested.Width/image.Width;

				scale = Math.Max(scaleW, scaleH);
				return new SizeF(image.Width*scale, image.Height*scale);
			}

			protected override ResolveImageResult ResizeImpl(ResizeContext context, DimensionsContext dimensions)
			{
				//Cropping example: we have image 800x600, need to resize and crop it to 200x200.
				//First we calculate size for this image, requirement here is to use entire area.
				//So, in out example size will be 266,7x200. It is wider than required area -> image should be cropped
				//Cropping is done by cloning the original image but only taking a rectangle of the original.
				var size = dimensions.Size;
				var s = new SizeF(size.Width + 0.5f, size.Height + 0.5f).ToSize();
				using (var bitmap = new Bitmap(s.Width, s.Height, PixelFormat.Format32bppArgb))
				using (var graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(Color.Transparent);

					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.DrawImage(Image, new Rectangle(0, 0, s.Width, s.Height), 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);

					var result = new MemoryStream();

					var offsetX = Math.Max(0, (bitmap.Width - context.Width)/2);
					var offsetY = Math.Max(0, (bitmap.Height - context.Height)/2);
					var offset = context.Center ? new Point(offsetX, offsetY) : new Point(0, 0);

					using (var cropped = bitmap.Clone(new Rectangle(offset.X, offset.Y, context.Width, context.Height), bitmap.PixelFormat))
					{
						cropped.Save(result, ImageFormat.Png);
					}

					return new ResolveImageResult {Stream = result, MimeType = "image/png"};
				}
			}
		}

		#endregion

		#region DefaultResizer

		private sealed class DefaultResizer : ResizerBase
		{
			public DefaultResizer(Image image) : base(image) { }
			protected override SizeF CalculateSize(SizeF image, SizeF requested, out float scale)
			{
				var size = (float)Math.Floor(requested.Width * image.Height / image.Width);
				if (size <= requested.Height)
				{
					scale = requested.Width / image.Width;
					return new SizeF(requested.Width, size);
				}
				size = (float)Math.Floor(requested.Height * image.Width / image.Height);
				{
					scale = requested.Height / image.Height;
					return new SizeF(size, requested.Height);
				}
			}

			protected override ResolveImageResult ResizeImpl(ResizeContext context, DimensionsContext dimensions)
			{
				var size = dimensions.Size;
				var scale = dimensions.Scale;
				var points = dimensions.Points;
				var min = dimensions.Min;
				//				var max = dimensions.Max;

				var dx = 0.5f * (Image.Width - size.Width);
				var dy = 0.5f * (context.Height - size.Height);

				using (var bitmap = new Bitmap(Image.Width, context.Height, PixelFormat.Format32bppArgb))
				using (var graphics = Graphics.FromImage(bitmap))
				{
					graphics.Clear(Color.Transparent);

					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.CompositingQuality = CompositingQuality.HighQuality;
					graphics.InterpolationMode = InterpolationMode.High;

					Point[] destination =
                    {
                        Point.Round(new PointF((points[0].X - min.X)*scale + dx, (points[0].Y - min.Y)*scale + dy)),
                        Point.Round(new PointF((points[1].X - min.X)*scale + dx, (points[1].Y - min.Y)*scale + dy)),
                        Point.Round(new PointF((points[3].X - min.X)*scale + dx, (points[3].Y - min.Y)*scale + dy))
                    };

					graphics.DrawImage(Image, destination);

					var result = new MemoryStream();
					bitmap.Save(result, ImageFormat.Png);
					return new ResolveImageResult { Stream = result, MimeType = "image/png" };
				}
			}
		}

		#endregion

		#region Context entities 

		private sealed class DimensionsContext
		{
			public SizeF Size { get; set; }
			public float Scale { get; set; }
			public PointF[] Points { get; set; }
			public PointF Min { get; set; }
			public PointF Max { get; set; }
		}

		private sealed class ResizeContext
		{
			public int Width { get; set; }
			public int Height { get;set; }
			public bool Center { get; set; }
		}

		private enum ImageResizeType
		{
			Default, 
			Crop, 
			BestSize
		}

		#endregion
	}
}