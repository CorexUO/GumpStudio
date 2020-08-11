// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.HTMLElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using Ultima;

namespace GumpStudio.Elements
{
    [Serializable]
    public class HTMLElement : ResizeableElement, IRunUOExportable
    {
        protected Bitmap imgBack;
        protected Bitmap imgDown;
        protected Bitmap imgLoc;
        protected Bitmap imgUp;
        protected bool mBackground;
        protected BackgroundElement mBGElement;
        protected Bitmap mCache;
        protected int mCliLocID;
        protected Font mFont;
        protected string mHTML;
        protected bool mScrollbar;
        protected HTMLElementType mTextType;

        [Description( "The ID of the localized message to display as the text of the HTML Element. Only Valid if TextType is set to Localized." )]
        [Editor( typeof( ClilocPropEditor ), typeof( UITypeEditor ) )]
        public int CliLocID
        {
            get => mCliLocID;
            set => mCliLocID = value;
        }

        [Editor( typeof( LargeTextPropEditor ), typeof( UITypeEditor ) )]
        [Description( "The HTML to display in the Element.  Only valid if TextType is set to HTML." )]
        public string HTML
        {
            get => mHTML;
            set => mHTML = value;
        }

        [Description( "Display a background behind the text of the element." )]
        public bool ShowBackground
        {
            get => mBackground;
            set => mBackground = value;
        }

        [Description( "Display scrollbars along the right side of the element." )]
        public bool ShowScrollbar
        {
            get => mScrollbar;
            set => mScrollbar = value;
        }

        [Description( "Switches between custom HTML, and Localized messages" )]
        public HTMLElementType TextType
        {
            get => mTextType;
            set => mTextType = value;
        }

        public override string Type => "HTML";

        public HTMLElement()
        {
            mHTML = "";
            mCliLocID = 0;
            mScrollbar = true;
            mBackground = true;
            mTextType = HTMLElementType.HTML;
            mHTML = "";
            mSize = new Size( 200, 100 );
            mFont = new Font( "Arial", 12f, FontStyle.Regular, GraphicsUnit.Point );
            RefreshCache();
        }

        public HTMLElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            info.GetInt32( "HTMLElementVersion" );
            mHTML = info.GetString( nameof( HTML ) );
            mCliLocID = info.GetInt32( "ClilocID" );
            mScrollbar = info.GetBoolean( "Scrollbar" );
            mBackground = info.GetBoolean( "Background" );
            mTextType = (HTMLElementType) info.GetInt32( nameof( TextType ) );
            mFont = new Font( "Arial", 12f, FontStyle.Regular, GraphicsUnit.Point );
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "HTMLElementVersion", 1 );
            info.AddValue( "HTML", mHTML );
            info.AddValue( "ClilocID", mCliLocID );
            info.AddValue( "Scrollbar", mScrollbar );
            info.AddValue( "Background", mBackground );
            info.AddValue( "TextType", (int) mTextType );
        }

        public override void RefreshCache()
        {
            imgUp = Gumps.GetGump( 250 );
            imgDown = Gumps.GetGump( 252 );
            imgLoc = Gumps.GetGump( 254 );
            imgBack = Gumps.GetGump( 256 );
            mBGElement = new BackgroundElement();
            mBGElement.GumpID = 3000;
        }

        public override void Render( Graphics Target )
        {
            SolidBrush solidBrush = new SolidBrush( Color.FromArgb( 70, Color.White ) );
            if ( !mBackground )
            {
                Target.FillRectangle( solidBrush, Bounds );
                Target.DrawRectangle( Pens.DarkGray, Bounds );
            }
            if ( mScrollbar )
            {
                Target.DrawImage( imgUp, X + Width - imgUp.Width, Y );
                Target.DrawImage( imgLoc, X + Width - imgLoc.Width, Y + imgUp.Height );
                Region clip = Target.Clip;
                Region region = new Region( new Rectangle( X + Width - imgBack.Width, Y + imgUp.Height + imgLoc.Height, imgBack.Width, Height - imgDown.Height - imgUp.Height - imgLoc.Height ) );
                Target.Clip = region;
                int height = imgBack.Height;
                int num = Y + Height - imgDown.Height;
                int y = Y + imgUp.Height + imgLoc.Height;
                while ( ( height >> 31 ^ y ) <= ( height >> 31 ^ num ) )
                {
                    Target.DrawImage( imgBack, X + Width - imgBack.Width, y );
                    y += height;
                }
                Target.Clip = clip;
                Target.DrawImage( imgDown, X + Width - imgDown.Width, Y + Height - imgDown.Height );
            }
            Rectangle rectangle1 = new Rectangle( Location, mBGElement.Size );
            Rectangle rectangle2;
            if ( mBackground )
            {
                mBGElement.Location = Location;
                rectangle2 = !mScrollbar ? new Rectangle( Location, Size ) : new Rectangle( Location, new Size( Width - imgBack.Width, Height ) );
                mBGElement.Size = rectangle2.Size;
                mBGElement.Render( Target );
            }
            else
                rectangle2 = Bounds;
            RectangleF layoutRectangle = new RectangleF( rectangle2.X, rectangle2.Y, rectangle2.Width, rectangle2.Height );
            Target.DrawString( mHTML, mFont, Brushes.Black, layoutRectangle );
            solidBrush.Dispose();
        }

        public string ToRunUOString()
        {
            string text = TextType == HTMLElementType.Localized ? $"AddHtmlLocalized({X}, {Y}, {Width}, {Height}, {CliLocID}, {ShowScrollbar.ToString().ToLower()}, {ShowBackground.ToString().ToLower()});" : $"AddHtml({X}, {Y}, {Width}, {Height}, \"{HTML.Replace( "\"", "\\\"" )}\", {ShowScrollbar.ToString().ToLower()});";

            return text;
        }
    }
}
