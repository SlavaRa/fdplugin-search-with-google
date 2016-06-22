using System.Linq;
using System.Windows.Forms;
using ASCompletion;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using WeifenLuo.WinFormsUI.Docking;
using PluginUI = OutputPanel.PluginUI;

namespace SearchWithGoogle.Helpers
{
    class FormHelper
    {
        public static OutputPanel.PluginUI GetOutputPanelPluginUI()
        {
            return GetPluginUI<OutputPanel.PluginUI>("54749f71-694b-47e0-9b05-e9417f39f20d");
        }

        public static ResultsPanel.PluginUI GetResultPanelPluginUI()
        {
            return GetPluginUI<ResultsPanel.PluginUI>("24df7cd8-e5f0-4171-86eb-7b2a577703ba");
        }

        static T GetPluginUI<T>(string pluginGUID)
        {
            foreach (var pane in PluginBase.MainForm.DockPanel.Panes)
            {
                foreach (var dockContent in pane.Contents)
                {
                    var content = (DockContent) dockContent;
                    if (content?.GetPersistString() != pluginGUID) continue;
                    foreach (var ui in content.Controls.OfType<T>())
                    {
                        return ui;
                    }
                }
            }
            return default(T);
        }

        public static RichTextBox GetOutputPanelTextBox()
        {
            var result = GetOutputPanelPluginUI().Controls.OfType<RichTextBox>().FirstOrDefault();
            return result;
        }

        public static ListViewEx GetResultsPanelListViewEx()
        {
            var result = GetResultPanelPluginUI().Controls.OfType<ListViewEx>().FirstOrDefault();
            return result;
        }
    }
}