using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GumpStudio.Plugins
{
	public class SnapToGrid : BasePlugin
	{
		public override PluginInfo Info { get; } = new PluginInfo("Snap To Grid", "1.1", "Bradley Uffner", "buffner@tkpups.com", "Allows elements to be snapped to a grid.");

		private readonly Settings _Config = new Settings();

		public override BaseConfig Config => _Config;

		protected override void OnLoaded()
		{
			base.OnLoaded();

			Designer.HookPreRender += OnPreRender;
			Designer.HookKeyDown += OnKeyDown;
		}

		protected override void OnUnloaded()
		{
			base.OnUnloaded();

			Designer.HookPreRender -= OnPreRender;
			Designer.HookKeyDown -= OnKeyDown;
		}

		private int SnapX(int x)
		{
			return x / _Config.GridSize.Width * _Config.GridSize.Width;
		}

		private int SnapY(int y)
		{
			return y / _Config.GridSize.Height * _Config.GridSize.Height;
		}

		private Point Snap(Point p)
		{
			p.X = SnapX(p.X);
			p.Y = SnapY(p.Y);

			return p;
		}

		public override void MouseMovement(MouseMovementEventArgs e)
		{
			if (e.Mode == MoveType.Move && !e.Keys.HasFlag(Keys.Shift))
			{
				e.Location = Snap(e.Location);
			}
		}

		private void OnKeyDown(object sender, ref KeyEventArgs e)
		{
			if (!_Config.Enabled)
			{
				return;
			}

			if (Designer.ActiveElement == null || sender != Designer.CanvasFocus)
			{
				return;
			}

			if (e.Handled || e.Modifiers.HasFlag(Keys.Shift))
			{
				return;
			}

			var modified = false;

			switch (e.KeyCode)
			{
				case Keys.Up:
				{
					foreach (var element in Designer.ElementStack.SelectedElements)
					{
						element.Y = SnapY(element.Y - _Config.GridSize.Height);

						modified = true;
					}
				}
				break;

				case Keys.Down:
				{
					foreach (var element in Designer.ElementStack.SelectedElements)
					{
						element.Y = SnapY(element.Y + _Config.GridSize.Height);

						modified = true;
					}
				}
				break;

				case Keys.Left:
				{
					foreach (var element in Designer.ElementStack.SelectedElements)
					{
						element.X = SnapX(element.X - _Config.GridSize.Width);

						modified = true;
					}
				}
				break;

				case Keys.Right:
				{
					foreach (var element in Designer.ElementStack.SelectedElements)
					{
						element.X = SnapX(element.X + _Config.GridSize.Width);

						modified = true;
					}
				}
				break;
			}

			if (modified)
			{
				e.Handled = true;

				Designer.CreateUndoPoint("Move Elements");
				Designer.CanvasImage.Invalidate();
			}
		}

		private void OnPreRender(Bitmap target)
		{
			if (!_Config.Enabled || !_Config.GridVisible)
			{
				return;
			}

			var argb = Color.FromKnownColor(_Config.GridColor).ToArgb();

			var rect = new Rectangle(0, 0, target.Width, target.Height);
			var data = target.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			var size = _Config.GridSize;

			var boundX = target.Width - 1;
			var boundY = target.Height - 1;

			var indexX = 0;

			while (indexX <= boundX)
			{
				var indexY = 0;

				while (indexY <= boundY)
				{
					Marshal.WriteInt32(data.Scan0, data.Stride * indexY + 4 * indexX, argb);

					indexY += size.Height;
				}

				indexX += size.Width;
			}

			target.UnlockBits(data);
		}

		[Serializable]
		public class Settings : BaseConfig
		{
			public override string Name => "Snap To Grid";

			public bool Enabled { get; set; } = true;

			public bool GridVisible { get; set; } = true;

			public Size GridSize { get; set; } = new Size(10, 10);

			public KnownColor GridColor { get; set; } = KnownColor.LightGray;
		}
	}
}