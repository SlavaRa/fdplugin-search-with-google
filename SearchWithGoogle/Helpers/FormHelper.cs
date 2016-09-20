using System.Linq;
using System.Windows.Forms;
using PluginCore;
using WeifenLuo.WinFormsUI.Docking;

namespace SearchWithGoogle.Helpers
{
    class FormHelper
    {
        public static OutputPanel.PluginUI GetOutputPanelPluginUI() => GetPluginUI<OutputPanel.PluginUI>("54749f71-694b-47e0-9b05-e9417f39f20d");

        public static ResultsPanel.PluginUI GetResultPanelPluginUI() => GetPluginUI<ResultsPanel.PluginUI>("24df7cd8-e5f0-4171-86eb-7b2a577703ba");

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

        public static RichTextBox GetOutputPanelTextBox() => GetContextMenuOwner<RichTextBox>(GetOutputPanelPluginUI());

        public static ListViewEx GetResultsPanelListViewEx() => GetContextMenuOwner<ListViewEx>(GetResultPanelPluginUI());

        static T GetContextMenuOwner<T>(Control pluginUI)
        {
            var result = pluginUI.Controls.OfType<T>().FirstOrDefault();
            return result;
        }
    }
}