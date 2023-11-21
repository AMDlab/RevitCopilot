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
using RevitCopilot.UI;

namespace RevitCopilot.Commands
{
    /// <summary>
    /// ボタンに実装するコマンド
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class OpenCopilotWindow : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var view = new RevitCopilotWindow();
            view.ShowDialog();
            if (view.DoCompile)
            {
                var compiler = new RoslynCompiler();
                compiler.CompileAndLoad(view.GetVM().CsMethod, commandData.Application.ActiveUIDocument.Document);
            }
            return Result.Succeeded;
        }
    }
}