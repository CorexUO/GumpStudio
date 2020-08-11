// Decompiled with JetBrains decompiler
// Type: GumpStudio.frmAboutBox
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Resources;
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

        private void cmdClose_Click( object sender, EventArgs e )
        {
            Close();
        }

        private void frmAboutBox_Load( object sender, EventArgs e )
        {
            lblVersion.Text = Resources.Core_Version__ + Assembly.GetExecutingAssembly().GetName().Version;
        }

        
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAboutBox));
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtAbout = new System.Windows.Forms.TextBox();
            this.cmdClose = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblHomepage = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBox1
            // 
            this.PictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("PictureBox1.Image")));
            this.PictureBox1.Location = new System.Drawing.Point(0, 0);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(454, 158);
            this.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.PictureBox1.TabIndex = 0;
            this.PictureBox1.TabStop = false;
            // 
            // txtAbout
            // 
            this.txtAbout.Location = new System.Drawing.Point(192, 80);
            this.txtAbout.Multiline = true;
            this.txtAbout.Name = "txtAbout";
            this.txtAbout.ReadOnly = true;
            this.txtAbout.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAbout.Size = new System.Drawing.Size(248, 152);
            this.txtAbout.TabIndex = 1;
            this.txtAbout.Text = GumpStudio.Properties.Resources.Gump_Studio_was_written___;
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(368, 240);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 0;
            this.cmdClose.Text = Resources.Close;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(8, 168);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(176, 23);
            this.Label1.TabIndex = 3;
            this.Label1.Text = Resources.C__Bradley_Uffner__2004;
            // 
            // lblVersion
            // 
            this.lblVersion.AutoSize = true;
            this.lblVersion.Location = new System.Drawing.Point(8, 248);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(42, 13);
            this.lblVersion.TabIndex = 4;
            this.lblVersion.Text = GumpStudio.Properties.Resources.Version;
            // 
            // lblHomepage
            // 
            this.lblHomepage.Location = new System.Drawing.Point(8, 192);
            this.lblHomepage.Name = "lblHomepage";
            this.lblHomepage.Size = new System.Drawing.Size(168, 23);
            this.lblHomepage.TabIndex = 5;
            this.lblHomepage.TabStop = true;
            this.lblHomepage.Text = "http://www.gumpstudio.com";
            // 
            // frmAboutBox
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(450, 272);
            this.Controls.Add(this.lblHomepage);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.txtAbout);
            this.Controls.Add(this.PictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmAboutBox";
            this.Text = Resources.About_Gump_Studio_NET;
            this.Load += new System.EventHandler(this.frmAboutBox_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void lblHomepage_LinkClicked( object sender, LinkLabelLinkClickedEventArgs e )
        {
            Process.Start( new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = "http://www.orbsydia.net"
            } );
        }

        public void SetText( string Text )
        {
            txtAbout.Text = "Gump Studio was designed and written by Bradley Uffner in 2004. It makes extensive use of a modified version of the UOSDK written by Krrios, available at www.RunUO.com. Artwork was created by Melanius, and several more ideas were contributed by the RunUO community.  Special thanks go to DarkStorm of the Wolfpack emulator for helping me to decode unifont.mul, allowing me to displaying UO fonts correctly.\r\n\r\n====Plugin Specific Information====\r\n" + Text;
        }
    }
}