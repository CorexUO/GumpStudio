using System;
using System.Drawing;

namespace GumpStudio.Plugins
{
	[Serializable]
	public class GridConfiguration
	{
		public bool Enabled { get; set; } = true;

		public bool GridVisible { get; set; } = true;

		public Size GridSize { get; set; } = new Size(10, 10);

		public Color GridColor { get; set; } = Color.LightGray;
	}
}