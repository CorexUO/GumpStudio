using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Ultima
{
	public sealed class Gumps
	{
		private static FileIndex m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", "gumpartLegacyMUL.uop", 0xFFFF, 12, ".tga", -1, true);

		private static Bitmap[] m_Cache;
		private static bool[] m_Removed;
		private static readonly Hashtable m_patched = new Hashtable();

		private static byte[] m_PixelBuffer;
		private static byte[] m_StreamBuffer;
		private static byte[] m_ColorTable;
		static Gumps()
		{
			if (m_FileIndex != null) {
				m_Cache = new Bitmap[m_FileIndex.Index.Length];
				m_Removed = new bool[m_FileIndex.Index.Length];
			}
			else {
				m_Cache = new Bitmap[0xFFFF];
				m_Removed = new bool[0xFFFF];
			}
		}
		/// <summary>
		/// ReReads gumpart
		/// </summary>
		public static void Reload()
		{
			try {
				m_FileIndex = new FileIndex("Gumpidx.mul", "Gumpart.mul", "gumpartLegacyMUL.uop", 12, -1, ".tga", -1, true);
				m_Cache = new Bitmap[m_FileIndex.Index.Length];
				m_Removed = new bool[m_FileIndex.Index.Length];
			}
			catch {
				m_FileIndex = null;
				m_Cache = new Bitmap[0xFFFF];
				m_Removed = new bool[0xFFFF];
			}

			m_PixelBuffer = null;
			m_StreamBuffer = null;
			m_ColorTable = null;
			m_patched.Clear();
		}

		public static int GetCount()
		{
			return m_Cache.Length;
		}

		/// <summary>
		/// Replaces Gump <see cref="m_Cache"/>
		/// </summary>
		/// <param name="index"></param>
		/// <param name="bmp"></param>
		public static void ReplaceGump(int index, Bitmap bmp)
		{
			m_Cache[index] = bmp;
			m_Removed[index] = false;
			if (m_patched.Contains(index)) {
				m_patched.Remove(index);
			}
		}

		/// <summary>
		/// Removes Gumpindex <see cref="m_Removed"/>
		/// </summary>
		/// <param name="index"></param>
		public static void RemoveGump(int index)
		{
			m_Removed[index] = true;
		}

		/// <summary>
		/// Tests if index is definied
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static bool IsValidIndex(int index)
		{
			if (m_FileIndex == null) {
				return false;
			}

			if (index > m_Cache.Length - 1) {
				return false;
			}

			if (m_Removed[index]) {
				return false;
			}

			if (m_Cache[index] != null) {
				return true;
			}


			if (!m_FileIndex.Valid(index, out var length, out var extra, out var patched)) {
				return false;
			}

			if (extra == -1) {
				return false;
			}

			var width = (extra >> 16) & 0xFFFF;
			var height = extra & 0xFFFF;

			if (width <= 0 || height <= 0) {
				return false;
			}

			return true;
		}

		public static byte[] GetRawGump(int index, out int width, out int height)
		{
			width = -1;
			height = -1;
			var stream = m_FileIndex.Seek(index, out var length, out var extra, out var patched);
			if (stream == null) {
				return null;
			}

			if (extra == -1) {
				return null;
			}

			width = (extra >> 16) & 0xFFFF;
			height = extra & 0xFFFF;
			if (width <= 0 || height <= 0) {
				return null;
			}

			var buffer = new byte[length];
			stream.Read(buffer, 0, length);
			stream.Close();
			return buffer;
		}

		/// <summary>
		/// Returns Bitmap of index and applies Hue
		/// </summary>
		/// <param name="index"></param>
		/// <param name="hue"></param>
		/// <param name="onlyHueGrayPixels"></param>
		/// <returns></returns>
		public static unsafe Bitmap GetGump(int index, Hue hue, bool onlyHueGrayPixels, out bool patched)
		{
			var stream = m_FileIndex.Seek(index, out var length, out var extra, out patched);

			if (stream == null) {
				return null;
			}

			if (extra == -1) {
				stream.Close();
				return null;
			}

			var width = (extra >> 16) & 0xFFFF;
			var height = extra & 0xFFFF;

			if (width <= 0 || height <= 0) {
				stream.Close();
				return null;
			}

			var bytesPerLine = width << 1;
			var bytesPerStride = (bytesPerLine + 3) & ~3;
			var bytesForImage = height * bytesPerStride;

			var pixelsPerStride = (width + 1) & ~1;
			var pixelsPerStrideDelta = pixelsPerStride - width;

			var pixelBuffer = m_PixelBuffer;

			if (pixelBuffer == null || pixelBuffer.Length < bytesForImage) {
				m_PixelBuffer = pixelBuffer = new byte[(bytesForImage + 2047) & ~2047];
			}

			var streamBuffer = m_StreamBuffer;

			if (streamBuffer == null || streamBuffer.Length < length) {
				m_StreamBuffer = streamBuffer = new byte[(length + 2047) & ~2047];
			}

			var colorTable = m_ColorTable;

			if (colorTable == null) {
				m_ColorTable = colorTable = new byte[128];
			}

			stream.Read(streamBuffer, 0, length);

			fixed (short* psHueColors = hue.Colors) {
				fixed (byte* pbStream = streamBuffer) {
					fixed (byte* pbPixels = pixelBuffer) {
						fixed (byte* pbColorTable = colorTable) {
							var pHueColors = (ushort*)psHueColors;
							var pHueColorsEnd = pHueColors + 32;

							var pColorTable = (ushort*)pbColorTable;

							var pColorTableOpaque = pColorTable;

							while (pHueColors < pHueColorsEnd) {
								*pColorTableOpaque++ = *pHueColors++;
							}

							var pPixelDataStart = (ushort*)pbPixels;

							var pLookup = (int*)pbStream;
							var pLookupEnd = pLookup + height;
							var pPixelRleStart = pLookup;
							int* pPixelRle;

							var pPixel = pPixelDataStart;
							var pRleEnd = pPixel;
							var pPixelEnd = pPixel + width;

							ushort color, count;

							if (onlyHueGrayPixels) {
								while (pLookup < pLookupEnd) {
									pPixelRle = pPixelRleStart + *pLookup++;
									pRleEnd = pPixel;

									while (pPixel < pPixelEnd) {
										color = *(ushort*)pPixelRle;
										count = *(1 + (ushort*)pPixelRle);
										++pPixelRle;

										pRleEnd += count;

										if (color != 0 && (color & 0x1F) == ((color >> 5) & 0x1F) && (color & 0x1F) == ((color >> 10) & 0x1F)) {
											color = pColorTable[color >> 10];
										}
										else if (color != 0) {
											color ^= 0x8000;
										}

										while (pPixel < pRleEnd) {
											*pPixel++ = color;
										}
									}

									pPixel += pixelsPerStrideDelta;
									pPixelEnd += pixelsPerStride;
								}
							}
							else {
								while (pLookup < pLookupEnd) {
									pPixelRle = pPixelRleStart + *pLookup++;
									pRleEnd = pPixel;

									while (pPixel < pPixelEnd) {
										color = *(ushort*)pPixelRle;
										count = *(1 + (ushort*)pPixelRle);
										++pPixelRle;

										pRleEnd += count;

										if (color != 0) {
											color = pColorTable[color >> 10];
										}

										while (pPixel < pRleEnd) {
											*pPixel++ = color;
										}
									}

									pPixel += pixelsPerStrideDelta;
									pPixelEnd += pixelsPerStride;
								}
							}
							stream.Close();
							return new Bitmap(width, height, bytesPerStride, PixelFormat.Format16bppArgb1555, (IntPtr)pPixelDataStart);
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns Bitmap of index
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public static unsafe Bitmap GetGump(int index)
		{
			return GetGump(index, out var patched);
		}

		/// <summary>
		/// Returns Bitmap of index and if verdata patched
		/// </summary>
		/// <param name="index"></param>
		/// <param name="patched"></param>
		/// <returns></returns>
		public static unsafe Bitmap GetGump(int index, out bool patched)
		{
			if (m_patched.Contains(index)) {
				patched = (bool)m_patched[index];
			}
			else {
				patched = false;
			}

			if (index > m_Cache.Length - 1) {
				return null;
			}

			if (m_Removed[index]) {
				return null;
			}

			if (m_Cache[index] != null) {
				return m_Cache[index];
			}

			var stream = m_FileIndex.Seek(index, out var length, out var extra, out patched);
			if (stream == null) {
				return null;
			}

			if (extra == -1) {
				stream.Close();
				return null;
			}
			if (patched) {
				m_patched[index] = true;
			}

			var width = (extra >> 16) & 0xFFFF;
			var height = extra & 0xFFFF;

			if (width <= 0 || height <= 0) {
				return null;
			}

			var bmp = new Bitmap(width, height, PixelFormat.Format16bppArgb1555);
			var bd = bmp.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format16bppArgb1555);

			if (m_StreamBuffer == null || m_StreamBuffer.Length < length) {
				m_StreamBuffer = new byte[length];
			}

			stream.Read(m_StreamBuffer, 0, length);

			fixed (byte* data = m_StreamBuffer) {
				var lookup = (int*)data;
				var dat = (ushort*)data;

				var line = (ushort*)bd.Scan0;
				var delta = bd.Stride >> 1;
				var count = 0;
				for (var y = 0; y < height; ++y, line += delta) {
					count = (*lookup++ * 2);

					var cur = line;
					var end = line + bd.Width;

					while (cur < end) {
						var color = dat[count++];
						var next = cur + dat[count++];

						if (color == 0) {
							cur = next;
						}
						else {
							color ^= 0x8000;
							while (cur < next) {
								*cur++ = color;
							}
						}
					}
				}
			}

			bmp.UnlockBits(bd);
			if (Files.CacheData) {
				return m_Cache[index] = bmp;
			}
			else {
				return bmp;
			}
		}

		public static unsafe void Save(string path)
		{
			var idx = Path.Combine(path, "Gumpidx.mul");
			var mul = Path.Combine(path, "Gumpart.mul");
			using (FileStream fsidx = new FileStream(idx, FileMode.Create, FileAccess.Write, FileShare.Write),
							  fsmul = new FileStream(mul, FileMode.Create, FileAccess.Write, FileShare.Write)) {
				using (BinaryWriter binidx = new BinaryWriter(fsidx),
									binmul = new BinaryWriter(fsmul)) {
					for (var index = 0; index < m_Cache.Length; index++) {
						if (m_Cache[index] == null) {
							m_Cache[index] = GetGump(index);
						}

						var bmp = m_Cache[index];
						if ((bmp == null) || (m_Removed[index])) {
							binidx.Write(-1); // lookup
							binidx.Write(-1); // length
							binidx.Write(-1); // extra
						}
						else {
							var bd = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, PixelFormat.Format16bppArgb1555);
							var line = (ushort*)bd.Scan0;
							var delta = bd.Stride >> 1;

							binidx.Write((int)fsmul.Position); //lookup
							var length = (int)fsmul.Position;
							var fill = 0;
							for (var i = 0; i < bmp.Height; ++i) {
								binmul.Write(fill);
							}
							for (var Y = 0; Y < bmp.Height; ++Y, line += delta) {
								var cur = line;

								var X = 0;
								var current = (int)fsmul.Position;
								fsmul.Seek(length + Y * 4, SeekOrigin.Begin);
								var offset = (current - length) / 4;
								binmul.Write(offset);
								fsmul.Seek(length + offset * 4, SeekOrigin.Begin);

								while (X < bd.Width) {
									var Run = 1;
									var c = cur[X];
									while ((X + Run) < bd.Width) {
										if (c != cur[X + Run]) {
											break;
										}

										++Run;
									}
									if (c == 0) {
										binmul.Write(c);
									}
									else {
										binmul.Write((ushort)(c ^ 0x8000));
									}

									binmul.Write((short)Run);
									X += Run;
								}
							}
							length = (int)fsmul.Position - length;
							binidx.Write(length);
							binidx.Write((bmp.Width << 16) + bmp.Height);
							bmp.UnlockBits(bd);
						}
					}
				}
			}
		}
	}
}