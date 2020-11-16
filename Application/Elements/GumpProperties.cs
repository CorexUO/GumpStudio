// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.GumpProperties
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace GumpStudio.Elements
{
	[Serializable]
	public class GumpProperties : ISerializable, ICloneable
	{
		protected bool mCloseable;
		protected bool mDisposeable;
		protected Point mLocation;
		protected bool mMoveable;
		protected int mType;

		public bool Closeable
		{
			get => mCloseable;
			set => mCloseable = value;
		}

		public bool Disposeable
		{
			get => mDisposeable;
			set => mDisposeable = value;
		}

		public Point Location
		{
			get => mLocation;
			set => mLocation = value;
		}

		public bool Moveable
		{
			get => mMoveable;
			set => mMoveable = value;
		}

		public int Type
		{
			get => mType;
			set => mType = value;
		}

		public GumpProperties()
		{
			mMoveable = true;
			mCloseable = true;
			mDisposeable = true;
		}

		protected GumpProperties(SerializationInfo info, StreamingContext context)
		{
			mMoveable = true;
			mCloseable = true;
			mDisposeable = true;
			info.GetInt32("Version");
			mLocation = (Point)info.GetValue(nameof(Location), typeof(Point));
			mMoveable = info.GetBoolean(nameof(Moveable));
			mCloseable = info.GetBoolean(nameof(Closeable));
			mDisposeable = info.GetBoolean(nameof(Disposeable));
			mType = info.GetInt32(nameof(Type));
		}

		public object Clone()
		{
			return MemberwiseClone();
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Version", 1);
			info.AddValue("Location", mLocation);
			info.AddValue("Moveable", mMoveable);
			info.AddValue("Closeable", mCloseable);
			info.AddValue("Disposeable", mDisposeable);
			info.AddValue("Type", mType);
		}
	}
}
