// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.ItemElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.Serialization;
using System.Windows.Forms;
using GumpStudio.Properties;
using Ultima;

namespace GumpStudio.Elements
{
    [Serializable]
    public class ItemElement : BaseElement, IRunUOExportable
    {
        protected Image ImageCache;
        protected Hue mHue;
        protected int mItemID;

        [TypeConverter( typeof( HuePropStringConverter ) )]
        [Browsable( true )]
        [Editor( typeof( HuePropEditor ), typeof( UITypeEditor ) )]
        public Hue Hue
        {
            get => mHue;
            set => mHue = value;
        }

        [Editor( typeof( ItemIDPropEditor ), typeof( UITypeEditor ) )]
        public int ItemID
        {
            get => mItemID;
            set
            {
                ImageCache = Art.GetStatic( value );
                if ( ImageCache == null )
                {
                    ImageCache = Art.GetStatic( mItemID );
                    //int num = (int) Interaction.MsgBox((object) "Invalid ItemID", MsgBoxStyle.OkOnly, (object) null);
                    MessageBox.Show( "Invalid ItemID" );
                }
                else
                {
                    mItemID = value;
                    mSize = ImageCache.Size;
                }
            }
        }

        public override string Type => "Item";

        public ItemElement()
        {
            mSize = new Size( 50, 50 );
            ItemID = 0;
            mHue = Hues.GetHue( 0 );
        }

        public ItemElement( SerializationInfo info, StreamingContext context )
          : base( info, context )
        {
            info.GetInt32( "ItemElementVersion" );
            mItemID = info.GetInt32( nameof( ItemID ) );
            mHue = Hues.GetHue( info.GetInt32( "HueIndex" ) );
            RefreshCache();
        }

        public override void GetObjectData( SerializationInfo info, StreamingContext context )
        {
            base.GetObjectData( info, context );
            info.AddValue( "ItemElementVersion", 1 );
            info.AddValue( "ItemID", mItemID );
            info.AddValue( "HueIndex", mHue.Index );
        }

        public override void RefreshCache()
        {
            if ( ImageCache != null )
                return;
            ImageCache = Art.GetStatic( mItemID );
        }

        public override void Render( Graphics Target )
        {
            try
            {
                if ( mHue.Index != 0 )
                {
                    Bitmap bmp = (Bitmap) ImageCache.Clone();
                    if ( bmp != null )
                    {
                        mHue.ApplyTo( bmp, false );
                        Target.DrawImage( bmp, Location );
                        bmp.Dispose();
                    }
                    else
                    {
                        Target.DrawLine( Pens.Red, X, Y, X + 30, Y + 30 );
                        Target.DrawLine( Pens.Red, X + 30, Y, X, Y + 30 );
                    }
                }
                else
                {
                    if ( ImageCache == null )
                        RefreshCache();
                    if ( ImageCache != null )
                    {
                        Target.DrawImage( ImageCache, Location );
                    }
                    else
                    {
                        Target.DrawLine( Pens.Red, X, Y, X + 30, Y + 30 );
                        Target.DrawLine( Pens.Red, X + 30, Y, X, Y + 30 );
                    }
                }
            }
            catch ( Exception ex )
            {
                ProjectData.SetProjectError( ex );
                MessageBox.Show( string.Format( Resources.Error_drawing_itemID___, ItemID.ToString() ) );
                ItemID = 1;
                ProjectData.ClearProjectError();
            }
        }

        public string ToRunUOString()
        {
            return $"AddImage({X}, {Y}, {ItemID});";
        }
    }
}
