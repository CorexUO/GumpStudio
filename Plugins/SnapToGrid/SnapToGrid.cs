using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using GumpStudio.Elements;
using GumpStudio.Forms;

namespace GumpStudio.Plugins
{
	public class SnapToGrid : BasePlugin
	{
		private DesignerForm _designer;
		private SnapToGridExtender _extender;

		protected GridConfiguration Config;

		public MenuItem ShowGridMenu { get; set; }

		public override PluginInfo Info { get; } = new PluginInfo("SnapToGrid", "1.0", "Bradley Uffner", "buffner@tkpups.com", "Allows elements to be snapped to a grid.");

		public SnapToGrid()
		{
			var gridSize = new Size(10, 10);

			Config = new GridConfiguration(gridSize, Color.LightGray, true);
		}

		private void DoConfigGridMenu(object Sender, EventArgs E)
		{
			_designer.picCanvas.Refresh();
		}

		private void DoToggleGridMenu(object Sender, EventArgs E)
		{
			Config.ShowGrid = !Config.ShowGrid;
			ShowGridMenu.Checked = Config.ShowGrid;
			_designer.picCanvas.Refresh();
		}

		private void HookKeyDown(object sender, ref KeyEventArgs e)
		{
			if (_designer.ActiveElement == null || sender != _designer.CanvasFocus || e.Modifiers.HasFlag(Keys.Shift) || !Config.ShowGrid) {
				return;
			}

			var modified = false;

			switch (e.KeyCode) {
				case Keys.Up: {
					IEnumerator enumerator = default;

					try {
						enumerator = _designer.ElementStack.GetSelectedElements().GetEnumerator();

						while (enumerator.MoveNext()) {
							var objectValue = RuntimeHelpers.GetObjectValue(enumerator.Current);
							var val = (BaseElement)objectValue;
							var val2 = val;
							val2.Y = val2.Y - Config.GridSize.Height;
							val.Y = _extender.SnapYToGrid(val.Y);
						}
					}
					finally {
						(enumerator as IDisposable)?.Dispose();
					}

					modified = true;
					_designer.CreateUndoPoint();

					break;
				}

				case Keys.Down: {
					IEnumerator enumerator2 = default;

					try {
						enumerator2 = _designer.ElementStack.GetSelectedElements().GetEnumerator();

						while (enumerator2.MoveNext()) {
							var objectValue2 = RuntimeHelpers.GetObjectValue(enumerator2.Current);
							var val3 = (BaseElement)objectValue2;
							var val2 = val3;
							val2.Y = val2.Y + Config.GridSize.Height;
							val3.Y = _extender.SnapYToGrid(val3.Y);
						}
					}
					finally {
						if (enumerator2 is IDisposable) {
							(enumerator2 as IDisposable).Dispose();
						}
					}

					modified = true;
					_designer.CreateUndoPoint();

					break;
				}

				case Keys.Left: {
					IEnumerator enumerator3 = default;

					try {
						enumerator3 = _designer.ElementStack.GetSelectedElements().GetEnumerator();

						while (enumerator3.MoveNext()) {
							var objectValue3 = RuntimeHelpers.GetObjectValue(enumerator3.Current);
							var val4 = (BaseElement)objectValue3;
							var val2 = val4;
							val2.X = val2.X - Config.GridSize.Width;
							val4.X = _extender.SnapXToGrid(val4.X);
						}
					}
					finally {
						if (enumerator3 is IDisposable disposable) {
							disposable.Dispose();
						}
					}

					modified = true;
					_designer.CreateUndoPoint();

					break;
				}

				case Keys.Right: {
					IEnumerator enumerator4 = default;

					try {
						enumerator4 = _designer.ElementStack.GetSelectedElements().GetEnumerator();

						while (enumerator4.MoveNext()) {
							var objectValue4 = RuntimeHelpers.GetObjectValue(enumerator4.Current);
							var val5 = (BaseElement)objectValue4;
							var val2 = val5;
							val2.X = val2.X + Config.GridSize.Width;
							val5.X = _extender.SnapXToGrid(val5.X);
						}
					}
					finally {
						if (enumerator4 is IDisposable) {
							(enumerator4 as IDisposable).Dispose();
						}
					}

					modified = true;
					_designer.CreateUndoPoint();

					break;
				}
			}

			if (modified) {
				e.Handled = true;
				_designer.picCanvas.Invalidate();
			}
		}

