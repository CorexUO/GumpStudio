using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Ultima
{
	public sealed class SpeechList
	{
		public static List<SpeechEntry> Entries { get; set; }

		private static readonly byte[] m_Buffer = new byte[128];

		static SpeechList()
		{
			Initialize();
		}

		/// <summary>
		/// Loads speech.mul in <see cref="SpeechList.Entries"/>
		/// </summary>
		public static void Initialize()
		{
			var path = Files.GetFilePath("speech.mul");
			if (path == null) {
				Entries = new List<SpeechEntry>(0);
				return;
			}
			Entries = new List<SpeechEntry>();
			using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)) {
				var buffer = new byte[fs.Length];
				unsafe {
					var order = 0;
					fs.Read(buffer, 0, buffer.Length);
					fixed (byte* data = buffer) {
						var bindat = data;
						var bindatend = bindat + buffer.Length;

						while (bindat != bindatend) {
							var id = (short)((*bindat++ >> 8) | (*bindat++)); //Swapped Endian
							var length = (short)((*bindat++ >> 8) | (*bindat++));
							if (length > 128) {
								length = 128;
							}

							for (var i = 0; i < length; ++i) {
								m_Buffer[i] = *bindat++;
							}

							var keyword = Encoding.UTF8.GetString(m_Buffer, 0, length);
							Entries.Add(new SpeechEntry(id, keyword, order));
							++order;
						}
					}
				}
			}
		}

		/// <summary>
		/// Saves speech.mul to <see cref="FileName"/>
		/// </summary>
		/// <param name="FileName"></param>
		public static void SaveSpeechList(string FileName)
		{
			Entries.Sort(new OrderComparer());
			using (var fs = new FileStream(FileName, FileMode.Create, FileAccess.Write, FileShare.Write)) {
				using (var bin = new BinaryWriter(fs)) {
					foreach (var entry in Entries) {
						bin.Write(NativeMethods.SwapEndian(entry.ID));
						var utf8String = Encoding.UTF8.GetBytes(entry.KeyWord);
						var length = (short)utf8String.Length;
						bin.Write(NativeMethods.SwapEndian(length));
						bin.Write(utf8String);
					}
				}
			}
		}

		public static void ExportToCSV(string FileName)
		{
			using (var Tex = new StreamWriter(new FileStream(FileName, FileMode.Create, FileAccess.ReadWrite), System.Text.Encoding.Unicode)) {
				Tex.WriteLine("Order;ID;KeyWord");
				foreach (var entry in Entries) {
					Tex.WriteLine(String.Format("{0};{1};{2}", entry.Order, entry.ID, entry.KeyWord));
				}
			}
		}

		public static void ImportFromCSV(string FileName)
		{
			Entries = new List<SpeechEntry>(0);
			if (!File.Exists(FileName)) {
				return;
			}

			using (var sr = new StreamReader(FileName)) {
				string line;
				while ((line = sr.ReadLine()) != null) {
					if ((line = line.Trim()).Length == 0 || line.StartsWith("#")) {
						continue;
					}

					if ((line.Contains("Order")) && (line.Contains("KeyWord"))) {
						continue;
					}

					try {
						var split = line.Split(';');
						if (split.Length < 3) {
							continue;
						}

						var order = ConvertStringToInt(split[0]);
						var id = ConvertStringToInt(split[1]);
						var word = split[2];
						word = word.Replace("\"", "");
						Entries.Add(new SpeechEntry((short)id, word, order));
					}
					catch { }
				}
			}
		}

		public static int ConvertStringToInt(string text)
		{
			int result;
			if (text.Contains("0x")) {
				var convert = text.Replace("0x", "");
				Int32.TryParse(convert, System.Globalization.NumberStyles.HexNumber, null, out result);
			}
			else {
				Int32.TryParse(text, System.Globalization.NumberStyles.Integer, null, out result);
			}

			return result;
		}

		#region SortComparer
		public class IDComparer : IComparer<SpeechEntry>
		{
			private readonly bool m_desc;

			public IDComparer(bool desc)
			{
				m_desc = desc;
			}

			public int Compare(SpeechEntry objA, SpeechEntry objB)
			{
				if (objA.ID == objB.ID) {
					return 0;
				}
				else if (m_desc) {
					return (objA.ID < objB.ID) ? 1 : -1;
				}
				else {
					return (objA.ID < objB.ID) ? -1 : 1;
				}
			}
		}

		public class KeyWordComparer : IComparer<SpeechEntry>
		{
			private readonly bool m_desc;

			public KeyWordComparer(bool desc)
			{
				m_desc = desc;
			}

			public int Compare(SpeechEntry objA, SpeechEntry objB)
			{
				if (m_desc) {
					return String.Compare(objB.KeyWord, objA.KeyWord);
				}
				else {
					return String.Compare(objA.KeyWord, objB.KeyWord);
				}
			}
		}

		public class OrderComparer : IComparer<SpeechEntry>
		{
			public int Compare(SpeechEntry objA, SpeechEntry objB)
			{
				if (objA.Order == objB.Order) {
					return 0;
				}
				else {
					return (objA.Order < objB.Order) ? -1 : 1;
				}
			}
		}

		#endregion

	}

	public sealed class SpeechEntry
	{
		public short ID { get; set; }
		public string KeyWord { get; set; }

		[Browsable(false)]
		public int Order { get; private set; }

		public SpeechEntry(short id, string keyword, int order)
		{
			ID = id;
			KeyWord = keyword;
			Order = order;
		}
	}

	[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
	public unsafe struct SpeechMul
	{
		public short id;
		public short length;
		public byte[] keyword;
	}
}
