// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.LabelElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Windows.Forms;
using GumpStudio.Properties;
using Ultima;
using UOFont;
using UnicodeFonts = UOFont.UnicodeFonts;

namespace GumpStudio.Elements
{
    [Serializable]
    public class LabelElement : BaseElement, IRunUOExportable
    {
        protected Bitmap mCache;
        protected bool mCropped;
        protected int mFontIndex;
        protected Hue mHue;
        protected string mText;
        protected bool mUnicode;
        protected bool mPartialHue;

        [MergableProperty( true )]
        public bool Cropped
        {
            get => mCropped;
            set
            {
                mCropped = value;
                RefreshCache();
            }
        }

        [MergableProperty( true )]
        public bool Unicode
        {
            get => mUnicode;
            set
            {
                mUnicode = value;
                if ( !value && mFontIndex > 12 )
                    mFontIndex = 12;
                RefreshCache();
            }
        }

        [Browsable( true )]
        [MergableProperty( true )]
        [Editor( typeof( FontPropEditor ), typeof( UITypeEditor ) )]
        public int Font
        {
            get => mFontIndex;
            set
            {
                if ( value >= 0 & value < ( mUnicode ? 13 : 10 ) )
                {
                    mFontIndex = value;
                    RefreshCache();
                }
                else
                {
                    MessageBox.Show( Resources.Font_Error, Resources.Font_Error, MessageBoxButtons.OK, MessageBoxIcon.Exclamation );
                }
            }
        }

        [MergableProperty( true )]
        public bool PartialHue
        {
            get => mPartialHue;
            set
            {
                mPartialHue = value;
                RefreshCache();
            }
        }

        [MergableProperty( true )]
        [Browsable( true )]
        [Editor( typeof( HuePropEditor ), typeof( UITypeEditor ) )]
        [TypeConverter( typeof( HuePropStringConverter ) )]
        public Hue Hue
        {
            get => mHue;
            set
            {
                mHue = value;
                RefreshCache();
            }
        }

        [Browsable( true )]
        [MergableProperty( true )]
        public override Size Size
        {
            get => base.Size;
            set
            {
                if ( !mCropped )
                    throw new ArgumentException( "Size may only be changed if the label is cropped." );
                mSize = value;
            }
        }

        [MergableProperty( true )]
        public string Text
        {
            get => mText;
            set
            {
                mText = value;
                RefreshCache();
            }
        }

        public override string Type => "Label";

        public LabelElement()
        {
            mFontIndex = 2;
            mCropped = false;
            mPartialHue = true;
            mUnicode = true;
            mHue = Hues.GetHue( 0 );
            mText = "New Label";
            try
            {
                RefreshCache();
            }
            catch ( Exception ex )
            {
            }
        }

        public LabelElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            mFontIndex = 2;
            int int32 = info.GetInt32( "LabelElementVersion" );
            mText = info.GetString( nameof( Text ) );
            mHue = Hues.GetHue( info.GetInt32( "HueIndex" ) );
            if ( int32 >= 3 )
            {
                mPartialHue = info.GetBoolean( nameof( PartialHue ) );
                mUnicode = info.GetBoolean( nameof( Unicode ) );
            }
            else
            {
                mPartialHue = true;
                mUnicode = true;
            }
            mFontIndex = info.GetInt32( "FontIndex" );
            if ( int32 <= 2 )
                --mFontIndex;
            if ( int32 >= 2 )
            {
                mCropped = info.GetBoolean( nameof( Cropped ) );
                mSize = (Size) info.GetValue( nameof( Size ), typeof( Size ) );
            }
            else
            {
                mCropped = false;
                Bitmap stringImage = UnicodeFonts.GetStringImage( mFontIndex, mText + " " );
                mSize = stringImage.Size;
                stringImage.Dispose();
            }
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "LabelElementVersion", 3 );
            info.AddValue( "Text", mText );
            info.AddValue( "HueIndex", mHue.Index );
            info.AddValue( "PartialHue", mPartialHue );
            info.AddValue( "Unicode", mUnicode );
            info.AddValue( "FontIndex", mFontIndex );
            info.AddValue( "Cropped", mCropped );
        }

        public override void RefreshCache()
        {
            if ( mCache != null )
                mCache.Dispose();

            mCache = mUnicode ? UnicodeFonts.GetStringImage( mFontIndex, mText + " " ) : throw new NotSupportedException( "ASCII Font?" );//Fonts.GetStringImage( mFontIndex, mText + " " );
            if ( ( mHue == null || mHue.Index == 0 ? 0 : 1 ) != 0 )
                mHue.ApplyTo( mCache, mPartialHue );
            if ( mCropped )
            {
                Bitmap bitmap = new Bitmap( mSize.Width, mSize.Height, PixelFormat.Format32bppArgb );
                Graphics graphics = Graphics.FromImage( bitmap );
                graphics.Clear( Color.Transparent );
                graphics.DrawImage( mCache, 0, 0 );
                graphics.Dispose();
                mCache.Dispose();
                mCache = bitmap;
            }
            mSize = mCache.Size;
        }

        public override void Render( Graphics Target )
        {
            if ( mCache == null )
                RefreshCache();
            Target.DrawImage( mCache, Location );
        }

        public string ToRunUOString()
        {
            return $"AddLabel({X}, {Y}, @\"{Text.Replace( "\"", "\\\"" )});";
        }
    }
}
