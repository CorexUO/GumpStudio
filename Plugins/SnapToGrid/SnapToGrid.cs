using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using GumpStudio.Elements;
using GumpStudio.Forms;

namespace GumpStudio.Plugins
{
	public class SnapToGrid : BasePlugin
	{
		private DesignerForm _Designer;
		private SnapToGridExtender _Extender;

		private readonly PropertyEditor _ConfigEditor = new PropertyEditor();

		public GridConfiguration Config { get; } = new GridConfiguration();

		public override PluginInfo Info { get; } = new PluginInfo("Snap To Grid", "1.1", "Bradley Uffner", "buffner@tkpups.com", "Allows elements to be snapped to a grid.");

		public MenuItem Menu { get; } = new MenuItem();

		public override void Load(DesignerForm designer)
		{
			if (IsLoaded && _Designer == designer)
			{
				return; 
			}

			_Designer = designer;

			if (_Extender == null)
			{
				_Extender = new SnapToGridExtender(_Designer);
			}

			LoadConfig();

			_Extender.Config = Config;

			_ConfigEditor.Text = Name;
			_ConfigEditor.SourceObject = Config;

			_ConfigEditor.PropertyValueChanged += OnConfigValueChanged;
			_ConfigEditor.FormClosing += OnConfigEditorClosing;

			Menu.Text = "Snap To Grid";
			Menu.Click += OnOpenSettingsEditor;

			_Designer.MenuPlugins.MenuItems.Add(Menu);
			_Designer.HookPreRender += OnRenderGrid;
			_Designer.HookKeyDown += OnKeyDown;
		}

		public void LoadConfig()
		{
			var path = Path.Combine(_Designer.AppPath, "SnapToGrid.bin");

			if (!File.Exists(path))
			{
				return;
			}

			using (var fileStream = new FileStream(path, FileMode.Open))
			{
				var bin = new BinaryFormatter();

				var config = (GridConfiguration)bin.Deserialize(fileStream);

				foreach (var f in typeof(GridConfiguration).GetFields())
				{
					f.SetValue(Config, f.GetValue(config));
				}

				foreach (var p in typeof(GridConfiguration).GetProperties())
				{
					if (p.CanRead && p.CanWrite)
					{
						p.SetValue(Config, p.GetValue(config));
					}
				}
			}
		}

		public void SaveConfig()
		{
			var path = Path.Combine(_Designer.AppPath, "SnapToGrid.bin");

			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				var bin = new BinaryFormatter();

				bin.Serialize(fileStream, Config);
			}
		}

		public override void InitializeElementExtenders(BaseElement element)
		{
			element.AddExtender(_Extender);
		}

		private void OnOpenSettingsEditor(object sender, EventArgs e)
		{
			_ConfigEditor.Show(_Designer);
		}

		private void OnConfigValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			_Designer.CanvasImage.Refresh();
		}

		private void OnConfigEditorClosing(object sender, FormClosingEventArgs e)
		{
			if (_ConfigEditor.ChangesPending)
			{
				SaveConfig();
			}
		}

		private void OnKeyDown(object sender, ref KeyEventArgs e)
		{
			if (!Config.Enabled || _Designer.ActiveElement == null || sender != _Designer.CanvasFocus || e.Modifiers.HasFlag(Keys.Shift))
			{
				return;
			}

			var modified = false;

			switch (e.KeyCode)
			{
				case Keys.Up:
				{
					foreach (var element in _Designer.ElementStack.GetSelectedElements())
					{
						element.Y -= Config.GridSize.Height;
						element.Y = _Extender.SnapYToGrid(element.Y);

						modified = true;
					}
				}
				break;

				case Keys.Down:
				{
					foreach (var element in _Designer.ElementStack.GetSelectedElements())
					{
						element.Y += Config.GridSize.Height;
						element.Y = _Extender.SnapYToGrid(element.Y);

						modified = true;
					}
				}
				break;

				case Keys.Left:
				{
					foreach (var element in _Designer.ElementStack.GetSelectedElements())
					{
						element.X -= Config.GridSize.Height;
						element.X = _Extender.SnapYToGrid(element.X);

						modified = true;
					}
				}
				break;

				case Keys.Right:
				{
					foreach (var element in _Designer.ElementStack.GetSelectedElements())
					{
						element.X += Config.GridSize.Height;
						element.X = _Extender.SnapYToGrid(element.X);

						modified = true;
					}
				}
				break;
			}

			if (modified)
			{
				e.Handled = true;

				_Designer.CreateUndoPoint("Move Elements");
				_Designer.CanvasImage.Invalidate();
			}
		}

		private void OnRenderGrid(Bitmap target)
		{
			if (!Config.Enabled || !Config.GridVisible)
			{
				return;
			}

			var rect = new Rectangle(0, 0, target.Width, target.Height);
			var data = target.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

			var size = _Extender.Config.GridSize;

			var boundX = target.Width - 1;
			var boundY = target.Height - 1;

			var indexX = 0;

			while (indexX <= boundX)
			{
				var indexY = 0;

				while (indexY <= boundY)
				{
					var offset = data.Stride * indexY + 4 * indexX;

					Marshal.WriteByte(data.Scan0, offset, Config.GridColor.R);
					Marshal.WriteByte(data.Scan0, offset + 1, Config.GridColor.G);
					Marshal.WriteByte(data.Scan0, offset + 2, Config.GridColor.B);
					Marshal.WriteByte(data.Scan0, offset + 3, Byte.MaxValue);

					indexY += size.Height;
				}

				indexX += size.Width;
			}

			target.UnlockBits(data);
		}

		public override void OnMouseMove(ref MouseMoveHookEventArgs e)
		{
			if (e.MoveMode == MoveModeType.Move && !e.Keys.HasFlag(Keys.Shift))
			{
				e.MouseLocation = _Extender.SnapToGrid(e.MouseLocation);
			}
		}
	}
}