// Decompiled with JetBrains decompiler
// Type: GumpStudio.DesignerForm
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

using GumpStudio.Elements;
using GumpStudio.Plugins;

using Ultima;
// ReSharper disable RedundantDelegateCreation

namespace GumpStudio.Forms
{
	public class DesignerForm : Form
	{
		private TextBox m_CanvasFocus;
		private ComboBox m_cboElements;
		private PropertyGrid m_ElementProperties;
		private Label m_Label1;
		private MainMenu m_MainMenu;
		private MenuItem m_MenuItem1;
		private MenuItem m_MenuItem10;
		private MenuItem m_MenuItem3;
		private MenuItem m_MenuItem4;
		private MenuItem m_MenuItem5;
		private MenuItem m_MenuItem9;
		private ContextMenu m_mnuContextMenu;
		private MenuItem m_mnuCopy;
		private MenuItem _mnuCut;
		private MenuItem _mnuDataFile;
		private MenuItem _mnuDelete;
		private MenuItem _mnuEdit;
		private MenuItem _mnuEditRedo;
		private MenuItem _mnuEditUndo;
		private MenuItem _mnuFile;
		private MenuItem _mnuFileExit;
		private MenuItem _mnuFileExport;
		private MenuItem _mnuFileImport;
		private MenuItem _mnuFileNew;
		private MenuItem _mnuFileOpen;
		private MenuItem _mnuFileSave;
		private MenuItem _mnuGumplingAddFolder;
		private MenuItem _mnuGumplingAddGumpling;
		private ContextMenu _mnuGumplingContext;
		private MenuItem _mnuGumplingDelete;
		private MenuItem _mnuGumplingMove;
		private MenuItem _mnuGumplingRename;
		private MenuItem _mnuHelp;
		private MenuItem _mnuHelpAbout;
		private MenuItem _mnuImportGumpling;
		private MenuItem _mnuMisc;
		private MenuItem _mnuMiscLoadGumpling;
		private MenuItem _mnuPage;
		private MenuItem _mnuPageAdd;
		private MenuItem _mnuPageClear;
		private MenuItem _mnuPageDelete;
		private MenuItem _mnuPageInsert;
		private MenuItem _mnuPaste;
		private MenuItem _mnuPluginManager;
		private MenuItem _mnuPlugins;
		private MenuItem _mnuSelectAll;
		private MenuItem _mnuShow0;
		private OpenFileDialog _OpenDialog;
		private Panel _Panel1;
		private Panel _Panel2;
		private Panel _Panel3;
		private Panel _Panel4;
		private PictureBox _picCanvas;
		private Panel _pnlCanvasScroller;
		private Panel _pnlToolbox;
		private Panel _pnlToolboxHolder;
		private SaveFileDialog _SaveDialog;
		private Splitter _Splitter1;
		private Splitter _Splitter2;
		private StatusBar _StatusBar;
		private TabPage _TabPage1;
		private TabControl _TabPager;
		private TabControl _tabToolbox;
		private TabPage _tpgCustom;
		private TabPage _tpgStandard;
		private TreeView _treGumplings;
		protected string AboutElementAppend;
		public BaseElement ActiveElement;
		public string AppPath;
		public decimal ArrowKeyDelta;
		protected ArrayList AvailablePlugins;
		protected Bitmap Canvas;
		private IContainer components;
		protected ClipBoardMode CopyMode;
		protected int CurrentUndoPoint;
		protected bool ElementChanged;
		public GroupElement ElementStack;
		protected string FileName;
		public TreeFolder GumplingsFolder;
		public TreeFolder GumplingTree;
		public GumpProperties GumpProperties;
		protected Point LastPos;
		protected ArrayList LoadedPlugins;
		protected Point mAnchor;
		protected Size mAnchorOffset;
		public int MaxUndoPoints;
		protected int MoveCount;
		protected MoveModeType MoveMode;
		public bool PluginClearsCanvas;
		public PluginInfo[] PluginTypesToLoad;
		protected ArrayList RegisteredTypes;
		protected Point ScrollPos;
		protected LinearGradientBrush SelBG;
		protected Rectangle SelectionRect;
		protected Pen SelFG;
		public bool ShouldClearActiveElement;
		protected bool ShowGrid;
		protected bool ShowPage0;
		protected bool ShowSelectionRect;
		public ArrayList Stacks;
		public bool SuppressUndoPoints;
		public TreeFolder UncategorizedFolder;
		protected ArrayList UndoPoints;

		public virtual TextBox CanvasFocus
		{

			get => m_CanvasFocus;
			[DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)]
			set => m_CanvasFocus = value;
		}



		public virtual MenuItem mnuFileExport
		{
			get => _mnuFileExport;
			set => _mnuFileExport = value;
		}

		public virtual MenuItem mnuFileImport
		{
			get => _mnuFileImport;
			set => _mnuFileImport = value;
		}

		public MenuItem mnuPlugins
		{
			get => _mnuPlugins;
			set => _mnuPlugins = value;
		}

		public PictureBox picCanvas
		{
			get => _picCanvas;
			set => _picCanvas = value;
		}

		public event HookKeyDownEventHandler HookKeyDown;
		public event HookPostRenderEventHandler HookPostRender;
		public event HookPreRenderEventHandler HookPreRender;

		public DesignerForm()
		{
			Closed += new EventHandler(DesignerForm_Closed);
			Closing += new CancelEventHandler(DesignerForm_Closing);
			ElementStack = new GroupElement(null, null, "CanvasStack", true);
			Stacks = new ArrayList();
			ShouldClearActiveElement = false;
			PluginClearsCanvas = false;
			AppPath = Application.StartupPath;
			ArrowKeyDelta = new decimal(1);
			ShowSelectionRect = false;
			MoveMode = MoveModeType.None;
			ShowGrid = false;
			ShowPage0 = true;
			ElementChanged = false;
			UndoPoints = new ArrayList();
			CurrentUndoPoint = -1;
			MaxUndoPoints = 25;
			SuppressUndoPoints = false;
			RegisteredTypes = new ArrayList();
			AvailablePlugins = new ArrayList();
			LoadedPlugins = new ArrayList();
			InitializeComponent();
			GlobalObjects.DesignerForm = this;
		}

		public void AddElement(BaseElement Element)
		{
			ElementStack.AddElement(Element);
			Element.Selected = true;
			SetActiveElement(Element, true);
			_picCanvas.Invalidate();
			CreateUndoPoint(Element.Name + " added");
		}

		public int AddPage()
		{
			Stacks.Add(new GroupElement(null, null, "CanvasStack", true));
			_TabPager.TabPages.Add(new TabPage(Convert.ToString(Stacks.Count - 1)));
			_TabPager.SelectedIndex = Stacks.Count - 1;
			ChangeActiveStack(Stacks.Count - 1);
			return Stacks.Count - 1;
		}

		public void BuildGumplingTree()
		{
			_treGumplings.Nodes.Clear();
			BuildGumplingTree(GumplingTree, null);
		}

		public void BuildGumplingTree(TreeFolder Item, TreeNode Node)
		{
			foreach (var child in Item.GetChildren()) {
				var objectValue = (TreeItem)RuntimeHelpers.GetObjectValue(child);
				var treeNode = new TreeNode {
					Text = objectValue.Text,
					Tag = objectValue
				};
				if (Node == null) {
					_treGumplings.Nodes.Add(treeNode);
				}
				else {
					Node.Nodes.Add(treeNode);
				}

				if (objectValue is TreeFolder) {
					BuildGumplingTree((TreeFolder)objectValue, treeNode);
				}
			}
		}

		protected void BuildToolbox()
		{
			IEnumerator enumerator1 = null;
			_pnlToolbox.Controls.Clear();
			try {
				enumerator1 = RegisteredTypes.GetEnumerator();
				var y = 0;
				while (enumerator1.MoveNext()) {
					var objectValue = (Type)RuntimeHelpers.GetObjectValue(enumerator1.Current);
					var instance = (BaseElement)Activator.CreateInstance(objectValue);
					var button = new Button {
						Text = instance.Type
					};
					var point = new Point(0, y);
					button.Location = point;
					button.FlatStyle = FlatStyle.System;
					button.Width = _pnlToolbox.Width;
					button.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					button.Tag = objectValue;
					y += button.Height - 1;
					_pnlToolbox.Controls.Add(button);
					button.Click += new EventHandler(CreateElementFromToolbox);
					if (instance.DispayInAbout()) {
						AboutElementAppend = AboutElementAppend + "\r\n\r\n" + instance.Type + ": " + instance.GetAboutText();
					}

					foreach (var loadedPlugin in LoadedPlugins) {
						((BasePlugin)RuntimeHelpers.GetObjectValue(loadedPlugin)).InitializeElementExtenders(instance);
					}
				}
			}
			catch (Exception ex) {
				MessageBox.Show($"Error\r\n{ex.Message}\n{ex.StackTrace}");
			}
			finally {
				if (enumerator1 is IDisposable disposable) {
					disposable.Dispose();
				}
			}
			BaseElement.ResetID();
			GumplingTree = new TreeFolder("Root");
			GumplingsFolder = new TreeFolder("My Gumplings");
			UncategorizedFolder = new TreeFolder("Uncategorized");
			GumplingTree.AddItem(GumplingsFolder);
			GumplingTree.AddItem(UncategorizedFolder);
			BuildGumplingTree();
		}

		private void cboElements_Click(object sender, EventArgs e)
		{
			foreach (var element in ElementStack.GetElements()) {
				((BaseElement)RuntimeHelpers.GetObjectValue(element)).Selected = false;
			}

			ActiveElement = null;
		}

