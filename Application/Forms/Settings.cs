// Decompiled with JetBrains decompiler
// Type: GumpStudio.Settings
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

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
        private IContainer components;

        public Settings()
        {
            this.InitializeComponent();
        }

        private void Cancel_Button_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        
        protected override void Dispose( bool disposing )
        {
            try
            {
                if ( ( !disposing || this.components == null ? 0 : 1 ) == 0 )
                    return;
                this.components.Dispose();
            }
            finally
            {
                base.Dispose( disposing );
            }
        }

        
        private void InitializeComponent()
        {
            this._TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._OK_Button = new System.Windows.Forms.Button();
            this._Cancel_Button = new System.Windows.Forms.Button();
            this._txtClientPath = new System.Windows.Forms.TextBox();
            this._Label1 = new System.Windows.Forms.Label();
            this._NumericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this._Label2 = new System.Windows.Forms.Label();
            this._TrackBar1 = new System.Windows.Forms.TrackBar();
            this._Label3 = new System.Windows.Forms.Label();
            this._Button1 = new System.Windows.Forms.Button();
            this._CheckBox1 = new System.Windows.Forms.CheckBox();
            this._TableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._NumericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._TrackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // _TableLayoutPanel1
            // 
            this._TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._TableLayoutPanel1.ColumnCount = 2;
            this._TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._TableLayoutPanel1.Controls.Add(this._OK_Button, 0, 0);
            this._TableLayoutPanel1.Controls.Add(this._Cancel_Button, 1, 0);
            this._TableLayoutPanel1.Location = new System.Drawing.Point(277, 274);
            this._TableLayoutPanel1.Name = "_TableLayoutPanel1";
            this._TableLayoutPanel1.RowCount = 1;
            this._TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
            this._TableLayoutPanel1.TabIndex = 0;
            // 
            // _OK_Button
            // 
            this._OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._OK_Button.Location = new System.Drawing.Point(3, 3);
            this._OK_Button.Name = "_OK_Button";
            this._OK_Button.Size = new System.Drawing.Size(67, 23);
            this._OK_Button.TabIndex = 0;
            this._OK_Button.Text = "OK";
            this._OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // _Cancel_Button
            // 
            this._Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._Cancel_Button.Location = new System.Drawing.Point(76, 3);
            this._Cancel_Button.Name = "_Cancel_Button";
            this._Cancel_Button.Size = new System.Drawing.Size(67, 23);
            this._Cancel_Button.TabIndex = 1;
            this._Cancel_Button.Text = "Cancel";
            this._Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // _txtClientPath
            // 
            this._txtClientPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtClientPath.Location = new System.Drawing.Point(85, 12);
            this._txtClientPath.Name = "_txtClientPath";
            this._txtClientPath.Size = new System.Drawing.Size(314, 20);
            this._txtClientPath.TabIndex = 1;
            // 
            // _Label1
            // 
            this._Label1.AutoSize = true;
            this._Label1.Location = new System.Drawing.Point(12, 15);
            this._Label1.Name = "_Label1";
            this._Label1.Size = new System.Drawing.Size(58, 13);
            this._Label1.TabIndex = 2;
            this._Label1.Text = "Client Path";
            // 
            // _NumericUpDown1
            // 
            this._NumericUpDown1.Location = new System.Drawing.Point(85, 38);
            this._NumericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._NumericUpDown1.Name = "_NumericUpDown1";
            this._NumericUpDown1.Size = new System.Drawing.Size(52, 20);
            this._NumericUpDown1.TabIndex = 3;
            this._NumericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _Label2
            // 
            this._Label2.AutoSize = true;
            this._Label2.Location = new System.Drawing.Point(12, 45);
            this._Label2.Name = "_Label2";
            this._Label2.Size = new System.Drawing.Size(67, 13);
            this._Label2.TabIndex = 4;
            this._Label2.Text = "Undo Levels";
            // 
            // _TrackBar1
            // 
            this._TrackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._TrackBar1.Location = new System.Drawing.Point(15, 94);
            this._TrackBar1.Maximum = 100;
            this._TrackBar1.Name = "_TrackBar1";
            this._TrackBar1.Size = new System.Drawing.Size(405, 45);
            this._TrackBar1.TabIndex = 5;
            this._TrackBar1.Value = 6;
            // 
            // _Label3
            // 
            this._Label3.AutoSize = true;
            this._Label3.Location = new System.Drawing.Point(12, 78);
            this._Label3.Name = "_Label3";
            this._Label3.Size = new System.Drawing.Size(115, 13);
            this._Label3.TabIndex = 6;
            this._Label3.Text = "Arrow key acceleration";
            // 
            // _Button1
            // 
            this._Button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._Button1.Location = new System.Drawing.Point(396, 11);
            this._Button1.Name = "_Button1";
            this._Button1.Size = new System.Drawing.Size(27, 20);
            this._Button1.TabIndex = 7;
            this._Button1.Text = "...";
            this._Button1.UseVisualStyleBackColor = true;
            // 
            // _CheckBox1
            // 
            this._CheckBox1.AutoSize = true;
            this._CheckBox1.Location = new System.Drawing.Point(15, 145);
            this._CheckBox1.Name = "_CheckBox1";
            this._CheckBox1.Size = new System.Drawing.Size(132, 17);
            this._CheckBox1.TabIndex = 8;
            this._CheckBox1.Text = "Pixel Perfect Selection";
            this._CheckBox1.UseVisualStyleBackColor = true;
            // 
            // Settings
            // 
            this.AcceptButton = this._OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._Cancel_Button;
            this.ClientSize = new System.Drawing.Size(435, 315);
            this.Controls.Add(this._CheckBox1);
            this.Controls.Add(this._Button1);
            this.Controls.Add(this._Label3);
            this.Controls.Add(this._TrackBar1);
            this.Controls.Add(this._Label2);
            this.Controls.Add(this._NumericUpDown1);
            this.Controls.Add(this._Label1);
            this.Controls.Add(this._txtClientPath);
            this.Controls.Add(this._TableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Settings";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this._TableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._NumericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._TrackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void OK_Button_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
