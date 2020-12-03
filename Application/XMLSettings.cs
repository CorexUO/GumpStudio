using System;
using System.Drawing;

namespace GumpStudio
{
	[Serializable]
	public sealed class XMLSettings : BaseConfig
	{
		public static XMLSettings AppSettings { get; } = new XMLSettings();

		public override string Name { get; } = "Settings";
		public override string FileName { get; } = "Settings.xml";
		public override ConfigFormat Format { get; } = ConfigFormat.Xml;

		public string ClientPath { get; set; } = Environment.SpecialFolder.ProgramFiles.ToString();

		public Size DesignerFormSize { get; set; } = new Size(1366, 768);

		public int UndoLevels { get; set; } = 50;
	}
}