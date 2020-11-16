// Decompiled with JetBrains decompiler
// Type: GumpStudio.PluginManager
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using GumpStudio.Forms;
using GumpStudio.Plugins;

namespace GumpStudio
{
	public class PluginManager : Form
	{
		private Button _cmdAdd;
		private Button _cmdCancel;
		private Button _cmdMoveDown;
		private Button _cmdMoveUp;
		private Button _cmdOK;
		private Button _cmdRemove;
		private GroupBox _GroupBox1;
		private Label _Label1;
		private Label _Label2;
		private Label _Label3;
		private Label _Label4;
		private Label _Label6;
		private ListBox _lstAvailable;
		private ListBox _lstLoaded;
		private TextBox _txtAuthor;
		private TextBox _txtDescription;
		private TextBox _txtEmail;
		private TextBox _txtVersion;
		public ArrayList AvailablePlugins;
		public ArrayList LoadedPlugins;
		public DesignerForm MainForm;
		public PluginInfo[] OrderList;

		public PluginManager()
		{
			Load += new EventHandler(PluginManager_Load);
			InitializeComponent();
		}

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			var pluginInfo = (PluginInfo)_lstAvailable.Items[_lstAvailable.SelectedIndex];
			_lstAvailable.Items.RemoveAt(_lstAvailable.SelectedIndex);
			_lstLoaded.Items.Add(pluginInfo);
		}

		private void cmdCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void cmdMoveDown_Click(object sender, EventArgs e)
		{
			var selectedIndex = _lstLoaded.SelectedIndex;
			if (selectedIndex >= _lstLoaded.Items.Count - 2)
			{
				return;
			}

			var objectValue = RuntimeHelpers.GetObjectValue(_lstLoaded.SelectedItem);
			_lstLoaded.Items.RemoveAt(selectedIndex);
			_lstLoaded.Items.Insert(selectedIndex + 1, RuntimeHelpers.GetObjectValue(objectValue));
			_lstLoaded.SelectedIndex = selectedIndex + 1;
		}

		private void cmdMoveUp_Click(object sender, EventArgs e)
		{
			var selectedIndex = _lstLoaded.SelectedIndex;
			if (selectedIndex <= 0)
			{
				return;
			}

			var objectValue = RuntimeHelpers.GetObjectValue(_lstLoaded.SelectedItem);
			_lstLoaded.Items.RemoveAt(selectedIndex);
			_lstLoaded.Items.Insert(selectedIndex - 1, RuntimeHelpers.GetObjectValue(objectValue));
			_lstLoaded.SelectedIndex = selectedIndex - 1;
		}

		private void cmdOK_Click(object sender, EventArgs e)
		{
			IEnumerator enumerator = null;
			MessageBox.Show("You will need to restart the program for plugin changes to take effect.");
			PluginInfo[] pluginInfoArray = null;
			try
			{
				foreach (var obj in _lstLoaded.Items)
				{
					var objectValue = (PluginInfo)RuntimeHelpers.GetObjectValue(obj);

					if (pluginInfoArray != null)
					{
						Array.Resize(ref pluginInfoArray, pluginInfoArray.Length + 1);
					}
					else
					{
						pluginInfoArray = new PluginInfo[1];
					}

					pluginInfoArray[pluginInfoArray.Length - 1] = objectValue;
				}
			}
			finally
			{
				if (enumerator is IDisposable)
				{
					(enumerator as IDisposable).Dispose();
				}
			}
			MainForm.PluginTypesToLoad = pluginInfoArray;
			MainForm.WritePluginsToLoad();
			DialogResult = DialogResult.OK;
		}

		private void cmdRemove_Click(object sender, EventArgs e)
		{
			var pluginInfo = (PluginInfo)_lstLoaded.Items[_lstLoaded.SelectedIndex];
			_lstLoaded.Items.RemoveAt(_lstLoaded.SelectedIndex);
			_lstAvailable.Items.Add(pluginInfo);
		}

