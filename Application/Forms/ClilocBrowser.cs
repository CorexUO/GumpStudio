// Decompiled with JetBrains decompiler
// Type: GumpStudio.ClilocBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private IContainer components;
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

        private void Cancel_Button_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ClilocBrowser_Load( object sender, EventArgs e )
        {
            foreach ( string file in Directory.GetFiles( XMLSettings.CurrentOptions.ClientPath, "Cliloc.*" ) )
            {
                cboLanguage.Items.Add( Path.GetExtension( file ).Substring( 1 ) );
            }

            if ( ClilocCache == null )
            {
                lstCliloc.SuspendLayout();

                foreach ( StringEntry entry in new StringList( "enu" ).Entries )
                {
                    lstCliloc.Items.Add( entry );
                }

                lstCliloc.ResumeLayout();
                ClilocCache = lstCliloc;
            }
            else
            {
                lstCliloc = ClilocCache;
            }
        }

        
        protected override void Dispose( bool disposing )
        {
            try
            {
                if ( ( !disposing || components == null ? 0 : 1 ) == 0 )
                {
                    return;
                }

                components?.Dispose();
            }
            finally
            {
                base.Dispose( disposing );
            }
        }

        private void InitializeComponent()
        {
            this.TableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.lstCliloc = new System.Windows.Forms.ListBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.cboLanguage = new System.Windows.Forms.ComboBox();
            this.TableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            this.TableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.TableLayoutPanel1.ColumnCount = 2;
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Controls.Add(this.OK_Button, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.Cancel_Button, 1, 0);
            this.TableLayoutPanel1.Location = new System.Drawing.Point(451, 392);
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 1;
            this.TableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.TableLayoutPanel1.Size = new System.Drawing.Size(146, 29);
            this.TableLayoutPanel1.TabIndex = 0;
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(67, 23);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = Resources.OK;
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Cancel_Button.Location = new System.Drawing.Point(76, 3);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(67, 23);
            this.Cancel_Button.TabIndex = 1;
            this.Cancel_Button.Text = Resources.Cancel;
            // 
            // lstCliloc
            // 
            this.lstCliloc.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstCliloc.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstCliloc.FormattingEnabled = true;
            this.lstCliloc.Location = new System.Drawing.Point(12, 12);
            this.lstCliloc.Name = "lstCliloc";
            this.lstCliloc.Size = new System.Drawing.Size(585, 368);
            this.lstCliloc.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(12, 400);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(55, 13);
            this.Label1.TabIndex = 2;
            this.Label1.Text = Resources.Language;
            // 
            // cboLanguage
            // 
            this.cboLanguage.FormattingEnabled = true;
            this.cboLanguage.Location = new System.Drawing.Point(73, 397);
            this.cboLanguage.Name = "cboLanguage";
            this.cboLanguage.Size = new System.Drawing.Size(121, 21);
            this.cboLanguage.TabIndex = 3;
            // 
            // ClilocBrowser
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(609, 433);
            this.Controls.Add(this.cboLanguage);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.lstCliloc);
            this.Controls.Add(this.TableLayoutPanel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClilocBrowser";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = @"Cliloc Browser";
            this.TableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void lstCliloc_DrawItem( object sender, DrawItemEventArgs e )
        {
            StringEntry stringEntry = (StringEntry) lstCliloc.Items[e.Index];
            e.DrawBackground();
            e.Graphics.DrawString( stringEntry.Number.ToString(), lstCliloc.Font, Brushes.Black, e.Bounds.X, e.Bounds.Top );
            e.Graphics.DrawString( stringEntry.Text, lstCliloc.Font, Brushes.Black, e.Bounds.X + 100, e.Bounds.Top );
        }

        private void OK_Button_Click( object sender, EventArgs e )
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}