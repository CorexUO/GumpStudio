// Decompiled with JetBrains decompiler
// Type: GumpStudio.FontBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using GumpStudio.Elements;

using UOFont;

namespace GumpStudio
{
	public class FontBrowser : UserControl
	{
		private bool fntunicode = true;
		private static readonly List<WeakReference> __ENCList = new List<WeakReference>();
		private const int fntshift = 0;
		[AccessedThroughProperty("lstFont")]
		private ListBox _lstFont;
		private readonly IContainer components;
		public int Value;

		public event FontBrowser.ValueChangedEventHandler ValueChanged;

		public FontBrowser()
		{
			Load += new EventHandler(FontBrowser_Load);
			lock (FontBrowser.__ENCList)
			{
				FontBrowser.__ENCList.Add(new WeakReference(this));
			}

			InitializeComponent();
		}

		public FontBrowser(int Value)
		  : this()
		{
			this.Value = Value;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void FontBrowser_Load(object sender, EventArgs e)
		{
			fntunicode = true;
			var arrayList = GlobalObjects.DesignerForm == null || GlobalObjects.DesignerForm.ElementStack == null ? null : GlobalObjects.DesignerForm.ElementStack.GetSelectedElements();
			if (arrayList != null)
			{
				foreach (var obj in arrayList)
				{
					if (obj is LabelElement && !((LabelElement)obj).Unicode)
					{
						fntunicode = false;
						break;
					}
				}
			}
			for (var index = 0; index < (fntunicode ? 13 : 10); ++index)
			{
				if (index >= 0)
				{
					_lstFont.Items.Add(index);
				}
			}
			_lstFont.SelectedIndex = Value;
		}


		private void InitializeComponent()
		{
			_lstFont = new System.Windows.Forms.ListBox();
			SuspendLayout();
			// 
			// _lstFont
			// 
			_lstFont.Dock = System.Windows.Forms.DockStyle.Fill;
			_lstFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
			_lstFont.IntegralHeight = false;
			_lstFont.Location = new System.Drawing.Point(0, 0);
			_lstFont.Name = "_lstFont";
			_lstFont.Size = new System.Drawing.Size(326, 282);
			_lstFont.TabIndex = 0;
			_lstFont.DrawItem += new System.Windows.Forms.DrawItemEventHandler(lstFont_DrawItem);
			_lstFont.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(lstFont_MeasureItem);
			_lstFont.DoubleClick += new System.EventHandler(lstFont_DoubleClick);
			// 
			// FontBrowser
			// 
			Controls.Add(_lstFont);
			Name = "FontBrowser";
			Size = new System.Drawing.Size(326, 282);
			Load += new System.EventHandler(FontBrowser_Load);
			ResumeLayout(false);

		}

		private void lstFont_DoubleClick(object sender, EventArgs e)
		{
			Value = _lstFont.SelectedIndex;
			var valueChanged = ValueChanged;
			valueChanged?.Invoke(Value);
		}

		private void lstFont_DrawItem(object sender, DrawItemEventArgs e)
		{
			if ((e.State & DrawItemState.Selected) > DrawItemState.None)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.Window, e.Bounds);
			}

			if (e.Index > (fntunicode ? 12 : 9))
			{
				return;
			}

			var bitmap = fntunicode ? UnicodeFonts.GetStringImage(e.Index, "ABCabc123!@#$АБВабв") : Fonts.GetStringImage(e.Index, "ABCabc123 */ АБВабв");
			e.Graphics.DrawImage(bitmap, e.Bounds.Location);
			bitmap.Dispose();
		}

		private void lstFont_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			if (e.Index > (fntunicode ? 12 : 9))
			{
				e.ItemHeight = 0;
			}
			else
			{
				var bitmap = fntunicode ? UnicodeFonts.GetStringImage(e.Index, "ABCabc123!@#$АБВабв") : Fonts.GetStringImage(e.Index, "ABCabc123 */ АБВабв");
				e.ItemHeight = bitmap.Height;
				bitmap.Dispose();
			}
		}

		public delegate void ValueChangedEventHandler(int Value);
	}
}
