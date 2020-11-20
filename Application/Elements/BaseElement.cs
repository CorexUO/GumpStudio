// Decompiled with JetBrains decompiler
// Type: GumpStudio.Elements.BaseElement
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using GumpStudio.Plugins;

using Microsoft.VisualBasic.CompilerServices;

namespace GumpStudio.Elements
{
	[Serializable]
	public abstract class BaseElement : ISerializable, ICloneable
	{
		protected static Hashtable mIDs = new Hashtable();
		protected string mComment;
		protected static ArrayList mExtenders;
		protected Point mLocation;
		protected string mName;
		protected GroupElement mParent;
		protected bool mSelected;
		protected Size mSize;

		[Browsable(false)]
		public virtual Rectangle Bounds => new Rectangle(mLocation, mSize);

		[Description("A user defined comment for this element. UO does not use this.")]
		[MergableProperty(false)]
		[ParenthesizePropertyName(true)]
		public string Comment
		{
			get => mComment;
			set => mComment = value;
		}

		[Browsable(false)]
		public virtual int Height
		{
			get => mSize.Height;
			set
			{
			}
		}

		[MergableProperty(false)]
		public Point Location
		{
			get => mLocation;
			set => mLocation = value;
		}

		[MergableProperty(false)]
		[ParenthesizePropertyName(true)]
		[Description("A name used to identify the element in the Editor, or in script.  UO does not use this.")]
		public string Name
		{
			get => mName;
			set => mName = value;
		}

		[Browsable(false)]
		[Description("The group elements that this element belongs to.")]
		public GroupElement Parent => mParent;

		[Browsable(false)]
		[Description("The group elements that this element belongs to.")]
		public GroupElement RootParent
		{
			get
			{
				var p = Parent;

				while (p?.Parent != null)
				{
					p = p.Parent;
				}

				return p;
			}
		}

		[Browsable(false)]
		public bool Selected
		{
			get => mSelected;
			set => mSelected = value;
		}

		[Browsable(false)]
		[MergableProperty(true)]
		public virtual Size Size
		{
			get => mSize;
			set
			{
			}
		}

		[MergableProperty(false)]
		public abstract string Type { get; }

		[Browsable(false)]
		public virtual int Width
		{
			get => mSize.Width;
			set
			{
			}
		}

		[Browsable(false)]
		public int X
		{
			get => mLocation.X;
			set => mLocation.X = value;
		}

		[Browsable(false)]
		public int Y
		{
			get => mLocation.Y;
			set => mLocation.Y = value;
		}

		[Description("The Z order of this element. Highest is on top.")]
		[MergableProperty(false)]
		public int Z
		{
			get => mParent.mElements.IndexOf(this);
			set
			{
				mParent.mElements.Remove(this);
				mParent.mElements.Insert(value, this);
			}
		}

		public event BaseElement.RepaintEventHandler Repaint;

		public event BaseElement.UpdateParentEventHandler UpdateParent;

		public BaseElement()
		{
			long num1;
			if (BaseElement.mIDs.Contains(Type))
			{
				var num2 = Conversions.ToLong(BaseElement.mIDs[Type]) + 1L;
				BaseElement.mIDs[Type] = num2;
				num1 = num2;
			}
			else
			{
				BaseElement.mIDs.Add(Type, 1);
				num1 = 1L;
			}
			mName = Type + " " + Conversions.ToString(num1);
		}

		public BaseElement(string Name)
		{
			mName = Name;
		}

		protected BaseElement(SerializationInfo info, StreamingContext context)
		{
			var int32 = info.GetInt32("BaseElementVersion");
			mName = info.GetString(nameof(Name));
			mLocation = (Point)info.GetValue(nameof(Location), typeof(Point));
			mSize = (Size)info.GetValue(nameof(Size), typeof(Size));
			mParent = (GroupElement)info.GetValue(nameof(Parent), typeof(GroupElement));
			if (int32 >= 2)
			{
				mComment = info.GetString(nameof(Comment));
			}
			else
			{
				mComment = "";
			}
		}

