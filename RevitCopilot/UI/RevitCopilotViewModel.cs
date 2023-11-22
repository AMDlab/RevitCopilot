using System.ComponentModel;

namespace RevitCopilot.UI
{
    public class RevitCopilotViewModel : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string prompt = Properties.Settings.Default.Prompt;
        public string Prompt
        {
            get => prompt;
            set
            {
                prompt = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Prompt)));
                Properties.Settings.Default.Prompt = value;
            }
        }
        private string csMethod = Properties.Settings.Default.CsMethod;
        
        public string CsMethod
        {
            get => csMethod;
            set
            {
                csMethod = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CsMethod)));
                Properties.Settings.Default.CsMethod = value;
            }
        }

        private string inputDeviceName;

        public string InputDeviceName
        {
            get => inputDeviceName;
            set
            {
                inputDeviceName = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(InputDeviceName)));
            }
        }
    }
}
