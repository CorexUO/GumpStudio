using System;
using System.Threading;
using System.Windows.Forms;

namespace GumpStudio
{
	public class SplashBox : Form
	{
		private static SplashBox _Instance;
		private static Thread _Thread;

		public SplashBox()
		{
			Load += OnSplashLoad;
			Click += OnSplashClick;

			InitializeComponent();
		}

		public static void DisplaySplash()
		{
			_Thread = new Thread(ThreadStartDisplay);
			_Thread.Start();
		}

		private void OnSplashClick(object sender, EventArgs e)
		{
			Hide();
			Dispose();
		}

		private void OnSplashLoad(object sender, EventArgs e)
		{
			CenterToScreen();
		}

		private void InitializeComponent()
		{
			SuspendLayout();

			// 
			// SplashBox
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			BackgroundImage = Properties.Resources.PictureBox1_Image;
			ClientSize = new System.Drawing.Size(453, 154);
			FormBorderStyle = FormBorderStyle.None;
			Name = "SplashBox";
			Text = "Gump Studio";
			TopMost = true;

			ResumeLayout(false);
		}

		private static void ThreadStartDisplay()
		{
			_Instance = new SplashBox();
			_Instance.Show();

			var now = DateTime.Now.AddSeconds(3);

			while (DateTime.Now < now)
			{
				Thread.Sleep(100);

				Application.DoEvents();
			}

			_Instance.Dispose();
			_Instance = null;
		}
	}
}