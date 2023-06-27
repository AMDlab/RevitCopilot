using Autodesk.Revit.UI;
using RevitCopilot.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace RevitCopilot
{
    public partial class RevitCopilotView : Page, IDockablePaneProvider
    {
        private RevitCopilotViewModel vm = new RevitCopilotViewModel();

        public RevitCopilotViewModel GetVM() => vm;

        public RevitCopilotView()
        {
            InitializeComponent();
            this.DataContext = vm;
        }

        public void SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = this;
            data.InitialState = new DockablePaneState
            {
                DockPosition = DockPosition.Tabbed,
                TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
            };
        }

        private void BtnQueryChatGPT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var chatGpt = new RevitCopilotManager();
                vm.CsMethod = chatGpt.GetCsMethodByChatgpt(vm.Prompt);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        private void BtnExecuteCSharpMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var compiler = new CompileManager();
                compiler.Compile(vm.CsMethod);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }
    }
}
