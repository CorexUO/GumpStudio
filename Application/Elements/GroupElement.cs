using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using Microsoft.VisualBasic.CompilerServices;

namespace GumpStudio.Elements
{
	[Serializable]
	public class GroupElement : BaseElement
	{
		internal ArrayList _Elements;
		internal bool _IsBaseWindow;

		public int Items => _Elements.Count;

		public override string Type => "Group";

		public IEnumerable<BaseElement> Elements => _Elements.Cast<BaseElement>();
		public IEnumerable<BaseElement> SelectedElements => GetSelectedElements();
		public IEnumerable<BaseElement> AllElements => GetElementsRecursive();

		public GroupElement(GroupElement Parent)
		  : this(Parent, null, null, false)
		{ }

		protected GroupElement(SerializationInfo info, StreamingContext context)
		  : base(info, context)
		{
			info.GetInt32("GroupElementVersion");

			_Elements = (ArrayList)info.GetValue("Elements", typeof(ArrayList));
			_IsBaseWindow = info.GetBoolean("IsBaseWindow");
		}

		public GroupElement(GroupElement Parent, ArrayList Elements, string Name)
		  : this(Parent, Elements, Name, false)
		{ }

		public GroupElement(GroupElement Parent, ArrayList elements, string Name, bool isBaseWindow)
		{
			_IsBaseWindow = isBaseWindow;

			_Elements = new ArrayList();

			if (Name != null)
			{
				_Name = Name;
			}

			if (elements != null)
			{
				_Elements.AddRange(elements);
			}

			Location = new Point(0, 0);
		}

		public override void AddContextMenus(ref MenuItem GroupMenu, ref MenuItem PositionMenu, ref MenuItem OrderMenu, ref MenuItem MiscMenu)
		{
			base.AddContextMenus(ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu);

			if (_Parent.GetSelectedElements().Count() > 1)
			{
				GroupMenu.MenuItems.Add(new MenuItem("Add Selection to Group", DoAddMenu));
			}

			GroupMenu.MenuItems.Add(new MenuItem("Break Group", DoBreakGroupMenu));
			MiscMenu.MenuItems.Add(new MenuItem("Export Gumpling", DoExportGumplingMenu));
		}

		public virtual void AddElement(BaseElement e)
		{
			if (_Elements.Contains(e))
			{
				return;
			}

			if (!_IsBaseWindow)
			{
				Rectangle rectangle;

				if (_Elements.Count == 0)
				{
					rectangle = e.Bounds;
				}
				else
				{
					rectangle = Rectangle.Union(Bounds, e.Bounds);

					if (X != rectangle.X | Y != rectangle.Y)
					{
						var dx = X - rectangle.X;
						var dy = Y - rectangle.Y;

						foreach (var mElement in _Elements)
						{
							var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(mElement);
							var location = objectValue.Location;
							location.Offset(dx, dy);
							objectValue.Location = location;
						}
					}
				}

				Location = rectangle.Location;
				_Size = rectangle.Size;

				var location1 = e.Location;

				location1.X -= rectangle.Location.X;
				location1.Y -= rectangle.Location.Y;

				e.Location = location1;
			}

			_Elements.Add(e);

			e.Reparent(this);

			AttachEvents(e);
		}

		public void AttachEvents(BaseElement Element)
		{
			Element.UpdateParent += RaiseUpdateEvent;
			Element.Repaint += RaiseRepaintEvent;
		}

		public void BreakGroup()
		{
			foreach (BaseElement element in _Elements)
			{
				_Parent.AddElement(element);

				element.Selected = true;
				element.Location = new Point(X + element.X, Y + element.Y);
			}

			_Parent.RemoveElement(this);
		}

		protected override object CloneMe()
		{
			var parent = (GroupElement)base.CloneMe();

			parent._Elements = new ArrayList();

			foreach (BaseElement element in _Elements)
			{
				var clone = element.Clone();

				parent._Elements.Add(clone);
				parent.AttachEvents(clone);

				clone.Reparent(parent);
			}

			return parent;
		}

