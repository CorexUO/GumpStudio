using System.Drawing;

using GumpStudio.Forms;

namespace GumpStudio.Plugins
{
	public class SnapToGridExtender : ElementExtender
	{
		private readonly DesignerForm _designer;
		public GridConfiguration Config { get; set; }

		public SnapToGridExtender(DesignerForm designerForm)
		{
			_designer = designerForm;
		}

		public int SnapXToGrid(int X)
		{
			return X / Config.GridSize.Width * Config.GridSize.Width;
		}

		public int SnapYToGrid(int Y)
		{
			return Y / Config.GridSize.Height * Config.GridSize.Height;
		}

		public Point SnapToGrid(Point Position)
		{
			var result = Position;
			result.X = SnapXToGrid(Position.X);
			result.Y = SnapXToGrid(Position.Y);

			return result;
		}
	}
}