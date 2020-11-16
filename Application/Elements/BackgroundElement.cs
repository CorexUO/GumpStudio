// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.BackgroundElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

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
	public class BackgroundElement : ResizeableElement, IDisposable, ICSharpExportable
	{
		protected int mGumpID;
		protected Image[] mMultImageCache;

		[Editor(typeof(GumpIDPropEditor), typeof(UITypeEditor))]
		public int GumpID
		{
			get => mGumpID;
			set
			{
				var flag = true;
				var num1 = 0;
				int num2;
				do
				{
					var gump = Gumps.GetGump(num1 + value);
					if (gump == null)
					{
						flag = false;
					}

					gump.Dispose();
					++num1;
					num2 = 8;
				}
				while (num1 <= num2);
				if (!flag)
				{
					//int num3 = (int) Interaction.MsgBox((object) "Invalid GumpID", MsgBoxStyle.OkOnly, (object) null);
					MessageBox.Show(Resources.Invalid_GumpID, Resources.Invalid_GumpID);
				}
				else
				{
					mGumpID = value;
					RefreshCache();
				}
			}
		}

		public override string Type => "Background";

		public BackgroundElement()
		{
			mMultImageCache = new Image[9];
			mSize = new Size(100, 100);
			mGumpID = 9200;
			RefreshCache();
		}

		public BackgroundElement(SerializationInfo info, StreamingContext context)
		  : base(info, context)
		{
			mMultImageCache = new Image[9];
			info.GetInt32("BackgroundElementVersion");
			GumpID = info.GetInt32(nameof(GumpID));
		}

		public void Dispose()
		{
			var index = 0;
			int num;
			do
			{
				if (mMultImageCache[index] != null)
				{
					mMultImageCache[index].Dispose();
				}

				++index;
				num = 8;
			}
			while (index <= num);
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("BackgroundElementVersion", 1);
			info.AddValue("GumpID", mGumpID);
		}

		public override void RefreshCache()
		{
			if (mMultImageCache == null)
			{
				mMultImageCache = new Image[9];
			}

			var index = 0;
			int num;
			do
			{
				if (mMultImageCache[index] != null)
				{
					mMultImageCache[index].Dispose();
				}

				mMultImageCache[index] = Gumps.GetGump(index + mGumpID);
				++index;
				num = 8;
			}
			while (index <= num);
		}

		public override void Render(Graphics Target)
		{
			if (mMultImageCache == null)
			{
				RefreshCache();
			}

			var index = 0;
			int num1;
			do
			{
				if (mMultImageCache[index] == null)
				{
					RefreshCache();
				}

				++index;
				num1 = 8;
			}
			while (index <= num1);
			var clip = Target.Clip;
			var rect = new Rectangle(X, Y, mMultImageCache[0].Width, mMultImageCache[0].Height);
			var region1 = new Region(rect);
			Target.Clip = region1;
			Target.DrawImage(mMultImageCache[0], Location);
			region1.Dispose();
			rect = new Rectangle(X, Y, Width - mMultImageCache[2].Width, Height);
			var region2 = new Region(rect);
			Target.Clip = region2;
			var width1 = mMultImageCache[1].Width;
			var num2 = Width - mMultImageCache[2].Width;
			var width2 = mMultImageCache[0].Width;
			Point point;
			while ((width1 >> 31 ^ width2) <= (width1 >> 31 ^ num2))
			{
				point = new Point(X + width2, Y);
				Target.DrawImage(mMultImageCache[1], point);
				width2 += width1;
			}
			region2.Dispose();
			rect = new Rectangle(X + Width - mMultImageCache[0].Width, Y, mMultImageCache[0].Width, Height);
			var region3 = new Region(rect);
			Target.Clip = region3;
			point = new Point(X + Width - mMultImageCache[2].Width, Y);
			Target.DrawImage(mMultImageCache[2], point);
			region3.Dispose();
			rect = new Rectangle(X, Y, mMultImageCache[0].Width, Height - mMultImageCache[6].Height);
			var region4 = new Region(rect);
			Target.Clip = region4;
			var height1 = mMultImageCache[3].Height;
			var num3 = Height - mMultImageCache[6].Height;
			var height2 = mMultImageCache[0].Height;
			while ((height1 >> 31 ^ height2) <= (height1 >> 31 ^ num3))
			{
				point = new Point(X, Y + height2);
				Target.DrawImage(mMultImageCache[3], point);
				height2 += height1;
			}
			region4.Dispose();
			rect = new Rectangle(X, Y + Height - mMultImageCache[6].Height, mMultImageCache[6].Width, mMultImageCache[6].Height);
			var region5 = new Region(rect);
			Target.Clip = region5;
			point = new Point(X, Y + Height - mMultImageCache[6].Height);
			Target.DrawImage(mMultImageCache[6], point);
			region5.Dispose();
			rect = new Rectangle(X, Y + Height - mMultImageCache[7].Height, Width - mMultImageCache[6].Width, mMultImageCache[7].Height);
			var region6 = new Region(rect);
			Target.Clip = region6;
			var width3 = mMultImageCache[7].Width;
			var num4 = Width - mMultImageCache[8].Width;
			var width4 = mMultImageCache[6].Width;
			while ((width3 >> 31 ^ width4) <= (width3 >> 31 ^ num4))
			{
				point = new Point(X + width4, Y + Height - mMultImageCache[7].Height);
				Target.DrawImage(mMultImageCache[7], point);
				width4 += width3;
			}
			region6.Dispose();
			rect = new Rectangle(X + Width - mMultImageCache[8].Width, Y + Height - mMultImageCache[8].Height, mMultImageCache[8].Width, mMultImageCache[8].Height);
			var region7 = new Region(rect);
			Target.Clip = region7;
			point = new Point(X + Width - mMultImageCache[8].Width, Y + Height - mMultImageCache[8].Height);
			Target.DrawImage(mMultImageCache[8], point);
			region7.Dispose();
			rect = new Rectangle(X + Width - mMultImageCache[5].Width, Y + mMultImageCache[2].Height, mMultImageCache[5].Width, Height - mMultImageCache[8].Height - mMultImageCache[2].Height);
			var region8 = new Region(rect);
			Target.Clip = region8;
			var height3 = mMultImageCache[5].Height;
			var num5 = Height - mMultImageCache[6].Height;
			var height4 = mMultImageCache[0].Height;
			while ((height3 >> 31 ^ height4) <= (height3 >> 31 ^ num5))
			{
				point = new Point(X + Width - mMultImageCache[5].Width, Y + height4);
				Target.DrawImage(mMultImageCache[5], point);
				height4 += height3;
			}
			region8.Dispose();
			rect = new Rectangle(X + mMultImageCache[3].Width, Y + mMultImageCache[1].Height, Width - mMultImageCache[3].Width - mMultImageCache[5].Width, Height - mMultImageCache[7].Height - mMultImageCache[1].Height);
			var region9 = new Region(rect);
			Target.Clip = region9;
			var width5 = mMultImageCache[4].Width;
			var num6 = Width - mMultImageCache[3].Width;
			var width6 = mMultImageCache[3].Width;
			while ((width5 >> 31 ^ width6) <= (width5 >> 31 ^ num6))
			{
				var height5 = mMultImageCache[4].Height;
				var num7 = Height - mMultImageCache[7].Height;
				var height6 = mMultImageCache[1].Height;
				while ((height5 >> 31 ^ height6) <= (height5 >> 31 ^ num7))
				{
					point = new Point(X + width6, Y + height6);
					Target.DrawImage(mMultImageCache[4], point);
					height6 += height5;
				}
				width6 += width5;
			}
			region9.Dispose();
			Target.Clip = clip;
		}

		public string ToCSharpString()
		{
			return $"AddBackground({X}, {Y}, {Width}, {Height}, {GumpID});";
		}
	}
}
