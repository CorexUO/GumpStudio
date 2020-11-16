// Decompiled with JetBrains decompiler
// Type: GumpStudio.frmSplash
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Threading;
using System.Windows.Forms;

namespace GumpStudio
{
	public class frmSplash : Form
	{
		private static frmSplash f;
		private static Thread t;

		public frmSplash()
		{
			Load += new EventHandler(frmSplash_Load);
			Click += new EventHandler(frmSplash_Click);
			InitializeComponent();
		}

		public static void DisplaySplash()
		{
			frmSplash.t = new Thread(new ThreadStart(frmSplash.ThreadStartDisplay));
			frmSplash.t.Start();
		}

		private static void FadeOut(Form f)
		{
			f.Dispose();
		}

		private void frmSplash_Click(object sender, EventArgs e)
		{
			frmSplash.FadeOut(this);
		}

		private void frmSplash_Load(object sender, EventArgs e)
		{
			CenterToScreen();
		}


		private void InitializeComponent()
		{
			SuspendLayout();
			// 
			// frmSplash
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			BackgroundImage = global::GumpStudio.Properties.Resources.PictureBox1_Image;
			ClientSize = new System.Drawing.Size(453, 154);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			Name = "frmSplash";
			Text = @"frmSplash";
			TopMost = true;
			Load += new System.EventHandler(FrmSplash_Load_1);
			ResumeLayout(false);

		}

		private static void ThreadStartDisplay()
		{
			frmSplash.f = new frmSplash();
			frmSplash.f.Show();
			var now = DateTime.Now;
			while (DateTime.Now < now + TimeSpan.FromSeconds(2))
			{
				Thread.Sleep(100);
				Application.DoEvents();
			}
			frmSplash.FadeOut(frmSplash.f);
		}

		private void FrmSplash_Load_1(object sender, EventArgs e)
		{

		}
	}
}
