using Autodesk.Revit.UI;
using RevitCopilot.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RevitCopilot.UI
{
    public partial class RevitCopilotWindow : Window
    {
        private readonly RevitCopilotViewModel vm = new RevitCopilotViewModel();
        private readonly AudioTranscription audioTranscription = new AudioTranscription();


        LoadingWindow loadingWindow = null;

        public bool DoCompile { get; private set; } = false;

        public RevitCopilotViewModel GetVM() => vm;

        public RevitCopilotWindow()
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

        private async void BtnQueryChatGPT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 読み込み中ウィンドウを表示
                loadingWindow = new LoadingWindow();
                loadingWindow.Owner = this;
                loadingWindow.Show();

                var chatGpt = new RevitCopilotManager();
                vm.CsMethod = await chatGpt.GetCsMethodByChatgpt(vm.Prompt);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            finally
            {
                // 読み込み中ウィンドウを閉じる
                loadingWindow?.Close();
            }
        }

        private void BtnExecuteCSharpMethod_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 設定をセーブ
                Properties.Settings.Default.Save();
                this.Close();
                DoCompile = true;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
        }

        private async void BtnRecVoice_Down(object sender, RoutedEventArgs e)
        {
            try
            {
                audioTranscription.StartRecording();

                // 読み込み中ウィンドウを表示
                loadingWindow = new LoadingWindow();
                loadingWindow.Owner = this;
                loadingWindow.Show();

                string transcribedText = await audioTranscription.TranscribeAudioAsync();
                vm.Prompt = transcribedText;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", ex.Message);
            }
            finally
            {
                // 読み込み中ウィンドウを閉じる
                loadingWindow?.Close();
            }
        }

        private void BtnChangeInputDevice(object sender, RoutedEventArgs e)
        {
            vm.InputDeviceName = audioTranscription.IncrementInputDeviceIndexAndGetPuroductName();
        }
    }
}
