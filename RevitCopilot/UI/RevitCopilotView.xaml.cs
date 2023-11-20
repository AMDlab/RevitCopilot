using Autodesk.Revit.UI;
using RevitCopilot.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitCopilot.UI
{
    public partial class RevitCopilotView : Page, IDockablePaneProvider
    {
        private readonly RevitCopilotViewModel vm = new RevitCopilotViewModel();
        private readonly AudioTranscription audioTranscription = new AudioTranscription();

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

        private async void BtnRecVoice_Down(object sender, RoutedEventArgs e)
        {
            var text = await audioTranscription.StartRecording();
            vm.Prompt = text;
        }
    }
}
