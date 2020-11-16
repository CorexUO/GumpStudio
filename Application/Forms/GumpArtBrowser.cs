// Decompiled with JetBrains decompiler
// Type: GumpStudio.GumpArtBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Ultima;
// ReSharper disable ArrangeThisQualifier
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantDelegateCreation

namespace GumpStudio
{
	public class GumpArtBrowser : Form
	{
		private Button _cmdCache;
		private Button _cmdOK;
		private Label _lblSize;
		private Label _lblWait;
		private ListBox _lstGump;
		private Panel _Panel1;
		private PictureBox _picFullSize;
		private ToolTip _ToolTip1;
		protected static GumpCacheEntry[] Cache;
		private IContainer components;
		public int GumpID;

		public GumpArtBrowser()
		{
			Load += GumpArtBrowser_Load;
			InitializeComponent();
		}

		protected void BuildCache()
		{
			_lblWait.Text = @"Please Wait, Generating Art Cache...";
			Show();
			Cache = null;
			_lstGump.Items.Clear();
			_lblWait.Visible = true;
			Application.DoEvents();
			var index = 0;
			int maxValue = UInt16.MaxValue;
			try
			{
				do
				{
					_lblWait.Text = $@"Please Wait, Generating Art Cache...  {(int)(100 * index / (double)maxValue)}%";
					Application.DoEvents();
					Bitmap gump;
					try
					{
						gump = Gumps.GetGump(index);
					}
					catch (Exception)
					{
						++index;

						return;
					}
					if (gump != null)
					{
						if (Cache != null)
						{
							Array.Resize(ref Cache, Cache.Length + 1);
						}
						else
						{
							Cache = new GumpCacheEntry[1];
						}

						Cache[Cache.Length - 1] = new GumpCacheEntry { ID = index, Size = gump.Size };
						gump.Dispose();
					}
					++index;
				}
				while (index <= maxValue);

				using (var fileStream = new FileStream(Application.StartupPath + "/GumpArt.cache", FileMode.Create))
				{
					new BinaryFormatter().Serialize(fileStream, Cache ?? throw new InvalidOperationException());
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(@"Error creating cache file:" + ex.Message);
			}
			finally
			{
				_lblWait.Visible = false;
				Application.DoEvents();
			}
		}

		private void cmdCache_Click(object sender, EventArgs e)
		{
			_cmdOK.Enabled = false;

			var result = MessageBox.Show(@"Rebuilding the cache may take several minutes depending on the speed of your computer.\r\nAre you sure you want to continue?", @"Rebuild Cache", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

			if (result == DialogResult.OK)
			{
				BuildCache();
				PopulateListbox();
			}
			_cmdOK.Enabled = true;
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			GumpID = Convert.ToInt32(_lstGump.SelectedItem);
			DialogResult = DialogResult.OK;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void GumpArtBrowser_Load(object sender, EventArgs e)
		{
			if (Cache == null)
			{
				FileStream fileStream = null;
				if (!File.Exists(Application.StartupPath + "/GumpArt.cache"))
				{
					BuildCache();
				}
				else
				{
					try
					{
						fileStream = new FileStream(Application.StartupPath + "/GumpArt.cache", FileMode.Open);
						Cache = (GumpCacheEntry[])new BinaryFormatter().Deserialize(fileStream);
					}
					catch (Exception ex)
					{
						MessageBox.Show(@"Error Reading cache file:\r\n" + ex.Message);
					}
					finally
					{
						fileStream?.Close();
					}
				}
			}
			PopulateListbox();
		}


		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			_lstGump = new System.Windows.Forms.ListBox();
			_Panel1 = new System.Windows.Forms.Panel();
			_picFullSize = new System.Windows.Forms.PictureBox();
			_lblSize = new System.Windows.Forms.Label();
			_lblWait = new System.Windows.Forms.Label();
			_cmdCache = new System.Windows.Forms.Button();
			_ToolTip1 = new System.Windows.Forms.ToolTip(components);
			_cmdOK = new System.Windows.Forms.Button();
			_Panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(_picFullSize)).BeginInit();
			SuspendLayout();
			// 
			// _lstGump
			// 
			_lstGump.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
								   | System.Windows.Forms.AnchorStyles.Left;
			_lstGump.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			_lstGump.IntegralHeight = false;
			_lstGump.Location = new System.Drawing.Point(8, 8);
			_lstGump.Name = "_lstGump";
			_lstGump.Size = new System.Drawing.Size(184, 320);
			_lstGump.TabIndex = 0;
			_lstGump.DrawItem += new System.Windows.Forms.DrawItemEventHandler(lstGump_DrawItem);
			_lstGump.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(lstGump_MeasureItem);
			_lstGump.SelectedIndexChanged += new System.EventHandler(lstGump_SelectedIndexChanged);
			_lstGump.DoubleClick += new System.EventHandler(lstGump_DoubleClick);
			// 
			// _Panel1
			// 
			_Panel1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
								   | System.Windows.Forms.AnchorStyles.Left)
								  | System.Windows.Forms.AnchorStyles.Right;
			_Panel1.AutoScroll = true;
			_Panel1.BackColor = System.Drawing.Color.Black;
			_Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_Panel1.Controls.Add(_picFullSize);
			_Panel1.Location = new System.Drawing.Point(200, 8);
			_Panel1.Name = "_Panel1";
			_Panel1.Size = new System.Drawing.Size(312, 288);
			_Panel1.TabIndex = 1;
			// 
			// _picFullSize
			// 
			_picFullSize.Location = new System.Drawing.Point(0, 0);
			_picFullSize.Name = "_picFullSize";
			_picFullSize.Size = new System.Drawing.Size(100, 50);
			_picFullSize.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			_picFullSize.TabIndex = 0;
			_picFullSize.TabStop = false;
			// 
			// _lblSize
			// 
			_lblSize.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
			_lblSize.AutoSize = true;
			_lblSize.Location = new System.Drawing.Point(200, 307);
			_lblSize.Name = "_lblSize";
			_lblSize.Size = new System.Drawing.Size(0, 13);
			_lblSize.TabIndex = 2;
			// 
			// _lblWait
			// 
			_lblWait.BackColor = System.Drawing.Color.Transparent;
			_lblWait.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_lblWait.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
			_lblWait.Location = new System.Drawing.Point(168, 131);
			_lblWait.Name = "_lblWait";
			_lblWait.Size = new System.Drawing.Size(184, 72);
			_lblWait.TabIndex = 1;
			_lblWait.Text = @"Please Wait, Generating Art Cache...";
			_lblWait.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			_lblWait.Visible = false;
			// 
			// _cmdCache
			// 
			_cmdCache.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_cmdCache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			_cmdCache.Location = new System.Drawing.Point(480, 304);
			_cmdCache.Name = "_cmdCache";
			_cmdCache.Size = new System.Drawing.Size(32, 23);
			_cmdCache.TabIndex = 3;
			_ToolTip1.SetToolTip(_cmdCache, "Rebuild Cache");
			_cmdCache.Click += new System.EventHandler(cmdCache_Click);
			// 
			// _cmdOK
			// 
			_cmdOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
			_cmdOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			_cmdOK.Location = new System.Drawing.Point(400, 304);
			_cmdOK.Name = "_cmdOK";
			_cmdOK.Size = new System.Drawing.Size(75, 23);
			_cmdOK.TabIndex = 4;
			_cmdOK.Text = @"OK";
			_cmdOK.Click += new System.EventHandler(cmdOK_Click);
			// 
			// GumpArtBrowser
			// 
			AcceptButton = _cmdOK;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new System.Drawing.Size(520, 334);
			Controls.Add(_cmdOK);
			Controls.Add(_cmdCache);
			Controls.Add(_lblWait);
			Controls.Add(_lblSize);
			Controls.Add(_Panel1);
			Controls.Add(_lstGump);
			Name = "GumpArtBrowser";
			Text = @"GumpID Browser";
			_Panel1.ResumeLayout(false);
			_Panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(_picFullSize)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		private void lstGump_DoubleClick(object sender, EventArgs e)
		{
			GumpID = Convert.ToInt32(_lstGump.SelectedItem);
			DialogResult = DialogResult.OK;
		}

		private void lstGump_DrawItem(object sender, DrawItemEventArgs e)
		{
			try
			{
				if (e.Index == -1)
				{
					return;
				}

				var size1 = new Size();
				var graphics = e.Graphics;
				var gump = Gumps.GetGump(Cache[e.Index].ID);
				var size2 = Cache[e.Index].Size;
				size1.Width = size2.Width <= 100 ? size2.Width : 100;
				size1.Height = size2.Height <= 100 ? size2.Height : 100;
				var rect = new Rectangle(e.Bounds.Location, size1);
				rect.Offset(45, 3);
				graphics.FillRectangle((e.State & DrawItemState.Selected) > DrawItemState.None ? SystemBrushes.Highlight : SystemBrushes.Window, e.Bounds);
				graphics.DrawString("0x" + Cache[e.Index].ID.ToString("X"), Font, SystemBrushes.WindowText, e.Bounds.X, e.Bounds.Y);
				graphics.DrawImage(gump, rect);
				gump.Dispose();
			}
			catch (Exception ex)
			{
				MessageBox.Show(@"There was an error rendering the gump art, try rebuilding the cache.\r\n\r\n" + ex.Message);
			}
		}

		private void lstGump_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			var height = Cache[e.Index].Size.Height;
			var num = height <= 100 ? (height >= 15 ? height : 15) : 100;
			e.ItemHeight = num + 5;
		}

		private void lstGump_SelectedIndexChanged(object sender, EventArgs e)
		{
			_picFullSize.Image?.Dispose();
			_picFullSize.Image = Gumps.GetGump(Convert.ToInt32(_lstGump.SelectedItem));
			_lblSize.Text = @"Width: " + Convert.ToString(_picFullSize.Image.Width) + @"   Height: " + Convert.ToString(_picFullSize.Image.Height);
		}

		private void PopulateListbox()
		{
			_lstGump.Items.Clear();
			foreach (var gumpCacheEntry in Cache)
			{
				_lstGump.Items.Add(gumpCacheEntry.ID);
			}

			_lstGump.SelectedItem = GumpID;
		}
	}
}
