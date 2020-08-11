// Decompiled with JetBrains decompiler
// Type: GumpStudio.FontBrowser
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using GumpStudio.Elements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using UOFont;

namespace GumpStudio
{
    public class FontBrowser : UserControl
    {
        private bool fntunicode = true;
        private static List<WeakReference> __ENCList = new List<WeakReference>();
        private const int fntshift = 0;
        [AccessedThroughProperty( "lstFont" )]
        private ListBox _lstFont;
        private IContainer components;
        public int Value;

        public event FontBrowser.ValueChangedEventHandler ValueChanged;

        public FontBrowser()
        {
            this.Load += new EventHandler( this.FontBrowser_Load );
            lock ( FontBrowser.__ENCList )
                FontBrowser.__ENCList.Add( new WeakReference( this ) );
            this.InitializeComponent();
        }

        public FontBrowser( int Value )
          : this()
        {
            this.Value = Value;
        }

        protected override void Dispose( bool disposing )
        {
            if ( disposing )
                components?.Dispose();
            base.Dispose( disposing );
        }

        private void FontBrowser_Load( object sender, EventArgs e )
        {
            this.fntunicode = true;
            ArrayList arrayList = GlobalObjects.DesignerForm == null || GlobalObjects.DesignerForm.ElementStack == null ? null : GlobalObjects.DesignerForm.ElementStack.GetSelectedElements();
            if ( arrayList != null )
            {
                foreach ( object obj in arrayList )
                {
                    if ( obj is LabelElement && !( (LabelElement) obj ).Unicode )
                    {
                        this.fntunicode = false;
                        break;
                    }
                }
            }
            for ( int index = 0 ; index < ( this.fntunicode ? 13 : 10 ) ; ++index )
            {
                if ( index >= 0 )
                    this._lstFont.Items.Add( index );
            }
            this._lstFont.SelectedIndex = this.Value;
        }

        
        private void InitializeComponent()
        {
            this._lstFont = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // _lstFont
            // 
            this._lstFont.Dock = System.Windows.Forms.DockStyle.Fill;
            this._lstFont.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this._lstFont.IntegralHeight = false;
            this._lstFont.Location = new System.Drawing.Point( 0, 0 );
            this._lstFont.Name = "_lstFont";
            this._lstFont.Size = new System.Drawing.Size( 326, 282 );
            this._lstFont.TabIndex = 0;
            this._lstFont.DrawItem += new System.Windows.Forms.DrawItemEventHandler( this.lstFont_DrawItem );
            this._lstFont.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler( this.lstFont_MeasureItem );
            this._lstFont.DoubleClick += new System.EventHandler( this.lstFont_DoubleClick );
            // 
            // FontBrowser
            // 
            this.Controls.Add( this._lstFont );
            this.Name = "FontBrowser";
            this.Size = new System.Drawing.Size( 326, 282 );
            this.Load += new System.EventHandler( this.FontBrowser_Load );
            this.ResumeLayout( false );

        }

        private void lstFont_DoubleClick( object sender, EventArgs e )
        {
            this.Value = this._lstFont.SelectedIndex;
            FontBrowser.ValueChangedEventHandler valueChanged = this.ValueChanged;
            valueChanged?.Invoke( this.Value );
        }

        private void lstFont_DrawItem( object sender, DrawItemEventArgs e )
        {
            if ( ( e.State & DrawItemState.Selected ) > DrawItemState.None )
                e.Graphics.FillRectangle( SystemBrushes.Highlight, e.Bounds );
            else
                e.Graphics.FillRectangle( SystemBrushes.Window, e.Bounds );
            if ( e.Index > ( this.fntunicode ? 12 : 9 ) )
                return;
            Bitmap bitmap = this.fntunicode ? UnicodeFonts.GetStringImage( e.Index, "ABCabc123!@#$АБВабв" ) : Fonts.GetStringImage( e.Index, "ABCabc123 */ АБВабв" );
            e.Graphics.DrawImage( bitmap, e.Bounds.Location );
            bitmap.Dispose();
        }

        private void lstFont_MeasureItem( object sender, MeasureItemEventArgs e )
        {
            if ( e.Index > ( this.fntunicode ? 12 : 9 ) )
            {
                e.ItemHeight = 0;
            }
            else
            {
                Bitmap bitmap = this.fntunicode ? UnicodeFonts.GetStringImage( e.Index, "ABCabc123!@#$АБВабв" ) : Fonts.GetStringImage( e.Index, "ABCabc123 */ АБВабв" );
                e.ItemHeight = bitmap.Height;
                bitmap.Dispose();
            }
        }

        public delegate void ValueChangedEventHandler( int Value );
    }
}
