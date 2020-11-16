// Decompiled with JetBrains decompiler
// Type: GumpStudio.frmAboutBox
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

using GumpStudio.Properties;

namespace GumpStudio.Forms
{
	public class frmAboutBox : Form
	{
		private Button cmdClose;
		private Label Label1;
		private LinkLabel lblHomepage;
		private Label lblVersion;
		private PictureBox PictureBox1;
		private TextBox txtAbout;

		public frmAboutBox()
		{
			Load += frmAboutBox_Load;
			InitializeComponent();
		}

		private void cmdClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void frmAboutBox_Load(object sender, EventArgs e)
		{
			lblVersion.Text = Resources.Core_Version__ + Assembly.GetExecutingAssembly().GetName().Version;
		}


		private void InitializeComponent()
		{
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAboutBox));
			PictureBox1 = new System.Windows.Forms.PictureBox();
			txtAbout = new System.Windows.Forms.TextBox();
			cmdClose = new System.Windows.Forms.Button();
			Label1 = new System.Windows.Forms.Label();
			lblVersion = new System.Windows.Forms.Label();
			lblHomepage = new System.Windows.Forms.LinkLabel();
			((System.ComponentModel.ISupportInitialize)(PictureBox1)).BeginInit();
			SuspendLayout();
			// 
			// PictureBox1
			// 
			PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox1.Image")));
			PictureBox1.Location = new System.Drawing.Point(0, 0);
			PictureBox1.Name = "PictureBox1";
			PictureBox1.Size = new System.Drawing.Size(454, 158);
			PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			PictureBox1.TabIndex = 0;
			PictureBox1.TabStop = false;
			// 
			// txtAbout
			// 
			txtAbout.Location = new System.Drawing.Point(192, 80);
			txtAbout.Multiline = true;
			txtAbout.Name = "txtAbout";
			txtAbout.ReadOnly = true;
			txtAbout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			txtAbout.Size = new System.Drawing.Size(248, 152);
			txtAbout.TabIndex = 1;
			txtAbout.Text = GumpStudio.Properties.Resources.Gump_Studio_was_written___;
			// 
			// cmdClose
			// 
			cmdClose.Location = new System.Drawing.Point(368, 240);
			cmdClose.Name = "cmdClose";
			cmdClose.Size = new System.Drawing.Size(75, 23);
			cmdClose.TabIndex = 0;
			cmdClose.Text = Resources.Close;
			cmdClose.Click += new System.EventHandler(cmdClose_Click);
			// 
			// Label1
			// 
			Label1.Location = new System.Drawing.Point(8, 168);
			Label1.Name = "Label1";
			Label1.Size = new System.Drawing.Size(176, 23);
			Label1.TabIndex = 3;
			Label1.Text = Resources.C__Bradley_Uffner__2004;
			// 
			// lblVersion
			// 
			lblVersion.AutoSize = true;
			lblVersion.Location = new System.Drawing.Point(8, 248);
			lblVersion.Name = "lblVersion";
			lblVersion.Size = new System.Drawing.Size(42, 13);
			lblVersion.TabIndex = 4;
			lblVersion.Text = GumpStudio.Properties.Resources.Version;
			// 
			// lblHomepage
			// 
			lblHomepage.Location = new System.Drawing.Point(8, 192);
			lblHomepage.Name = "lblHomepage";
			lblHomepage.Size = new System.Drawing.Size(168, 23);
			lblHomepage.TabIndex = 5;
			lblHomepage.TabStop = true;
			lblHomepage.Text = "http://www.gumpstudio.com";
			// 
			// frmAboutBox
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(450, 272);
			Controls.Add(lblHomepage);
			Controls.Add(lblVersion);
			Controls.Add(Label1);
			Controls.Add(cmdClose);
			Controls.Add(txtAbout);
			Controls.Add(PictureBox1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			Name = "frmAboutBox";
			Text = Resources.About_Gump_Studio_NET;
			Load += new System.EventHandler(frmAboutBox_Load);
			((System.ComponentModel.ISupportInitialize)(PictureBox1)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		private void lblHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Process.Start(new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = "http://www.orbsydia.net"
			});
		}

		public void SetText(string Text)
		{
			txtAbout.Text = "Gump Studio was designed and written by Bradley Uffner in 2004. It makes extensive use of a modified version of the UOSDK written by Krrios, available at www.RunUO.com. Artwork was created by Melanius, and several more ideas were contributed by the RunUO community.  Special thanks go to DarkStorm of the Wolfpack emulator for helping me to decode unifont.mul, allowing me to displaying UO fonts correctly.\r\n\r\n====Plugin Specific Information====\r\n" + Text;
		}
	}
}