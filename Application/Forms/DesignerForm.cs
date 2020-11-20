using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using GumpStudio.Elements;
using GumpStudio.Plugins;

using Ultima;

namespace GumpStudio.Forms
{
	public sealed partial class DesignerForm : Form
	{
		private ComboBox _ComboElements;
		private PropertyGrid _ElementProperties;
		private Label _Label;
		private MainMenu _Menu;
		private MenuItem _MenuMain1;
		private MenuItem _MenuMain2;
		private MenuItem _MenuMain3;
		private MenuItem _MenuMain4;
		private MenuItem _MenuMain5;
		private MenuItem _MenuMain6;
		private ContextMenu _MenuContext;
		private MenuItem _MenuCopy;
		private MenuItem _MenuCut;
		private MenuItem _MenuDataFile;
		private MenuItem _MenuDelete;
		private MenuItem _MenuEdit;
		private MenuItem _MenuEditRedo;
		private MenuItem _MenuEditUndo;
		private MenuItem _MenuFile;
		private MenuItem _MenuFileExit;
		private MenuItem _MenuFileNew;
		private MenuItem _MenuFileOpen;
		private MenuItem _MenuFileSave;
		private MenuItem _MenuGumplingAddFolder;
		private MenuItem _MenuGumplingAddGumpling;
		private ContextMenu _MenuGumplingContext;
		private MenuItem _MenuGumplingDelete;
		private MenuItem _MenuGumplingMove;
		private MenuItem _MenuGumplingRename;
		private MenuItem _MenuHelp;
		private MenuItem _MenuHelpAbout;
		private MenuItem _MenuImportGumpling;
		private MenuItem _MenuMisc;
		private MenuItem _MenuMiscLoadGumpling;
		private MenuItem _MenuPage;
		private MenuItem _MenuPageAdd;
		private MenuItem _MenuPageClear;
		private MenuItem _MenuPageDelete;
		private MenuItem _MenuPageInsert;
		private MenuItem _MenuPaste;
		private MenuItem _MenuPluginManager;
		private MenuItem _MenuSelectAll;
		private MenuItem _MenuShowPage0;
		private OpenFileDialog _OpenDialog;
		private Panel _Panel1;
		private Panel _Panel2;
		private Panel _Panel3;
		private Panel _Panel4;
		private Panel _PanelCanvasScroller;
		private Panel _PanelToolbox;
		private Panel _Panel7;
		private SaveFileDialog _SaveDialog;
		private Splitter _Splitter1;
		private Splitter _Splitter2;
		private StatusBar _StatusBar;
		private TabPage _TabPage;
		private TabControl _TabPages;
		private TabControl _TabToolbox;
		private TabPage _PageCustom;
		private TabPage _PageStandard;
		private TreeView _Gumplings;
		private IContainer _Components;
	}

	public sealed partial class DesignerForm
	{
		private Point _LastPosition;
		private Point _Anchor;
		private Size _AnchorOffset;
		private Rectangle _SelectionRect;

		private LinearGradientBrush _SelectionBG;
		private Pen _SelectionFG;

		private bool _ElementChanged;
		private bool _ShowPage0 = true;
		private bool _ShowSelectionRect;

		private int _MoveCount;
		private int _CurrentUndoPoint = -1;

		private string _FileName = String.Empty;
		private string _AboutElementAppend = String.Empty;

		private MoveModeType _MoveMode;

		private Bitmap _Canvas;

		private readonly ArrayList _UndoPoints = new ArrayList();
		private readonly ArrayList _RegisteredTypes = new ArrayList();

		private readonly HashSet<BasePlugin> _AvailablePlugins = new HashSet<BasePlugin>();
		private readonly HashSet<BasePlugin> _LoadedPlugins = new HashSet<BasePlugin>();

		public HashSet<PluginInfo> PluginsInfo { get; } = new HashSet<PluginInfo>();

		public List<GroupElement> Stacks { get; } = new List<GroupElement>();

		public decimal ArrowKeyDelta { get; set; } = 1;

		public int MaxUndoPoints { get; set; } = 50;

		public bool PluginClearsCanvas { get; set; }
		public bool SuppressUndoPoints { get; set; }
		public bool ShouldClearActiveElement { get; set; }

		public TreeFolder UncategorizedFolder { get; set; }
		public TreeFolder GumplingsFolder { get; set; }
		public TreeFolder GumplingTree { get; set; }

		public MenuItem MenuFileExport { get; set; }
		public MenuItem MenuFileImport { get; set; }
		public MenuItem MenuPlugins { get; set; }
		public PictureBox ImageCanvas { get; set; }

		public BaseElement ActiveElement { get; set; }

		public GroupElement ElementStack { get; set; } = new GroupElement(null, null, "CanvasStack", true);

		public GumpProperties GumpProperties { get; set; } = new GumpProperties();

		public TextBox CanvasFocus { get; [DebuggerNonUserCode, MethodImpl(MethodImplOptions.Synchronized)] set; }

		public IEnumerable<BaseElement> AllElements => Stacks.OfType<GroupElement>().SelectMany(s => s.AllElements);

		public string AppPath => Application.StartupPath;

		public event HookKeyDownEventHandler HookKeyDown;
		public event HookPostRenderEventHandler HookPostRender;
		public event HookPreRenderEventHandler HookPreRender;

		public DesignerForm()
		{
			Closed += DesignerForm_Closed;
			Closing += DesignerForm_Closing;

			InitializeComponent();

			GlobalObjects.DesignerForm = this;
		}

		public void NormalizeNames()
		{
			var elements = AllElements.ToArray();

			var nums = String.Join(String.Empty, Enumerable.Range(0, 9)).ToCharArray();

			foreach (var o in elements)
			{
				o.Name = Regex.Replace(o.Name, "Copy Of", String.Empty, RegexOptions.IgnoreCase).TrimEnd(nums).Trim();
			}

			var buttons = 0;
			var switches = 0;

			foreach (var g in elements.GroupBy(o => o.GetType()))
			{
				var index = 0;

				foreach (var group in g.ToLookup(o => o.Name))
				{
					var name = group.Key;
					var count = group.Count();

					if (count == 0)
					{
						continue;
					}

					foreach (var o in group)
					{
						if (o is TextEntryElement te)
						{
							te.ID = index;
						}

						if (o is ButtonElement)
						{
							o.Name = $"{name}{++buttons}";
						}
						else if (o is CheckboxElement)
						{
							o.Name = $"{name}{++switches}";
						}
						else
						{
							o.Name = $"{name}{++index}";
						}
					}
				}
			}
		}

		public void AddElement(BaseElement element)
		{
			ElementStack.AddElement(element);

			element.Selected = true;

			SetActiveElement(element, true);

			ImageCanvas.Invalidate();

			CreateUndoPoint(element.Name + " added");
		}

		public int AddPage()
		{
			var index = Stacks.Count;

			Stacks.Add(new GroupElement(null, null, "CanvasStack", true));

			_TabPages.TabPages.Add(new TabPage(index.ToString()));

			_TabPages.SelectedIndex = index;

			ChangeActiveStack(index);

			return index;
		}

		public void BuildGumplingTree()
		{
			_Gumplings.Nodes.Clear();

			BuildGumplingTree(GumplingTree, null);
		}

		public void BuildGumplingTree(TreeFolder Item, TreeNode Node)
		{
			foreach (TreeItem item in Item.GetChildren())
			{
				var treeNode = new TreeNode
				{
					Text = item.Text,
					Tag = item
				};

				if (Node == null)
				{
					_Gumplings.Nodes.Add(treeNode);
				}
				else
				{
					Node.Nodes.Add(treeNode);
				}

				if (item is TreeFolder folder)
				{
					BuildGumplingTree(folder, treeNode);
				}
			}
		}