		public override void InitializeElementExtenders(BaseElement Element)
		{
			Element.AddExtender(_extender);
		}

		public override void Load(DesignerForm frmDesigner)
		{
			_designer = frmDesigner;

			LoadConfig();

			if (_extender == null) {
				_extender = new SnapToGridExtender(_designer);
			}

			_extender.Config = Config;

			var menuItem = new MenuItem("Snap To Grid", ToggleSnapToGrid) {
				Checked = Config.ShowGrid
			};

			_designer.mnuPlugins.MenuItems.Add(menuItem);
			_designer.HookPreRender += RenderGrid;
			_designer.HookKeyDown += HookKeyDown;
		}

		protected void LoadConfig()
		{
			if (!File.Exists(_designer.AppPath + "\\Plugins\\SnapToGrid.config")) {
				return;
			}

			var fileStream = new FileStream(_designer.AppPath + "\\Plugins\\SnapToGrid.config", FileMode.Open);
			var binaryFormatter = new BinaryFormatter();
			Config = (GridConfiguration)binaryFormatter.Deserialize(fileStream);
			fileStream.Close();
		}

		public override void MouseMoveHook(ref MouseMoveHookEventArgs e)
		{
			if (!Config.ShowGrid) {
				return;
			}

			if (e.MoveMode == MoveModeType.Move && !e.Keys.HasFlag(Keys.Shift)) {
				e.MouseLocation = _extender.SnapToGrid(e.MouseLocation);
			}
		}

		private void RenderGrid(Bitmap Target)
		{
			var rect = new Rectangle(0, 0, Target.Width, Target.Height);
			var bitmapData = Target.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			checked {
				if (Config.ShowGrid) {
					var num = Target.Width - 1;
					var width = _extender.Config.GridSize.Width;
					var num2 = num;
					var num3 = 0;

					while (true) {
						var num4 = (width >> 31) ^ num3;
						var num5 = (width >> 31) ^ num2;

						if (num4 > num5) {
							break;
						}

						var num6 = Target.Height - 1;
						var height = _extender.Config.GridSize.Height;
						var num7 = num6;
						var num8 = 0;

						while (true) {
							var num9 = (height >> 31) ^ num8;
							num5 = (height >> 31) ^ num7;

							if (num9 > num5) {
								break;
							}

							var num10 = bitmapData.Stride * num8 + 4 * num3;
							Marshal.WriteByte(bitmapData.Scan0, num10, Config.GridColor.R);
							Marshal.WriteByte(bitmapData.Scan0, num10 + 1, Config.GridColor.G);
							Marshal.WriteByte(bitmapData.Scan0, num10 + 2, Config.GridColor.B);
							Marshal.WriteByte(bitmapData.Scan0, num10 + 3, Byte.MaxValue);
							num8 += height;
						}

						num3 += width;
					}
				}

				Target.UnlockBits(bitmapData);
			}
		}

		protected void SaveConfig()
		{
			var fileStream = new FileStream(_designer.AppPath + "\\Plugins\\SnapToGrid.config", FileMode.Create);
			var binaryFormatter = new BinaryFormatter();
			binaryFormatter.Serialize(fileStream, Config);
			fileStream.Close();
		}

		private void ToggleSnapToGrid(object sender, EventArgs e)
		{
			Config.ShowGrid = !Config.ShowGrid;
			((MenuItem)sender).Checked = Config.ShowGrid;
			_designer.picCanvas.Invalidate();
		}
	}
}