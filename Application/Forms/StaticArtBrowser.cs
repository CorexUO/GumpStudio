// Decompiled with JetBrains decompiler
// Type: GumpStudio.NewStaticArtBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using GumpStudio.Properties;

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

using Ultima;
// ReSharper disable RedundantNameQualifier

namespace GumpStudio
{
	public class StaticArtBrowser : Form
	{
		private bool SearchSomething;
		private Button _cmdCache;
		private Button _cmdSearch;
		private Label _lblID;
		private Label _lblName;
		private Label _lblSize;
		private Label _lblWait;
		private PictureBox _picCanvas;
		private ToolTip _ToolTip1;
		private TextBox _txtSearch;
		private VScrollBar _vsbScroller;
		protected static Bitmap BlankCache;
		protected bool BuildingCache;
		protected static GumpCacheEntry[] Cache;
		private IContainer components;
		protected Size DisplaySize;
		protected Point HoverPos;
		protected int NumX;
		protected int NumY;
		protected static Bitmap[] RowCache;
		protected int SelectedIndex;
		protected int StartIndex;

		public int ItemID
		{
			get => Cache[SelectedIndex].ID;
			set
			{
				if (Cache == null) {
					return;
				}

				for (var index = 0; index < Cache.Length; index++) {
					if (Cache[index].ID != value) {
						continue;
					}

					SelectedIndex = index;
					_lblName.Text = Resources.Name__ + TileData.ItemTable[ItemID].Name;
					_lblSize.Text = Resources.Size__ + Conversions.ToString(Art.GetStatic(ItemID).Width) + " x " + Conversions.ToString(Art.GetStatic(ItemID).Height);
				}
			}
		}

