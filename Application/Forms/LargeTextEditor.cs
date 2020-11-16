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
        private IContainer components;

        public TextBox txtText => _txtText;

        public LargeTextEditor()
        {
            this.InitializeComponent();
        }

        private void cmdCancel_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void cmdOK_Click( object sender, EventArgs e )
        {
            this.DialogResult = DialogResult.OK;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing && this.components != null )
                this.components.Dispose();
            base.Dispose( disposing );
        }


        private void InitializeComponent()
        {
            this._txtText = new System.Windows.Forms.TextBox();
            this._cmdCancel = new System.Windows.Forms.Button();
            this._cmdOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _txtText
            // 
            this._txtText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._txtText.Location = new System.Drawing.Point(8, 8);
            this._txtText.Multiline = true;
            this._txtText.Name = "_txtText";
            this._txtText.Size = new System.Drawing.Size(280, 224);
            this._txtText.TabIndex = 0;
            // 
            // _cmdCancel
            // 
            this._cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cmdCancel.Location = new System.Drawing.Point(212, 240);
            this._cmdCancel.Name = "_cmdCancel";
            this._cmdCancel.Size = new System.Drawing.Size(75, 23);
            this._cmdCancel.TabIndex = 1;
            this._cmdCancel.Text = "Cancel";
            this._cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
            // 
            // _cmdOK
            // 
            this._cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._cmdOK.Location = new System.Drawing.Point(124, 240);
            this._cmdOK.Name = "_cmdOK";
            this._cmdOK.Size = new System.Drawing.Size(75, 23);
            this._cmdOK.TabIndex = 2;
            this._cmdOK.Text = "OK";
            this._cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
            // 
            // LargeTextEditor
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this._cmdCancel;
            this.ClientSize = new System.Drawing.Size(296, 270);
            this.Controls.Add(this._cmdOK);
            this.Controls.Add(this._cmdCancel);
            this.Controls.Add(this._txtText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "LargeTextEditor";
            this.Text = "Text Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
