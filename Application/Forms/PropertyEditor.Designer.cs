
namespace GumpStudio
{
	partial class PropertyEditor
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._Properties = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// _Properties
			// 
			this._Properties.Dock = System.Windows.Forms.DockStyle.Fill;
			this._Properties.Location = new System.Drawing.Point(0, 0);
			this._Properties.Name = "_Properties";
			this._Properties.Size = new System.Drawing.Size(284, 361);
			this._Properties.TabIndex = 0;
			// 
			// PropertyEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 361);
			this.Controls.Add(this._Properties);
			this.Name = "PropertyEditor";
			this.Text = "Properties";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _Properties;
	}
}