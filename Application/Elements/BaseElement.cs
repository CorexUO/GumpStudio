using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows.Forms;

using GumpStudio.Forms;
using GumpStudio.Plugins;

namespace GumpStudio.Elements
{
	[Serializable]
	public abstract class BaseElement : ISerializable, ICloneable
	{
		protected static readonly Hashtable _IDs = new Hashtable();

		internal static void ResetID()
		{
			_IDs.Clear();
		}

		protected string _Comment;
		protected Point _Location;
		protected string _Name;
		protected GroupElement _Parent;
		protected bool _Selected;
		protected Size _Size;

		[Browsable(false)]
		public virtual Rectangle Bounds => new Rectangle(_Location, _Size);

		[Description("A user defined comment for this element. UO does not use this.")]
		[MergableProperty(false)]
		[ParenthesizePropertyName(true)]
		public string Comment { get => _Comment; set => _Comment = value; }

		[Browsable(false)]
		public virtual int Height { get => _Size.Height; set { } }

		[MergableProperty(false)]
		public Point Location { get => _Location; set => _Location = value; }

		[MergableProperty(false)]
		[ParenthesizePropertyName(true)]
		[Description("A name used to identify the element in the Editor, or in script.  UO does not use this.")]
		public string Name { get => _Name; set => _Name = value; }

		[Browsable(false)]
		[Description("The group elements that this element belongs to.")]
		public GroupElement Parent => _Parent;

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
		public bool Selected { get => _Selected; set => _Selected = value; }

		[Browsable(false)]
		[MergableProperty(true)]
		public virtual Size Size { get => _Size; set { } }

		[MergableProperty(false)]
		public abstract string Type { get; }

		[Browsable(false)]
		public virtual int Width { get => _Size.Width; set { } }

		[Browsable(false)]
		public int X { get => _Location.X; set => _Location.X = value; }

		[Browsable(false)]
		public int Y { get => _Location.Y; set => _Location.Y = value; }

		[Description("The Z order of this element. Highest is on top.")]
		[MergableProperty(false)]
		public int Z
		{
			get => _Parent._Elements.IndexOf(this);
			set
			{
				_Parent._Elements.Remove(this);
				_Parent._Elements.Insert(value, this);
			}
		}

		public event RepaintEventHandler Repaint;
		public event UpdateParentEventHandler UpdateParent;

		public BaseElement()
		{
			long num1;

			if (_IDs.Contains(Type))
			{
				_IDs[Type] = num1 = 1L + Convert.ToInt64(_IDs[Type]);
			}
			else
			{
				_IDs.Add(Type, num1 = 1L);
			}

			_Name = Type + " " + num1;
		}

		public BaseElement(string Name)
		{
			_Name = Name;
		}

		protected BaseElement(SerializationInfo info, StreamingContext context)
		{
			var version = info.GetInt32("BaseElementVersion");

			_Name = info.GetString("Name");
			_Location = (Point)info.GetValue("Location", typeof(Point));
			_Size = (Size)info.GetValue("Size", typeof(Size));
			_Parent = (GroupElement)info.GetValue("Parent", typeof(GroupElement));

			if (version >= 2)
			{
				_Comment = info.GetString("Comment");
			}
			else
			{
				_Comment = String.Empty;
			}
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("BaseElementVersion", 2);
			info.AddValue("Name", _Name);
			info.AddValue("Location", _Location);
			info.AddValue("Size", _Size);
			info.AddValue("Parent", _Parent);
			info.AddValue("Comment", _Comment);
		}

		public virtual void AddContextMenus(ref MenuItem groupMenu, ref MenuItem positionMenu, ref MenuItem orderMenu, ref MenuItem miscMenu)
		{
			var num = _Parent._Elements.Count - 1;

			if (num > 0)
			{
				if (Z < num)
				{
					orderMenu.MenuItems.Add(new MenuItem("Move Front", DoMoveFrontMenu));
					orderMenu.MenuItems.Add(new MenuItem("Move First", DoMoveFirstMenu));
				}

				if (Z >= 1)
				{
					orderMenu.MenuItems.Add(new MenuItem("Move Back", DoMoveBackMenu));
					orderMenu.MenuItems.Add(new MenuItem("Move Last", DoMoveLastMenu));
				}
			}

			var selected = _Parent.GetSelectedElements().Count();

			if (selected > 1)
			{
				positionMenu.MenuItems.Add(new MenuItem("Align Lefts", DoAlignLeftsMenu));
				positionMenu.MenuItems.Add(new MenuItem("Align Rights", DoAlignRightsMenu));
				positionMenu.MenuItems.Add(new MenuItem("Align Tops", DoAlignTopsMenu));
				positionMenu.MenuItems.Add(new MenuItem("Align Bottoms", DoAlignBottomsMenu));
				positionMenu.MenuItems.Add(new MenuItem("-"));
				positionMenu.MenuItems.Add(new MenuItem("Center Horizontally", DoAlignCentersMenu));
				positionMenu.MenuItems.Add(new MenuItem("Center Vertically", DoAlignMiddlesMenu));

				if (selected > 2)
				{
					positionMenu.MenuItems.Add(new MenuItem("-"));
					positionMenu.MenuItems.Add(new MenuItem("Equalize Vertical Spacing", DoVerticalSpacingMenu));
					positionMenu.MenuItems.Add(new MenuItem("Equalize Horizontal Spacing", DoHorizontalSpacingMenu));
				}
			}
		}

