// Decompiled with JetBrains decompiler
// Type: GumpStudio.HuePickerControl
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Microsoft.VisualBasic.CompilerServices;

using Ultima;

namespace GumpStudio
{
	public class HuePickerControl : UserControl
	{
		private ComboBox _cboQuick;
		private ListBox _lstHue;
		private StatusBar _StatusBar;
		private ToolTip _ToolTip1;
		private readonly object[] _hueNames;
		private IContainer components;
		protected Hue mHue;

		public Hue Hue
		{
			get => mHue;
			set => mHue = value;
		}

		public event HuePickerControl.ValueChangedEventHandler ValueChanged;

		public HuePickerControl()
		{
			Load += new EventHandler(HuePickerControl_Load);
			InitializeComponent();
		}

		public HuePickerControl(Hue InitialHue)
		  : this()
		{
			mHue = InitialHue;
		}

		private void cboQuick_SelectedIndexChanged(object sender, EventArgs e)
		{
			var s = "0";
			switch (_cboQuick.Text)
			{
				case "Colors":
					s = "0";
					break;
				case "Skin":
					s = "1001";
					break;
				case "Hair":
					s = "1101";
					break;
				case "Interesting #1":
					s = "1049";
					break;
				case "Pinks":
					s = "1200";
					break;
				case "Elemental Weapons":
					s = "1254";
					break;
				case "Interesting #2":
					s = "1278";
					break;
				case "Blues":
					s = "1300";
					break;
				case "Elemental Wear":
					s = "1354";
					break;
				case "Greens":
					s = "1400";
					break;
				case "Oranges":
					s = "1500";
					break;
				case "Reds":
					s = "1600";
					break;
				case "Yellows":
					s = "1700";
					break;
				case "Neutrals":
					s = "1800";
					break;
				case "Snakes":
					s = "2000";
					break;
				case "Birds":
					s = "2100";
					break;
				case "Slimes":
					s = "2200";
					break;
				case "Animals":
					s = "2300";
					break;
				case "Metals":
					s = "2400";
					break;
			}
			_lstHue.SelectedIndex = _lstHue.FindString(s);
			_lstHue.Focus();
		}

		protected static Color Convert555ToARGB(short Col)
		{
			return Color.FromArgb(((short)(Col >> 10) & 31) * 8, ((short)(Col >> 5) & 31) * 8, (Col & 31) * 8);
		}

		private void HuePickerControl_Load(object sender, EventArgs e)
		{
			_lstHue.Items.Clear();
			foreach (var hue in Hues.List)
			{
				if (hue.Index == mHue.Index)
				{
					_lstHue.SelectedIndex = _lstHue.Items.Add(hue);
				}
				else
				{
					_lstHue.Items.Add(hue);
				}
			}
			_StatusBar.Text = Conversions.ToString(mHue.Index) + ": " + mHue.Name;
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			_lstHue = new System.Windows.Forms.ListBox();
			_StatusBar = new System.Windows.Forms.StatusBar();
			_cboQuick = new System.Windows.Forms.ComboBox();
			_ToolTip1 = new System.Windows.Forms.ToolTip(components);
			SuspendLayout();
			// 
			// _lstHue
			// 
			_lstHue.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			_lstHue.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			_lstHue.IntegralHeight = false;
			_lstHue.Location = new System.Drawing.Point(0, 0);
			_lstHue.Name = "_lstHue";
			_lstHue.Size = new System.Drawing.Size(208, 244);
			_lstHue.TabIndex = 0;
			_lstHue.DrawItem += new System.Windows.Forms.DrawItemEventHandler(lstHue_DrawItem);
			_lstHue.SelectedIndexChanged += new System.EventHandler(lstHue_SelectedIndexChanged);
			_lstHue.DoubleClick += new System.EventHandler(lstHue_DoubleClick);
			// 
			// _StatusBar
			// 
			_StatusBar.Location = new System.Drawing.Point(0, 0);
			_StatusBar.Name = "_StatusBar";
			_StatusBar.Size = new System.Drawing.Size(100, 22);
			_StatusBar.TabIndex = 0;
			// 
			// _cboQuick
			// 
			_cboQuick.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_cboQuick.DropDownWidth = 120;
			_cboQuick.Items.AddRange(new object[] {
			"Colors",
			"Skin",
			"Hair",
			"Interesting #1",
			"Pinks",
			"Elemental Weapons",
			"Interesting #2",
			"Blues",
			"Elemental Wear",
			"Greens",
			"Oranges",
			"Reds",
			"Yellows",
			"Neutrals",
			"Snakes",
			"Birds",
			"Slimes",
			"Animals",
			"Metals"});
			_cboQuick.Location = new System.Drawing.Point(144, 243);
			_cboQuick.Name = "_cboQuick";
			_cboQuick.Size = new System.Drawing.Size(64, 21);
			_cboQuick.TabIndex = 2;
			_ToolTip1.SetToolTip(_cboQuick, "Bookmarks");
			_cboQuick.SelectedIndexChanged += new System.EventHandler(cboQuick_SelectedIndexChanged);
			// 
			// HuePickerControl
			// 
			Controls.Add(_cboQuick);
			Controls.Add(_StatusBar);
			Controls.Add(_lstHue);
			Name = "HuePickerControl";
			Size = new System.Drawing.Size(208, 264);
			ResumeLayout(false);
		}

		private void lstHue_DoubleClick(object sender, EventArgs e)
		{
			var valueChanged = ValueChanged;
			valueChanged?.Invoke(mHue);
		}

		private void lstHue_DrawItem(object sender, DrawItemEventArgs e)
		{
			var graphics1 = e.Graphics;
			graphics1.FillRectangle(Brushes.White, e.Bounds);
			if ((e.State & DrawItemState.Selected) > DrawItemState.None)
			{
				var rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 50, _lstHue.ItemHeight);
				graphics1.FillRectangle(SystemBrushes.Highlight, rect);
			}
			else
			{
				var rect = new Rectangle(e.Bounds.X, e.Bounds.Y, 50, _lstHue.ItemHeight);
				graphics1.FillRectangle(SystemBrushes.Window, rect);
			}
			var num1 = (e.Bounds.Width - 35) / 32f;
			var hue = (Hue)_lstHue.Items[e.Index];
			var graphics2 = graphics1;
			var s = hue.Index.ToString();
			var font = e.Font;
			var black = Brushes.Black;
			var bounds1 = e.Bounds;
			double num2 = bounds1.X + 3;
			bounds1 = e.Bounds;
			double y1 = bounds1.Y;
			graphics2.DrawString(s, font, black, (float)num2, (float)y1);
			var num3 = 0;
			foreach (var color in hue.Colors)
			{
				var bounds2 = e.Bounds;
				var x = bounds2.X + 35 + (int)Math.Round(num3 * (double)num1);
				bounds2 = e.Bounds;
				var y2 = bounds2.Y;
				var width = (int)Math.Round(num1 + 1.0);
				bounds2 = e.Bounds;
				var height = bounds2.Height;
				var rect = new Rectangle(x, y2, width, height);
				graphics1.FillRectangle(new SolidBrush(HuePickerControl.Convert555ToARGB(color)), rect);
				++num3;
			}
		}

		private void lstHue_SelectedIndexChanged(object sender, EventArgs e)
		{
			mHue = (Hue)_lstHue.SelectedItem;
			if (mHue == null)
			{
				return;
			}

			_StatusBar.Text = Conversions.ToString(mHue.Index) + ": " + mHue.Name;
		}

		public delegate void ValueChangedEventHandler(Hue Hue);
	}
}
