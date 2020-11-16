// Decompiled with JetBrains decompiler
// Type: GumpStudio.Settings
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Windows.Forms;

using Microsoft.VisualBasic.CompilerServices;

namespace GumpStudio
{
	[DesignerGenerated]
	public class Settings : Form
	{
		private Button _Button1;
		private Button _Cancel_Button;
		private CheckBox _CheckBox1;
		private Label _Label1;
		private Label _Label2;
		private Label _Label3;
		private NumericUpDown _NumericUpDown1;
		private Button _OK_Button;
		private TableLayoutPanel _TableLayoutPanel1;
		private TrackBar _TrackBar1;
		private TextBox _txtClientPath;
		private readonly IContainer components;

		public Settings()
		{
			InitializeComponent();
		}

		private void Cancel_Button_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}


		protected override void Dispose(bool disposing)
		{
			try
			{
				if ((!disposing || components == null ? 0 : 1) == 0)
				{
					return;
				}

				components.Dispose();
			}
			finally
			{
				base.Dispose(disposing);
			}
		}


		private void InitializeComponent()
		{
			_TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			_OK_Button = new System.Windows.Forms.Button();
			_Cancel_Button = new System.Windows.Forms.Button();
			_txtClientPath = new System.Windows.Forms.TextBox();
			_Label1 = new System.Windows.Forms.Label();
			_NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
			_Label2 = new System.Windows.Forms.Label();
			_TrackBar1 = new System.Windows.Forms.TrackBar();
			_Label3 = new System.Windows.Forms.Label();
			_Button1 = new System.Windows.Forms.Button();
			_CheckBox1 = new System.Windows.Forms.CheckBox();
			_TableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(_NumericUpDown1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(_TrackBar1)).BeginInit();
			SuspendLayout();
			// 
			// _TableLayoutPanel1
			// 
			_TableLayoutPanel1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			_TableLayoutPanel1.ColumnCount = 2;
			_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			_TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			_TableLayoutPanel1.Controls.Add(_OK_Button, 0, 0);
			_TableLayoutPanel1.Controls.Add(_Cancel_Button, 1, 0);
			_TableLayoutPanel1.Location = new System.Drawing.Point(277, 274);
			_TableLayoutPanel1.Name = "_TableLayoutPanel1";
			_TableLayoutPanel1.RowCount = 1;
			_TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			_TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
			_TableLayoutPanel1.TabIndex = 0;
			// 
			// _OK_Button
			// 
			_OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			_OK_Button.Location = new System.Drawing.Point(3, 3);
			_OK_Button.Name = "_OK_Button";
			_OK_Button.Size = new System.Drawing.Size(67, 23);
			_OK_Button.TabIndex = 0;
			_OK_Button.Text = "OK";
			_OK_Button.Click += new System.EventHandler(OK_Button_Click);
			// 
			// _Cancel_Button
			// 
			_Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
			_Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_Cancel_Button.Location = new System.Drawing.Point(76, 3);
			_Cancel_Button.Name = "_Cancel_Button";
			_Cancel_Button.Size = new System.Drawing.Size(67, 23);
			_Cancel_Button.TabIndex = 1;
			_Cancel_Button.Text = "Cancel";
			_Cancel_Button.Click += new System.EventHandler(Cancel_Button_Click);
			// 
			// _txtClientPath
			// 
			_txtClientPath.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			_txtClientPath.Location = new System.Drawing.Point(85, 12);
			_txtClientPath.Name = "_txtClientPath";
			_txtClientPath.Size = new System.Drawing.Size(314, 20);
			_txtClientPath.TabIndex = 1;
			// 
			// _Label1
			// 
			_Label1.AutoSize = true;
			_Label1.Location = new System.Drawing.Point(12, 15);
			_Label1.Name = "_Label1";
			_Label1.Size = new System.Drawing.Size(58, 13);
			_Label1.TabIndex = 2;
			_Label1.Text = "Client Path";
			// 
			// _NumericUpDown1
			// 
			_NumericUpDown1.Location = new System.Drawing.Point(85, 38);
			_NumericUpDown1.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			_NumericUpDown1.Name = "_NumericUpDown1";
			_NumericUpDown1.Size = new System.Drawing.Size(52, 20);
			_NumericUpDown1.TabIndex = 3;
			_NumericUpDown1.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			// 
			// _Label2
			// 
			_Label2.AutoSize = true;
			_Label2.Location = new System.Drawing.Point(12, 45);
			_Label2.Name = "_Label2";
			_Label2.Size = new System.Drawing.Size(67, 13);
			_Label2.TabIndex = 4;
			_Label2.Text = "Undo Levels";
			// 
			// _TrackBar1
			// 
			_TrackBar1.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			_TrackBar1.Location = new System.Drawing.Point(15, 94);
			_TrackBar1.Maximum = 100;
			_TrackBar1.Name = "_TrackBar1";
			_TrackBar1.Size = new System.Drawing.Size(405, 45);
			_TrackBar1.TabIndex = 5;
			_TrackBar1.Value = 6;
			// 
			// _Label3
			// 
			_Label3.AutoSize = true;
			_Label3.Location = new System.Drawing.Point(12, 78);
			_Label3.Name = "_Label3";
			_Label3.Size = new System.Drawing.Size(115, 13);
			_Label3.TabIndex = 6;
			_Label3.Text = "Arrow key acceleration";
			// 
			// _Button1
			// 
			_Button1.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
			_Button1.Location = new System.Drawing.Point(396, 11);
			_Button1.Name = "_Button1";
			_Button1.Size = new System.Drawing.Size(27, 20);
			_Button1.TabIndex = 7;
			_Button1.Text = "...";
			_Button1.UseVisualStyleBackColor = true;
			// 
			// _CheckBox1
			// 
			_CheckBox1.AutoSize = true;
			_CheckBox1.Location = new System.Drawing.Point(15, 145);
			_CheckBox1.Name = "_CheckBox1";
			_CheckBox1.Size = new System.Drawing.Size(132, 17);
			_CheckBox1.TabIndex = 8;
			_CheckBox1.Text = "Pixel Perfect Selection";
			_CheckBox1.UseVisualStyleBackColor = true;
			// 
			// Settings
			// 
			AcceptButton = _OK_Button;
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			CancelButton = _Cancel_Button;
			ClientSize = new System.Drawing.Size(435, 315);
			Controls.Add(_CheckBox1);
			Controls.Add(_Button1);
			Controls.Add(_Label3);
			Controls.Add(_TrackBar1);
			Controls.Add(_Label2);
			Controls.Add(_NumericUpDown1);
			Controls.Add(_Label1);
			Controls.Add(_txtClientPath);
			Controls.Add(_TableLayoutPanel1);
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			Name = "Settings";
			ShowInTaskbar = false;
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			Text = "Settings";
			_TableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(_NumericUpDown1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(_TrackBar1)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}

		private void OK_Button_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