		[Description("Creates a copy of this element.")]
		public BaseElement Clone()
		{
			return (BaseElement)CloneMe();
		}

		[Description("Creates a copy of this element.")]
		protected virtual object CloneMe()
		{
			var element = (BaseElement)MemberwiseClone();

			element.RefreshCache();

			RefreshCache();

			return element;
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
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				selectedElement.Y = Y + Height - selectedElement.Height;
			}

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignCentersMenu(object sender, EventArgs e)
		{
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				if (selectedElement != this)
				{
					selectedElement.X = (int)Math.Round(X + Width / 2.0 - selectedElement.Width / 2.0);
				}
			}

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignLeftsMenu(object sender, EventArgs e)
		{
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				selectedElement.X = X;
			}

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignMiddlesMenu(object sender, EventArgs e)
		{
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				if (selectedElement != this)
				{
					selectedElement.Y = (int)Math.Round(Y + Height / 2.0 - selectedElement.Height / 2.0);
				}
			}

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignRightsMenu(object sender, EventArgs e)
		{
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				selectedElement.X = X + Width - selectedElement.Width;
			}

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected virtual void DoAlignTopsMenu(object sender, EventArgs e)
		{
			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				selectedElement.Y = Y;
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

			var elements = new List<BaseElement>();

			var num2 = Int32.MaxValue;

			foreach (var selectedElement in _Parent.GetSelectedElements())
			{
				var num3 = selectedElement.Width / 2 + selectedElement.X;

				if (num3 < num2)
				{
					num2 = num3;
				}

				if (num3 > num1)
				{
					num1 = num3;
				}

				elements.Add(selectedElement);
			}

			elements.Sort(new ElementHorizontalSorter());

			var num4 = (num1 - num2) / (double)(elements.Count - 1);

			double num5 = num2;

			foreach (var element in elements)
			{
				element.X = (int)Math.Round(num5 - element.Width / 2.0);

				num5 += num4;
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

			var elements = new List<BaseElement>();

			var num2 = Int32.MaxValue;

			foreach (var element in _Parent.GetSelectedElements())
			{
				var num3 = element.Height / 2 + element.Y;

				if (num3 < num2)
				{
					num2 = num3;
				}

				if (num3 > num1)
				{
					num1 = num3;
				}

				elements.Add(element);
			}

			elements.Sort(new ElementVerticalSorter());

			var num4 = (num1 - num2) / (double)(elements.Count - 1);

			double num5 = num2;

			foreach (var element in elements)
			{
				element.Y = (int)Math.Round(num5 - element.Height / 2.0);

				num5 += num4;
			}

			RaiseRepaintEvent(this);

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		public virtual void DrawBoundingBox(Graphics Target, bool Active)
		{
			using (var pen = new Pen(Active ? Color.White : Color.DarkGray))
			{
				var bounds = Bounds;

				bounds.Inflate(3, 3);

				Target.DrawRectangle(Pens.Gray, bounds);

				bounds.Inflate(1, 1);

				Target.DrawRectangle(pen, bounds);
			}
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

		public virtual MoveType HitTest(Point Location)
		{
			var rectangle = Rectangle.Inflate(Bounds, 3, 3);
			var moveModeType = MoveType.None;

			if (rectangle.Contains(Location))
			{
				moveModeType = MoveType.Move;
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
			_Parent._Elements.Remove(this);
			_Parent._Elements.Add(this);

			GlobalObjects.DesignerForm.CreateUndoPoint("Move first");
		}

		public void MoveFront()
		{
			if (Z < Parent._Elements.Count - 1)
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
			Repaint?.Invoke(RuntimeHelpers.GetObjectValue(sender));
		}

		public void RaiseUpdateEvent(BaseElement Element, bool ClearSelected)
		{
			UpdateParent?.Invoke(Element, ClearSelected);
		}

		public abstract void RefreshCache();

		public abstract void Render(Graphics Target);

		public void Reparent(GroupElement Parent)
		{
			if (_Parent != null)
			{
				UpdateParent -= _Parent.RaiseUpdateEvent;
			}

			_Parent = Parent;
		}

		public override string ToString()
		{
			return _Name;
		}

		object ICloneable.Clone()
		{
			return Clone();
		}

		[Description("A sorter that will sort elemenets in the ascending order of thier Horizontal center point.")]
		protected class ElementHorizontalSorter : IComparer<BaseElement>
		{
			public int Compare(BaseElement x, BaseElement y)
			{
				var num1 = (x.X + x.Width) / 2;
				var num2 = (y.X + y.Width) / 2;

				if (num1 > num2)
				{
					return 1;
				}

				return num1 >= num2 ? 0 : -1;
			}
		}

		[Description("A sorter that will sort elemenets in the ascending order of thier Vertical center point.")]
		protected class ElementVerticalSorter : IComparer<BaseElement>
		{
			public int Compare(BaseElement x, BaseElement y)
			{
				var num1 = (x.Y + x.Height) / 2;
				var num2 = (y.Y + y.Height) / 2;

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
