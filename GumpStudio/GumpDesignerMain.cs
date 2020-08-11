// Decompiled with JetBrains decompiler
// Type: GumpStudio.GumpDesignerMain
// Assembly: GumpStudioCore, Version=1.8.3024.24259, Culture=neutral, PublicKeyToken=null
// MVID: A77D32E5-7519-4865-AA26-DCCB34429732
// Assembly location: C:\GumpStudio_1_8_R3_quinted-02\GumpStudioCore.dll

using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GumpStudio.Forms;
using GumpStudio.Properties;
using Squirrel;

namespace GumpStudio
{
    internal sealed class GumpDesignerMain
    {
        [STAThread]
        public static void Main()
        {

            try
            {
                Task.Run(() =>
                {
                    using (var mgr = UpdateManager.GitHubUpdateManager("https://github.com/Reetus/GumpStudio"))
                    {
                        return mgr.Result.UpdateApp();
                    }
                }).ContinueWith((re) =>
                {

                });
            }
            catch (Exception)
            {
                MessageBox.Show(Resources.Failed_update_check_, Resources.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            Application.EnableVisualStyles();
            Application.Run( new DesignerForm() );
        }
    }
}
