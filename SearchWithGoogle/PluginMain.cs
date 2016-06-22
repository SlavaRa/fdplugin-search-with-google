using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Completion;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace SearchWithGoogle
{
    public class PluginMain : IPlugin
    {
        string settingFilename;
        readonly ToolStripMenuItem editorMenuItem = new ToolStripMenuItem("&Search with Google");
        readonly ToolStripMenuItem searchMenuItem = new ToolStripMenuItem("&Search with Google");

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name => nameof(SearchWithGoogle);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid => "72ec4500-294a-4f24-97aa-7761fb1def6f";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author => "SlavaRa";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description => $"{nameof(SearchWithGoogle)} Plugin";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help => "http://www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings { get; private set; }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
            CreateMenuItems();
            UpdateMenuItems();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    UpdateMenuItems();
                    break;
                case EventType.UIStarted:
                    ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                    UpdateMenuItems();
                    break;
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            var dataPath = Path.Combine(PathHelper.DataDir, Name);
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            settingFilename = Path.Combine(dataPath, $"{nameof(Settings)}.fdb");
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            Settings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else Settings = (Settings) ObjectSerializer.Deserialize(settingFilename, Settings);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, Settings);

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.FileSwitch | EventType.UIStarted);

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        void CreateMenuItems()
        {
            editorMenuItem.Click += OnEditorMenuItemClick;
            searchMenuItem.Click += OnEditorMenuItemClick;
            AddMenuitem(PluginBase.MainForm.EditorMenu.Items, editorMenuItem);
            AddMenuitem(((ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("SearchMenu")).DropDownItems, searchMenuItem);
        }

        static void AddMenuitem(ToolStripItemCollection items, ToolStripItem item)
        {
            var index = FindItemIndex(items, TextHelper.GetString("ASCompletion.Label.GotoDeclaration"));
            items.Insert(index, new ToolStripSeparator());
            items.Insert(index, item);
        }

        static int FindItemIndex(ToolStripItemCollection items, string text)
        {
            for (var i = 0; i < items.Count; i++)
            {
                if (items[i].Text == text) return i;
            }
            return items.Count;
        }

        /// <summary>
        /// Updates the state of the menu items
        /// </summary>
        void UpdateMenuItems()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var enabled = sci.SelTextSize > 0;
            editorMenuItem.Enabled = enabled;
            searchMenuItem.Enabled = enabled;
        }

        static void Search()
        {
            var text = PluginBase.MainForm.CurrentDocument.SciControl.SelText;
            ProcessHelper.StartAsync("https://www.google.by/search?q=" + text);
        }

        #endregion

        void OnResolvedContextChanged(ResolvedContext resolved) => UpdateMenuItems();

        static void OnEditorMenuItemClick(object sender, EventArgs eventArgs) => Search();
    }
}