		private void InitializeComponent()
		{
			_Label1 = new System.Windows.Forms.Label();
			_cmdMoveUp = new System.Windows.Forms.Button();
			_cmdMoveDown = new System.Windows.Forms.Button();
			_cmdOK = new System.Windows.Forms.Button();
			_GroupBox1 = new System.Windows.Forms.GroupBox();
			_txtDescription = new System.Windows.Forms.TextBox();
			_txtVersion = new System.Windows.Forms.TextBox();
			_txtEmail = new System.Windows.Forms.TextBox();
			_txtAuthor = new System.Windows.Forms.TextBox();
			_Label4 = new System.Windows.Forms.Label();
			_Label3 = new System.Windows.Forms.Label();
			_Label2 = new System.Windows.Forms.Label();
			_cmdCancel = new System.Windows.Forms.Button();
			_cmdAdd = new System.Windows.Forms.Button();
			_cmdRemove = new System.Windows.Forms.Button();
			_lstAvailable = new System.Windows.Forms.ListBox();
			_Label6 = new System.Windows.Forms.Label();
			_lstLoaded = new System.Windows.Forms.ListBox();
			_GroupBox1.SuspendLayout();
			SuspendLayout();
			// 
			// _Label1
			// 
			_Label1.AutoSize = true;
			_Label1.Location = new System.Drawing.Point(8, 8);
			_Label1.Name = "_Label1";
			_Label1.Size = new System.Drawing.Size(80, 13);
			_Label1.TabIndex = 2;
			_Label1.Text = "Loaded Plugins";
			// 
			// _cmdMoveUp
			// 
			_cmdMoveUp.Enabled = false;
			_cmdMoveUp.Image = global::GumpStudio.Properties.Resources.cmdMoveUp_Image;
			_cmdMoveUp.Location = new System.Drawing.Point(154, 24);
			_cmdMoveUp.Name = "_cmdMoveUp";
			_cmdMoveUp.Size = new System.Drawing.Size(28, 32);
			_cmdMoveUp.TabIndex = 3;
			_cmdMoveUp.Click += new System.EventHandler(cmdMoveUp_Click);
			// 
			// _cmdMoveDown
			// 
			_cmdMoveDown.Enabled = false;
			_cmdMoveDown.Image = global::GumpStudio.Properties.Resources.cmdMoveDown_Image;
			_cmdMoveDown.Location = new System.Drawing.Point(154, 104);
			_cmdMoveDown.Name = "_cmdMoveDown";
			_cmdMoveDown.Size = new System.Drawing.Size(28, 32);
			_cmdMoveDown.TabIndex = 4;
			_cmdMoveDown.Click += new System.EventHandler(cmdMoveDown_Click);
			// 
			// _cmdOK
			// 
			_cmdOK.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			_cmdOK.Location = new System.Drawing.Point(176, 304);
			_cmdOK.Name = "_cmdOK";
			_cmdOK.Size = new System.Drawing.Size(72, 23);
			_cmdOK.TabIndex = 6;
			_cmdOK.Text = "OK";
			_cmdOK.Click += new System.EventHandler(cmdOK_Click);
			// 
			// _GroupBox1
			// 
			_GroupBox1.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			_GroupBox1.Controls.Add(_txtDescription);
			_GroupBox1.Controls.Add(_txtVersion);
			_GroupBox1.Controls.Add(_txtEmail);
			_GroupBox1.Controls.Add(_txtAuthor);
			_GroupBox1.Controls.Add(_Label4);
			_GroupBox1.Controls.Add(_Label3);
			_GroupBox1.Controls.Add(_Label2);
			_GroupBox1.Location = new System.Drawing.Point(8, 144);
			_GroupBox1.Name = "_GroupBox1";
			_GroupBox1.Size = new System.Drawing.Size(320, 154);
			_GroupBox1.TabIndex = 7;
			_GroupBox1.TabStop = false;
			_GroupBox1.Text = "Description";
			// 
			// _txtDescription
			// 
			_txtDescription.Location = new System.Drawing.Point(6, 19);
			_txtDescription.Multiline = true;
			_txtDescription.Name = "_txtDescription";
			_txtDescription.ReadOnly = true;
			_txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			_txtDescription.Size = new System.Drawing.Size(308, 51);
			_txtDescription.TabIndex = 7;
			// 
			// _txtVersion
			// 
			_txtVersion.Location = new System.Drawing.Point(56, 76);
			_txtVersion.Name = "_txtVersion";
			_txtVersion.ReadOnly = true;
			_txtVersion.Size = new System.Drawing.Size(258, 20);
			_txtVersion.TabIndex = 6;
			// 
			// _txtEmail
			// 
			_txtEmail.Location = new System.Drawing.Point(56, 128);
			_txtEmail.Name = "_txtEmail";
			_txtEmail.ReadOnly = true;
			_txtEmail.Size = new System.Drawing.Size(258, 20);
			_txtEmail.TabIndex = 5;
			// 
			// _txtAuthor
			// 
			_txtAuthor.Location = new System.Drawing.Point(56, 102);
			_txtAuthor.Name = "_txtAuthor";
			_txtAuthor.ReadOnly = true;
			_txtAuthor.Size = new System.Drawing.Size(258, 20);
			_txtAuthor.TabIndex = 4;
			// 
			// _Label4
			// 
			_Label4.AutoSize = true;
			_Label4.Location = new System.Drawing.Point(8, 79);
			_Label4.Name = "_Label4";
			_Label4.Size = new System.Drawing.Size(42, 13);
			_Label4.TabIndex = 2;
			_Label4.Text = "Version";
			_Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _Label3
			// 
			_Label3.AutoSize = true;
			_Label3.Location = new System.Drawing.Point(6, 131);
			_Label3.Name = "_Label3";
			_Label3.Size = new System.Drawing.Size(44, 13);
			_Label3.TabIndex = 1;
			_Label3.Text = "Contact";
			_Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _Label2
			// 
			_Label2.AutoSize = true;
			_Label2.Location = new System.Drawing.Point(12, 105);
			_Label2.Name = "_Label2";
			_Label2.Size = new System.Drawing.Size(38, 13);
			_Label2.TabIndex = 0;
			_Label2.Text = "Author";
			_Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// _cmdCancel
			// 
			_cmdCancel.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
			_cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			_cmdCancel.Location = new System.Drawing.Point(254, 304);
			_cmdCancel.Name = "_cmdCancel";
			_cmdCancel.Size = new System.Drawing.Size(75, 23);
			_cmdCancel.TabIndex = 8;
			_cmdCancel.Text = "Cancel";
			_cmdCancel.Click += new System.EventHandler(cmdCancel_Click);
			// 
			// _cmdAdd
			// 
			_cmdAdd.Enabled = false;
			_cmdAdd.Image = global::GumpStudio.Properties.Resources.cmdAdd_Image;
			_cmdAdd.Location = new System.Drawing.Point(154, 56);
			_cmdAdd.Name = "_cmdAdd";
			_cmdAdd.Size = new System.Drawing.Size(28, 23);
			_cmdAdd.TabIndex = 9;
			_cmdAdd.Click += new System.EventHandler(cmdAdd_Click);
			// 
			// _cmdRemove
			// 
			_cmdRemove.Enabled = false;
			_cmdRemove.Image = global::GumpStudio.Properties.Resources.cmdRemove_Image;
			_cmdRemove.Location = new System.Drawing.Point(154, 80);
			_cmdRemove.Name = "_cmdRemove";
			_cmdRemove.Size = new System.Drawing.Size(28, 23);
			_cmdRemove.TabIndex = 10;
			_cmdRemove.Click += new System.EventHandler(cmdRemove_Click);
			// 
			// _lstAvailable
			// 
			_lstAvailable.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left);
			_lstAvailable.IntegralHeight = false;
			_lstAvailable.Location = new System.Drawing.Point(188, 24);
			_lstAvailable.Name = "_lstAvailable";
			_lstAvailable.Size = new System.Drawing.Size(140, 112);
			_lstAvailable.TabIndex = 11;
			_lstAvailable.SelectedIndexChanged += new System.EventHandler(Plugins_SelectedIndexChanged);
			// 
			// _Label6
			// 
			_Label6.AutoSize = true;
			_Label6.Location = new System.Drawing.Point(184, 8);
			_Label6.Name = "_Label6";
			_Label6.Size = new System.Drawing.Size(87, 13);
			_Label6.TabIndex = 12;
			_Label6.Text = "Available Plugins";
			// 
			// _lstLoaded
			// 
			_lstLoaded.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left);
			_lstLoaded.IntegralHeight = false;
			_lstLoaded.Location = new System.Drawing.Point(8, 24);
			_lstLoaded.Name = "_lstLoaded";
			_lstLoaded.Size = new System.Drawing.Size(140, 112);
			_lstLoaded.TabIndex = 13;
			_lstLoaded.SelectedIndexChanged += new System.EventHandler(Plugins_SelectedIndexChanged);
			// 
			// PluginManager
			// 
			AcceptButton = _cmdOK;
			AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			CancelButton = _cmdCancel;
			ClientSize = new System.Drawing.Size(336, 336);
			Controls.Add(_lstLoaded);
			Controls.Add(_Label6);
			Controls.Add(_lstAvailable);
			Controls.Add(_cmdCancel);
			Controls.Add(_GroupBox1);
			Controls.Add(_cmdOK);
			Controls.Add(_Label1);
			Controls.Add(_cmdRemove);
			Controls.Add(_cmdAdd);
			Controls.Add(_cmdMoveDown);
			Controls.Add(_cmdMoveUp);
			MaximizeBox = false;
			MaximumSize = new System.Drawing.Size(352, 1200);
			MinimizeBox = false;
			MinimumSize = new System.Drawing.Size(352, 370);
			Name = "PluginManager";
			Text = "Plugin Manager";
			Load += new System.EventHandler(PluginManager_Load);
			_GroupBox1.ResumeLayout(false);
			_GroupBox1.PerformLayout();
			ResumeLayout(false);
			PerformLayout();

		}

		private void PluginManager_Load(object sender, EventArgs e)
		{
			_lstLoaded.Items.Clear();
			_lstAvailable.Items.Clear();

			if (OrderList != null)
			{
				foreach (var order in OrderList)
				{
					var flag = false;

					foreach (var availablePlugin in AvailablePlugins)
					{
						if (((BasePlugin)RuntimeHelpers.GetObjectValue(availablePlugin)).Info.Equals(order))
						{
							flag = true;
						}
					}

					if (flag)
					{
						_lstLoaded.Items.Add(order);
					}
				}
			}

			foreach (var availablePlugin in AvailablePlugins)
			{
				var objectValue = (BasePlugin)RuntimeHelpers.GetObjectValue(availablePlugin);

				if (!objectValue.IsLoaded)
				{
					var info = objectValue.Info;

					if (!_lstLoaded.Items.Contains(info))
					{
						_lstAvailable.Items.Add(info);
					}
				}
			}
		}

		private void Plugins_SelectedIndexChanged(object sender, EventArgs e)
		{
			var listBox = (ListBox)sender;

			if (listBox.SelectedIndex == -1)
			{
				return;
			}

			var selectedItem = (PluginInfo)listBox.SelectedItem;

			_txtAuthor.Text = selectedItem.AuthorName;
			_txtEmail.Text = selectedItem.AuthorContact;
			_txtVersion.Text = selectedItem.Version;
			_txtDescription.Text = selectedItem.Description;
			if (_lstLoaded.SelectedIndex > 0)
			{
				_cmdMoveUp.Enabled = true;
			}
			else
			{
				_cmdMoveUp.Enabled = false;
			}

			if (_lstLoaded.SelectedIndex < listBox.Items.Count - 1)
			{
				_cmdMoveDown.Enabled = true;
			}
			else
			{
				_cmdMoveDown.Enabled = false;
			}

			if (_lstAvailable.SelectedIndex == -1)
			{
				_cmdAdd.Enabled = false;
			}
			else
			{
				_cmdAdd.Enabled = true;
			}

			if (_lstLoaded.SelectedIndex == -1)
			{
				_cmdRemove.Enabled = false;
			}
			else
			{
				_cmdRemove.Enabled = true;
			}
		}
	}
}
