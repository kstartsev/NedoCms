using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using NedoCms.Common.Extensions;

namespace NedoCms.Common.Images
{
	// aliases for memory and file cache to make code a bit easier to read
	
	public sealed partial class ImageResult
	{
		private const string MemoryCacheKey = "D20AB4410BA140D88685423AAC1E00B0";
		private const string FileCacheKey = "5FFF10A876534C7DB4CE9F8C44B5DA82";

		// TODO: let user set this via configuration
		private const string CacheDirectory = "~/Temp/Images";

		private const int MemoryMaxCount = 100;
		private const int FileMaxCount = 1000;

		private const int MemorySizeLimit = 1048576;
		private const int FileSizeLimit = 16777216;

		/// <summary>
		/// Updates cache items with new value
		/// </summary>
		/// <typeparam name="TValue">Value type</typeparam>
		/// <param name="cache">Cache to be updated</param>
		/// <param name="count">Max count of elements allowed in cache</param>
		/// <param name="key">The key for new element</param>
		/// <param name="value">New item</param>
		private static void UpdateCache<TValue>(IDictionary<string, CacheItem<TValue>> cache, int count, string key, CacheItem<TValue> value)
		{
			try
			{
				while (cache.Count > count)
				{
					cache.Remove(cache.Where(x => x.Value.AccessCount == cache.Min(y => y.Value.AccessCount)).Select(x => x.Key).First());
				}

				cache[key] = value;
			}
			catch (Exception) {}
		}

		/// <summary>
		/// Gets memory cache from http cache
		/// </summary>
		/// <param name="context">Container for cache</param>
		/// <returns>Returns found or newly created memory cache</returns>
		private static Dictionary<string, CacheItem<byte[]>> GetMemoryCache(ControllerContext context)
		{
			return context.HttpContext.Cache.GetOrAdd(MemoryCacheKey, key => new Dictionary<string, CacheItem<byte[]>>());
		}

		/// <summary>
		/// Updates memory cache with new values
		/// </summary>
		/// <param name="memoryCache">Cache to be updated</param>
		/// <param name="imageKey">Key to update</param>
		/// <param name="result">Contains value for cache</param>
		private static void UpdateMemoryCache(Dictionary<string, CacheItem<byte[]>> memoryCache, string imageKey, ResolveImageResult result)
		{
			var size = (int) result.Stream.Length;
			if (size <= MemorySizeLimit)
			{
				try
				{
					var buffer = new byte[size];
					result.Stream.Read(buffer, 0, size);

					var memoryItem = new CacheItem<byte[]>(buffer, result.MimeType, size) {AccessCount = 1};

					UpdateCache(memoryCache, MemoryMaxCount, imageKey, memoryItem);
				}
				catch
				{
					memoryCache.Remove(imageKey);
				}
			}
		}

		/// <summary>
		/// Gets file cache from http cache
		/// </summary>
		/// <param name="context">Container for cache</param>
		/// <returns>Returns found or newly created file cache</returns>
		private static Dictionary<string, CacheItem<string>> GetFileCache(ControllerContext context)
		{
			return context.HttpContext.Cache.GetOrAdd(FileCacheKey, key =>
			{
				// each time we create new file cache we want to remove existing files
				CreateOrCleanDirectory(context.HttpContext.Server.MapPath(CacheDirectory));

				return new Dictionary<string, CacheItem<string>>();
			});
		}

		/// <summary>
		/// Updates file cache with new values
		/// </summary>
		/// <param name="fileCache">Cache to be updated</param>
		/// <param name="imageKey">Key to update</param>
		/// <param name="result">Contains value for cache</param>
		/// <param name="directory">Directory where resulting cache item should be placed</param>
		private static void UpdateFileCache(Dictionary<string, CacheItem<string>> fileCache, string imageKey, ResolveImageResult result, string directory)
		{
			var size = (int)result.Stream.Length;
			if (size <= FileSizeLimit)
			{
				try
				{
					var path = Path.Combine(directory, Path.GetRandomFileName());

					using (var stream = File.OpenWrite(path))
					{
						result.Stream.Seek(0, SeekOrigin.Begin);
						result.Stream.CopyTo(stream);
					}

					var fileItem = new CacheItem<string>(path, result.MimeType, size);

					UpdateCache(fileCache, FileMaxCount, imageKey, fileItem);
				}
				catch
				{
					fileCache.Remove(imageKey);
				}
			}
		}

		/// <summary>
		/// Tries to get item from memory or file cache
		/// </summary>
		/// <param name="key">Key to get value</param>
		/// <param name="memoryCache">Mem cache to look into</param>
		/// <param name="fileCache">File cache to look into</param>
		/// <returns>Found result or null</returns>
		private static ResolveImageResult TryFromCache(string key, Dictionary<string, CacheItem<byte[]>> memoryCache, Dictionary<string, CacheItem<string>> fileCache)
		{
			CacheItem<byte[]> memoryItem;
			if (memoryCache.TryGetValue(key, out memoryItem))
			{
				memoryItem.AccessCount++;
				return new ResolveImageResult {Stream = new MemoryStream(memoryItem.Value), MimeType = memoryItem.MimeType};
			}

			CacheItem<string> fileItem;
			if (fileCache.TryGetValue(key, out fileItem))
			{
				fileItem.AccessCount++;

				if (fileItem.Size <= MemorySizeLimit)
				{
					return new ResolveImageResult {Stream = File.OpenRead(fileItem.Value), MimeType = fileItem.MimeType};
				}

				try
				{
					using (var reader = new BinaryReader(File.OpenRead(fileItem.Value)))
					{
						var inMemory = new CacheItem<byte[]>(reader.ReadBytes(fileItem.Size), fileItem.MimeType, fileItem.Size);
						UpdateCache(memoryCache, MemoryMaxCount, key, inMemory);
						return new ResolveImageResult {Stream = new MemoryStream(inMemory.Value), MimeType = inMemory.MimeType};
					}
				}
				catch
				{
					memoryCache.Remove(key);
					fileCache.Remove(key);
				}
			}

			return null;
		}

		/// <summary>
		/// Safely cleans directory or creates such dir if it is not found
		/// </summary>
		/// <param name="path">Path where directory should be created</param>
		private static void CreateOrCleanDirectory(string path)
		{
			try
			{
				var directory = new DirectoryInfo(path);
				if (directory.Exists)
				{
					foreach (var file in directory.EnumerateFiles())
					{
						try
						{
							file.Delete();
						}
						catch (Exception) {}
					}
				}
				else directory.Create();
			}
			catch (Exception) {}
		}

		/// <summary>
		/// Simple cache item wrapping value with type <see cref="TValue" />
		/// </summary>
		internal sealed class CacheItem<TValue>
		{
			public int AccessCount;

			public CacheItem(TValue value, string mimeType, int size)
			{
				Value = value;
				MimeType = mimeType;
				Size = size;
			}

			public TValue Value { get; private set; }
			public string MimeType { get; private set; }
			public int Size { get; private set; }
		}
	}
}