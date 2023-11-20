using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NAudio.Wave;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Windows;
using Newtonsoft.Json.Linq;
using RevitCopilot.Properties;
using System.Reflection;

namespace RevitCopilot.Commands
{
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