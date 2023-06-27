using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using RevitCopilot.Forms;
using RevitCopilot.Properties;
using System.Windows.Interop;

namespace RevitCopilot
{
    public class RevitCopilotApp : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            // リボンパネルの作成
            RibbonPanel panel = a.CreateRibbonPanel("RevitCopilot");
            string dllPath = Assembly.GetExecutingAssembly().Location;
            PushButtonData button =
                new PushButtonData("SwitchDisplayButton", "Switch Display", dllPath, "RevitCopilot.SwitchDisplay")
                {
                    LargeImage = GetImage(Resources.ChatgptLogo.GetHbitmap())
                };
            panel.AddItem(button);

            // ドッカブルペインの作成＆設定
            var dockablePane = new RevitCopilotView();
            DockablePaneProviderData dockablePaneProviderData = new DockablePaneProviderData
            {
                FrameworkElement = dockablePane,
                InitialState = new DockablePaneState
                {
                    DockPosition = DockPosition.Tabbed,
                    TabBehind = DockablePanes.BuiltInDockablePanes.ProjectBrowser
                }
            };
            dockablePane.SetupDockablePane(dockablePaneProviderData);

            // ドッカブルペインの登録
            DockablePaneId dpid = new DockablePaneId(new Guid("{D7C963CE-B7CA-426A-8D51-6E8254D21157}"));
            a.RegisterDockablePane(dpid, "RevitCopilot Window", dockablePane);

            // イベント登録
            a.ControlledApplication.DocumentOpened += DocumentOpened;
            a.ControlledApplication.DocumentChanged += DocumentChanged;

            return Result.Succeeded;
        }
        private void DocumentOpened(object sender, DocumentOpenedEventArgs e)
        {
            // Revitドキュメントを設定
            var res = RevitDocuments.SetRevitDocuments(e.Document);
            Debug.Assert(res);
        }
        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        private void DocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            // Revitドキュメントを設定
            var res = RevitDocuments.SetRevitDocuments(e.GetDocument());
            Debug.Assert(res);
        }
        private BitmapSource GetImage(IntPtr bm)
        {
            BitmapSource bmSource = Imaging.CreateBitmapSourceFromHBitmap(bm,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            return bmSource;
        }
    }
    /// <summary>
    /// ボタンに実装するコマンド
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class SwitchDisplay : IExternalCommand
    {
        public Result Execute(
            ExternalCommandData commandData,
            ref string message,
            ElementSet elements)
        {
            DockablePaneId dpid = new DockablePaneId(new Guid("{D7C963CE-B7CA-426A-8D51-6E8254D21157}"));
            DockablePane dp = commandData.Application.GetDockablePane(dpid);
            if (dp.IsShown())
            {
                dp.Hide();
            }
            else
            {
                dp.Show();
            }
            return Result.Succeeded;
        }
    }
}
