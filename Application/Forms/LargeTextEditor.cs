// Decompiled with JetBrains decompiler
// Type: GumpStudio.LargeTextEditor
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GumpStudio
{
	public class LargeTextEditor : Form
	{
		private Button _cmdCancel;
		private Button _cmdOK;
		private TextBox _txtText;
		private readonly IContainer components;

		public TextBox txtText => _txtText;

		public LargeTextEditor()
		{
			InitializeComponent();
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null) {
				components.Dispose();
			}

			base.Dispose(disposing);
		}


		private void InitializeComponent()
		{
			_txtText = new System.Windows.Forms.TextBox();
			_cmdCancel = new System.Windows.Forms.Button();
			_cmdOK = new System.Windows.Forms.Button();
			SuspendLayout();
			// 
			// _txtText
			// 
			_txtText.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			_txtText.Location = new System.Drawing.Point(8, 8);
			_txtText.Multiline = true;
			_txtText.Name = "_txtText";
			_txtText.Size = new System.Drawing.Size(280, 224);
			_txtText.TabIndex = 0;
			// 
			// _cmdCancel
			// 
			_cmdCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cmdCancel.Location = new System.Drawing.Point(212, 240);
			_cmdCancel.Name = "_cmdCancel";
			_cmdCancel.Size = new System.Drawing.Size(75, 23);
			_cmdCancel.TabIndex = 1;
			_cmdCancel.Text = "Cancel";
			_cmdCancel.Click += new System.EventHandler(cmdCancel_Click);
			// 
			// _cmdOK
			// 
			_cmdOK.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_cmdOK.Location = new System.Drawing.Point(124, 240);
			_cmdOK.Name = "_cmdOK";
			_cmdOK.Size = new System.Drawing.Size(75, 23);
			_cmdOK.TabIndex = 2;
			_cmdOK.Text = "OK";
			_cmdOK.Click += new System.EventHandler(cmdOK_Click);
			// 
			// LargeTextEditor
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			CancelButton = _cmdCancel;
			ClientSize = new System.Drawing.Size(296, 270);
			Controls.Add(_cmdOK);
			Controls.Add(_cmdCancel);
			Controls.Add(_txtText);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			Name = "LargeTextEditor";
			Text = "Text Editor";
			ResumeLayout(false);
			PerformLayout();

		}
	}
}