		private void cboElements_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetActiveElement((BaseElement)m_cboElements.SelectedItem, false);
			_picCanvas.Invalidate();
		}

		protected void ChangeActiveElementEventHandler(BaseElement e, bool DeselectOthers)
		{
			SetActiveElement(e, DeselectOthers);
			_picCanvas.Invalidate();
		}

		public void ChangeActiveStack(int StackID)
		{
			if (StackID > Stacks.Count - 1) {
				return;
			}

			SetActiveElement(null, true);
			if (ElementStack != null) {
				ElementStack.UpdateParent -= new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
				ElementStack.Repaint -= new BaseElement.RepaintEventHandler(RefreshView);
			}
			ElementStack = (GroupElement)Stacks[StackID];
			ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint += new BaseElement.RepaintEventHandler(RefreshView);
			_picCanvas.Invalidate();
		}

		public void ClearContextMenu(Menu menu)
		{
			var num = menu.MenuItems.Count - 1;
			for (var index = 0; index <= num; ++index) {
				var menuItem = menu.MenuItems[0];
				menu.MenuItems.RemoveAt(0);
			}
		}

		public void ClearGump()
		{
			_TabPager.TabPages.Clear();
			_TabPager.TabPages.Add(new TabPage("0"));
			Stacks.Clear();
			BaseElement.ResetID();
			ElementStack = new GroupElement(null, null, "Element Stack", true);
			Stacks.Add(ElementStack);
			GumpProperties = new GumpProperties();
			ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint += new BaseElement.RepaintEventHandler(RefreshView);
			SetActiveElement(null);
			_picCanvas.Invalidate();
			FileName = "";
			Text = @"Gump Studio (-Unsaved Gump-)";
			ChangeActiveStack(0);
			UndoPoints = new ArrayList();
			CreateUndoPoint("Blank");
			_mnuEditUndo.Enabled = false;
			_mnuEditRedo.Enabled = false;
		}

		public void Copy()
		{
			var arrayList = new ArrayList();

			foreach (var selectedElement in ElementStack.GetSelectedElements()) {
				arrayList.Add(((BaseElement)RuntimeHelpers.GetObjectValue(selectedElement)).Clone());
			}

			Clipboard.SetDataObject(arrayList);
			CopyMode = ClipBoardMode.Copy;
		}

		public void CreateElementFromToolbox(object sender, EventArgs e)
		{
			AddElement((BaseElement)Activator.CreateInstance((Type)((Control)sender).Tag));
			_picCanvas.Invalidate();
			_picCanvas.Focus();
		}

		public void CreateUndoPoint()
		{
			CreateUndoPoint("Unknown Action");
		}

		public void CreateUndoPoint(string Action)
		{
			if (SuppressUndoPoints) {
				return;
			}

			var stopwatch = new Stopwatch();
			stopwatch.Start();
			while (CurrentUndoPoint < UndoPoints.Count - 1) {
				var undoPoint = (UndoPoint)UndoPoints[CurrentUndoPoint + 1];
				UndoPoints.RemoveAt(CurrentUndoPoint + 1);
			}
			var undoPoint1 = new UndoPoint(this) {
				Text = Action
			};
			if (UndoPoints.Count > MaxUndoPoints) {
				UndoPoints.RemoveAt(0);
			}

			UndoPoints.Add(undoPoint1);
			CurrentUndoPoint = UndoPoints.Count - 1;
			_mnuEditUndo.Enabled = true;
			_mnuEditRedo.Enabled = false;
			stopwatch.Stop();
		}

		public void Cut()
		{
			IEnumerator enumerator = null;
			var arrayList = new ArrayList();
			try {
				foreach (var selectedElement in ElementStack.GetSelectedElements()) {
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
					arrayList.Add(objectValue);
				}
			}
			finally {
				(enumerator as IDisposable)?.Dispose();
			}
			Clipboard.SetDataObject(arrayList);
			DeleteSelectedElements();
			CopyMode = ClipBoardMode.Cut;
		}

		protected void DeleteSelectedElements()
		{
			IEnumerator enumerator = null;
			var arrayList = new ArrayList();
			arrayList.AddRange(ElementStack.GetElements());
			var flag = false;
			try {
				foreach (var obj in arrayList) {
					var objectValue = RuntimeHelpers.GetObjectValue(obj);
					flag = true;
					var e = (BaseElement)objectValue;
					if (e.Selected) {
						ElementStack.RemoveElement(e);
					}
				}
			}
			finally {
				(enumerator as IDisposable)?.Dispose();
			}
			SetActiveElement(GetLastSelectedControl());
			_picCanvas.Invalidate();
			if (!flag) {
				return;
			}

			CreateUndoPoint("Delete Elements");
		}

		private void DesignerForm_Closed(object sender, EventArgs e)
		{
			SelFG?.Dispose();
			SelBG?.Dispose();
			WritePluginsToLoad();
		}

		private void DesignerForm_Closing(object sender, CancelEventArgs e)
		{
			IEnumerator enumerator = null;
			try {
				foreach (var availablePlugin in AvailablePlugins) {
					var objectValue = (BasePlugin)RuntimeHelpers.GetObjectValue(availablePlugin);
					if (objectValue.IsLoaded) {
						objectValue.Unload();
					}
				}
			}
			finally {
				(enumerator as IDisposable)?.Dispose();
			}
		}

		private void DesignerForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			XMLSettings.CurrentOptions.DesignerFormSize = WindowState != FormWindowState.Normal ? RestoreBounds.Size : Size;
			XMLSettings.Save(this, XMLSettings.CurrentOptions);
		}

		private void DesignerForm_KeyDown(object sender, KeyEventArgs e)
		{
			var hookKeyDown = HookKeyDown;

			hookKeyDown?.Invoke(ActiveControl, ref e);

			if (e.Handled || ActiveControl != CanvasFocus) {
				return;
			}

			var flag = false;
			if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back ? 1 : 0) != 0) {
				DeleteSelectedElements();
				e.Handled = true;
				flag = true;
			}
			else if (e.KeyCode == Keys.Up) {
				IEnumerator enumerator = null;
				try {
					foreach (var selectedElement in ElementStack.GetSelectedElements()) {
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
						var location = objectValue.Location;
						location.Offset(0, -Convert.ToInt32(ArrowKeyDelta));
						objectValue.Location = location;
					}
				}
				finally {
					(enumerator as IDisposable)?.Dispose();
				}
				ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
			}
			else if (e.KeyCode == Keys.Down) {
				IEnumerator enumerator = null;
				try {
					foreach (var selectedElement in ElementStack.GetSelectedElements()) {
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
						var location = objectValue.Location;
						location.Offset(0, Convert.ToInt32(ArrowKeyDelta));
						objectValue.Location = location;
					}
				}
				finally {
					(enumerator as IDisposable)?.Dispose();
				}
				ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
			}
			else if (e.KeyCode == Keys.Left) {
				IEnumerator enumerator = null;
				try {
					foreach (var selectedElement in ElementStack.GetSelectedElements()) {
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
						var location = objectValue.Location;
						location.Offset(-Convert.ToInt32(ArrowKeyDelta), 0);
						objectValue.Location = location;
					}
				}
				finally {
					(enumerator as IDisposable)?.Dispose();
				}
				ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
			}
			else if (e.KeyCode == Keys.Right) {
				IEnumerator enumerator = null;
				try {
					foreach (var selectedElement in ElementStack.GetSelectedElements()) {
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
						var location = objectValue.Location;
						location.Offset(Convert.ToInt32(ArrowKeyDelta), 0);
						objectValue.Location = location;
					}
				}
				finally {
					(enumerator as IDisposable)?.Dispose();
				}
				ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
			}
			else if (e.KeyCode == Keys.Next) {
				var index = (ActiveElement == null ? ElementStack.GetElements().Count - 1 : ActiveElement.Z) - 1;
				if (index < 0) {
					index = ElementStack.GetElements().Count - 1;
				}

				if (index >= 0 & index <= ElementStack.GetElements().Count - 1) {
					SetActiveElement((BaseElement)ElementStack.GetElements()[index], true);
				}
			}
			else if (e.KeyCode == Keys.Prior) {
				var index = (ActiveElement == null ? ElementStack.GetElements().Count - 1 : ActiveElement.Z) + 1;
				if (index > ElementStack.GetElements().Count - 1) {
					index = 0;
				}

				SetActiveElement((BaseElement)ElementStack.GetElements()[index], true);
			}
			if (Decimal.Compare(ArrowKeyDelta, new decimal(10)) > 0) {
				ArrowKeyDelta = new decimal(10);
			}

			if (!flag) {
				return;
			}

			_picCanvas.Invalidate();
			m_ElementProperties.SelectedObjects = m_ElementProperties.SelectedObjects;
		}

		private void DesignerForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode != Keys.Up && e.KeyCode != Keys.Down) {
				var keyCode = (int)e.KeyCode;
			}
			if ((e.KeyCode == Keys.Right ? 1 : 0) == 0) {
				return;
			}

			CreateUndoPoint("Move element");
			ArrowKeyDelta = new decimal(1);
		}

		private void DesignerForm_Load(object sender, EventArgs e)
		{
			XMLSettings.CurrentOptions = XMLSettings.Load(this);

			if (!File.Exists(Path.Combine(XMLSettings.CurrentOptions.ClientPath, "art.mul")) && !File.Exists(Path.Combine(XMLSettings.CurrentOptions.ClientPath, "artLegacyMUL.uop"))) {
				var folderBrowserDialog = new FolderBrowserDialog { SelectedPath = Environment.SpecialFolder.ProgramFiles.ToString(), Description = @"Select the folder that contains the UO data (.mul) files you want to use." };

				if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
					if (File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "art.mul")) || File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "artLegacyMUL.uop"))) {
						XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
						XMLSettings.Save(this, XMLSettings.CurrentOptions);
					}
					else {
						MessageBox.Show(@"This path does not contain a file named ""art.mul"", it is most likely not the correct path. Gump Studio can not run without valid client data files.", "Data Files");
						Close();
						return;
					}
				}
				else {
					Close();
					return;
				}
			}
			//Client.Directories.Add( XMLSettings.CurrentOptions.ClientPath );
			Files.CacheData = false;
			Files.SetMulPath(XMLSettings.CurrentOptions.ClientPath);
			Size = XMLSettings.CurrentOptions.DesignerFormSize;
			MaxUndoPoints = XMLSettings.CurrentOptions.UndoLevels;
			_picCanvas.Width = 1600;
			_picCanvas.Height = 1200;
			CenterToScreen();
			frmSplash.DisplaySplash();
			EnumeratePlugins();
			Canvas = new Bitmap(_picCanvas.Width, _picCanvas.Height, PixelFormat.Format32bppRgb);
			Activate();
			GumpProperties = new GumpProperties();
			ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint += new BaseElement.RepaintEventHandler(RefreshView);
			Stacks.Clear();
			Stacks.Add(ElementStack);
			ChangeActiveStack(0);
			RegisteredTypes.Clear();
			RegisteredTypes.Add(typeof(LabelElement));
			RegisteredTypes.Add(typeof(ImageElement));
			RegisteredTypes.Add(typeof(TiledElement));
			RegisteredTypes.Add(typeof(BackgroundElement));
			RegisteredTypes.Add(typeof(AlphaElement));
			RegisteredTypes.Add(typeof(CheckboxElement));
			RegisteredTypes.Add(typeof(RadioElement));
			RegisteredTypes.Add(typeof(ItemElement));
			RegisteredTypes.Add(typeof(TextEntryElement));
			RegisteredTypes.Add(typeof(ButtonElement));
			RegisteredTypes.Add(typeof(HTMLElement));
			BuildToolbox();
			SelFG = new Pen(Color.Blue, 2f);
			SelBG = new LinearGradientBrush(new Rectangle(0, 0, 50, 50), Color.FromArgb(90, Color.Blue), Color.FromArgb(110, Color.Blue), LinearGradientMode.ForwardDiagonal) {
				WrapMode = WrapMode.TileFlipXY
			};
			CreateUndoPoint("Blank");
			_mnuEditUndo.Enabled = false;
		}

		protected void DrawBoundingBox(Graphics Target, BaseElement Element)
		{
			var bounds = Element.Bounds;
			Target.DrawRectangle(Pens.Red, bounds);
			bounds.Offset(1, 1);
			Target.DrawRectangle(Pens.Black, bounds);
		}

		private void ElementProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name == "Name") {
				m_cboElements.Items.Clear();
				m_cboElements.Items.AddRange(ElementStack.GetElements().ToArray());
				m_cboElements.SelectedItem = RuntimeHelpers.GetObjectValue(m_ElementProperties.SelectedObject);
			}
			_picCanvas.Invalidate();
			CreateUndoPoint("Property Changed");
		}

		protected void EnumeratePlugins()
		{
			PluginTypesToLoad = GetPluginsToLoad();

			foreach (var file in Directory.GetFiles(Application.StartupPath, "*.dll", SearchOption.AllDirectories)) {
				if (file.EndsWith("Ultima.dll", StringComparison.OrdinalIgnoreCase) || file.EndsWith("UOFont.dll", StringComparison.OrdinalIgnoreCase)) {
					continue;
				}

				foreach (var type in Assembly.LoadFile(file).GetTypes()) {
					try {
						if (type.IsSubclassOf(typeof(BasePlugin)) && !type.IsAbstract) {
							var instance = (BasePlugin)Activator.CreateInstance(type);
							var pluginInfo = instance.Info;
							AboutElementAppend = AboutElementAppend + "\r\n" + pluginInfo.Name + ": " + pluginInfo.Description + "\r\nAuthor: " + pluginInfo.AuthorName + "  (" + pluginInfo.AuthorContact + ")\r\nVersion: " + pluginInfo.Version + "\r\n";
							AvailablePlugins.Add(instance);
						}

						if (type.IsSubclassOf(typeof(BaseElement)) && !type.IsAbstract) {
							RegisteredTypes.Add(type);
						}
					}
					catch (Exception ex) {
						var exception = ex;
						MessageBox.Show("Error loading plugin: " + type.Name + "(" + file + ")\r\n\r\n" + exception.Message);
					}
				}
			}

			if (PluginTypesToLoad == null) {
				return;
			}

			foreach (var pluginInfo1 in PluginTypesToLoad) {
				IEnumerator enumerator = null;
				try {
					foreach (var availablePlugin in AvailablePlugins) {
						var objectValue = (BasePlugin)RuntimeHelpers.GetObjectValue(availablePlugin);
						var pluginInfo2 = objectValue.Info;
						if (pluginInfo1.Equals(pluginInfo2)) {
							objectValue.Load(this);
							LoadedPlugins.Add(objectValue);
						}
					}
				}
				finally {
					(enumerator as IDisposable)?.Dispose();
				}
			}
		}

		protected void GetContextMenu(ref BaseElement Element, ContextMenu Menu)
		{
			var GroupMenu = new MenuItem("Grouping");
			var PositionMenu = new MenuItem("Positioning");
			var OrderMenu = new MenuItem("Order");
			var MiscMenu = new MenuItem("Misc");
			var menuItem = new MenuItem("Edit");
			menuItem.MenuItems.Add(new MenuItem("Cut", new EventHandler(mnuCut_Click)));
			menuItem.MenuItems.Add(new MenuItem("Copy", new EventHandler(mnuCopy_Click)));
			menuItem.MenuItems.Add(new MenuItem("Paste", new EventHandler(mnuPaste_Click)));
			menuItem.MenuItems.Add(new MenuItem("Delete", new EventHandler(mnuDelete_Click)));
			Menu.MenuItems.Add(menuItem);
			Menu.MenuItems.Add(new MenuItem("-"));
			Menu.MenuItems.Add(GroupMenu);
			Menu.MenuItems.Add(PositionMenu);
			Menu.MenuItems.Add(OrderMenu);
			Menu.MenuItems.Add(new MenuItem("-"));
			Menu.MenuItems.Add(MiscMenu);
			if (ElementStack.GetSelectedElements().Count >= 2) {
				GroupMenu.MenuItems.Add(new MenuItem("Create Group", new EventHandler(mnuGroupCreate_Click)));
			}

			Element?.AddContextMenus(ref GroupMenu, ref PositionMenu, ref OrderMenu, ref MiscMenu);
			if (GroupMenu.MenuItems.Count == 0) {
				GroupMenu.Enabled = false;
			}

			if (PositionMenu.MenuItems.Count == 0) {
				PositionMenu.Enabled = false;
			}

			if (OrderMenu.MenuItems.Count == 0) {
				OrderMenu.Enabled = false;
			}

			if (MiscMenu.MenuItems.Count != 0) {
				return;
			}

			MiscMenu.Enabled = false;
		}

		public BaseElement GetLastSelectedControl()
		{
			BaseElement baseElement = null;
			IEnumerator enumerator = null;
			try {
				foreach (var element in ElementStack.GetElements()) {
					baseElement = (BaseElement)RuntimeHelpers.GetObjectValue(element);
				}
			}
			finally {
				(enumerator as IDisposable)?.Dispose();
			}
			return baseElement;
		}

		protected PluginInfo[] GetPluginsToLoad()
		{
			PluginInfo[] pluginInfoArray = null;
			if (File.Exists(Application.StartupPath + "\\LoadInfo.bin")) {
				var fileStream = new FileStream(Application.StartupPath + "\\LoadInfo.bin", FileMode.Open);
				pluginInfoArray = (PluginInfo[])new BinaryFormatter().Deserialize(fileStream);
				fileStream.Close();
			}
			return pluginInfoArray;
		}

		protected Rectangle GetPositiveRect(Rectangle Rect)
		{
			if (Rect.Height < 0) {
				Rect.Height = Math.Abs(Rect.Height);
				Rect.Y -= Rect.Height;
			}
			if (Rect.Width < 0) {
				Rect.Width = Math.Abs(Rect.Width);
				Rect.X -= Rect.Width;
			}
			return Rect;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				components?.Dispose();
			}

			base.Dispose(disposing);
		}


		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerForm));
			_pnlToolboxHolder = new System.Windows.Forms.Panel();
			_Panel4 = new System.Windows.Forms.Panel();
			_tabToolbox = new System.Windows.Forms.TabControl();
			_tpgStandard = new System.Windows.Forms.TabPage();
			_pnlToolbox = new System.Windows.Forms.Panel();
			_tpgCustom = new System.Windows.Forms.TabPage();
			_treGumplings = new System.Windows.Forms.TreeView();
			m_Label1 = new System.Windows.Forms.Label();
			_StatusBar = new System.Windows.Forms.StatusBar();
			_Splitter1 = new System.Windows.Forms.Splitter();
			_Panel1 = new System.Windows.Forms.Panel();
			_Panel2 = new System.Windows.Forms.Panel();
			_pnlCanvasScroller = new System.Windows.Forms.Panel();
			_picCanvas = new System.Windows.Forms.PictureBox();
			_TabPager = new System.Windows.Forms.TabControl();
			_TabPage1 = new System.Windows.Forms.TabPage();
			_Splitter2 = new System.Windows.Forms.Splitter();
			_Panel3 = new System.Windows.Forms.Panel();
			m_cboElements = new System.Windows.Forms.ComboBox();
			m_ElementProperties = new System.Windows.Forms.PropertyGrid();
			m_CanvasFocus = new System.Windows.Forms.TextBox();
			_OpenDialog = new System.Windows.Forms.OpenFileDialog();
			_SaveDialog = new System.Windows.Forms.SaveFileDialog();
			m_mnuContextMenu = new System.Windows.Forms.ContextMenu();
			m_MainMenu = new System.Windows.Forms.MainMenu(components);
			_mnuFile = new System.Windows.Forms.MenuItem();
			_mnuFileNew = new System.Windows.Forms.MenuItem();
			m_MenuItem9 = new System.Windows.Forms.MenuItem();
			_mnuFileOpen = new System.Windows.Forms.MenuItem();
			_mnuFileSave = new System.Windows.Forms.MenuItem();
			_mnuFileImport = new System.Windows.Forms.MenuItem();
			_mnuFileExport = new System.Windows.Forms.MenuItem();
			m_MenuItem5 = new System.Windows.Forms.MenuItem();
			_mnuFileExit = new System.Windows.Forms.MenuItem();
			_mnuEdit = new System.Windows.Forms.MenuItem();
			_mnuEditUndo = new System.Windows.Forms.MenuItem();
			_mnuEditRedo = new System.Windows.Forms.MenuItem();
			m_MenuItem3 = new System.Windows.Forms.MenuItem();
			_mnuCut = new System.Windows.Forms.MenuItem();
			m_mnuCopy = new System.Windows.Forms.MenuItem();
			_mnuPaste = new System.Windows.Forms.MenuItem();
			_mnuDelete = new System.Windows.Forms.MenuItem();
			m_MenuItem4 = new System.Windows.Forms.MenuItem();
			_mnuSelectAll = new System.Windows.Forms.MenuItem();
			_mnuMisc = new System.Windows.Forms.MenuItem();
			_mnuMiscLoadGumpling = new System.Windows.Forms.MenuItem();
			_mnuImportGumpling = new System.Windows.Forms.MenuItem();
			_mnuDataFile = new System.Windows.Forms.MenuItem();
			_mnuPage = new System.Windows.Forms.MenuItem();
			_mnuPageAdd = new System.Windows.Forms.MenuItem();
			_mnuPageInsert = new System.Windows.Forms.MenuItem();
			_mnuPageDelete = new System.Windows.Forms.MenuItem();
			_mnuPageClear = new System.Windows.Forms.MenuItem();
			m_MenuItem10 = new System.Windows.Forms.MenuItem();
			_mnuShow0 = new System.Windows.Forms.MenuItem();
			_mnuPlugins = new System.Windows.Forms.MenuItem();
			_mnuPluginManager = new System.Windows.Forms.MenuItem();
			_mnuHelp = new System.Windows.Forms.MenuItem();
			_mnuHelpAbout = new System.Windows.Forms.MenuItem();
			_mnuGumplingContext = new System.Windows.Forms.ContextMenu();
			_mnuGumplingRename = new System.Windows.Forms.MenuItem();
			_mnuGumplingMove = new System.Windows.Forms.MenuItem();
			_mnuGumplingDelete = new System.Windows.Forms.MenuItem();
			m_MenuItem1 = new System.Windows.Forms.MenuItem();
			_mnuGumplingAddGumpling = new System.Windows.Forms.MenuItem();
			_mnuGumplingAddFolder = new System.Windows.Forms.MenuItem();
			_pnlToolboxHolder.SuspendLayout();
			_Panel4.SuspendLayout();
			_tabToolbox.SuspendLayout();
			_tpgStandard.SuspendLayout();
			_tpgCustom.SuspendLayout();
			_Panel1.SuspendLayout();
			_Panel2.SuspendLayout();
			_pnlCanvasScroller.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(_picCanvas)).BeginInit();
			_TabPager.SuspendLayout();
			_Panel3.SuspendLayout();
			SuspendLayout();
			// 
			// _pnlToolboxHolder
			// 
			_pnlToolboxHolder.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_pnlToolboxHolder.Controls.Add(_Panel4);
			_pnlToolboxHolder.Controls.Add(m_Label1);
			_pnlToolboxHolder.Dock = System.Windows.Forms.DockStyle.Left;
			_pnlToolboxHolder.Location = new System.Drawing.Point(0, 0);
			_pnlToolboxHolder.Name = "_pnlToolboxHolder";
			_pnlToolboxHolder.Size = new System.Drawing.Size(128, 685);
			_pnlToolboxHolder.TabIndex = 0;
			// 
			// _Panel4
			// 
			_Panel4.Controls.Add(_tabToolbox);
			_Panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			_Panel4.Location = new System.Drawing.Point(0, 16);
			_Panel4.Name = "_Panel4";
			_Panel4.Size = new System.Drawing.Size(124, 665);
			_Panel4.TabIndex = 1;
			// 
			// _tabToolbox
			// 
			_tabToolbox.Controls.Add(_tpgStandard);
			_tabToolbox.Controls.Add(_tpgCustom);
			_tabToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			_tabToolbox.Location = new System.Drawing.Point(0, 0);
			_tabToolbox.Multiline = true;
			_tabToolbox.Name = "_tabToolbox";
			_tabToolbox.SelectedIndex = 0;
			_tabToolbox.Size = new System.Drawing.Size(124, 665);
			_tabToolbox.TabIndex = 1;
			// 
			// _tpgStandard
			// 
			_tpgStandard.Controls.Add(_pnlToolbox);
			_tpgStandard.Location = new System.Drawing.Point(4, 22);
			_tpgStandard.Name = "_tpgStandard";
			_tpgStandard.Size = new System.Drawing.Size(116, 639);
			_tpgStandard.TabIndex = 0;
			_tpgStandard.Text = "Standard";
			// 
			// _pnlToolbox
			// 
			_pnlToolbox.AutoScroll = true;
			_pnlToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			_pnlToolbox.Location = new System.Drawing.Point(0, 0);
			_pnlToolbox.Name = "_pnlToolbox";
			_pnlToolbox.Size = new System.Drawing.Size(116, 639);
			_pnlToolbox.TabIndex = 1;
			// 
			// _tpgCustom
			// 
			_tpgCustom.Controls.Add(_treGumplings);
			_tpgCustom.Location = new System.Drawing.Point(4, 22);
			_tpgCustom.Name = "_tpgCustom";
			_tpgCustom.Size = new System.Drawing.Size(116, 639);
			_tpgCustom.TabIndex = 1;
			_tpgCustom.Text = "Gumplings";
			// 
			// _treGumplings
			// 
			_treGumplings.Dock = System.Windows.Forms.DockStyle.Fill;
			_treGumplings.Location = new System.Drawing.Point(0, 0);
			_treGumplings.Name = "_treGumplings";
			_treGumplings.Size = new System.Drawing.Size(116, 639);
			_treGumplings.TabIndex = 1;
			_treGumplings.DoubleClick += new System.EventHandler(treGumplings_DoubleClick);
			_treGumplings.MouseUp += new System.Windows.Forms.MouseEventHandler(treGumplings_MouseUp);
			// 
			// m_Label1
			// 
			m_Label1.BackColor = System.Drawing.SystemColors.ControlDark;
			m_Label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			m_Label1.Dock = System.Windows.Forms.DockStyle.Top;
			m_Label1.Location = new System.Drawing.Point(0, 0);
			m_Label1.Name = "m_Label1";
			m_Label1.Size = new System.Drawing.Size(124, 16);
			m_Label1.TabIndex = 0;
			m_Label1.Text = "Toolbox";
			m_Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// _StatusBar
			// 
			_StatusBar.Location = new System.Drawing.Point(0, 685);
			_StatusBar.Name = "_StatusBar";
			_StatusBar.Size = new System.Drawing.Size(1350, 23);
			_StatusBar.TabIndex = 0;
			// 
			// _Splitter1
			// 
			_Splitter1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_Splitter1.Location = new System.Drawing.Point(128, 0);
			_Splitter1.MinSize = 80;
			_Splitter1.Name = "_Splitter1";
			_Splitter1.Size = new System.Drawing.Size(3, 685);
			_Splitter1.TabIndex = 1;
			_Splitter1.TabStop = false;
			// 
			// _Panel1
			// 
			_Panel1.Controls.Add(_Panel2);
			_Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			_Panel1.Location = new System.Drawing.Point(131, 0);
			_Panel1.Name = "_Panel1";
			_Panel1.Size = new System.Drawing.Size(1219, 685);
			_Panel1.TabIndex = 2;
			// 
			// _Panel2
			// 
			_Panel2.Controls.Add(_pnlCanvasScroller);
			_Panel2.Controls.Add(_TabPager);
			_Panel2.Controls.Add(_Splitter2);
			_Panel2.Controls.Add(_Panel3);
			_Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			_Panel2.Location = new System.Drawing.Point(0, 0);
			_Panel2.Name = "_Panel2";
			_Panel2.Size = new System.Drawing.Size(1219, 685);
			_Panel2.TabIndex = 0;
			// 
			// _pnlCanvasScroller
			// 
			_pnlCanvasScroller.AutoScroll = true;
			_pnlCanvasScroller.AutoScrollMargin = new System.Drawing.Size(1, 1);
			_pnlCanvasScroller.AutoScrollMinSize = new System.Drawing.Size(1, 1);
			_pnlCanvasScroller.BackColor = System.Drawing.Color.Silver;
			_pnlCanvasScroller.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_pnlCanvasScroller.Controls.Add(_picCanvas);
			_pnlCanvasScroller.Dock = System.Windows.Forms.DockStyle.Fill;
			_pnlCanvasScroller.Location = new System.Drawing.Point(0, 24);
			_pnlCanvasScroller.Name = "_pnlCanvasScroller";
			_pnlCanvasScroller.Size = new System.Drawing.Size(949, 661);
			_pnlCanvasScroller.TabIndex = 2;
			_pnlCanvasScroller.MouseLeave += new System.EventHandler(pnlCanvasScroller_MouseLeave);
			// 
			// _picCanvas
			// 
			_picCanvas.BackColor = System.Drawing.Color.Black;
			_picCanvas.Location = new System.Drawing.Point(0, 0);
			_picCanvas.Name = "_picCanvas";
			_picCanvas.Size = new System.Drawing.Size(1600, 1200);
			_picCanvas.TabIndex = 0;
			_picCanvas.TabStop = false;
			_picCanvas.Paint += new System.Windows.Forms.PaintEventHandler(picCanvas_Paint);
			_picCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(picCanvas_MouseDown);
			_picCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(picCanvas_MouseMove);
			_picCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(picCanvas_MouseUp);
			// 
			// _TabPager
			// 
			_TabPager.Controls.Add(_TabPage1);
			_TabPager.Dock = System.Windows.Forms.DockStyle.Top;
			_TabPager.HotTrack = true;
			_TabPager.Location = new System.Drawing.Point(0, 0);
			_TabPager.Name = "_TabPager";
			_TabPager.SelectedIndex = 0;
			_TabPager.Size = new System.Drawing.Size(949, 24);
			_TabPager.TabIndex = 3;
			_TabPager.SelectedIndexChanged += new System.EventHandler(TabPager_SelectedIndexChanged);
			// 
			// _TabPage1
			// 
			_TabPage1.Location = new System.Drawing.Point(4, 22);
			_TabPage1.Name = "_TabPage1";
			_TabPage1.Size = new System.Drawing.Size(941, 0);
			_TabPage1.TabIndex = 0;
			_TabPage1.Text = "0";
			// 
			// _Splitter2
			// 
			_Splitter2.Dock = System.Windows.Forms.DockStyle.Right;
			_Splitter2.Location = new System.Drawing.Point(949, 0);
			_Splitter2.Name = "_Splitter2";
			_Splitter2.Size = new System.Drawing.Size(22, 685);
			_Splitter2.TabIndex = 1;
			_Splitter2.TabStop = false;
			// 
			// _Panel3
			// 
			_Panel3.Controls.Add(m_cboElements);
			_Panel3.Controls.Add(m_ElementProperties);
			_Panel3.Controls.Add(m_CanvasFocus);
			_Panel3.Dock = System.Windows.Forms.DockStyle.Right;
			_Panel3.Location = new System.Drawing.Point(971, 0);
			_Panel3.Name = "_Panel3";
			_Panel3.Size = new System.Drawing.Size(248, 685);
			_Panel3.TabIndex = 0;
			// 
			// m_cboElements
			// 
			m_cboElements.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			m_cboElements.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			m_cboElements.Location = new System.Drawing.Point(0, 8);
			m_cboElements.Name = "m_cboElements";
			m_cboElements.Size = new System.Drawing.Size(240, 21);
			m_cboElements.TabIndex = 1;
			m_cboElements.SelectedIndexChanged += new System.EventHandler(cboElements_SelectedIndexChanged);
			m_cboElements.Click += new System.EventHandler(cboElements_Click);
			// 
			// m_ElementProperties
			// 
			m_ElementProperties.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right);
			m_ElementProperties.Cursor = System.Windows.Forms.Cursors.HSplit;
			m_ElementProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
			m_ElementProperties.Location = new System.Drawing.Point(0, 32);
			m_ElementProperties.Name = "m_ElementProperties";
			m_ElementProperties.Size = new System.Drawing.Size(240, 651);
			m_ElementProperties.TabIndex = 0;
			m_ElementProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(ElementProperties_PropertyValueChanged);
			// 
			// m_CanvasFocus
			// 
			m_CanvasFocus.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			m_CanvasFocus.Location = new System.Drawing.Point(16, 635);
			m_CanvasFocus.Name = "m_CanvasFocus";
			m_CanvasFocus.Size = new System.Drawing.Size(100, 20);
			m_CanvasFocus.TabIndex = 1;
			m_CanvasFocus.Text = "TextBox1";
			// 
			// m_MainMenu
			// 
			m_MainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuFile,
			_mnuEdit,
			_mnuMisc,
			_mnuPage,
			_mnuPlugins,
			_mnuHelp});
			// 
			// _mnuFile
			// 
			_mnuFile.Index = 0;
			_mnuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuFileNew,
			m_MenuItem9,
			_mnuFileOpen,
			_mnuFileSave,
			_mnuFileImport,
			_mnuFileExport,
			m_MenuItem5,
			_mnuFileExit});
			_mnuFile.Text = "File";
			// 
			// _mnuFileNew
			// 
			_mnuFileNew.Index = 0;
			_mnuFileNew.Text = "New";
			_mnuFileNew.Click += new System.EventHandler(mnuFileNew_Click);
			// 
			// m_MenuItem9
			// 
			m_MenuItem9.Index = 1;
			m_MenuItem9.Text = "-";
			// 
			// _mnuFileOpen
			// 
			_mnuFileOpen.Index = 2;
			_mnuFileOpen.Text = "Open";
			_mnuFileOpen.Click += new System.EventHandler(mnuFileOpen_Click);
			// 
			// _mnuFileSave
			// 
			_mnuFileSave.Index = 3;
			_mnuFileSave.Text = "Save";
			_mnuFileSave.Click += new System.EventHandler(mnuFileSave_Click);
			// 
			// _mnuFileImport
			// 
			_mnuFileImport.Enabled = false;
			_mnuFileImport.Index = 4;
			_mnuFileImport.Text = "Import";
			// 
			// _mnuFileExport
			// 
			_mnuFileExport.Enabled = false;
			_mnuFileExport.Index = 5;
			_mnuFileExport.Text = "Export";
			// 
			// m_MenuItem5
			// 
			m_MenuItem5.Index = 6;
			m_MenuItem5.Text = "-";
			// 
			// _mnuFileExit
			// 
			_mnuFileExit.Index = 7;
			_mnuFileExit.Text = "Exit";
			_mnuFileExit.Click += new System.EventHandler(mnuFileExit_Click);
			// 
			// _mnuEdit
			// 
			_mnuEdit.Index = 1;
			_mnuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuEditUndo,
			_mnuEditRedo,
			m_MenuItem3,
			_mnuCut,
			m_mnuCopy,
			_mnuPaste,
			_mnuDelete,
			m_MenuItem4,
			_mnuSelectAll});
			_mnuEdit.Text = "Edit";
			// 
			// _mnuEditUndo
			// 
			_mnuEditUndo.Enabled = false;
			_mnuEditUndo.Index = 0;
			_mnuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			_mnuEditUndo.Text = "Undo";
			_mnuEditUndo.Click += new System.EventHandler(mnuEditUndo_Click);
			// 
			// _mnuEditRedo
			// 
			_mnuEditRedo.Enabled = false;
			_mnuEditRedo.Index = 1;
			_mnuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			_mnuEditRedo.Text = "Redo";
			_mnuEditRedo.Click += new System.EventHandler(mnuEditRedo_Click);
			// 
			// m_MenuItem3
			// 
			m_MenuItem3.Index = 2;
			m_MenuItem3.Text = "-";
			// 
			// _mnuCut
			// 
			_mnuCut.Index = 3;
			_mnuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			_mnuCut.Text = "Cu&t";
			_mnuCut.Click += new System.EventHandler(mnuCut_Click);
			// 
			// m_mnuCopy
			// 
			m_mnuCopy.Index = 4;
			m_mnuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			m_mnuCopy.Text = "&Copy";
			m_mnuCopy.Click += new System.EventHandler(mnuCopy_Click);
			// 
			// _mnuPaste
			// 
			_mnuPaste.Index = 5;
			_mnuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			_mnuPaste.Text = "&Paste";
			_mnuPaste.Click += new System.EventHandler(mnuPaste_Click);
			// 
			// _mnuDelete
			// 
			_mnuDelete.Index = 6;
			_mnuDelete.Text = "Delete";
			_mnuDelete.Click += new System.EventHandler(mnuDelete_Click);
			// 
			// m_MenuItem4
			// 
			m_MenuItem4.Index = 7;
			m_MenuItem4.Text = "-";
			// 
			// _mnuSelectAll
			// 
			_mnuSelectAll.Index = 8;
			_mnuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			_mnuSelectAll.Text = "Select &All";
			_mnuSelectAll.Click += new System.EventHandler(mnuSelectAll_Click);
			// 
			// _mnuMisc
			// 
			_mnuMisc.Index = 2;
			_mnuMisc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuMiscLoadGumpling,
			_mnuImportGumpling,
			_mnuDataFile});
			_mnuMisc.Text = "Misc";
			// 
			// _mnuMiscLoadGumpling
			// 
			_mnuMiscLoadGumpling.Index = 0;
			_mnuMiscLoadGumpling.Text = "Load gumpling";
			_mnuMiscLoadGumpling.Click += new System.EventHandler(MenuItem2_Click);
			// 
			// _mnuImportGumpling
			// 
			_mnuImportGumpling.Index = 1;
			_mnuImportGumpling.Text = "Import Gumpling";
			_mnuImportGumpling.Click += new System.EventHandler(mnuImportGumpling_Click);
			// 
			// _mnuDataFile
			// 
			_mnuDataFile.Index = 2;
			_mnuDataFile.Text = "Data File Path";
			_mnuDataFile.Click += new System.EventHandler(mnuDataFile_Click);
			// 
			// _mnuPage
			// 
			_mnuPage.Index = 3;
			_mnuPage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuPageAdd,
			_mnuPageInsert,
			_mnuPageDelete,
			_mnuPageClear,
			m_MenuItem10,
			_mnuShow0});
			_mnuPage.Text = "Page";
			// 
			// _mnuPageAdd
			// 
			_mnuPageAdd.Index = 0;
			_mnuPageAdd.Text = "Add Page";
			_mnuPageAdd.Click += new System.EventHandler(mnuAddPage_Click);
			// 
			// _mnuPageInsert
			// 
			_mnuPageInsert.Index = 1;
			_mnuPageInsert.Text = "Insert Page";
			_mnuPageInsert.Click += new System.EventHandler(mnuPageInsert_Click);
			// 
			// _mnuPageDelete
			// 
			_mnuPageDelete.Index = 2;
			_mnuPageDelete.Text = "Delete Page";
			_mnuPageDelete.Click += new System.EventHandler(mnuPageDelete_Click);
			// 
			// _mnuPageClear
			// 
			_mnuPageClear.Index = 3;
			_mnuPageClear.Text = "Clear Page";
			_mnuPageClear.Click += new System.EventHandler(mnuPageClear_Click);
			// 
			// m_MenuItem10
			// 
			m_MenuItem10.Index = 4;
			m_MenuItem10.Text = "-";
			// 
			// _mnuShow0
			// 
			_mnuShow0.Checked = true;
			_mnuShow0.Index = 5;
			_mnuShow0.Text = "Always Show Page 0";
			_mnuShow0.Click += new System.EventHandler(mnuShow0_Click);
			// 
			// _mnuPlugins
			// 
			_mnuPlugins.Index = 4;
			_mnuPlugins.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuPluginManager});
			_mnuPlugins.Text = "Plug-Ins";
			// 
			// _mnuPluginManager
			// 
			_mnuPluginManager.Index = 0;
			_mnuPluginManager.Text = "Manager";
			_mnuPluginManager.Click += new System.EventHandler(mnuPluginManager_Click);
			// 
			// _mnuHelp
			// 
			_mnuHelp.Index = 5;
			_mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuHelpAbout});
			_mnuHelp.Text = "Help";
			// 
			// _mnuHelpAbout
			// 
			_mnuHelpAbout.Index = 0;
			_mnuHelpAbout.Text = "About...";
			_mnuHelpAbout.Click += new System.EventHandler(mnuHelpAbout_Click);
			// 
			// _mnuGumplingContext
			// 
			_mnuGumplingContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_mnuGumplingRename,
			_mnuGumplingMove,
			_mnuGumplingDelete,
			m_MenuItem1,
			_mnuGumplingAddGumpling,
			_mnuGumplingAddFolder});
			// 
			// _mnuGumplingRename
			// 
			_mnuGumplingRename.Index = 0;
			_mnuGumplingRename.Text = "Rename";
			// 
			// _mnuGumplingMove
			// 
			_mnuGumplingMove.Index = 1;
			_mnuGumplingMove.Text = "Move";
			// 
			// _mnuGumplingDelete
			// 
			_mnuGumplingDelete.Index = 2;
			_mnuGumplingDelete.Text = "Delete";
			// 
			// m_MenuItem1
			// 
			m_MenuItem1.Index = 3;
			m_MenuItem1.Text = "-";
			// 
			// _mnuGumplingAddGumpling
			// 
			_mnuGumplingAddGumpling.Index = 4;
			_mnuGumplingAddGumpling.Text = "Add Gumpling";
			// 
			// _mnuGumplingAddFolder
			// 
			_mnuGumplingAddFolder.Index = 5;
			_mnuGumplingAddFolder.Text = "Add Folder";
			// 
			// DesignerForm
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new System.Drawing.Size(1350, 708);
			Controls.Add(_Panel1);
			Controls.Add(_Splitter1);
			Controls.Add(_pnlToolboxHolder);
			Controls.Add(_StatusBar);
			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			Menu = m_MainMenu;
			Name = "DesignerForm";
			Text = "Gump Studio (-Unsaved Gump-)";
			FormClosing += new System.Windows.Forms.FormClosingEventHandler(DesignerForm_FormClosing);
			Load += new System.EventHandler(DesignerForm_Load);
			KeyDown += new System.Windows.Forms.KeyEventHandler(DesignerForm_KeyDown);
			KeyUp += new System.Windows.Forms.KeyEventHandler(DesignerForm_KeyUp);
			_pnlToolboxHolder.ResumeLayout(false);
			_Panel4.ResumeLayout(false);
			_tabToolbox.ResumeLayout(false);
			_tpgStandard.ResumeLayout(false);
			_tpgCustom.ResumeLayout(false);
			_Panel1.ResumeLayout(false);
			_Panel2.ResumeLayout(false);
			_pnlCanvasScroller.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(_picCanvas)).EndInit();
			_TabPager.ResumeLayout(false);
			_Panel3.ResumeLayout(false);
			_Panel3.PerformLayout();
			ResumeLayout(false);

		}

		private sealed class DeserializationBinder : SerializationBinder
		{
			public static readonly DeserializationBinder Instance = new DeserializationBinder();

			private DeserializationBinder()
			{ }

			public override Type BindToType(string assemblyName, string typeName)
			{
				var asm = Assembly.GetExecutingAssembly();

				if (assemblyName.Contains("GumpStudioCore")) {
					assemblyName = asm.FullName;
				}

				return Type.GetType($"{typeName}, {assemblyName}");
			}
		}

		public void LoadFrom(string Path)
		{
			IEnumerator enumerator = null;
			_StatusBar.Text = "Loading gump...";
			FileStream fileStream = null;
			Stacks.Clear();
			_TabPager.TabPages.Clear();
			try {
				fileStream = new FileStream(Path, FileMode.Open);
				var binaryFormatter = new BinaryFormatter { Binder = DeserializationBinder.Instance };
				Stacks = (ArrayList)binaryFormatter.Deserialize(fileStream);
				try {
					GumpProperties = (GumpProperties)binaryFormatter.Deserialize(fileStream);
				}
				catch (Exception ex) {
					var exception = ex;
					GumpProperties = new GumpProperties();
					MessageBox.Show(exception.InnerException.Message);
				}
				SetActiveElement(null, true);
				RefreshElementList();
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message);
			}
			finally {
				fileStream?.Close();
			}
			var num1 = 0;
			try {
				foreach (var stack in Stacks) {
					RuntimeHelpers.GetObjectValue(stack);
					_TabPager.TabPages.Add(new TabPage(num1.ToString()));
					++num1;
				}
			}
			finally {
				(enumerator as IDisposable)?.Dispose();
			}
			ChangeActiveStack(0);
			ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint += new BaseElement.RepaintEventHandler(RefreshView);
			_StatusBar.Text = "";
		}

		private void MenuItem2_Click(object sender, EventArgs e)
		{
			_OpenDialog.Filter = "Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";
			if (_OpenDialog.ShowDialog() != DialogResult.OK) {
				return;
			}

			var fileStream = new FileStream(_OpenDialog.FileName, FileMode.Open);
			var groupElement = (GroupElement)new BinaryFormatter { Binder = DeserializationBinder.Instance }.Deserialize(fileStream);
			groupElement.mIsBaseWindow = false;
			groupElement.RecalculateBounds();
			var point = new Point(0, 0);
			groupElement.Location = point;
			fileStream.Close();
			AddElement(groupElement);
		}

		private void mnuAddPage_Click(object sender, EventArgs e)
		{
			AddPage();
			CreateUndoPoint("Add page");
		}

		private void mnuCopy_Click(object sender, EventArgs e)
		{
			Copy();
		}

		private void mnuCut_Click(object sender, EventArgs e)
		{
			Cut();
			CreateUndoPoint();
		}

		private void mnuDataFile_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog {
				Description = "Select the folder that contains the UO data (.mul) files you want to use."
			};
			if (folderBrowserDialog.ShowDialog() != DialogResult.OK) {
				return;
			}

			if (File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "art.mul"))) {
				XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
				XMLSettings.Save(this, XMLSettings.CurrentOptions);
				//int num = (int) Interaction.MsgBox( (object) "New path set, please restart Gump Studio to activate your changes.", MsgBoxStyle.OkOnly, (object) "Data Files" );
				MessageBox.Show("New path set, please restart Gump Studio to activate your changes.", "Data Files");
			}
			else {
				//int num1 = (int) Interaction.MsgBox( (object) "This path does not contain a file named \"art.mul\", it is most likely not the correct path.", MsgBoxStyle.OkOnly, (object) "Data Files" );
				MessageBox.Show("This path does not contain a file named \"art.mul\", it is most likely not the correct path.", "Data Files");
			}
		}

		private void mnuDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedElements();
		}

		private void mnuEditRedo_Click(object sender, EventArgs e)
		{
			Redo();
		}

		private void mnuEditUndo_Click(object sender, EventArgs e)
		{
			Undo();
		}

		private void mnuFileExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void mnuFileNew_Click(object sender, EventArgs e)
		{
			//if ( Interaction.MsgBox( (object) "Are you sure you want to start a new gump?", MsgBoxStyle.YesNo | MsgBoxStyle.Question, (object) null ) != MsgBoxResult.Yes )
			//    return;

			var result = MessageBox.Show("Are you sure you want to start a new gump?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (result != DialogResult.OK) {
				return;
			}

			ClearGump();
		}

		private void mnuFileOpen_Click(object sender, EventArgs e)
		{
			_OpenDialog.CheckFileExists = true;
			_OpenDialog.Filter = @"Gump|*.gump";
			if (_OpenDialog.ShowDialog() == DialogResult.OK) {
				LoadFrom(_OpenDialog.FileName);
				FileName = Path.GetFileName(_OpenDialog.FileName);
				Text = "Gump Studio (" + FileName + ")";
			}
			_picCanvas.Invalidate();
		}

		private void mnuFileSave_Click(object sender, EventArgs e)
		{
			_SaveDialog.AddExtension = true;
			_SaveDialog.DefaultExt = "gump";
			_SaveDialog.Filter = "Gump|*.gump";
			if (_SaveDialog.ShowDialog() != DialogResult.OK) {
				return;
			}

			SaveTo(_SaveDialog.FileName);
			FileName = Path.GetFileName(_SaveDialog.FileName);
			Text = "Gump Studio (" + FileName + ")";
		}

		private void mnuGroupCreate_Click(object sender, EventArgs e)
		{
			IEnumerator enumerator1 = null;
			var arrayList = new ArrayList();
			try {
				foreach (var element in ElementStack.GetElements()) {
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(element);
					if (objectValue.Selected) {
						arrayList.Add(objectValue);
					}
				}
			}
			finally {
				(enumerator1 as IDisposable)?.Dispose();
			}
			if (arrayList.Count >= 2) {
				IEnumerator enumerator2 = null;
				var groupElement = new GroupElement(ElementStack, null, "New Group");
				try {
					foreach (var obj in arrayList) {
						var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(obj);
						groupElement.AddElement(objectValue);
						ElementStack.RemoveElement(objectValue);
						ElementStack.RemoveEvents(objectValue);
					}
				}
				finally {
					(enumerator2 as IDisposable)?.Dispose();
				}
				AddElement(groupElement);
				_picCanvas.Invalidate();
			}
			CreateUndoPoint();
		}

		private void mnuHelpAbout_Click(object sender, EventArgs e)
		{
			var frmAboutBox = new frmAboutBox();
			frmAboutBox.SetText(AboutElementAppend);
			var num = (int)frmAboutBox.ShowDialog();
		}

		private void mnuImportGumpling_Click(object sender, EventArgs e)
		{
			_OpenDialog.Filter = @"Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";
			if (_OpenDialog.ShowDialog() != DialogResult.OK) {
				return;
			}

			var fileStream = new FileStream(_OpenDialog.FileName, FileMode.Open);
			var Gumpling = (GroupElement)new BinaryFormatter { Binder = DeserializationBinder.Instance }.Deserialize(fileStream);
			Gumpling.mIsBaseWindow = false;
			Gumpling.RecalculateBounds();
			var point = new Point(0, 0);
			Gumpling.Location = point;
			fileStream.Close();
			UncategorizedFolder.AddItem(new TreeGumpling(Path.GetFileName(_OpenDialog.FileName), Gumpling));
			BuildGumplingTree();
		}

		private void mnuPageClear_Click(object sender, EventArgs e)
		{
			ElementStack = new GroupElement(null, null, "Element Stack", true);
			CreateUndoPoint("Clear Page");
		}

		private void mnuPageDelete_Click(object sender, EventArgs e)
		{
			if (_TabPager.SelectedIndex == 0) {
				MessageBox.Show(@"Page 0 can not be deleted.");

			}
			else {
				var selectedIndex = _TabPager.SelectedIndex;
				var num2 = _TabPager.TabCount - 1;
				for (var index = selectedIndex + 1; index <= num2; ++index) {
					_TabPager.TabPages[index].Text = Convert.ToString(index - 1);
				}

				Stacks.RemoveAt(selectedIndex);
				_TabPager.TabPages.RemoveAt(selectedIndex);
				ChangeActiveStack(selectedIndex - 1);
				CreateUndoPoint("Delete page");
			}
		}

		private void mnuPageInsert_Click(object sender, EventArgs e)
		{
			if (_TabPager.SelectedIndex == 0) {
				//int num1 = (int) Interaction.MsgBox( (object) "Page 0 may not be moved.", MsgBoxStyle.OkOnly, (object) null );
				MessageBox.Show(@"Page 0 may not be moved.");
			}
			else {
				var tabCount = _TabPager.TabCount;
				var selectedIndex = _TabPager.SelectedIndex;
				var num2 = _TabPager.TabCount - 1;
				for (var index = selectedIndex; index <= num2; ++index) {
					_TabPager.TabPages.RemoveAt(selectedIndex);
				}

				_TabPager.TabPages.Add(new TabPage(selectedIndex.ToString()));
				var num3 = tabCount;
				for (var index = selectedIndex + 1; index <= num3; ++index) {
					_TabPager.TabPages.Add(new TabPage(index.ToString()));
				}

				var groupElement = new GroupElement(null, null, "Element Stack", true);
				Stacks.Insert(selectedIndex, groupElement);
				ChangeActiveStack(selectedIndex);
				_TabPager.SelectedIndex = selectedIndex;
				CreateUndoPoint("Insert page");
			}
		}

		private void mnuPaste_Click(object sender, EventArgs e)
		{
			Paste();
			CreateUndoPoint();
		}

		private void mnuPluginManager_Click(object sender, EventArgs e)
		{
			var num = (int)new PluginManager() {
				AvailablePlugins = AvailablePlugins,
				LoadedPlugins = LoadedPlugins,
				OrderList = PluginTypesToLoad,
				MainForm = this
			}.ShowDialog();
		}

		private void mnuSelectAll_Click(object sender, EventArgs e)
		{
			SelectAll();
		}

		private void mnuShow0_Click(object sender, EventArgs e)
		{
			ShowPage0 = !ShowPage0;
			_mnuShow0.Checked = ShowPage0;
			_picCanvas.Refresh();
		}

		public void Paste()
		{
			var dataObject = Clipboard.GetDataObject();
			var arrayList = new ArrayList();
			var data = (ArrayList)dataObject.GetData(typeof(ArrayList));
			if (data != null) {
				SetActiveElement(null, true);

				foreach (var obj in data) {
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(obj);
					if (CopyMode == ClipBoardMode.Copy) {
						objectValue.Name = "Copy of " + objectValue.Name;
					}

					objectValue.Selected = true;
					ElementStack.AddElement(objectValue);
				}
			}
			_picCanvas.Invalidate();
		}

		private void picCanvas_MouseDown(object sender, MouseEventArgs e)
		{
			CanvasFocus.Focus();
			var point = new Point(e.X, e.Y);
			mAnchor = point;
			var Element = ElementStack.GetElementFromPoint(point);
			if ((ActiveElement == null || ActiveElement.HitTest(point) == MoveModeType.None ? 0 : 1) != 0) {
				Element = ActiveElement;
			}

			if (Element != null) {
				MoveMode = Element.HitTest(point);
				if ((ActiveElement == null || ActiveElement.HitTest(point) != MoveModeType.None ? 0 : 1) != 0) {
					if (Element.Selected) {
						if ((ModifierKeys & Keys.Control) > Keys.None) {
							Element.Selected = false;
						}
						else {
							SetActiveElement(Element, false);
						}
					}
					else {
						SetActiveElement(Element, (ModifierKeys & Keys.Control) <= Keys.None);
					}
				}
				else if (ActiveElement == null) {
					SetActiveElement(Element, false);
				}
				else if (ActiveElement != null && (ModifierKeys & Keys.Control) > Keys.None) {
					ActiveElement.Selected = false;
					var selectedElements = ElementStack.GetSelectedElements();
					if (selectedElements.Count > 0) {
						SetActiveElement((BaseElement)selectedElements[0], false);
					}
					else {
						SetActiveElement(null, true);
						MoveMode = MoveModeType.None;
					}
				}
			}
			else {
				MoveMode = MoveModeType.None;
				if ((e.Button & MouseButtons.Left) > MouseButtons.None) {
					SetActiveElement(null, (ModifierKeys & Keys.Control) <= Keys.None);
				}
			}
			_picCanvas.Invalidate();
			LastPos = point;
			if (ActiveElement != null) {
				mAnchorOffset.Width = ActiveElement.X - point.X;
				mAnchorOffset.Height = ActiveElement.Y - point.Y;
			}
			ElementChanged = false;
			MoveCount = 0;
		}

		private void picCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			var point1 = new Point(e.X, e.Y);
			var num1 = point1.X - LastPos.X;
			var num2 = point1.Y - LastPos.Y;
			var baseElement = ElementStack.GetElementFromPoint(point1);
			if ((ActiveElement == null || ActiveElement.HitTest(point1) == MoveModeType.None ? 0 : 1) != 0) {
				baseElement = ActiveElement;
			}

			if (MoveMode == MoveModeType.Move) {
				point1.Offset(mAnchorOffset.Width, mAnchorOffset.Height);
			}

			var e1 = new MouseMoveHookEventArgs {
				Keys = ModifierKeys,
				MouseButtons = e.Button,
				MouseLocation = point1,
				MoveMode = MoveMode
			};

			foreach (var loadedPlugin in LoadedPlugins) {
				((BasePlugin)RuntimeHelpers.GetObjectValue(loadedPlugin)).MouseMoveHook(ref e1);
				point1 = e1.MouseLocation;
			}

			if ((MoveMode != MoveModeType.None || Math.Abs(num1) <= 0 || Math.Abs(num2) <= 0 ? 0 : 1) != 0) {
				MoveMode = MoveModeType.SelectionBox;
			}

			if (e.Button != MouseButtons.Left) {
				if (baseElement != null) {
					switch (baseElement.HitTest(point1)) {
						case MoveModeType.ResizeTopLeft:
						case MoveModeType.ResizeBottomRight:
							Cursor = Cursors.SizeNWSE;
							break;
						case MoveModeType.ResizeTopRight:
						case MoveModeType.ResizeBottomLeft:
							Cursor = Cursors.SizeNESW;
							break;
						case MoveModeType.Move:
							Cursor = Cursors.SizeAll;
							break;
						case MoveModeType.ResizeLeft:
						case MoveModeType.ResizeRight:
							Cursor = Cursors.SizeWE;
							break;
						case MoveModeType.ResizeTop:
						case MoveModeType.ResizeBottom:
							Cursor = Cursors.SizeNS;
							break;
						default:
							Cursor = Cursors.Default;
							break;
					}
				}
				else {
					Cursor = Cursors.Default;
				}
			}
			else {
				++MoveCount;
				if (MoveCount > 100) {
					MoveCount = 2;
				}

				var rectangle = new Rectangle(0, 0, _picCanvas.Width, _picCanvas.Height);
				Cursor.Clip = _picCanvas.RectangleToScreen(rectangle);
				if (MoveMode != MoveModeType.None) {
					switch (MoveMode) {
						case MoveModeType.ResizeTopLeft:
						case MoveModeType.ResizeBottomRight:
							Cursor = Cursors.SizeNWSE;
							break;
						case MoveModeType.ResizeTopRight:
						case MoveModeType.ResizeBottomLeft:
							Cursor = Cursors.SizeNESW;
							break;
						case MoveModeType.Move:
							Cursor = Cursors.SizeAll;
							break;
						case MoveModeType.ResizeLeft:
						case MoveModeType.ResizeRight:
							Cursor = Cursors.SizeWE;
							break;
						case MoveModeType.ResizeTop:
						case MoveModeType.ResizeBottom:
							Cursor = Cursors.SizeNS;
							break;
						default:
							Cursor = Cursors.Default;
							break;
					}
					if (MoveCount >= 2) {
						ElementChanged = true;
					}
				}
				switch (MoveMode) {
					case MoveModeType.SelectionBox:
						rectangle = new Rectangle(mAnchor, new Size(point1.X - mAnchor.X, point1.Y - mAnchor.Y));
						SelectionRect = GetPositiveRect(rectangle);
						ShowSelectionRect = true;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeTopLeft:
						point1.Offset(3, 0);
						var point2 = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);
						ActiveElement.Location = point1;
						var size1 = ActiveElement.Size;
						var location1 = ActiveElement.Location;
						size1.Width = point2.X - point1.X;
						size1.Height = point2.Y - point1.Y;
						if (size1.Width < 1) {
							location1.X = point2.X - 1;
							size1.Width = 1;
						}
						if (size1.Height < 1) {
							location1.Y = point2.Y - 1;
							size1.Height = 1;
						}
						ActiveElement.Size = size1;
						ActiveElement.Location = location1;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeTopRight:
						point1.Offset(-3, 0);
						var point3 = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);
						var location2 = ActiveElement.Location;
						location2.Y = point1.Y;
						ActiveElement.Location = location2;
						var size2 = ActiveElement.Size;
						size2.Height = point3.Y - point1.Y;
						size2.Width = point1.X - ActiveElement.X;
						if (size2.Height < 1) {
							location2.Y = point3.Y - 1;
							size2.Height = 1;
						}
						if (size2.Width < 1) {
							size2.Width = 1;
						}

						location2.X = ActiveElement.Location.X;
						ActiveElement.Size = size2;
						ActiveElement.Location = location2;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeBottomRight:

						if (ActiveElement == null) {
							break;
						}

						point1.Offset(-3, -3);
						var size3 = ActiveElement.Size;
						size3.Width = point1.X - ActiveElement.X;
						size3.Height = point1.Y - ActiveElement.Y;
						if (size3.Width < 1) {
							size3.Width = 1;
						}

						if (size3.Height < 1) {
							size3.Height = 1;
						}

						ActiveElement.Size = size3;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeBottomLeft:

						if (ActiveElement == null) {
							break;
						}

						point1.Offset(0, -3);
						var point4 = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);
						var location3 = ActiveElement.Location;
						location3.X = point1.X;
						ActiveElement.Location = location3;
						var size4 = ActiveElement.Size;
						size4.Width = point4.X - point1.X;
						size4.Height = point1.Y - ActiveElement.Y;
						if (size4.Width < 1) {
							location3.X = point4.X - 1;
							size4.Width = 1;
						}
						if (size4.Height < 1) {
							size4.Height = 1;
						}

						location3.Y = ActiveElement.Y;
						ActiveElement.Size = size4;
						ActiveElement.Location = location3;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.Move:

						if (ActiveElement == null) {
							break;
						}

						IEnumerator enumerator2 = null;
						var location4 = ActiveElement.Location;
						ActiveElement.Location = point1;
						var dx = ActiveElement.X - location4.X;
						var dy = ActiveElement.Y - location4.Y;
						try {
							foreach (var selectedElement in ElementStack.GetSelectedElements()) {
								var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(selectedElement);
								if (objectValue != ActiveElement) {
									var location5 = objectValue.Location;
									location5.Offset(dx, dy);
									objectValue.Location = location5;
								}
							}
						}
						finally {
							(enumerator2 as IDisposable)?.Dispose();
						}
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeLeft:
						point1.Offset(3, 0);
						var point5 = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);
						var y = ActiveElement.Y;
						ActiveElement.Location = point1;
						var size5 = ActiveElement.Size;
						var location6 = ActiveElement.Location;
						size5.Width = point5.X - point1.X;
						if (size5.Width < 1) {
							location6.X = point5.X - 1;
							size5.Width = 1;
						}
						location6.Y = y;
						ActiveElement.Size = size5;
						ActiveElement.Location = location6;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeTop:
						point1.Offset(0, 3);
						var point6 = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);
						var x = ActiveElement.X;
						ActiveElement.Location = point1;
						var size6 = ActiveElement.Size;
						var location7 = ActiveElement.Location;
						size6.Height = point6.Y - point1.Y;
						if (size6.Height < 1) {
							location7.Y = point6.Y - 1;
							size6.Height = 1;
						}
						location7.X = x;
						ActiveElement.Size = size6;
						ActiveElement.Location = location7;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeRight:
						point1.Offset(-3, 0);
						var size7 = ActiveElement.Size;
						size7.Width = point1.X - ActiveElement.X;
						if (size7.Width < 1) {
							size7.Width = 1;
						}

						ActiveElement.Size = size7;
						_picCanvas.Invalidate();
						break;
					case MoveModeType.ResizeBottom:
						point1.Offset(0, -3);
						var size8 = ActiveElement.Size;
						size8.Height = point1.Y - ActiveElement.Y;
						if (size8.Height < 1) {
							size8.Height = 1;
						}

						ActiveElement.Size = size8;
						_picCanvas.Invalidate();
						break;
				}
			}
			LastPos = point1;
		}

		private void picCanvas_MouseUp(object sender, MouseEventArgs e)
		{
			var rectangle = new Rectangle();
			var point = new Point(e.X, e.Y);
			ElementStack.GetElementFromPoint(point);
			ShowSelectionRect = false;
			Cursor.Clip = rectangle;
			if (MoveMode == MoveModeType.SelectionBox) {
				BaseElement Element = null;

				foreach (var element in ElementStack.GetElements()) {
					var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(element);
					if (objectValue.ContainsTest(SelectionRect)) {
						objectValue.Selected = true;
						Element = objectValue;
					}
					else if ((ModifierKeys & Keys.Control) <= Keys.None) {
						objectValue.Selected = false;
					}
				}

				SetActiveElement(Element, false);
			}
			if ((MoveMode == MoveModeType.None || MoveMode == MoveModeType.SelectionBox || !ElementChanged ? 0 : 1) != 0) {
				CreateUndoPoint("Element Moved");
				ElementChanged = false;
			}
			if ((e.Button & MouseButtons.Right) > MouseButtons.None) {
				var mnuContextMenu = m_mnuContextMenu;
				GetContextMenu(ref ActiveElement, mnuContextMenu);
				mnuContextMenu.Show(_picCanvas, point);
				ClearContextMenu(mnuContextMenu);
			}
			SetActiveElement(ActiveElement, false);
			_picCanvas.Invalidate();
			MoveMode = MoveModeType.None;
			mAnchorOffset = new Size(0, 0);
		}

		private void picCanvas_Paint(object sender, PaintEventArgs e)
		{
			Render(e.Graphics);
		}

		private void pnlCanvasScroller_MouseLeave(object sender, EventArgs e)
		{
			Cursor = Cursors.Default;
		}

		public void RebuildTabPages()
		{
			_TabPager.TabPages.Clear();
			var num = -1;

			foreach (var stack in Stacks) {
				var objectValue = RuntimeHelpers.GetObjectValue(stack);
				++num;
				_TabPager.TabPages.Add(new TabPage(Convert.ToString(num)));
				if (ElementStack == objectValue) {
					_TabPager.SelectedIndex = num;
				}
			}
		}

		public void Redo()
		{
			if (CurrentUndoPoint < UndoPoints.Count) {
				++CurrentUndoPoint;
				RevertToUndoPoint(CurrentUndoPoint);
			}
			if (CurrentUndoPoint == UndoPoints.Count - 1) {
				_mnuEditRedo.Enabled = false;
			}

			_mnuEditUndo.Enabled = true;
		}

		public void RefreshElementList()
		{
			m_cboElements.Items.Clear();
			m_cboElements.Items.AddRange(ElementStack.GetElements().ToArray());
		}

		public void RefreshView(object sender)
		{
			RefreshElementList();
			m_cboElements.SelectedItem = ActiveElement;
			if (ElementStack.GetSelectedElements().Count > 1) {
				m_ElementProperties.SelectedObjects = ElementStack.GetSelectedElements().ToArray();
			}
			else {
				m_ElementProperties.SelectedObject = ActiveElement;
			}
		}

		protected void Render(Graphics Target)
		{
			var Target1 = Graphics.FromImage(Canvas);
			if (!PluginClearsCanvas) {
				Target1.Clear(Color.Black);
			}

			var hookPreRender = HookPreRender;
			hookPreRender?.Invoke(Canvas);
			if (Stacks.Count > 0 && (!ShowPage0 || ElementStack == Stacks[0] ? 0 : 1) != 0) {
				((BaseElement)Stacks[0]).Render(Target1);
			}

			ElementStack.Render(Target1);

			foreach (var element in ElementStack.GetElements()) {
				var objectValue = (BaseElement)RuntimeHelpers.GetObjectValue(element);
				if ((!objectValue.Selected || objectValue == ActiveElement ? 0 : 1) != 0) {
					objectValue.DrawBoundingBox(Target1, false);
				}
			}

			ActiveElement?.DrawBoundingBox(Target1, true);
			if (ShowSelectionRect) {
				Target1.FillRectangle(SelBG, SelectionRect);
				Target1.DrawRectangle(SelFG, SelectionRect);
			}
			var hookPostRender = HookPostRender;
			hookPostRender?.Invoke(Canvas);
			Target1.Dispose();
			Target.DrawImage(Canvas, 0, 0);
		}

		public void RevertToUndoPoint(int Index)
		{
			var undoPoint = (UndoPoint)UndoPoints[Index];
			GumpProperties = (GumpProperties)undoPoint.GumpProperties.Clone();
			Stacks = new ArrayList();
			foreach (var obj in undoPoint.Stack) {
				var objectValue = (GroupElement)RuntimeHelpers.GetObjectValue(obj);
				var groupElement = (GroupElement)objectValue.Clone();
				Stacks.Add(groupElement);
				if (undoPoint.ElementStack == objectValue) {
					ElementStack = groupElement;
				}
			}

			RebuildTabPages();
			_picCanvas.Invalidate();
			SetActiveElement(null, true);
			CurrentUndoPoint = Index;
		}

		public void SaveTo(string Path)
		{
			_StatusBar.Text = $@"Saving gump...";
			ElementStack.UpdateParent -= new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint -= new BaseElement.RepaintEventHandler(RefreshView);
			var fileStream = new FileStream(Path, FileMode.Create);
			var binaryFormatter = new BinaryFormatter { Binder = DeserializationBinder.Instance };
			binaryFormatter.Serialize(fileStream, Stacks);
			binaryFormatter.Serialize(fileStream, GumpProperties);
			fileStream.Close();
			ElementStack.UpdateParent += new BaseElement.UpdateParentEventHandler(ChangeActiveElementEventHandler);
			ElementStack.Repaint += new BaseElement.RepaintEventHandler(RefreshView);
			_StatusBar.Text = "";
		}

		public void SelectAll()
		{
			foreach (var selectedElement in ElementStack.GetSelectedElements()) {
				((BaseElement)RuntimeHelpers.GetObjectValue(selectedElement)).Selected = true;
			}

			_picCanvas.Invalidate();
		}

		public void SetActiveElement(BaseElement e)
		{
			SetActiveElement(e, false);
		}

		public void SetActiveElement(BaseElement Element, bool DeselectOthers)
		{
			if (DeselectOthers) {
				foreach (var element in ElementStack.GetElements()) {
					((BaseElement)RuntimeHelpers.GetObjectValue(element)).Selected = false;
				}
			}
			if (ActiveElement != Element) {
				RefreshElementList();
				ActiveElement = Element;
				m_cboElements.SelectedItem = Element;
				if (Element != null) {
					Element.Selected = true;
				}
			}
			if (ElementStack.GetSelectedElements().Count > 1) {
				m_ElementProperties.SelectedObjects = ElementStack.GetSelectedElements().ToArray();
			}
			else if (Element != null) {
				m_ElementProperties.SelectedObject = Element;
			}
			else {
				m_ElementProperties.SelectedObject = GumpProperties;
			}
		}

		public Point SnapLocToGrid(Point Position, Size GridSize)
		{
			var point = Position;
			point.X = point.X / GridSize.Width * GridSize.Width;
			point.Y = point.Y / GridSize.Height * GridSize.Height;
			return point;
		}

		private void TabPager_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_TabPager.SelectedIndex != -1) {
				ChangeActiveStack(_TabPager.SelectedIndex);
			}

			RefreshElementList();
		}

		private void treGumplings_DoubleClick(object sender, EventArgs e)
		{
			if (_treGumplings.SelectedNode.Tag == null || !(_treGumplings.SelectedNode.Tag is TreeGumpling)) {
				return;
			}

			var groupElement = (GroupElement)((TreeGumpling)_treGumplings.SelectedNode.Tag).Gumpling.Clone();
			groupElement.mIsBaseWindow = false;
			groupElement.RecalculateBounds();
			var point = new Point(0, 0);
			groupElement.Location = point;
			AddElement(groupElement);
		}

		private void treGumplings_MouseUp(object sender, MouseEventArgs e)
		{
			_treGumplings.SelectedNode = _treGumplings.GetNodeAt(new Point(e.X, e.Y));
		}

		public void Undo()
		{
			--CurrentUndoPoint;
			RevertToUndoPoint(CurrentUndoPoint);
			if (CurrentUndoPoint == 0) {
				_mnuEditUndo.Enabled = false;
			}

			_mnuEditRedo.Enabled = true;
		}

		public void WritePluginsToLoad()
		{
			if (PluginTypesToLoad != null) {
				var fileStream = new FileStream(Application.StartupPath + "\\LoadInfo.bin", FileMode.Create);
				new BinaryFormatter { Binder = DeserializationBinder.Instance }.Serialize(fileStream, PluginTypesToLoad);
				fileStream.Close();
			}
			else {
				if (!File.Exists(Application.StartupPath + "\\LoadInfo.bin")) {
					return;
				}

				File.Delete(Application.StartupPath + "\\LoadInfo.bin");
			}
		}

		public delegate void HookKeyDownEventHandler(object sender, ref KeyEventArgs e);
		public delegate void HookPostRenderEventHandler(Bitmap Target);
		public delegate void HookPreRenderEventHandler(Bitmap Target);
	}
}