		public virtual void AddContextMenus(ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu)
		{
			var num = mParent.GetElements().Count - 1;
			if (num > 0)
			{
				if (Z < num)
				{
					OrderMenu.MenuItems.Add(new MenuItem("Move Front", new EventHandler(DoMoveFrontMenu)));
					OrderMenu.MenuItems.Add(new MenuItem("Move First", new EventHandler(DoMoveFirstMenu)));
				}
				if (Z >= 1)
				{
					OrderMenu.MenuItems.Add(new MenuItem("Move Back", new EventHandler(DoMoveBackMenu)));
					OrderMenu.MenuItems.Add(new MenuItem("Move Last", new EventHandler(DoMoveLastMenu)));
				}
			}
			if (mParent.GetSelectedElements().Count > 1)
			{
				PositionMenu.MenuItems.Add(new MenuItem("Align Lefts", new EventHandler(DoAlignLeftsMenu)));
				PositionMenu.MenuItems.Add(new MenuItem("Align Rights", new EventHandler(DoAlignRightsMenu)));
				PositionMenu.MenuItems.Add(new MenuItem("Align Tops", new EventHandler(DoAlignTopsMenu)));
				PositionMenu.MenuItems.Add(new MenuItem("Align Bottoms", new EventHandler(DoAlignBottomsMenu)));
				PositionMenu.MenuItems.Add(new MenuItem("-"));
				PositionMenu.MenuItems.Add(new MenuItem("Center Horizontally", new EventHandler(DoAlignCentersMenu)));
				PositionMenu.MenuItems.Add(new MenuItem("Center Vertically", new EventHandler(DoAlignMiddlesMenu)));
				if (mParent.GetSelectedElements().Count > 2)
				{
					PositionMenu.MenuItems.Add(new MenuItem("-"));
					PositionMenu.MenuItems.Add(new MenuItem("Equalize Vertical Spacing", new EventHandler(DoVerticalSpacingMenu)));
					PositionMenu.MenuItems.Add(new MenuItem("Equalize Horizontal Spacing", new EventHandler(DoHorizontalSpacingMenu)));
				}
			}
			if (BaseElement.mExtenders == null)
			{
				return;
			}

			IEnumerator enumerator = null;
			try
			{
				foreach (var mExtender in BaseElement.mExtenders)
				{
					((ElementExtender)RuntimeHelpers.GetObjectValue(mExtender)).AddContextMenus(ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu);
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
		}

		[Description("Inserts an element extender into this element type's extendor lsit.")]
		public void AddExtender(ElementExtender Extender)
		{
			if (BaseElement.mExtenders == null)
			{
				BaseElement.mExtenders = new ArrayList();
			}

			if (BaseElement.mExtenders.Contains(Extender))
			{
				return;
			}

			BaseElement.mExtenders.Add(Extender);
		}

		[Description("Creates a copy of this element.")]
		public BaseElement Clone()
		{
			return (BaseElement)CloneMe();
		}

		[Description("Creates a copy of this element.")]
		protected virtual object CloneMe()
		{
			var baseElement = (BaseElement)MemberwiseClone();
			baseElement.RefreshCache();
			RefreshCache();
			return baseElement;
		}

		public virtual bool ContainsTest(Rectangle Rect)
		{
			return Rect.IntersectsWith(Bounds);
		}

		public virtual void DebugDump()
		{
		}

		public virtual bool DispayInAbout()
		{
			return false;
		}

		protected virtual void DoAlignBottomsMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
					objectValue.Y = Y + Height - objectValue.Height;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignCentersMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = RuntimeHelpers.GetObjectValue(selectedElement);
					if (objectValue != this)
					{
						var baseElement = (BaseElement)objectValue;
						baseElement.X = (int)Math.Round(X + Width / 2.0 - baseElement.Width / 2.0);
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignLeftsMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					((BaseElement)RuntimeHelpers.GetObjectValue(selectedElement)).X = X;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignMiddlesMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = RuntimeHelpers.GetObjectValue(selectedElement);
					if (objectValue != this)
					{
						var baseElement = (BaseElement)objectValue;
						baseElement.Y = (int)Math.Round(Y + Height / 2.0 - baseElement.Height / 2.0);
					}
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignRightsMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
					objectValue.X = X + Width - objectValue.Width;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignTopsMenu(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					((BaseElement)RuntimeHelpers.GetObjectValue(selectedElement)).Y = Y;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoDebugDumpMenu(object sender, EventArgs e)
		{
			DebugDump();
		}

		protected virtual void DoHorizontalSpacingMenu(object sender, EventArgs e)
		{
			var num1 = 0;
			IEnumerator enumerator1 = null;
			IEnumerator enumerator2 = null;
			var arrayList = new ArrayList();
			var num2 = Int32.MaxValue;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
					var num3 = objectValue.Width / 2 + objectValue.X;
					if (num3 < num2)
					{
						num2 = num3;
					}

					if (num3 > num1)
					{
						num1 = num3;
					}

					arrayList.Add(objectValue);
				}
			}
			finally
			{
				if (enumerator1 is IDisposable)
				{
					(enumerator1 as IDisposable).Dispose();
				}
			}
			arrayList.Sort(new BaseElement.ElementHorizontalSorter());
			var num4 = (num1 - num2) / (double)(arrayList.Count - 1);
			double num5 = num2;
			try
			{
				foreach (var obj in arrayList)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(obj);
					objectValue.X = (int)Math.Round(num5 - objectValue.Width / 2.0);
					num5 += num4;
				}
			}
			finally
			{
				if (enumerator2 is IDisposable)
				{
					(enumerator2 as IDisposable).Dispose();
				}
			}
			RaiseRepaintEvent(this);
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoMoveBackMenu(object sender, EventArgs e)
		{
			MoveBack();
		}

		protected virtual void DoMoveFirstMenu(object sender, EventArgs e)
		{
			MoveFirst();
		}

		protected virtual void DoMoveFrontMenu(object sender, EventArgs e)
		{
			MoveFront();
		}

		protected virtual void DoMoveLastMenu(object sender, EventArgs e)
		{
			MoveLast();
		}

		protected virtual void DoVerticalSpacingMenu(object sender, EventArgs e)
		{
			var num1 = 0;
			IEnumerator enumerator1 = null;
			IEnumerator enumerator2 = null;
			var arrayList = new ArrayList();
			var num2 = Int32.MaxValue;
			try
			{
				foreach (var selectedElement in mParent.GetSelectedElements())
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
					var num3 = objectValue.Height / 2 + objectValue.Y;
					if (num3 < num2)
					{
						num2 = num3;
					}

					if (num3 > num1)
					{
						num1 = num3;
					}

					arrayList.Add(objectValue);
				}
			}
			finally
			{
				if (enumerator1 is IDisposable)
				{
					(enumerator1 as IDisposable).Dispose();
				}
			}
			arrayList.Sort(new BaseElement.ElementVerticalSorter());
			var num4 = (num1 - num2) / (double)(arrayList.Count - 1);
			double num5 = num2;
			try
			{
				foreach (var obj in arrayList)
				{
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(obj);
					objectValue.Y = (int)Math.Round(num5 - objectValue.Height / 2.0);
					num5 += num4;
				}
			}
			finally
			{
				if (enumerator2 is IDisposable)
				{
					(enumerator2 as IDisposable).Dispose();
				}
			}
			RaiseRepaintEvent(this);
			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		public virtual void DrawBoundingBox(Graphics Target, bool Active)
		{
			var bounds = Bounds;
			bounds.Inflate(3, 3);
			var pen = !Active ? new Pen(Color.DarkGray) : new Pen(Color.White);
			Target.DrawRectangle(Pens.Gray, bounds);
			bounds.Inflate(1, 1);
			Target.DrawRectangle(pen, bounds);
			pen.Dispose();
		}

		public virtual string GetAboutText()
		{
			return "You should override GetAboutText() to change this.";
		}

		[Description("Returns the Element's location, taking into account the offset of all parent elements.  Export plugins should use this to get the position for scripts.")]
		public virtual Point GetAbsolutePosition()
		{
			if (Parent == null)
			{
				return Location;
			}

			var absolutePosition = Parent.GetAbsolutePosition();
			absolutePosition.Offset(X, Y);
			return absolutePosition;
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("BaseElementVersion", 2);
			info.AddValue("Name", mName);
			info.AddValue("Location", mLocation);
			info.AddValue("Size", mSize);
			info.AddValue("Parent", mParent);
			info.AddValue("Comment", mComment);
		}

		public virtual MoveModeType HitTest(Point Location)
		{
			var rectangle = Rectangle.Inflate(Bounds, 3, 3);
			var moveModeType = MoveModeType.None;
			if (rectangle.Contains(Location))
			{
				moveModeType = MoveModeType.Move;
			}

			return moveModeType;
		}

		public void MoveBack()
		{
			if (Z > 0)
			{
				--Z;
			}

			GlobalObjects.DesignerForm.CreateUndoPoint("Move back");
		}

		public void MoveFirst()
		{
			mParent.mElements.Remove(this);
			mParent.mElements.Add(this);
			GlobalObjects.DesignerForm.CreateUndoPoint("Move first");
		}

		public void MoveFront()
		{
			if (Z < Parent.mElements.Count - 1)
			{
				++Z;
			}

			GlobalObjects.DesignerForm.CreateUndoPoint("Move front");
		}

		public void MoveLast()
		{
			Z = 0;
			GlobalObjects.DesignerForm.CreateUndoPoint("Move last");
		}

		public void RaiseRepaintEvent(object sender)
		{
			var repaint = Repaint;
			if (repaint == null)
			{
				return;
			}

			repaint(RuntimeHelpers.GetObjectValue(sender));
		}

		public void RaiseUpdateEvent(BaseElement Element, bool ClearSelected)
		{
			var updateParent = UpdateParent;
			if (updateParent == null)
			{
				return;
			}

			updateParent(Element, ClearSelected);
		}

		public abstract void RefreshCache();

		public abstract void Render(Graphics Target);

		public void Reparent(GroupElement Parent)
		{
			if (mParent != null)
			{
				UpdateParent -= new BaseElement.UpdateParentEventHandler(mParent.RaiseUpdateEvent);
			}

			mParent = Parent;
		}

		internal static void ResetID()
		{
			BaseElement.mIDs = new Hashtable();
		}

		public override string ToString()
		{
			return mName;
		}

		object ICloneable.Clone()
		{
			return null;
		}

		[Description("A sorter that will sort elemenets in the ascending order of thier Horizontal center point.")]
		protected class ElementHorizontalSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				var baseElement1 = (BaseElement)x;
				var baseElement2 = (BaseElement)y;
				var num1 = (baseElement1.X + baseElement1.Width) / 2;
				var num2 = (baseElement2.X + baseElement2.Width) / 2;
				if (num1 > num2)
				{
					return 1;
				}

				return num1 >= num2 ? 0 : -1;
			}
		}

		[Description("A sorter that will sort elemenets in the ascending order of thier Vertical center point.")]
		protected class ElementVerticalSorter : IComparer
		{
			public int Compare(object x, object y)
			{
				var baseElement1 = (BaseElement)x;
				var baseElement2 = (BaseElement)y;
				var num1 = (baseElement1.Y + baseElement1.Height) / 2;
				var num2 = (baseElement2.Y + baseElement2.Height) / 2;
				if (num1 > num2)
				{
					return 1;
				}

				return num1 >= num2 ? 0 : -1;
			}
		}

		public delegate void RepaintEventHandler(object sender);

		public delegate void UpdateParentEventHandler(BaseElement Element, bool ClearSelected);
	}
}
