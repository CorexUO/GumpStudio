// Decompiled with JetBrains decompiler
// Type: GumpStudio.ClilocBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using GumpStudio.Properties;

using Ultima;

namespace GumpStudio.Forms
{
	public class ClilocBrowser : Form
	{
		protected static ListBox ClilocCache;
		protected int mClilocID;
		private Button Cancel_Button;
		private ComboBox cboLanguage;
		private readonly IContainer components;
		private Label Label1;
		private ListBox lstCliloc;
		private Button OK_Button;
		private TableLayoutPanel TableLayoutPanel1;

		public int ClilocID
		{
			get => mClilocID;
			set => mClilocID = value;
		}


		public ClilocBrowser()
		{
			Load += ClilocBrowser_Load;
			InitializeComponent();
		}

		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void ClilocBrowser_Load(object sender, EventArgs e)
		{
			foreach (var file in Directory.GetFiles(XMLSettings.CurrentOptions.ClientPath, "Cliloc.*")) {
				cboLanguage.Items.Add(Path.GetExtension(file).Substring(1));
			}

			if (ClilocCache == null) {
				lstCliloc.SuspendLayout();

				foreach (var entry in new StringList("enu").Entries) {
					lstCliloc.Items.Add(entry);
				}

				lstCliloc.ResumeLayout();
				ClilocCache = lstCliloc;
			}
			else {
				lstCliloc = ClilocCache;
			}
		}


		protected override void Dispose(bool disposing)
		{
			try {
				if ((!disposing || components == null ? 0 : 1) == 0) {
					return;
				}

				components?.Dispose();
			}
			finally {
				base.Dispose(disposing);
			}
		}

		private void InitializeComponent()
		{
			TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			OK_Button = new System.Windows.Forms.Button();
			Cancel_Button = new System.Windows.Forms.Button();
			lstCliloc = new System.Windows.Forms.ListBox();
			Label1 = new System.Windows.Forms.Label();
			cboLanguage = new System.Windows.Forms.ComboBox();
			TableLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// TableLayoutPanel1
			// 
			TableLayoutPanel1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			TableLayoutPanel1.ColumnCount = 2;
			TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			TableLayoutPanel1.Controls.Add(OK_Button, 0, 0);
			TableLayoutPanel1.Controls.Add(Cancel_Button, 1, 0);
			TableLayoutPanel1.Location = new System.Drawing.Point(451, 392);
			TableLayoutPanel1.Name = "TableLayoutPanel1";
			TableLayoutPanel1.RowCount = 1;
			TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
			TableLayoutPanel1.TabIndex = 0;
			// 
			// OK_Button
			// 
			OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			OK_Button.Location = new System.Drawing.Point(3, 3);
			OK_Button.Name = "OK_Button";
			OK_Button.Size = new System.Drawing.Size(67, 23);
			OK_Button.TabIndex = 0;
			OK_Button.Text = Resources.OK;
			// 
			// Cancel_Button
			// 
			Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			Cancel_Button.Location = new System.Drawing.Point(76, 3);
			Cancel_Button.Name = "Cancel_Button";
			Cancel_Button.Size = new System.Drawing.Size(67, 23);
			Cancel_Button.TabIndex = 1;
			Cancel_Button.Text = Resources.Cancel;
			// 
			// lstCliloc
			// 
			lstCliloc.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			lstCliloc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
			lstCliloc.FormattingEnabled = true;
			lstCliloc.Location = new System.Drawing.Point(12, 12);
			lstCliloc.Name = "lstCliloc";
			lstCliloc.Size = new System.Drawing.Size(585, 368);
			lstCliloc.TabIndex = 1;
			// 
			// Label1
			// 
			Label1.AutoSize = true;
			Label1.Location = new System.Drawing.Point(12, 400);
			Label1.Name = "Label1";
			Label1.Size = new System.Drawing.Size(55, 13);
			Label1.TabIndex = 2;
			Label1.Text = Resources.Language;
			// 
			// cboLanguage
			// 
			cboLanguage.FormattingEnabled = true;
			cboLanguage.Location = new System.Drawing.Point(73, 397);
			cboLanguage.Name = "cboLanguage";
			cboLanguage.Size = new System.Drawing.Size(121, 21);
			cboLanguage.TabIndex = 3;
			// 
			// ClilocBrowser
			// 
			AcceptButton = OK_Button;
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			CancelButton = Cancel_Button;
			ClientSize = new System.Drawing.Size(609, 433);
			Controls.Add(cboLanguage);
			Controls.Add(Label1);
			Controls.Add(lstCliloc);
			Controls.Add(TableLayoutPanel1);
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "ClilocBrowser";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = @"Cliloc Browser";
			TableLayoutPanel1.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();

		}

		private void lstCliloc_DrawItem(object sender, DrawItemEventArgs e)
		{
			var stringEntry = (StringEntry)lstCliloc.Items[e.Index];
			e.DrawBackground();
			e.Graphics.DrawString(stringEntry.Number.ToString(), lstCliloc.Font, Brushes.Black, e.Bounds.X, e.Bounds.Top);
			e.Graphics.DrawString(stringEntry.Text, lstCliloc.Font, Brushes.Black, e.Bounds.X + 100, e.Bounds.Top);
		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}