using System;
using System.Drawing;
using System.Windows.Forms;

using GumpStudio.Elements;

namespace GumpStudio.Plugins
{
	public class MouseMovementEventArgs : EventArgs
	{
		public BaseElement Element { get; }

		public Keys Keys { get; set; }
		public MouseButtons Button { get; set; }
		public Point Location { get; set; }
		public MoveType Mode { get; set; }

		public MouseMovementEventArgs(BaseElement element)
		{
			Element = element;
		}
	}
}