		public override void DebugDump()
		{
			base.DebugDump();

			foreach (BaseElement element in _Elements)
			{
				element.DebugDump();
			}
		}

		protected void DoAddMenu(object sender, EventArgs e)
		{
			var arrayList = new ArrayList(_Parent._Elements);

			foreach (BaseElement element in arrayList)
			{
				if (element != this && element.Selected)
				{
					AddElement(element);

					_Parent.RemoveElement(element);

					element.Selected = false;
				}
			}
		}

		protected void DoBreakGroupMenu(object sender, EventArgs e)
		{
			BreakGroup();

			_Parent.RaiseUpdateEvent(null, false);

			GlobalObjects.DesignerForm.CreateUndoPoint();
		}

		protected void DoExportGumplingMenu(object sender, EventArgs e)
		{
			try
			{
				var saveFileDialog = new SaveFileDialog
				{
					Filter = "Gumpling|*.gumpling",
					AddExtension = true
				};

				if (saveFileDialog.ShowDialog() == DialogResult.OK)
				{
					var parent = _Parent;

					_Parent.RemoveElement(this);
					_Parent = null;

					using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
					{
						new BinaryFormatter().Serialize(fileStream, this);
					}

					_Parent = parent;
					_Parent.AddElement(this);
				}

				saveFileDialog.Dispose();
			}
			catch (Exception ex)
			{
				ProjectData.SetProjectError(ex);
				MessageBox.Show(ex.Message);
				ProjectData.ClearProjectError();
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("GroupElementVersion", 1);
			info.AddValue("Elements", _Elements);
			info.AddValue("IsBaseWindow", _IsBaseWindow);
		}

		public BaseElement GetElementFromPoint(Point p)
		{
			BaseElement baseElement = null;

			foreach (BaseElement element in _Elements)
			{
				var bounds = element.Bounds;

				bounds.Inflate(7, 7);

				if (bounds.Contains(p))
				{
					baseElement = element;
				}
			}

			return baseElement;
		}

		public IEnumerable<BaseElement> GetElementsRecursive()
		{
			foreach (BaseElement element in _Elements)
			{
				yield return element;

				if (element is GroupElement g)
				{
					foreach (var e in g.GetElementsRecursive())
					{
						yield return e;
					}
				}
			}
		}

		public IEnumerable<BaseElement> GetSelectedElements()
		{
			foreach (BaseElement element in _Elements)
			{
				if (element.Selected)
				{
					yield return element;
				}

				if (element is GroupElement g)
				{
					foreach (var e in g.GetSelectedElements())
					{
						yield return e;
					}
				}
			}
		}

		public void RecalculateBounds()
		{
			var count = _Elements.Count;

			if (count < 1)
			{
				return;
			}

			var e = (BaseElement)_Elements[0];
			var a = e.Bounds;

			if (count > 1)
			{
				var num = count - 1;

				for (var index = 1; index <= num; ++index)
				{
					e = (BaseElement)_Elements[index];
					a = Rectangle.Union(a, e.Bounds);
				}
			}

			_Size = a.Size;

			RaiseRepaintEvent(this);
		}

		public override void RefreshCache()
		{
			foreach (BaseElement element in _Elements)
			{
				element.RefreshCache();
			}
		}

		public void RemoveElement(BaseElement e)
		{
			_Elements.Remove(e);

			RemoveEvents(e);
		}

		public void RemoveEvents(BaseElement Element)
		{
			Element.UpdateParent -= RaiseUpdateEvent;
			Element.Repaint -= RaiseRepaintEvent;
		}

		public override void Render(Graphics Target)
		{
			Target.TranslateTransform(X, Y);

			foreach (BaseElement element in _Elements)
			{
				element.Render(Target);
			}

			Target.TranslateTransform(-X, -Y);
		}
	}
}
