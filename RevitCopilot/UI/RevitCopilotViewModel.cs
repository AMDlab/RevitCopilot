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
            "\r\nusing Autodesk.Revit.UI;" +
            "\r\nusing System.Linq;" +
            "\r\n" +
            "\r\npublic class WallCounter" +
            "\r\n{" +
            "\r\n    private UIDocument uiDoc;" +
            "\r\n" +
            "\r\n    // コンストラクタで UIDocument を初期化します。" +
            "\r\n    public WallCounter(UIDocument uiDocument)" +
            "\r\n    {" +
            "\r\n        this.uiDoc = uiDocument;" +
            "\r\n    }" +
            "\r\n" +
            "\r\n    public void CountAndDisplayWalls(Document document)" +
            "\r\n    {" +
            "\r\n        // ドキュメントから壁のインスタンスを取得" +
            "\r\n        FilteredElementCollector collector = new FilteredElementCollector(document);" +
            "\r\n        collector.OfClass(typeof(Wall));" +
            "\r\n" +
            "\r\n        // 取得した壁の総数をカウント" +
            "\r\n        int wallCount = collector.Count();" +
            "\r\n" +
            "\r\n        // タスクダイアログで結果を表示" +
            "\r\n        TaskDialog taskDialog = new TaskDialog(\"壁インスタンスの件数\");" +
            "\r\n        taskDialog.MainInstruction = \"壁インスタンスの数\";" +
            "\r\n        taskDialog.MainContent = $\"プロジェクト内には合計 {wallCount} 個の壁インスタンスがあります。\";" +
            "\r\n        taskDialog.Show();" +
            "\r\n    }" +
            "\r\n}";
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
