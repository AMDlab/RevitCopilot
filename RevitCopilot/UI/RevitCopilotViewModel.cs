using System.ComponentModel;

namespace RevitCopilot.UI
{
    public class RevitCopilotViewModel : INotifyPropertyChanged
    {
        public virtual event PropertyChangedEventHandler PropertyChanged = delegate { };

        private string prompt = "壁インスタンスの数を数えてタスクダイアログで表示してください。";
        public string Prompt
        {
            get => prompt;
            set
            {
                prompt = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Prompt)));
            }
        }
        private string csMethod =
            "using Autodesk.Revit.DB;" +
            "\nusing Autodesk.Revit.UI;" +
            "\n" +
            "\npublic class WallCounter" +
            "\n{" +
            "\n    public void CountWalls(Document document)" +
            "\n    {" +
            "\n        // 壁のインスタンスを取得する" +
            "\n        FilteredElementCollector collector = new FilteredElementCollector(document);" +
            "\n        collector.OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType();" +
            "\n        int wallCount = collector.ToElements().Count;" +
            "\n" +
            "\n        // タスクダイアログに壁のインスタンス数を表示する" +
            "\n        TaskDialog.Show(\"Wall Count\", \"The number of wall instances is \" + wallCount.ToString() + \".\");" +
            "\n    }" +
            "\n}" +
            "\n";
        public string CsMethod
        {
            get => csMethod;
            set
            {
                csMethod = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CsMethod)));
            }
        }
    }
}