		internal virtual ToolTip ToolTip1
		{

			get => _ToolTip1;
			[DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
			set => _ToolTip1 = value;
		}

		internal virtual TextBox txtSearch
		{

			get => _txtSearch;
			[DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
			set => _txtSearch = value;
		}

		internal virtual VScrollBar vsbScroller
		{

			get => _vsbScroller;
			[DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
			set
			{
				var scrollEventHandler = new ScrollEventHandler(vsbScroller_Scroll);
				if (_vsbScroller != null) {
					_vsbScroller.Scroll -= scrollEventHandler;
				}

				_vsbScroller = value;
				if (_vsbScroller == null) {
					return;
				}

				_vsbScroller.Scroll += scrollEventHandler;
			}
		}

		public StaticArtBrowser()
		{
			Load += new EventHandler(NewStaticArtBrowser_Load);
			Resize += new EventHandler(NewStaticArtBrowser_Resize);
			DisplaySize = new Size(45, 45);
			HoverPos = new Point(-1, -1);
			SelectedIndex = 0;
			BuildingCache = false;
			InitializeComponent();
		}

		protected void BuildCache()
		{
			if (BuildingCache) {
				return;
			}

			BuildingCache = true;

			_lblWait.Text = Resources.Generating_static_art_cache;

			Show();

			FileStream fileStream = null;

			try {
				Cache = null;

				_lblWait.Visible = true;

				Application.DoEvents();

				var upperBound = TileData.ItemTable.GetUpperBound(0);

				for (var index = 0; index <= upperBound; ++index) {
					if (index / 128.0 == Conversion.Int(index / 128.0)) {
						_lblWait.Text = Resources.Generating_static_art_cache + Strings.Format(index / (double)TileData.ItemTable.GetUpperBound(0) * 100.0, "Fixed") + "%";
						Application.DoEvents();
					}

					var bitmap = Art.GetStatic(index);

					if (bitmap == null) {
						continue;
					}

					Cache = Cache != null ? (GumpCacheEntry[])Utils.CopyArray(Cache, new GumpCacheEntry[Cache.Length + 1]) : new GumpCacheEntry[1];
					Cache[Cache.Length - 1] = new GumpCacheEntry { ID = index, Size = bitmap.Size, Name = TileData.ItemTable[index].Name };
				}

				fileStream = new FileStream(Application.StartupPath + "/StaticArt.cache", FileMode.Create);
				new BinaryFormatter().Serialize(fileStream, Cache);
			}
			catch (Exception ex) {
				ProjectData.SetProjectError(ex);
				MessageBox.Show(Resources.Error_creating_cache + ex.Message);
				ProjectData.ClearProjectError();
			}
			finally {
				fileStream?.Close();
				_lblWait.Visible = false;
				Application.DoEvents();
				BuildingCache = false;
			}
		}

		private void cmdCache_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show(Resources.Rebuild_longtime, Resources.Question, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (result != DialogResult.OK) {
				return;
			}

			BuildCache();
			RowCache = new Bitmap[(int)Math.Round(Cache.Length / (double)NumX) + 1 + 1];
			ItemID = 0;
			_picCanvas.Invalidate();
		}

		private void cmdSearch_Click(object sender, EventArgs e)
		{
			var index1 = -1;
			var index2 = SelectedIndex == -1 ? 0 : SelectedIndex;
			while (index1 == -1 & index2 < Cache.Length - 1) {
				++index2;
				if (Strings.InStr(Cache[index2].Name, txtSearch.Text, CompareMethod.Text) > 0) {
					index1 = index2;
				}
			}
			if (index1 != -1) {
				ItemID = Cache[index1].ID;
			}

			if (index1 == -1 & index2 > 0 && !SearchSomething) {
				SelectedIndex = 0;
				SearchSomething = true;
				cmdSearch_Click(RuntimeHelpers.GetObjectValue(sender), e);
			}
			vsbScroller.Value = SelectedIndex / NumX;
			vsbScroller_Scroll(vsbScroller, null);
			SearchSomething = false;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		protected void DrawGrid(Graphics g)
		{
			var numX = NumX;
			for (var index = 0; index <= numX; ++index) {
				g.DrawLine(Pens.Black, index * DisplaySize.Width, 0, index * DisplaySize.Width, (NumY + 1) * DisplaySize.Height);
			}

			var num = NumY + 1;
			for (var index = 0; index <= num; ++index) {
				g.DrawLine(Pens.Black, 0, index * DisplaySize.Height, NumX * DisplaySize.Width, index * DisplaySize.Height);
			}
		}

		protected void DrawHover(Graphics g)
		{
			var x = HoverPos.X;
			var y = HoverPos.Y;
			var index = StartIndex + x + y * NumX;
			if (index >= Cache.Length) {
				return;
			}

			var id = Cache[index].ID;
			var bitmap = Art.GetStatic(id);
			var point = new Point {
				X = (int)Math.Round(x * DisplaySize.Width + DisplaySize.Width / 2.0) - (int)Math.Round(bitmap.Width / 2.0) - 3
			};
			if (point.X < 0) {
				point.X = 0;
			}

			if (point.X + bitmap.Width > _picCanvas.Width) {
				point.X = _picCanvas.Width - bitmap.Width - 3;
			}

			point.Y = (int)Math.Round(y * DisplaySize.Height + DisplaySize.Height / 2.0) - (int)Math.Round(bitmap.Height / 2.0) - 3;
			if (point.Y < 0) {
				point.Y = 0;
			}

			if (point.Y + bitmap.Height > _picCanvas.Height) {
				point.Y = _picCanvas.Height - bitmap.Height - 3;
			}

			var rect = new Rectangle(point, bitmap.Size);
			var solidBrush = new SolidBrush(Color.FromArgb(SByte.MaxValue, Color.Black));
			g.FillRectangle(solidBrush, point.X + 5, point.Y + 5, rect.Width, rect.Height);
			g.FillRectangle(Brushes.White, rect);
			g.DrawRectangle(Pens.Black, rect);
			g.DrawImage(bitmap, point);
			_lblName.Text = Resources.Name__ + TileData.ItemTable[id].Name;
			_lblSize.Text = Resources.Size__ + Conversions.ToString(bitmap.Width) + " x " + Conversions.ToString(bitmap.Height);
			_lblID.Text = @"ID: " + Conversions.ToString(id) + @" - hex:" + Conversion.Hex(id);
			bitmap.Dispose();
			solidBrush.Dispose();
		}

		protected Bitmap GetRowImage(int Row)
		{
			if (Row >= RowCache.Length) {
				if (BlankCache != null) {
					return BlankCache;
				}

				var bitmap = new Bitmap(NumX * DisplaySize.Width, DisplaySize.Height, PixelFormat.Format16bppRgb565);
				var graphics = Graphics.FromImage(bitmap);
				graphics.Clear(Color.Gray);
				graphics.Dispose();
				BlankCache = bitmap;
				return bitmap;
			}
			if (RowCache[Row] != null) {
				return RowCache[Row];
			}

			var bitmap1 = new Bitmap(NumX * DisplaySize.Width, DisplaySize.Height, PixelFormat.Format16bppRgb565);
			var graphics1 = Graphics.FromImage(bitmap1);
			graphics1.Clear(Color.Gray);
			var clip = graphics1.Clip;
			var rect = new Rectangle(0, 0, NumX * DisplaySize.Width, NumY * DisplaySize.Height);
			var region1 = new Region(rect);
			graphics1.Clip = region1;
			var num = NumX - 1;
			for (var index1 = 0; index1 <= num; ++index1) {
				var index2 = Row * NumX + index1;
				if (index2 < Cache.Length) {
					var bitmap2 = Art.GetStatic(Cache[index2].ID);
					rect = new Rectangle(index1 * DisplaySize.Width, 0, DisplaySize.Width, DisplaySize.Height);
					var region2 = new Region(rect);
					graphics1.Clip = region2;
					graphics1.FillRectangle(Brushes.White, index1 * DisplaySize.Width, 0, DisplaySize.Width, DisplaySize.Height);
					graphics1.DrawImage(bitmap2, index1 * DisplaySize.Width + 1, 0);
					bitmap2.Dispose();
					region2.Dispose();
				}
			}
			graphics1.Clip = clip;
			graphics1.Dispose();
			RowCache[Row] = bitmap1;
			return bitmap1;
		}

		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			_picCanvas = new System.Windows.Forms.PictureBox();
			_vsbScroller = new System.Windows.Forms.VScrollBar();
			_txtSearch = new System.Windows.Forms.TextBox();
			_cmdSearch = new System.Windows.Forms.Button();
			_lblName = new System.Windows.Forms.Label();
			_lblSize = new System.Windows.Forms.Label();
			_cmdCache = new System.Windows.Forms.Button();
			_lblWait = new System.Windows.Forms.Label();
			_ToolTip1 = new System.Windows.Forms.ToolTip(components);
			_lblID = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(_picCanvas)).BeginInit();
			SuspendLayout();
			// 
			// _picCanvas
			// 
			_picCanvas.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left);
			_picCanvas.Location = new System.Drawing.Point(0, 0);
			_picCanvas.Name = "_picCanvas";
			_picCanvas.Size = new System.Drawing.Size(488, 396);
			_picCanvas.TabIndex = 0;
			_picCanvas.TabStop = false;
			_picCanvas.Paint += new System.Windows.Forms.PaintEventHandler(picCanvas_Paint);
			_picCanvas.DoubleClick += new System.EventHandler(picCanvas_DoubleClick);
			_picCanvas.MouseLeave += new System.EventHandler(picCanvas_MouseLeave);
			_picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(picCanvas_MouseMove);
			_picCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(picCanvas_MouseUp);
			// 
			// _vsbScroller
			// 
			_vsbScroller.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left);
			_vsbScroller.Location = new System.Drawing.Point(488, 0);
			_vsbScroller.Name = "_vsbScroller";
			_vsbScroller.Size = new System.Drawing.Size(17, 396);
			_vsbScroller.TabIndex = 3;
			_vsbScroller.Scroll += new System.Windows.Forms.ScrollEventHandler(vsbScroller_Scroll);
			// 
			// _txtSearch
			// 
			_txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			_txtSearch.Location = new System.Drawing.Point(56, 403);
			_txtSearch.Name = "_txtSearch";
			_txtSearch.Size = new System.Drawing.Size(100, 20);
			_txtSearch.TabIndex = 4;
			// 
			// _cmdSearch
			// 
			_cmdSearch.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_cmdSearch.Location = new System.Drawing.Point(160, 403);
			_cmdSearch.Name = "_cmdSearch";
			_cmdSearch.Size = new System.Drawing.Size(50, 20);
			_cmdSearch.TabIndex = 5;
			_cmdSearch.Text = "Search";
			_cmdSearch.Click += new System.EventHandler(cmdSearch_Click);
			// 
			// _lblName
			// 
			_lblName.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_lblName.AutoSize = true;
			_lblName.Location = new System.Drawing.Point(216, 405);
			_lblName.Name = "_lblName";
			_lblName.Size = new System.Drawing.Size(38, 13);
			_lblName.TabIndex = 6;
			_lblName.Text = "Name:";
			// 
			// _lblSize
			// 
			_lblSize.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_lblSize.AutoSize = true;
			_lblSize.Location = new System.Drawing.Point(408, 405);
			_lblSize.Name = "_lblSize";
			_lblSize.Size = new System.Drawing.Size(30, 13);
			_lblSize.TabIndex = 7;
			_lblSize.Text = "Size:";
			// 
			// _cmdCache
			// 
			_cmdCache.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_cmdCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_cmdCache.Location = new System.Drawing.Point(0, 402);
			_cmdCache.Name = "_cmdCache";
			_cmdCache.Size = new System.Drawing.Size(50, 23);
			_cmdCache.TabIndex = 9;
			_cmdCache.Text = "Cache";
			_ToolTip1.SetToolTip(_cmdCache, "Rebuild Art Cache");
			_cmdCache.Click += new System.EventHandler(cmdCache_Click);
			// 
			// _lblWait
			// 
			_lblWait.BackColor = System.Drawing.Color.Transparent;
			_lblWait.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			_lblWait.Location = new System.Drawing.Point(164, 159);
			_lblWait.Name = "_lblWait";
			_lblWait.Size = new System.Drawing.Size(184, 104);
			_lblWait.TabIndex = 10;
			_lblWait.Text = "Please Wait, Generating Static Art Cache...";
			_lblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			_lblWait.Visible = false;
			// 
			// _lblID
			// 
			_lblID.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_lblID.AutoSize = true;
			_lblID.Location = new System.Drawing.Point(216, 429);
			_lblID.Name = "_lblID";
			_lblID.Size = new System.Drawing.Size(21, 13);
			_lblID.TabIndex = 11;
			_lblID.Text = "ID:";
			// 
			// StaticArtBrowser
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new System.Drawing.Size(505, 451);
			Controls.Add(_lblID);
			Controls.Add(_lblWait);
			Controls.Add(_cmdCache);
			Controls.Add(_lblSize);
			Controls.Add(_lblName);
			Controls.Add(_cmdSearch);
			Controls.Add(_txtSearch);
			Controls.Add(_vsbScroller);
			Controls.Add(_picCanvas);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			MaximumSize = new System.Drawing.Size(521, 3000);
			MinimumSize = new System.Drawing.Size(521, 200);
			Name = "StaticArtBrowser";
			SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			Text = "Static Art Browser";
			Load += new System.EventHandler(NewStaticArtBrowser_Load);
			Resize += new System.EventHandler(NewStaticArtBrowser_Resize);
			((System.ComponentModel.ISupportInitialize)(_picCanvas)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		private void NewStaticArtBrowser_Load(object sender, EventArgs e)
		{
			if (Cache == null) {
				FileStream fileStream = null;
				if (!File.Exists(Application.StartupPath + "/StaticArt.cache")) {
					BuildCache();
				}
				else {
					try {
						fileStream = new FileStream(Application.StartupPath + "/StaticArt.cache", FileMode.Open);
						Cache = (GumpCacheEntry[])new BinaryFormatter().Deserialize(fileStream);
					}
					catch (Exception ex) {
						ProjectData.SetProjectError(ex);
						//int num = (int) Interaction.MsgBox( (object) ( "Error reading cache file:\r\n" + ex.Message ), MsgBoxStyle.OkOnly, (object) null );
						MessageBox.Show("Error reading cache file:\r\n" + ex.Message);
						ProjectData.ClearProjectError();
					}
					finally {
						fileStream?.Close();
					}
				}
			}
			_picCanvas.Width = ClientSize.Width - vsbScroller.Width;
			Show();
			vsbScroller.Maximum = (int)Math.Round(Cache.Length / (double)NumX) + 1;
			vsbScroller.LargeChange = NumY - 1;
			if (RowCache == null) {
				RowCache = new Bitmap[(int)Math.Round(Cache.Length / (double)NumX) + 1 + 1];
			}

			vsbScroller.Value = SelectedIndex / NumX;
			vsbScroller_Scroll(vsbScroller, null);
			_lblName.Text = Resources.Name__ + TileData.ItemTable[Cache[SelectedIndex].ID].Name;
			_lblSize.Text = Resources.Size__ + Conversions.ToString(Cache[SelectedIndex].Size.Width) + " x " + Conversions.ToString(Cache[SelectedIndex].Size.Height);
		}

		private void NewStaticArtBrowser_Resize(object sender, EventArgs e)
		{
			var num1 = 11;
			var num2 = _picCanvas.Height / DisplaySize.Height;
			if (!(num1 != NumX | num2 != NumY)) {
				return;
			}

			NumX = num1;
			NumY = num2;
			if (Cache == null) {
				return;
			}

			vsbScroller.Maximum = (int)Math.Round(Cache.Length / (double)NumX) + 1;
			vsbScroller.LargeChange = NumY - 1;
			_picCanvas.Invalidate();
		}

		private void picCanvas_DoubleClick(object sender, EventArgs e)
		{
			if (BuildingCache) {
				return;
			}

			DialogResult = DialogResult.OK;
		}

		private void picCanvas_MouseLeave(object sender, EventArgs e)
		{
			HoverPos = new Point(-1, -1);
			_picCanvas.Invalidate();
			_lblName.Text = Resources.Name__ + TileData.ItemTable[Cache[SelectedIndex].ID].Name;
			_lblSize.Text = Resources.Size__ + Conversions.ToString(Cache[SelectedIndex].Size.Width) + " x " + Conversions.ToString(Cache[SelectedIndex].Size.Height);
			_lblID.Text = "ID: " + Conversions.ToString(Cache[SelectedIndex].ID) + "(0x" + Conversion.Hex(Cache[SelectedIndex].ID) + ")";
		}

		private void picCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			var point = new Point(e.X / DisplaySize.Width, e.Y / DisplaySize.Height);
			if (point.X >= 11 || !(point.X != HoverPos.X | point.Y != HoverPos.Y)) {
				return;
			}

			HoverPos = point;
			_picCanvas.Invalidate();
		}

		private void picCanvas_MouseUp(object sender, MouseEventArgs e)
		{
			var index = e.X / DisplaySize.Width + e.Y / DisplaySize.Height * NumX + StartIndex;
			if (index >= Cache.Length) {
				return;
			}

			ItemID = Cache[index].ID;
			_picCanvas.Invalidate();
		}

		private void picCanvas_Paint(object sender, PaintEventArgs e)
		{
			try {
				Render(e.Graphics);
				if (HoverPos.Equals(new Point(-1, -1))) {
					return;
				}

				DrawHover(e.Graphics);
			}
			catch (Exception) {
			}
		}

		public void Render(Graphics g)
		{
			if (Cache == null | RowCache == null) {
				return;
			}

			var rect = new Rectangle();
			g.Clear(Color.Gray);
			DrawGrid(g);
			var clip = g.Clip;
			var num = StartIndex / NumX;
			var flag = false;
			var numY = NumY;
			for (var index = 0; index <= numY; ++index) {
				g.DrawImage(GetRowImage(index + num), 0, index * DisplaySize.Height);
				if ((flag || index + num != SelectedIndex / NumX ? 0 : 1) != 0) {
					flag = true;
					rect = new Rectangle(SelectedIndex % NumX * DisplaySize.Width, index * DisplaySize.Height, DisplaySize.Width, DisplaySize.Height);
				}
			}
			DrawGrid(g);
			if (flag) {
				var region = new Region(rect);
				rect.Inflate(5, 5);
				var solidBrush = new SolidBrush(Color.FromArgb(SByte.MaxValue, Color.Blue));
				g.FillRectangle(solidBrush, rect);
				g.DrawRectangle(Pens.Blue, rect);
				solidBrush.Dispose();
				rect.Inflate(-5, -5);
				g.Clip = region;
				g.DrawImage(Art.GetStatic(Cache[SelectedIndex].ID), rect.Location);
				g.Clip = clip;
				region.Dispose();
			}
			g.Clip = clip;
		}

		private void vsbScroller_Scroll(object sender, ScrollEventArgs e)
		{
			StartIndex = vsbScroller.Value * NumX;
			_picCanvas.Invalidate();
		}
	}
}