		private void BuildToolbox()
		{
			_PanelToolbox.Controls.Clear();

			try
			{
				var y = 0;

				foreach (Type type in _RegisteredTypes)
				{
					var instance = (BaseElement)Activator.CreateInstance(type);

					var button = new Button
					{
						Text = instance.Type,
						Location = new Point(0, y),
						FlatStyle = FlatStyle.System,
						Width = _PanelToolbox.Width,
						Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
						Tag = type
					};

					y += button.Height - 1;

					_PanelToolbox.Controls.Add(button);

					button.Click += CreateElementFromToolbox;

					if (instance.DispayInAbout())
					{
						_AboutElementAppend += $"{Environment.NewLine}{instance.Type}: {instance.GetAboutText()}{Environment.NewLine}";
					}

					foreach (var plugin in _LoadedPlugins)
					{
						plugin.InitializeElementExtenders(instance);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
			}

			BaseElement.ResetID();

			GumplingTree = new TreeFolder("Root");
			GumplingsFolder = new TreeFolder("My Gumplings");
			UncategorizedFolder = new TreeFolder("Uncategorized");

			GumplingTree.AddItem(GumplingsFolder);
			GumplingTree.AddItem(UncategorizedFolder);

			BuildGumplingTree();
		}

		private void ComboElements_Click(object sender, EventArgs e)
		{
			foreach (var element in ElementStack.Elements)
			{
				element.Selected = false;
			}

			ActiveElement = null;
		}

		private void ComboElements_SelectedIndexChanged(object sender, EventArgs e)
		{
			SetActiveElement(_ComboElements.SelectedItem as BaseElement, false);

			ImageCanvas.Invalidate();
		}

		private void ChangeActiveElementEventHandler(BaseElement e, bool DeselectOthers)
		{
			SetActiveElement(e, DeselectOthers);

			ImageCanvas.Invalidate();
		}

		public void ChangeActiveStack(int stackID)
		{
			if (stackID >= Stacks.Count)
			{
				return;
			}

			SetActiveElement(null, true);

			if (ElementStack != null)
			{
				ElementStack.UpdateParent -= ChangeActiveElementEventHandler;
				ElementStack.Repaint -= RefreshView;
			}

			ElementStack = Stacks[stackID];

			ElementStack.UpdateParent += ChangeActiveElementEventHandler;
			ElementStack.Repaint += RefreshView;

			ImageCanvas.Invalidate();
		}

		public void ClearContextMenu(Menu menu)
		{
			menu.MenuItems.Clear();
		}

		public void ClearGump()
		{
			BaseElement.ResetID();

			_FileName = String.Empty;

			Text = "Gump Studio (-Unsaved Gump-)";

			_TabPages.TabPages.Clear();
			_TabPages.TabPages.Add(new TabPage("0"));

			ElementStack = new GroupElement(null, null, "Element Stack", true);
			ElementStack.UpdateParent += ChangeActiveElementEventHandler;
			ElementStack.Repaint += RefreshView;

			Stacks.Clear();
			Stacks.Add(ElementStack);

			SetActiveElement(null);

			ImageCanvas.Invalidate();

			ChangeActiveStack(0);

			_UndoPoints.Clear();

			CreateUndoPoint("Blank");

			_MenuEditUndo.Enabled = false;
			_MenuEditRedo.Enabled = false;
		}

		public void Copy()
		{
			Clipboard.SetDataObject(ElementStack.GetSelectedElements().Select(e => e.Clone()).ToArray());
		}

		public void CreateElementFromToolbox(object sender, EventArgs e)
		{
			if (sender is Control c && c.Tag is Type t && typeof(BaseElement).IsAssignableFrom(t))
			{
				AddElement((BaseElement)Activator.CreateInstance(t));
			}

			ImageCanvas.Invalidate();
			ImageCanvas.Focus();
		}

		public void CreateUndoPoint()
		{
			CreateUndoPoint("Unknown Action");
		}

		public void CreateUndoPoint(string action)
		{
			if (SuppressUndoPoints)
			{
				return;
			}

			while (_UndoPoints.Count - 1 > _CurrentUndoPoint)
			{
				_UndoPoints.RemoveAt(_CurrentUndoPoint + 1);
			}

			while (_UndoPoints.Count >= MaxUndoPoints)
			{
				_UndoPoints.RemoveAt(0);
			}

			_CurrentUndoPoint = _UndoPoints.Add(new UndoPoint(this)
			{
				Text = action
			});

			_MenuEditUndo.Enabled = true;
			_MenuEditRedo.Enabled = false;
		}

		public void Cut()
		{
			Clipboard.SetDataObject(ElementStack.GetSelectedElements().ToArray());

			DeleteSelectedElements();
		}

		private void DeleteSelectedElements()
		{
			var flag = false;

			var elements = ElementStack.Elements.ToArray();

			foreach (var element in elements)
			{
				flag = true;

				if (element.Selected)
				{
					ElementStack.RemoveElement(element);
				}
			}

			SetActiveElement(GetLastSelectedControl());

			ImageCanvas.Invalidate();

			if (flag)
			{
				CreateUndoPoint("Delete Elements");
			}
		}

		private void DesignerForm_Closed(object sender, EventArgs e)
		{
			_SelectionFG?.Dispose();
			_SelectionBG?.Dispose();

			WritePluginsToLoad();
		}

		private void DesignerForm_Closing(object sender, CancelEventArgs e)
		{
			foreach (var plugin in _AvailablePlugins)
			{
				if (plugin.IsLoaded)
				{
					plugin.Unload();
				}
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

			if (e.Handled || ActiveControl != CanvasFocus)
			{
				return;
			}

			var flag = false;

			if ((e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back ? 1 : 0) != 0)
			{
				DeleteSelectedElements();

				flag = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Up)
			{
				foreach (var selectedElement in ElementStack.GetSelectedElements())
				{
					var location = selectedElement.Location;

					location.Offset(0, -Convert.ToInt32(ArrowKeyDelta));

					selectedElement.Location = location;
				}

				//ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Down)
			{
				foreach (var selectedElement in ElementStack.GetSelectedElements())
				{
					var location = selectedElement.Location;

					location.Offset(0, Convert.ToInt32(ArrowKeyDelta));

					selectedElement.Location = location;
				}

				//ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Left)
			{
				foreach (var selectedElement in ElementStack.GetSelectedElements())
				{
					var location = selectedElement.Location;

					location.Offset(-Convert.ToInt32(ArrowKeyDelta), 0);

					selectedElement.Location = location;
				}

				//ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Right)
			{
				foreach (var selectedElement in ElementStack.GetSelectedElements())
				{
					var location = selectedElement.Location;

					location.Offset(Convert.ToInt32(ArrowKeyDelta), 0);

					selectedElement.Location = location;
				}

				//ArrowKeyDelta = Decimal.Multiply(ArrowKeyDelta, new decimal(106, 0, 0, false, 2));
				flag = true;
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Next)
			{
				var index = (ActiveElement == null ? ElementStack._Elements.Count - 1 : ActiveElement.Z) - 1;

				if (index < 0)
				{
					index = ElementStack._Elements.Count - 1;
				}

				if (index >= 0 & index <= ElementStack._Elements.Count - 1)
				{
					SetActiveElement((BaseElement)ElementStack._Elements[index], true);
				}

				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Prior)
			{
				var index = (ActiveElement == null ? ElementStack._Elements.Count - 1 : ActiveElement.Z) + 1;

				if (index > ElementStack._Elements.Count - 1)
				{
					index = 0;
				}

				SetActiveElement((BaseElement)ElementStack._Elements[index], true);

				e.Handled = true;
			}
			/*
			if (Decimal.Compare(ArrowKeyDelta, new decimal(10)) > 0)
			{
				ArrowKeyDelta = new decimal(10);
			}
			*/
			if (flag)
			{
				ImageCanvas.Invalidate();

				_ElementProperties.SelectedObjects = _ElementProperties.SelectedObjects;
			}
		}

		private void DesignerForm_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
			{
				CreateUndoPoint("Move element");
				//ArrowKeyDelta = new decimal(1);
			}
		}

		private void DesignerForm_Load(object sender, EventArgs e)
		{
			XMLSettings.CurrentOptions = XMLSettings.Load(this);

			if (!File.Exists(Path.Combine(XMLSettings.CurrentOptions.ClientPath, "client.exe")))
			{
				var folderBrowserDialog = new FolderBrowserDialog
				{
					SelectedPath = Environment.SpecialFolder.ProgramFiles.ToString(),
					Description = @"Select the folder that contains a copy of Ultima Online."
				};

				do
				{
					if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
					{
						Close();
						return;
					}
				}
				while (!File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "client.exe")));

				XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
				XMLSettings.Save(this, XMLSettings.CurrentOptions);
			}

			Files.CacheData = false;
			Files.SetMulPath(XMLSettings.CurrentOptions.ClientPath);

			Size = XMLSettings.CurrentOptions.DesignerFormSize;
			MaxUndoPoints = XMLSettings.CurrentOptions.UndoLevels;

			ImageCanvas.Width = 1600;
			ImageCanvas.Height = 1200;

			CenterToScreen();

			EnumeratePlugins();

			_Canvas = new Bitmap(ImageCanvas.Width, ImageCanvas.Height, PixelFormat.Format32bppRgb);

			Activate();

			GumpProperties = new GumpProperties();

			ElementStack.UpdateParent += ChangeActiveElementEventHandler;
			ElementStack.Repaint += RefreshView;

			Stacks.Clear();
			Stacks.Add(ElementStack);

			ChangeActiveStack(0);

			_RegisteredTypes.Clear();

			_RegisteredTypes.Add(typeof(LabelElement));
			_RegisteredTypes.Add(typeof(ImageElement));
			_RegisteredTypes.Add(typeof(TiledElement));
			_RegisteredTypes.Add(typeof(BackgroundElement));
			_RegisteredTypes.Add(typeof(AlphaElement));
			_RegisteredTypes.Add(typeof(CheckboxElement));
			_RegisteredTypes.Add(typeof(RadioElement));
			_RegisteredTypes.Add(typeof(ItemElement));
			_RegisteredTypes.Add(typeof(TextEntryElement));
			_RegisteredTypes.Add(typeof(ButtonElement));
			_RegisteredTypes.Add(typeof(HTMLElement));

			BuildToolbox();

			_SelectionFG = new Pen(Color.Blue, 2f);

			_SelectionBG = new LinearGradientBrush(new Rectangle(0, 0, 50, 50), Color.FromArgb(90, Color.Blue), Color.FromArgb(110, Color.Blue), LinearGradientMode.ForwardDiagonal)
			{
				WrapMode = WrapMode.TileFlipXY
			};

			CreateUndoPoint("Blank");

			_MenuEditUndo.Enabled = false;

			SplashBox.DisplaySplash();
		}

		private void ElementProperties_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name == "Name")
			{
				_ComboElements.Items.Clear();
				_ComboElements.Items.AddRange(ElementStack.Elements.ToArray());
				_ComboElements.SelectedItem = RuntimeHelpers.GetObjectValue(_ElementProperties.SelectedObject);
			}

			ImageCanvas.Invalidate();

			CreateUndoPoint("Property Changed");
		}

		private void EnumeratePlugins()
		{
			PluginsInfo.Clear();
			PluginsInfo.UnionWith(GetPluginsToLoad());

			var appendInfo = new StringBuilder();

			foreach (var file in Directory.GetFiles(Application.StartupPath, "*.dll", SearchOption.AllDirectories))
			{
				try
				{
					if (file.EndsWith("Ultima.dll", StringComparison.OrdinalIgnoreCase) || file.EndsWith("UOFont.dll", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}

					var asm = Assembly.LoadFile(file);

					foreach (var type in asm.GetTypes())
					{
						if (type.IsAbstract)
						{
							continue;
						}

						if (type.IsSubclassOf(typeof(BasePlugin)))
						{
							try
							{
								var plugin = (BasePlugin)Activator.CreateInstance(type);

								if (plugin != null)
								{
									_AvailablePlugins.Add(plugin);

									appendInfo.AppendLine();
									appendInfo.AppendLine($"{plugin.Info.Name} {plugin.Info.Version} by {plugin.Info.AuthorName} ({plugin.Info.AuthorContact})");
									appendInfo.AppendLine();
								}
							}
							catch (Exception ex)
							{
								MessageBox.Show($"Error loading '{file}' ({type.FullName}):{Environment.NewLine}{ex.Message}");
							}
						}

						if (type.IsSubclassOf(typeof(BaseElement)))
						{
							_RegisteredTypes.Add(type);
						}
					}
				}
				catch
				{ }
			}

			if (PluginsInfo == null)
			{
				return;
			}

			foreach (var plugin in _AvailablePlugins)
			{
				if (PluginsInfo.Contains(plugin.Info))
				{
					plugin.Load(this);

					_LoadedPlugins.Add(plugin);
				}
			}
		}

		private void GetContextMenu(ref BaseElement element, ContextMenu menu)
		{
			var groupMenu = new MenuItem("Grouping");
			var positionMenu = new MenuItem("Positioning");
			var orderMenu = new MenuItem("Order");
			var miscMenu = new MenuItem("Misc");
			var editMenu = new MenuItem("Edit");

			editMenu.MenuItems.Add(new MenuItem("Cut", MenuCut_Click));
			editMenu.MenuItems.Add(new MenuItem("Copy", MenuCopy_Click));
			editMenu.MenuItems.Add(new MenuItem("Paste", MenuPaste_Click));
			editMenu.MenuItems.Add(new MenuItem("Delete", MenuDelete_Click));

			menu.MenuItems.Add(editMenu);
			menu.MenuItems.Add(new MenuItem("-"));
			menu.MenuItems.Add(groupMenu);
			menu.MenuItems.Add(positionMenu);
			menu.MenuItems.Add(orderMenu);
			menu.MenuItems.Add(new MenuItem("-"));
			menu.MenuItems.Add(miscMenu);

			if (ElementStack.GetSelectedElements().Count() > 1)
			{
				groupMenu.MenuItems.Add(new MenuItem("Create Group", MenuGroupCreate_Click));
			}

			element?.AddContextMenus(ref groupMenu, ref positionMenu, ref orderMenu, ref miscMenu);

			if (groupMenu.MenuItems.Count == 0)
			{
				groupMenu.Enabled = false;
			}

			if (positionMenu.MenuItems.Count == 0)
			{
				positionMenu.Enabled = false;
			}

			if (orderMenu.MenuItems.Count == 0)
			{
				orderMenu.Enabled = false;
			}

			if (miscMenu.MenuItems.Count == 0)
			{
				miscMenu.Enabled = false;
			}
		}

		public BaseElement GetLastSelectedControl()
		{
			return ElementStack.Elements.LastOrDefault();
		}

		private PluginInfo[] GetPluginsToLoad()
		{
			var path = Path.Combine(Application.StartupPath, "LoadInfo.bin");

			if (File.Exists(path))
			{
				using (var fileStream = new FileStream(path, FileMode.Open))
				{
					return (PluginInfo[])new BinaryFormatter().Deserialize(fileStream);
				}
			}

			return Array.Empty<PluginInfo>();
		}

		private Rectangle GetPositiveRect(Rectangle rect)
		{
			if (rect.Height < 0)
			{
				rect.Height = Math.Abs(rect.Height);
				rect.Y -= rect.Height;
			}

			if (rect.Width < 0)
			{
				rect.Width = Math.Abs(rect.Width);
				rect.X -= rect.Width;
			}

			return rect;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_Components?.Dispose();
			}

			base.Dispose(disposing);
		}

		private void InitializeComponent()
		{
			_Components = new System.ComponentModel.Container();

			var resources = new System.ComponentModel.ComponentResourceManager(typeof(DesignerForm));

			_Panel7 = new System.Windows.Forms.Panel();
			_Panel4 = new System.Windows.Forms.Panel();
			_TabToolbox = new System.Windows.Forms.TabControl();
			_PageStandard = new System.Windows.Forms.TabPage();
			_PanelToolbox = new System.Windows.Forms.Panel();
			_PageCustom = new System.Windows.Forms.TabPage();
			_Gumplings = new System.Windows.Forms.TreeView();
			_Label = new System.Windows.Forms.Label();
			_StatusBar = new System.Windows.Forms.StatusBar();
			_Splitter1 = new System.Windows.Forms.Splitter();
			_Panel1 = new System.Windows.Forms.Panel();
			_Panel2 = new System.Windows.Forms.Panel();
			_PanelCanvasScroller = new System.Windows.Forms.Panel();
			ImageCanvas = new System.Windows.Forms.PictureBox();
			_TabPages = new System.Windows.Forms.TabControl();
			_TabPage = new System.Windows.Forms.TabPage();
			_Splitter2 = new System.Windows.Forms.Splitter();
			_Panel3 = new System.Windows.Forms.Panel();
			_ComboElements = new System.Windows.Forms.ComboBox();
			_ElementProperties = new System.Windows.Forms.PropertyGrid();
			CanvasFocus = new System.Windows.Forms.TextBox();
			_OpenDialog = new System.Windows.Forms.OpenFileDialog();
			_SaveDialog = new System.Windows.Forms.SaveFileDialog();
			_MenuContext = new System.Windows.Forms.ContextMenu();
			_Menu = new System.Windows.Forms.MainMenu(_Components);
			_MenuFile = new System.Windows.Forms.MenuItem();
			_MenuFileNew = new System.Windows.Forms.MenuItem();
			_MenuMain5 = new System.Windows.Forms.MenuItem();
			_MenuFileOpen = new System.Windows.Forms.MenuItem();
			_MenuFileSave = new System.Windows.Forms.MenuItem();
			MenuFileImport = new System.Windows.Forms.MenuItem();
			MenuFileExport = new System.Windows.Forms.MenuItem();
			_MenuMain4 = new System.Windows.Forms.MenuItem();
			_MenuFileExit = new System.Windows.Forms.MenuItem();
			_MenuEdit = new System.Windows.Forms.MenuItem();
			_MenuEditUndo = new System.Windows.Forms.MenuItem();
			_MenuEditRedo = new System.Windows.Forms.MenuItem();
			_MenuMain2 = new System.Windows.Forms.MenuItem();
			_MenuCut = new System.Windows.Forms.MenuItem();
			_MenuCopy = new System.Windows.Forms.MenuItem();
			_MenuPaste = new System.Windows.Forms.MenuItem();
			_MenuDelete = new System.Windows.Forms.MenuItem();
			_MenuMain3 = new System.Windows.Forms.MenuItem();
			_MenuSelectAll = new System.Windows.Forms.MenuItem();
			_MenuMisc = new System.Windows.Forms.MenuItem();
			_MenuMiscLoadGumpling = new System.Windows.Forms.MenuItem();
			_MenuImportGumpling = new System.Windows.Forms.MenuItem();
			_MenuDataFile = new System.Windows.Forms.MenuItem();
			_MenuPage = new System.Windows.Forms.MenuItem();
			_MenuPageAdd = new System.Windows.Forms.MenuItem();
			_MenuPageInsert = new System.Windows.Forms.MenuItem();
			_MenuPageDelete = new System.Windows.Forms.MenuItem();
			_MenuPageClear = new System.Windows.Forms.MenuItem();
			_MenuMain6 = new System.Windows.Forms.MenuItem();
			_MenuShowPage0 = new System.Windows.Forms.MenuItem();
			MenuPlugins = new System.Windows.Forms.MenuItem();
			_MenuPluginManager = new System.Windows.Forms.MenuItem();
			_MenuHelp = new System.Windows.Forms.MenuItem();
			_MenuHelpAbout = new System.Windows.Forms.MenuItem();
			_MenuGumplingContext = new System.Windows.Forms.ContextMenu();
			_MenuGumplingRename = new System.Windows.Forms.MenuItem();
			_MenuGumplingMove = new System.Windows.Forms.MenuItem();
			_MenuGumplingDelete = new System.Windows.Forms.MenuItem();
			_MenuMain1 = new System.Windows.Forms.MenuItem();
			_MenuGumplingAddGumpling = new System.Windows.Forms.MenuItem();
			_MenuGumplingAddFolder = new System.Windows.Forms.MenuItem();

			((System.ComponentModel.ISupportInitialize)(ImageCanvas)).BeginInit();

			_Panel7.SuspendLayout();
			_Panel4.SuspendLayout();
			_TabToolbox.SuspendLayout();
			_PageStandard.SuspendLayout();
			_PageCustom.SuspendLayout();
			_Panel1.SuspendLayout();
			_Panel2.SuspendLayout();
			_PanelCanvasScroller.SuspendLayout();
			_TabPages.SuspendLayout();
			_Panel3.SuspendLayout();

			SuspendLayout();
			// 
			// _pnlToolboxHolder
			// 
			_Panel7.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_Panel7.Controls.Add(_Panel4);
			_Panel7.Controls.Add(_Label);
			_Panel7.Dock = System.Windows.Forms.DockStyle.Left;
			_Panel7.Location = new System.Drawing.Point(0, 0);
			_Panel7.Name = "_pnlToolboxHolder";
			_Panel7.Size = new System.Drawing.Size(128, 685);
			_Panel7.TabIndex = 0;
			// 
			// _Panel4
			// 
			_Panel4.Controls.Add(_TabToolbox);
			_Panel4.Dock = System.Windows.Forms.DockStyle.Fill;
			_Panel4.Location = new System.Drawing.Point(0, 16);
			_Panel4.Name = "_Panel4";
			_Panel4.Size = new System.Drawing.Size(124, 665);
			_Panel4.TabIndex = 1;
			// 
			// _tabToolbox
			// 
			_TabToolbox.Controls.Add(_PageStandard);
			_TabToolbox.Controls.Add(_PageCustom);
			_TabToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			_TabToolbox.Location = new System.Drawing.Point(0, 0);
			_TabToolbox.Multiline = true;
			_TabToolbox.Name = "_tabToolbox";
			_TabToolbox.SelectedIndex = 0;
			_TabToolbox.Size = new System.Drawing.Size(124, 665);
			_TabToolbox.TabIndex = 1;
			// 
			// _tpgStandard
			// 
			_PageStandard.Controls.Add(_PanelToolbox);
			_PageStandard.Location = new System.Drawing.Point(4, 22);
			_PageStandard.Name = "_tpgStandard";
			_PageStandard.Size = new System.Drawing.Size(116, 639);
			_PageStandard.TabIndex = 0;
			_PageStandard.Text = "Standard";
			// 
			// _pnlToolbox
			// 
			_PanelToolbox.AutoScroll = true;
			_PanelToolbox.Dock = System.Windows.Forms.DockStyle.Fill;
			_PanelToolbox.Location = new System.Drawing.Point(0, 0);
			_PanelToolbox.Name = "_pnlToolbox";
			_PanelToolbox.Size = new System.Drawing.Size(116, 639);
			_PanelToolbox.TabIndex = 1;
			// 
			// _tpgCustom
			// 
			_PageCustom.Controls.Add(_Gumplings);
			_PageCustom.Location = new System.Drawing.Point(4, 22);
			_PageCustom.Name = "_tpgCustom";
			_PageCustom.Size = new System.Drawing.Size(116, 639);
			_PageCustom.TabIndex = 1;
			_PageCustom.Text = "Gumplings";
			// 
			// _treGumplings
			// 
			_Gumplings.Dock = System.Windows.Forms.DockStyle.Fill;
			_Gumplings.Location = new System.Drawing.Point(0, 0);
			_Gumplings.Name = "_treGumplings";
			_Gumplings.Size = new System.Drawing.Size(116, 639);
			_Gumplings.TabIndex = 1;
			_Gumplings.DoubleClick += new System.EventHandler(Gumplings_DoubleClick);
			_Gumplings.MouseUp += new System.Windows.Forms.MouseEventHandler(Gumplings_MouseUp);
			// 
			// m_Label1
			// 
			_Label.BackColor = System.Drawing.SystemColors.ControlDark;
			_Label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_Label.Dock = System.Windows.Forms.DockStyle.Top;
			_Label.Location = new System.Drawing.Point(0, 0);
			_Label.Name = "m_Label1";
			_Label.Size = new System.Drawing.Size(124, 16);
			_Label.TabIndex = 0;
			_Label.Text = "Toolbox";
			_Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
			_Panel2.Controls.Add(_PanelCanvasScroller);
			_Panel2.Controls.Add(_TabPages);
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
			_PanelCanvasScroller.AutoScroll = true;
			_PanelCanvasScroller.AutoScrollMargin = new System.Drawing.Size(1, 1);
			_PanelCanvasScroller.AutoScrollMinSize = new System.Drawing.Size(1, 1);
			_PanelCanvasScroller.BackColor = System.Drawing.Color.Silver;
			_PanelCanvasScroller.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			_PanelCanvasScroller.Controls.Add(ImageCanvas);
			_PanelCanvasScroller.Dock = System.Windows.Forms.DockStyle.Fill;
			_PanelCanvasScroller.Location = new System.Drawing.Point(0, 24);
			_PanelCanvasScroller.Name = "_pnlCanvasScroller";
			_PanelCanvasScroller.Size = new System.Drawing.Size(949, 661);
			_PanelCanvasScroller.TabIndex = 2;
			_PanelCanvasScroller.MouseLeave += new System.EventHandler(CanvasScroller_MouseLeave);
			// 
			// _picCanvas
			// 
			ImageCanvas.BackColor = System.Drawing.Color.Black;
			ImageCanvas.Location = new System.Drawing.Point(0, 0);
			ImageCanvas.Name = "_picCanvas";
			ImageCanvas.Size = new System.Drawing.Size(1600, 1200);
			ImageCanvas.TabIndex = 0;
			ImageCanvas.TabStop = false;
			ImageCanvas.Paint += new System.Windows.Forms.PaintEventHandler(ImageCanvas_Paint);
			ImageCanvas.MouseDown += new System.Windows.Forms.MouseEventHandler(ImageCanvas_MouseDown);
			ImageCanvas.MouseMove += new System.Windows.Forms.MouseEventHandler(ImageCanvas_MouseMove);
			ImageCanvas.MouseUp += new System.Windows.Forms.MouseEventHandler(ImageCanvas_MouseUp);
			// 
			// _TabPager
			// 
			_TabPages.Controls.Add(_TabPage);
			_TabPages.Dock = System.Windows.Forms.DockStyle.Top;
			_TabPages.HotTrack = true;
			_TabPages.Location = new System.Drawing.Point(0, 0);
			_TabPages.Name = "_TabPager";
			_TabPages.SelectedIndex = 0;
			_TabPages.Size = new System.Drawing.Size(949, 24);
			_TabPages.TabIndex = 3;
			_TabPages.SelectedIndexChanged += new System.EventHandler(TabPages_SelectedIndexChanged);
			// 
			// _TabPage1
			// 
			_TabPage.Location = new System.Drawing.Point(4, 22);
			_TabPage.Name = "_TabPage1";
			_TabPage.Size = new System.Drawing.Size(941, 0);
			_TabPage.TabIndex = 0;
			_TabPage.Text = "0";
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
			_Panel3.Controls.Add(_ComboElements);
			_Panel3.Controls.Add(_ElementProperties);
			_Panel3.Controls.Add(CanvasFocus);
			_Panel3.Dock = System.Windows.Forms.DockStyle.Right;
			_Panel3.Location = new System.Drawing.Point(971, 0);
			_Panel3.Name = "_Panel3";
			_Panel3.Size = new System.Drawing.Size(248, 685);
			_Panel3.TabIndex = 0;
			// 
			// m_cboElements
			// 
			_ComboElements.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_ComboElements.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			_ComboElements.Location = new System.Drawing.Point(0, 8);
			_ComboElements.Name = "m_cboElements";
			_ComboElements.Size = new System.Drawing.Size(240, 21);
			_ComboElements.TabIndex = 1;
			_ComboElements.SelectedIndexChanged += new System.EventHandler(ComboElements_SelectedIndexChanged);
			_ComboElements.Click += new System.EventHandler(ComboElements_Click);
			// 
			// m_ElementProperties
			// 
			_ElementProperties.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
			_ElementProperties.Cursor = System.Windows.Forms.Cursors.HSplit;
			_ElementProperties.LineColor = System.Drawing.SystemColors.ScrollBar;
			_ElementProperties.Location = new System.Drawing.Point(0, 32);
			_ElementProperties.Name = "m_ElementProperties";
			_ElementProperties.Size = new System.Drawing.Size(240, 651);
			_ElementProperties.TabIndex = 0;
			_ElementProperties.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(ElementProperties_PropertyValueChanged);
			// 
			// m_CanvasFocus
			// 
			CanvasFocus.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
			CanvasFocus.Location = new System.Drawing.Point(16, 635);
			CanvasFocus.Name = "m_CanvasFocus";
			CanvasFocus.Size = new System.Drawing.Size(100, 20);
			CanvasFocus.TabIndex = 1;
			CanvasFocus.Text = "TextBox1";
			// 
			// m_MainMenu
			// 
			_Menu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuFile,
			_MenuEdit,
			_MenuMisc,
			_MenuPage,
			MenuPlugins,
			_MenuHelp});
			// 
			// _mnuFile
			// 
			_MenuFile.Index = 0;
			_MenuFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuFileNew,
			_MenuMain5,
			_MenuFileOpen,
			_MenuFileSave,
			MenuFileImport,
			MenuFileExport,
			_MenuMain4,
			_MenuFileExit});
			_MenuFile.Text = "File";
			// 
			// _mnuFileNew
			// 
			_MenuFileNew.Index = 0;
			_MenuFileNew.Text = "New";
			_MenuFileNew.Click += new System.EventHandler(MenuFileNew_Click);
			// 
			// m_MenuItem9
			// 
			_MenuMain5.Index = 1;
			_MenuMain5.Text = "-";
			// 
			// _mnuFileOpen
			// 
			_MenuFileOpen.Index = 2;
			_MenuFileOpen.Text = "Open";
			_MenuFileOpen.Click += new System.EventHandler(MenuFileOpen_Click);
			// 
			// _mnuFileSave
			// 
			_MenuFileSave.Index = 3;
			_MenuFileSave.Text = "Save";
			_MenuFileSave.Click += new System.EventHandler(MenuFileSave_Click);
			// 
			// _mnuFileImport
			// 
			MenuFileImport.Enabled = false;
			MenuFileImport.Index = 4;
			MenuFileImport.Text = "Import";
			// 
			// _mnuFileExport
			// 
			MenuFileExport.Enabled = false;
			MenuFileExport.Index = 5;
			MenuFileExport.Text = "Export";
			// 
			// m_MenuItem5
			// 
			_MenuMain4.Index = 6;
			_MenuMain4.Text = "-";
			// 
			// _mnuFileExit
			// 
			_MenuFileExit.Index = 7;
			_MenuFileExit.Text = "Exit";
			_MenuFileExit.Click += new System.EventHandler(MenuFileExit_Click);
			// 
			// _mnuEdit
			// 
			_MenuEdit.Index = 1;
			_MenuEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuEditUndo,
			_MenuEditRedo,
			_MenuMain2,
			_MenuCut,
			_MenuCopy,
			_MenuPaste,
			_MenuDelete,
			_MenuMain3,
			_MenuSelectAll});
			_MenuEdit.Text = "Edit";
			// 
			// _mnuEditUndo
			// 
			_MenuEditUndo.Enabled = false;
			_MenuEditUndo.Index = 0;
			_MenuEditUndo.Shortcut = System.Windows.Forms.Shortcut.CtrlZ;
			_MenuEditUndo.Text = "Undo";
			_MenuEditUndo.Click += new System.EventHandler(MenuEditUndo_Click);
			// 
			// _mnuEditRedo
			// 
			_MenuEditRedo.Enabled = false;
			_MenuEditRedo.Index = 1;
			_MenuEditRedo.Shortcut = System.Windows.Forms.Shortcut.CtrlY;
			_MenuEditRedo.Text = "Redo";
			_MenuEditRedo.Click += new System.EventHandler(MenuEditRedo_Click);
			// 
			// m_MenuItem3
			// 
			_MenuMain2.Index = 2;
			_MenuMain2.Text = "-";
			// 
			// _mnuCut
			// 
			_MenuCut.Index = 3;
			_MenuCut.Shortcut = System.Windows.Forms.Shortcut.CtrlX;
			_MenuCut.Text = "Cu&t";
			_MenuCut.Click += new System.EventHandler(MenuCut_Click);
			// 
			// m_mnuCopy
			// 
			_MenuCopy.Index = 4;
			_MenuCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
			_MenuCopy.Text = "&Copy";
			_MenuCopy.Click += new System.EventHandler(MenuCopy_Click);
			// 
			// _mnuPaste
			// 
			_MenuPaste.Index = 5;
			_MenuPaste.Shortcut = System.Windows.Forms.Shortcut.CtrlV;
			_MenuPaste.Text = "&Paste";
			_MenuPaste.Click += new System.EventHandler(MenuPaste_Click);
			// 
			// _mnuDelete
			// 
			_MenuDelete.Index = 6;
			_MenuDelete.Text = "Delete";
			_MenuDelete.Click += new System.EventHandler(MenuDelete_Click);
			// 
			// m_MenuItem4
			// 
			_MenuMain3.Index = 7;
			_MenuMain3.Text = "-";
			// 
			// _mnuSelectAll
			// 
			_MenuSelectAll.Index = 8;
			_MenuSelectAll.Shortcut = System.Windows.Forms.Shortcut.CtrlA;
			_MenuSelectAll.Text = "Select &All";
			_MenuSelectAll.Click += new System.EventHandler(MenuSelectAll_Click);
			// 
			// _mnuMisc
			// 
			_MenuMisc.Index = 2;
			_MenuMisc.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuMiscLoadGumpling,
			_MenuImportGumpling,
			_MenuDataFile});
			_MenuMisc.Text = "Misc";
			// 
			// _mnuMiscLoadGumpling
			// 
			_MenuMiscLoadGumpling.Index = 0;
			_MenuMiscLoadGumpling.Text = "Load gumpling";
			_MenuMiscLoadGumpling.Click += new System.EventHandler(MenuMiscLoadGumpling_Click);
			// 
			// _mnuImportGumpling
			// 
			_MenuImportGumpling.Index = 1;
			_MenuImportGumpling.Text = "Import Gumpling";
			_MenuImportGumpling.Click += new System.EventHandler(MenuImportGumpling_Click);
			// 
			// _mnuDataFile
			// 
			_MenuDataFile.Index = 2;
			_MenuDataFile.Text = "Data File Path";
			_MenuDataFile.Click += new System.EventHandler(MenuDataFile_Click);
			// 
			// _mnuPage
			// 
			_MenuPage.Index = 3;
			_MenuPage.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuPageAdd,
			_MenuPageInsert,
			_MenuPageDelete,
			_MenuPageClear,
			_MenuMain6,
			_MenuShowPage0});
			_MenuPage.Text = "Page";
			// 
			// _mnuPageAdd
			// 
			_MenuPageAdd.Index = 0;
			_MenuPageAdd.Text = "Add Page";
			_MenuPageAdd.Click += new System.EventHandler(MenuAddPage_Click);
			// 
			// _mnuPageInsert
			// 
			_MenuPageInsert.Index = 1;
			_MenuPageInsert.Text = "Insert Page";
			_MenuPageInsert.Click += new System.EventHandler(MenuPageInsert_Click);
			// 
			// _mnuPageDelete
			// 
			_MenuPageDelete.Index = 2;
			_MenuPageDelete.Text = "Delete Page";
			_MenuPageDelete.Click += new System.EventHandler(MenuPageDelete_Click);
			// 
			// _mnuPageClear
			// 
			_MenuPageClear.Index = 3;
			_MenuPageClear.Text = "Clear Page";
			_MenuPageClear.Click += new System.EventHandler(MenuPageClear_Click);
			// 
			// m_MenuItem10
			// 
			_MenuMain6.Index = 4;
			_MenuMain6.Text = "-";
			// 
			// _mnuShow0
			// 
			_MenuShowPage0.Checked = true;
			_MenuShowPage0.Index = 5;
			_MenuShowPage0.Text = "Always Show Page 0";
			_MenuShowPage0.Click += new System.EventHandler(MenuShowPage0_Click);
			// 
			// _mnuPlugins
			// 
			MenuPlugins.Index = 4;
			MenuPlugins.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuPluginManager});
			MenuPlugins.Text = "Plug-Ins";
			// 
			// _mnuPluginManager
			// 
			_MenuPluginManager.Index = 0;
			_MenuPluginManager.Text = "Manager";
			_MenuPluginManager.Click += new System.EventHandler(MenuPluginManager_Click);
			// 
			// _mnuHelp
			// 
			_MenuHelp.Index = 5;
			_MenuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
			_MenuHelpAbout});
			_MenuHelp.Text = "Help";
			// 
			// _mnuHelpAbout
			// 
			_MenuHelpAbout.Index = 0;
			_MenuHelpAbout.Text = "About...";
			_MenuHelpAbout.Click += new System.EventHandler(MenuHelpAbout_Click);
			// 
			// _mnuGumplingContext
			// 
			_MenuGumplingContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[]
			{
				_MenuGumplingRename,
				_MenuGumplingMove,
				_MenuGumplingDelete,
				_MenuMain1,
				_MenuGumplingAddGumpling,
				_MenuGumplingAddFolder
			});
			// 
			// _mnuGumplingRename
			// 
			_MenuGumplingRename.Index = 0;
			_MenuGumplingRename.Text = "Rename";
			// 
			// _mnuGumplingMove
			// 
			_MenuGumplingMove.Index = 1;
			_MenuGumplingMove.Text = "Move";
			// 
			// _mnuGumplingDelete
			// 
			_MenuGumplingDelete.Index = 2;
			_MenuGumplingDelete.Text = "Delete";
			// 
			// m_MenuItem1
			// 
			_MenuMain1.Index = 3;
			_MenuMain1.Text = "-";
			// 
			// _mnuGumplingAddGumpling
			// 
			_MenuGumplingAddGumpling.Index = 4;
			_MenuGumplingAddGumpling.Text = "Add Gumpling";
			// 
			// _mnuGumplingAddFolder
			// 
			_MenuGumplingAddFolder.Index = 5;
			_MenuGumplingAddFolder.Text = "Add Folder";
			// 
			// DesignerForm
			// 
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			ClientSize = new System.Drawing.Size(1350, 708);

			Controls.Add(_Panel1);
			Controls.Add(_Splitter1);
			Controls.Add(_Panel7);
			Controls.Add(_StatusBar);

			Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			KeyPreview = true;
			Menu = _Menu;
			Name = "DesignerForm";
			Text = "Gump Studio (-Unsaved Gump-)";

			Load += new System.EventHandler(DesignerForm_Load);
			KeyDown += new System.Windows.Forms.KeyEventHandler(DesignerForm_KeyDown);
			KeyUp += new KeyEventHandler(DesignerForm_KeyUp);
			FormClosing += new FormClosingEventHandler(DesignerForm_FormClosing);

			((ISupportInitialize)ImageCanvas).EndInit();

			_Panel7.ResumeLayout(false);
			_Panel4.ResumeLayout(false);
			_TabToolbox.ResumeLayout(false);
			_PageStandard.ResumeLayout(false);
			_PageCustom.ResumeLayout(false);
			_Panel1.ResumeLayout(false);
			_Panel2.ResumeLayout(false);
			_PanelCanvasScroller.ResumeLayout(false);
			_TabPages.ResumeLayout(false);
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

				if (assemblyName.Contains("GumpStudioCore"))
				{
					assemblyName = asm.FullName;
				}

				return Type.GetType($"{typeName}, {assemblyName}");
			}
		}

		public void LoadFrom(string Path)
		{
			_StatusBar.Text = "Loading gump...";

			Stacks.Clear();

			_TabPages.TabPages.Clear();

			try
			{
				using (var fileStream = new FileStream(Path, FileMode.Open))
				{
					var binaryFormatter = new BinaryFormatter { Binder = DeserializationBinder.Instance };

					var list = (ICollection)binaryFormatter.Deserialize(fileStream);

					Stacks.Clear();
					Stacks.AddRange(list.Cast<GroupElement>());

					try
					{
						GumpProperties = (GumpProperties)binaryFormatter.Deserialize(fileStream);
					}
					catch (Exception ex)
					{
						GumpProperties = new GumpProperties();

						MessageBox.Show(ex.InnerException.Message);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			SetActiveElement(null, true);
			RefreshElementList();

			var page = -1;

			foreach (var stack in Stacks)
			{
				_TabPages.TabPages.Add(new TabPage((++page).ToString()));
			}

			NormalizeNames();

			foreach (var o in AllElements)
			{
				o.Selected = false;
			}

			ChangeActiveStack(0);

			ElementStack.UpdateParent += ChangeActiveElementEventHandler;
			ElementStack.Repaint += RefreshView;

			_StatusBar.Text = String.Empty;
		}

		private void MenuMiscLoadGumpling_Click(object sender, EventArgs e)
		{
			_OpenDialog.Filter = "Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";

			if (_OpenDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			try
			{
				using (var fileStream = new FileStream(_OpenDialog.FileName, FileMode.Open))
				{
					var bin = new BinaryFormatter
					{
						Binder = DeserializationBinder.Instance
					};

					var groupElement = (GroupElement)bin.Deserialize(fileStream);

					groupElement._IsBaseWindow = false;
					groupElement.Selected = false;

					groupElement.RecalculateBounds();

					groupElement.Location = Point.Empty;

					AddElement(groupElement);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			NormalizeNames();
		}

		private void MenuAddPage_Click(object sender, EventArgs e)
		{
			AddPage();
			CreateUndoPoint("Add page");
		}

		private void MenuCopy_Click(object sender, EventArgs e)
		{
			Copy();
		}

		private void MenuCut_Click(object sender, EventArgs e)
		{
			Cut();
			CreateUndoPoint();
		}

		private void MenuDataFile_Click(object sender, EventArgs e)
		{
			var folderBrowserDialog = new FolderBrowserDialog
			{
				SelectedPath = Environment.SpecialFolder.ProgramFiles.ToString(),
				Description = @"Select the folder that contains a copy of Ultima Online."
			};

			do
			{
				if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
				{
					return;
				}
			}
			while (!File.Exists(Path.Combine(folderBrowserDialog.SelectedPath, "client.exe")));

			XMLSettings.CurrentOptions.ClientPath = folderBrowserDialog.SelectedPath;
			XMLSettings.Save(this, XMLSettings.CurrentOptions);

			MessageBox.Show("Changes will be applied after restarting Gump Studio.", "Data Files");
		}

		private void MenuDelete_Click(object sender, EventArgs e)
		{
			DeleteSelectedElements();
		}

		private void MenuEditRedo_Click(object sender, EventArgs e)
		{
			Redo();
		}

		private void MenuEditUndo_Click(object sender, EventArgs e)
		{
			Undo();
		}

		private void MenuFileExit_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void MenuFileNew_Click(object sender, EventArgs e)
		{
			var result = MessageBox.Show("Are you sure you want to start a new gump?", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

			if (result == DialogResult.OK)
			{
				ClearGump();
			}
		}

		private void MenuFileOpen_Click(object sender, EventArgs e)
		{
			_OpenDialog.CheckFileExists = true;
			_OpenDialog.Filter = @"Gump|*.gump";

			if (_OpenDialog.ShowDialog() == DialogResult.OK)
			{
				LoadFrom(_OpenDialog.FileName);

				_FileName = Path.GetFileName(_OpenDialog.FileName);
				Text = $"Gump Studio ({_FileName})";
			}

			ImageCanvas.Invalidate();
		}

		private void MenuFileSave_Click(object sender, EventArgs e)
		{
			_SaveDialog.AddExtension = true;
			_SaveDialog.DefaultExt = "gump";
			_SaveDialog.Filter = "Gump|*.gump";

			if (_SaveDialog.ShowDialog() == DialogResult.OK)
			{
				SaveTo(_SaveDialog.FileName);

				_FileName = Path.GetFileName(_SaveDialog.FileName);
				Text = $"Gump Studio ({_FileName})";
			}
		}

		private void MenuGroupCreate_Click(object sender, EventArgs e)
		{
			var elements = new List<BaseElement>(ElementStack.GetSelectedElements());

			if (elements.Count >= 2)
			{
				var groupElement = new GroupElement(ElementStack, null, "New Group");

				foreach (var element in elements)
				{
					groupElement.AddElement(element);

					ElementStack.RemoveElement(element);
					ElementStack.RemoveEvents(element);
				}

				AddElement(groupElement);

				ImageCanvas.Invalidate();
			}

			CreateUndoPoint();
		}

		private void MenuHelpAbout_Click(object sender, EventArgs e)
		{
			var aboutBox = new AboutBox();

			aboutBox.SetText(_AboutElementAppend);
			aboutBox.ShowDialog();
		}

		private void MenuImportGumpling_Click(object sender, EventArgs e)
		{
			_OpenDialog.Filter = @"Gumpling (*.gumpling)|*.gumpling|Gump (*.gump)|*.gump";

			if (_OpenDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			using (var fileStream = new FileStream(_OpenDialog.FileName, FileMode.Open))
			{
				var bin = new BinaryFormatter
				{
					Binder = DeserializationBinder.Instance
				};

				var gumpling = (GroupElement)bin.Deserialize(fileStream);

				gumpling._IsBaseWindow = false;
				gumpling.Selected = false;

				gumpling.RecalculateBounds();

				gumpling.Location = Point.Empty;

				UncategorizedFolder.AddItem(new TreeGumpling(Path.GetFileName(_OpenDialog.FileName), gumpling));
			}

			BuildGumplingTree();
		}

		private void MenuPageClear_Click(object sender, EventArgs e)
		{
			ElementStack = new GroupElement(null, null, "Element Stack", true);

			CreateUndoPoint("Clear Page");
		}

		private void MenuPageDelete_Click(object sender, EventArgs e)
		{
			if (_TabPages.SelectedIndex == 0)
			{
				MessageBox.Show("First page cannot be deleted.");
			}
			else
			{
				var selected = _TabPages.SelectedIndex;

				Stacks.RemoveAt(selected);

				_TabPages.TabPages.RemoveAt(selected);

				var index = -1;

				foreach (TabPage page in _TabPages.TabPages)
				{
					page.Text = (++index).ToString();
				}

				ChangeActiveStack(Math.Min(selected, _TabPages.TabCount - 1));

				CreateUndoPoint("Delete Page");
			}
		}

		private void MenuPageInsert_Click(object sender, EventArgs e)
		{
			if (_TabPages.SelectedIndex == 0)
			{
				MessageBox.Show("First page cannot be moved.");
			}
			else
			{
				Stacks.Insert(_TabPages.SelectedIndex, new GroupElement(null, null, "Element Stack", true));

				_TabPages.TabPages.Insert(_TabPages.SelectedIndex, new TabPage());

				var index = -1;

				foreach (TabPage page in _TabPages.TabPages)
				{
					page.Text = (++index).ToString();
				}

				ChangeActiveStack(_TabPages.SelectedIndex);

				CreateUndoPoint("Insert Page");
			}
		}

		private void MenuPaste_Click(object sender, EventArgs e)
		{
			Paste();
			CreateUndoPoint();
		}

		private void MenuPluginManager_Click(object sender, EventArgs e)
		{
			new PluginManager()
			{
				AvailablePlugins = _AvailablePlugins,
				LoadedPlugins = _LoadedPlugins,
				OrderList = PluginsInfo,
				MainForm = this
			}.ShowDialog();
		}

		private void MenuSelectAll_Click(object sender, EventArgs e)
		{
			SelectAll();
		}

		private void MenuShowPage0_Click(object sender, EventArgs e)
		{
			_MenuShowPage0.Checked = _ShowPage0 = !_ShowPage0;

			ImageCanvas.Refresh();
		}

		public void Paste()
		{
			try
			{
				var obj = Clipboard.GetDataObject();
				var data = (ICollection)(obj.GetData(typeof(ArrayList)) ?? obj.GetData(typeof(BaseElement[])));

				if (data != null)
				{
					SetActiveElement(null, true);

					foreach (var element in data.OfType<BaseElement>())
					{
						element.Selected = true;

						ElementStack.AddElement(element);
					}

					NormalizeNames();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			ImageCanvas.Invalidate();
		}

		private void ImageCanvas_MouseDown(object sender, MouseEventArgs e)
		{
			CanvasFocus.Focus();

			var point = _Anchor = new Point(e.X, e.Y);

			var element = ElementStack.GetElementFromPoint(point);

			if (ActiveElement != null && ActiveElement.HitTest(point) != MoveModeType.None)
			{
				element = ActiveElement;
			}

			if (element != null)
			{
				_MoveMode = element.HitTest(point);

				if (ActiveElement != null && ActiveElement.HitTest(point) == MoveModeType.None)
				{
					if (element.Selected)
					{
						if ((ModifierKeys & Keys.Control) > Keys.None)
						{
							element.Selected = false;
						}
						else
						{
							SetActiveElement(element, false);
						}
					}
					else
					{
						SetActiveElement(element, !ModifierKeys.HasFlag(Keys.Control));
					}
				}
				else if (ActiveElement == null)
				{
					SetActiveElement(element, false);
				}
				else if (ActiveElement != null && ModifierKeys.HasFlag(Keys.Control))
				{
					ActiveElement.Selected = false;

					var first = ElementStack.GetSelectedElements().FirstOrDefault();

					if (first != null)
					{
						SetActiveElement(first, false);
					}
					else
					{
						SetActiveElement(null, true);

						_MoveMode = MoveModeType.None;
					}
				}
			}
			else
			{
				_MoveMode = MoveModeType.None;

				if (e.Button.HasFlag(MouseButtons.Left))
				{
					SetActiveElement(null, !ModifierKeys.HasFlag(Keys.Control));
				}
			}

			ImageCanvas.Invalidate();

			_LastPosition = point;

			if (ActiveElement != null)
			{
				_AnchorOffset.Width = ActiveElement.X - point.X;
				_AnchorOffset.Height = ActiveElement.Y - point.Y;
			}

			_ElementChanged = false;
			_MoveCount = 0;
		}

		private void ImageCanvas_MouseMove(object sender, MouseEventArgs e)
		{
			var point = new Point(e.X, e.Y);

			var element = ElementStack.GetElementFromPoint(point);

			if (ActiveElement != null && ActiveElement.HitTest(point) != MoveModeType.None)
			{
				element = ActiveElement;
			}

			if (_MoveMode == MoveModeType.Move)
			{
				point.Offset(_AnchorOffset.Width, _AnchorOffset.Height);
			}

			var args = new MouseMoveHookEventArgs
			{
				Keys = ModifierKeys,
				MouseButtons = e.Button,
				MouseLocation = point,
				MoveMode = _MoveMode
			};

			foreach (var plugin in _LoadedPlugins)
			{
				plugin.MouseMoveHook(ref args);
			}

			point = args.MouseLocation;

			if (_MoveMode == MoveModeType.None && Math.Abs(point.X - _LastPosition.X) > 0 && Math.Abs(point.Y - _LastPosition.Y) > 0)
			{
				_MoveMode = MoveModeType.SelectionBox;
			}

			if (e.Button != MouseButtons.Left)
			{
				if (element != null)
				{
					switch (element.HitTest(point))
					{
						case MoveModeType.ResizeTopLeft: Cursor = Cursors.SizeNWSE; break;
						case MoveModeType.ResizeBottomRight: Cursor = Cursors.SizeNWSE; break;
						case MoveModeType.ResizeTopRight: Cursor = Cursors.SizeNESW; break;
						case MoveModeType.ResizeBottomLeft: Cursor = Cursors.SizeNESW; break;
						case MoveModeType.ResizeLeft: Cursor = Cursors.SizeWE; break;
						case MoveModeType.ResizeRight: Cursor = Cursors.SizeWE; break;
						case MoveModeType.ResizeTop: Cursor = Cursors.SizeNS; break;
						case MoveModeType.ResizeBottom: Cursor = Cursors.SizeNS; break;
						case MoveModeType.Move: Cursor = Cursors.SizeAll; break;
						default: Cursor = Cursors.Default; break;
					}
				}
				else
				{
					Cursor = Cursors.Default;
				}
			}
			else
			{
				++_MoveCount;

				if (_MoveCount > 100)
				{
					_MoveCount = 2;
				}

				var rectangle = new Rectangle(0, 0, ImageCanvas.Width, ImageCanvas.Height);

				Cursor.Clip = ImageCanvas.RectangleToScreen(rectangle);

				if (_MoveMode != MoveModeType.None)
				{
					switch (_MoveMode)
					{
						case MoveModeType.ResizeTopLeft: Cursor = Cursors.SizeNWSE; break;
						case MoveModeType.ResizeBottomRight: Cursor = Cursors.SizeNWSE; break;
						case MoveModeType.ResizeTopRight: Cursor = Cursors.SizeNESW; break;
						case MoveModeType.ResizeBottomLeft: Cursor = Cursors.SizeNESW; break;
						case MoveModeType.ResizeLeft: Cursor = Cursors.SizeWE; break;
						case MoveModeType.ResizeRight: Cursor = Cursors.SizeWE; break;
						case MoveModeType.ResizeTop: Cursor = Cursors.SizeNS; break;
						case MoveModeType.ResizeBottom: Cursor = Cursors.SizeNS; break;
						case MoveModeType.Move: Cursor = Cursors.SizeAll; break;
						default: Cursor = Cursors.Default; break;
					}

					if (_MoveCount > 1)
					{
						_ElementChanged = true;
					}
				}

				switch (_MoveMode)
				{
					case MoveModeType.SelectionBox:
					{
						rectangle = new Rectangle(_Anchor, new Size(point.X - _Anchor.X, point.Y - _Anchor.Y));

						_SelectionRect = GetPositiveRect(rectangle);

						_ShowSelectionRect = true;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeTopLeft:
					{
						point.Offset(3, 0);

						var loc = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);

						ActiveElement.Location = point;

						var p = ActiveElement.Location;
						var s = ActiveElement.Size;

						s.Width = loc.X - point.X;
						s.Height = loc.Y - point.Y;

						if (s.Width < 1)
						{
							p.X = loc.X - 1;
							s.Width = 1;
						}

						if (s.Height < 1)
						{
							p.Y = loc.Y - 1;
							s.Height = 1;
						}

						ActiveElement.Size = s;
						ActiveElement.Location = p;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeTopRight:
					{
						point.Offset(-3, 0);

						var loc = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);

						var p = ActiveElement.Location;

						p.Y = point.Y;

						ActiveElement.Location = p;

						var s = ActiveElement.Size;

						s.Height = loc.Y - point.Y;
						s.Width = Math.Max(1, point.X - ActiveElement.X);

						if (s.Height < 1)
						{
							p.Y = loc.Y - 1;
							s.Height = 1;
						}

						p.X = ActiveElement.Location.X;

						ActiveElement.Size = s;
						ActiveElement.Location = p;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeBottomRight:
					{
						if (ActiveElement == null)
						{
							break;
						}

						point.Offset(-3, -3);

						var s = ActiveElement.Size;

						s.Width = Math.Max(1, point.X - ActiveElement.X);
						s.Height = Math.Max(1, point.Y - ActiveElement.Y);

						ActiveElement.Size = s;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeBottomLeft:
					{
						if (ActiveElement == null)
						{
							break;
						}

						point.Offset(0, -3);

						var loc = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);

						var p = ActiveElement.Location;

						p.X = point.X;

						ActiveElement.Location = p;

						var s = ActiveElement.Size;

						s.Width = loc.X - point.X;
						s.Height = Math.Max(1, point.Y - ActiveElement.Y);

						if (s.Width < 1)
						{
							p.X = loc.X - 1;
							s.Width = 1;
						}

						p.Y = ActiveElement.Y;

						ActiveElement.Size = s;
						ActiveElement.Location = p;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.Move:
					{
						if (ActiveElement == null)
						{
							break;
						}

						var p = ActiveElement.Location;

						ActiveElement.Location = point;

						var dx = ActiveElement.X - p.X;
						var dy = ActiveElement.Y - p.Y;

						foreach (var o in ElementStack.GetSelectedElements())
						{
							if (o != ActiveElement)
							{
								var loc = o.Location;

								loc.Offset(dx, dy);

								o.Location = loc;
							}
						}

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeLeft:
					{
						point.Offset(3, 0);

						var loc = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);

						var y = ActiveElement.Y;

						ActiveElement.Location = point;

						var p = ActiveElement.Location;
						var s = ActiveElement.Size;

						s.Width = loc.X - point.X;

						if (s.Width < 1)
						{
							p.X = loc.X - 1;
							s.Width = 1;
						}

						p.Y = y;

						ActiveElement.Size = s;
						ActiveElement.Location = p;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeTop:
					{
						point.Offset(0, 3);

						var loc = new Point(ActiveElement.X + ActiveElement.Width, ActiveElement.Y + ActiveElement.Height);

						var x = ActiveElement.X;

						ActiveElement.Location = point;

						var p = ActiveElement.Location;
						var s = ActiveElement.Size;

						s.Height = loc.Y - point.Y;

						if (s.Height < 1)
						{
							p.Y = loc.Y - 1;
							s.Height = 1;
						}

						p.X = x;

						ActiveElement.Size = s;
						ActiveElement.Location = p;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeRight:
					{
						point.Offset(-3, 0);

						var s = ActiveElement.Size;

						s.Width = Math.Max(1, point.X - ActiveElement.X);

						ActiveElement.Size = s;

						ImageCanvas.Invalidate();
					}
					break;

					case MoveModeType.ResizeBottom:
					{
						point.Offset(0, -3);

						var s = ActiveElement.Size;

						s.Height = Math.Max(1, point.Y - ActiveElement.Y);

						ActiveElement.Size = s;

						ImageCanvas.Invalidate();
					}
					break;
				}
			}

			_LastPosition = point;
		}

		private void ImageCanvas_MouseUp(object sender, MouseEventArgs e)
		{
			var rectangle = Rectangle.Empty;

			var point = new Point(e.X, e.Y);

			ElementStack.GetElementFromPoint(point);

			_ShowSelectionRect = false;

			Cursor.Clip = rectangle;

			if (_MoveMode == MoveModeType.SelectionBox)
			{
				BaseElement selected = null;

				foreach (var element in ElementStack.Elements)
				{
					if (element.ContainsTest(_SelectionRect))
					{
						element.Selected = true;
						selected = element;
					}
					else if (!ModifierKeys.HasFlag(Keys.Control))
					{
						element.Selected = false;
					}
				}

				SetActiveElement(selected, false);
			}

			if (_MoveMode != MoveModeType.None && _MoveMode != MoveModeType.SelectionBox && _ElementChanged)
			{
				CreateUndoPoint("Element Moved");
				_ElementChanged = false;
			}

			if (e.Button.HasFlag(MouseButtons.Right))
			{
				var menu = _MenuContext;
				var active = ActiveElement;

				GetContextMenu(ref active, menu);

				ActiveElement = active;

				menu.Show(ImageCanvas, point);

				ClearContextMenu(menu);
			}

			SetActiveElement(ActiveElement, false);

			ImageCanvas.Invalidate();

			_MoveMode = MoveModeType.None;
			_AnchorOffset = Size.Empty;
		}

		private void ImageCanvas_Paint(object sender, PaintEventArgs e)
		{
			Render(e.Graphics);
		}

		private void CanvasScroller_MouseLeave(object sender, EventArgs e)
		{
			Cursor = Cursors.Default;
		}

		public void RebuildTabPages()
		{
			_TabPages.TabPages.Clear();

			var num = -1;

			foreach (var stack in Stacks)
			{
				_TabPages.TabPages.Add(new TabPage((++num).ToString()));

				if (ElementStack == stack)
				{
					_TabPages.SelectedIndex = num;
				}
			}
		}

		public void Redo()
		{
			if (_CurrentUndoPoint < _UndoPoints.Count)
			{
				RevertToUndoPoint(++_CurrentUndoPoint);
			}

			if (_CurrentUndoPoint == _UndoPoints.Count - 1)
			{
				_MenuEditRedo.Enabled = false;
			}

			_MenuEditUndo.Enabled = true;
		}

		public void RefreshElementList()
		{
			_ComboElements.Items.Clear();
			_ComboElements.Items.AddRange(ElementStack.Elements.ToArray());
		}

		public void RefreshView(object sender)
		{
			RefreshElementList();

			_ComboElements.SelectedItem = ActiveElement;

			var selected = ElementStack.GetSelectedElements().ToArray();

			if (selected.Length > 1)
			{
				_ElementProperties.SelectedObjects = selected;
			}
			else
			{
				_ElementProperties.SelectedObject = ActiveElement;
			}
		}

		private void Render(Graphics target)
		{
			var g = Graphics.FromImage(_Canvas);

			if (!PluginClearsCanvas)
			{
				g.Clear(Color.Black);
			}

			var hookPreRender = HookPreRender;

			hookPreRender?.Invoke(_Canvas);

			if (Stacks.Count > 0 && _ShowPage0 && Stacks[0] is BaseElement e && e != ElementStack)
			{
				e.Render(g);
			}

			ElementStack.Render(g);

			foreach (var element in ElementStack.Elements)
			{
				if (element.Selected && element != ActiveElement)
				{
					element.DrawBoundingBox(g, false);
				}
			}

			ActiveElement?.DrawBoundingBox(g, true);

			if (_ShowSelectionRect)
			{
				g.FillRectangle(_SelectionBG, _SelectionRect);
				g.DrawRectangle(_SelectionFG, _SelectionRect);
			}

			HookPostRender?.Invoke(_Canvas);

			g.Dispose();

			target.DrawImage(_Canvas, 0, 0);
		}

		public void RevertToUndoPoint(int index)
		{
			var undoPoint = (UndoPoint)_UndoPoints[index];

			GumpProperties = (GumpProperties)undoPoint.GumpProperties.Clone();

			Stacks.Clear();

			foreach (GroupElement group in undoPoint.Stack)
			{
				var clone = (GroupElement)group.Clone();

				Stacks.Add(clone);

				if (undoPoint.ElementStack == group)
				{
					ElementStack = clone;
				}
			}

			RebuildTabPages();

			ImageCanvas.Invalidate();

			SetActiveElement(null, true);

			_CurrentUndoPoint = index;
		}

		public void SaveTo(string path)
		{
			_StatusBar.Text = $@"Saving gump...";

			ElementStack.UpdateParent -= ChangeActiveElementEventHandler;
			ElementStack.Repaint -= RefreshView;

			using (var fileStream = new FileStream(path, FileMode.Create))
			{
				var bin = new BinaryFormatter
				{
					Binder = DeserializationBinder.Instance
				};

				bin.Serialize(fileStream, Stacks);
				bin.Serialize(fileStream, GumpProperties);
			}

			ElementStack.UpdateParent += ChangeActiveElementEventHandler;
			ElementStack.Repaint += RefreshView;

			_StatusBar.Text = String.Empty;
		}

		public void SelectAll()
		{
			foreach (var selectedElement in ElementStack.GetSelectedElements())
			{
				selectedElement.Selected = true;
			}

			ImageCanvas.Invalidate();
		}

		public void SetActiveElement(BaseElement e)
		{
			SetActiveElement(e, false);
		}

		public void SetActiveElement(BaseElement element, bool deselectOthers)
		{
			if (deselectOthers)
			{
				foreach (var e in ElementStack.Elements)
				{
					e.Selected = false;
				}
			}

			if (ActiveElement != element)
			{
				RefreshElementList();

				ActiveElement = element;

				_ComboElements.SelectedItem = element;

				if (element != null)
				{
					element.Selected = true;
				}
			}

			var selected = ElementStack.GetSelectedElements().ToArray();

			if (selected.Length > 1)
			{
				_ElementProperties.SelectedObjects = selected;
			}
			else if (element != null)
			{
				_ElementProperties.SelectedObject = element;
			}
			else
			{
				_ElementProperties.SelectedObject = GumpProperties;
			}
		}

		public Point SnapLocToGrid(Point position, Size gridSize)
		{
			var point = position;

			point.X = point.X / gridSize.Width * gridSize.Width;
			point.Y = point.Y / gridSize.Height * gridSize.Height;

			return point;
		}

		private void TabPages_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_TabPages.SelectedIndex >= 0)
			{
				ChangeActiveStack(_TabPages.SelectedIndex);
			}

			RefreshElementList();
		}

		private void Gumplings_DoubleClick(object sender, EventArgs e)
		{
			if (_Gumplings.SelectedNode.Tag == null || !(_Gumplings.SelectedNode.Tag is TreeGumpling tg))
			{
				return;
			}

			var group = (GroupElement)tg.Gumpling.Clone();

			group._IsBaseWindow = false;

			group.RecalculateBounds();

			group.Location = Point.Empty;

			AddElement(group);
		}

		private void Gumplings_MouseUp(object sender, MouseEventArgs e)
		{
			_Gumplings.SelectedNode = _Gumplings.GetNodeAt(new Point(e.X, e.Y));
		}

		public void Undo()
		{
			RevertToUndoPoint(--_CurrentUndoPoint);

			if (_CurrentUndoPoint <= 0)
			{
				_MenuEditUndo.Enabled = false;
			}

			_MenuEditRedo.Enabled = true;
		}

		public void WritePluginsToLoad()
		{
			var path = Path.Combine(Application.StartupPath, "LoadInfo.bin");

			if (PluginsInfo != null && PluginsInfo.Count > 0)
			{
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					var bin = new BinaryFormatter
					{
						Binder = DeserializationBinder.Instance
					};

					bin.Serialize(fileStream, PluginsInfo.ToArray());
				}
			}
			else if (File.Exists(path))
			{
				File.Delete(path);
			}
		}

		public delegate void HookKeyDownEventHandler(object sender, ref KeyEventArgs e);
		public delegate void HookPostRenderEventHandler(Bitmap Target);
		public delegate void HookPreRenderEventHandler(Bitmap Target);
	}
}
