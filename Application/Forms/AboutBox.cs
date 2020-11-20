using System;
using System.Reflection;
using System.Windows.Forms;

using GumpStudio.Properties;

namespace GumpStudio.Forms
{
	public class AboutBox : Form
	{
		private Label Label1;
		private LinkLabel lblHomepage;
		private Label lblVersion;
		private PictureBox PictureBox1;
		private FlowLayoutPanel flowLayoutPanel1;
		private TextBox txtAbout;

		public AboutBox()
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
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutBox));
			PictureBox1 = new System.Windows.Forms.PictureBox();
			txtAbout = new System.Windows.Forms.TextBox();
			Label1 = new System.Windows.Forms.Label();
			lblVersion = new System.Windows.Forms.Label();
			lblHomepage = new System.Windows.Forms.LinkLabel();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			((System.ComponentModel.ISupportInitialize)(PictureBox1)).BeginInit();
			flowLayoutPanel1.SuspendLayout();
			SuspendLayout();
			// 
			// PictureBox1
			// 
			PictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
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
			txtAbout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			txtAbout.Dock = System.Windows.Forms.DockStyle.Fill;
			txtAbout.Location = new System.Drawing.Point(0, 158);
			txtAbout.Multiline = true;
			txtAbout.Name = "txtAbout";
			txtAbout.ReadOnly = true;
			txtAbout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			txtAbout.Size = new System.Drawing.Size(454, 154);
			txtAbout.TabIndex = 1;
			txtAbout.TabStop = false;
			txtAbout.Text = "Gump Studio was written by Bradley Uffner in January of 2003.";
			// 
			// Label1
			// 
			Label1.Anchor = System.Windows.Forms.AnchorStyles.None;
			Label1.AutoSize = true;
			Label1.Location = new System.Drawing.Point(6, 3);
			Label1.Name = "Label1";
			Label1.Size = new System.Drawing.Size(120, 13);
			Label1.TabIndex = 3;
			Label1.Text = "(C) Bradley Uffner, 2004";
			// 
			// lblVersion
			// 
			lblVersion.AutoSize = true;
			lblVersion.Location = new System.Drawing.Point(138, 3);
			lblVersion.Name = "lblVersion";
			lblVersion.Size = new System.Drawing.Size(42, 13);
			lblVersion.TabIndex = 4;
			lblVersion.Text = "Version";
			// 
			// lblHomepage
			// 
			lblHomepage.Anchor = System.Windows.Forms.AnchorStyles.None;
			lblHomepage.AutoSize = true;
			lblHomepage.Location = new System.Drawing.Point(132, 3);
			lblHomepage.Name = "lblHomepage";
			lblHomepage.Size = new System.Drawing.Size(0, 13);
			lblHomepage.TabIndex = 5;
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoSize = true;
			flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			flowLayoutPanel1.Controls.Add(Label1);
			flowLayoutPanel1.Controls.Add(lblHomepage);
			flowLayoutPanel1.Controls.Add(lblVersion);
			flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			flowLayoutPanel1.Location = new System.Drawing.Point(0, 312);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(3);
			flowLayoutPanel1.Size = new System.Drawing.Size(454, 19);
			flowLayoutPanel1.TabIndex = 6;
			flowLayoutPanel1.WrapContents = false;
			// 
			// AboutBox
			// 
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			AutoSize = true;
			ClientSize = new System.Drawing.Size(454, 331);
			Controls.Add(txtAbout);
			Controls.Add(flowLayoutPanel1);
			Controls.Add(PictureBox1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			Name = "AboutBox";
			Text = "About Gump Studio.NET";
			Load += new System.EventHandler(frmAboutBox_Load);
			((System.ComponentModel.ISupportInitialize)(PictureBox1)).EndInit();
			flowLayoutPanel1.ResumeLayout(false);
			flowLayoutPanel1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		private void lblHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			/*Process.Start(new ProcessStartInfo
			{
				UseShellExecute = true,
				FileName = "http://www.orbsydia.net"
			});*/
		}

		private const string _Text = @"
Gump Studio was originally designed and written by Bradley Uffner in 2004.

UI artwork by Melanius.
Ultima SDK by Krrios.
UOFonts by DarkStorm.
";

		public void SetText(string text)
		{
			txtAbout.Text = $"{_Text}{Environment.NewLine}{Environment.NewLine}====Plugin Specific Information===={Environment.NewLine}{Environment.NewLine}" + text;
		}
	}
}