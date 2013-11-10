using System;
using System.IO;

namespace NedoCms.Common.Extensions
{
	public static class StreamExtensions
	{
		/// <summary>
		/// Reads whole content of the stream.
		/// </summary>
		public static byte[] ReadWhole(this Stream s)
		{
			if (s == null) throw new ArgumentNullException("s");
			if (!s.CanRead) throw new ArgumentException("Steam doesn't support read");

			using (var memory = new MemoryStream())
			{
				var buffer = new byte[4*1024];
				while (true)
				{
					var bytesRead = s.Read(buffer, 0, buffer.Length);
					memory.Write(buffer, 0, bytesRead);
					if (bytesRead < buffer.Length) break;
				}
				return memory.ToArray();
			}
		}
	}
}