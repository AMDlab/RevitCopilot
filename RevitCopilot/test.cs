using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

public class MyClass
{
    public void MyMethod(Document document)
    {
        try
        {
            // 壁の種類を収集するためのフィルタを作成
            FilteredElementCollector collector = new FilteredElementCollector(document);
            collector.OfClass(typeof(WallType));

            // 収集した壁の種類の名前を格納するためのリストを作成
            List<string> wallTypeNames = new List<string>();

            // 各壁の種類に対して
            foreach (WallType wallType in collector)
            {
                // 壁の種類の名前をリストに追加
                wallTypeNames.Add(wallType.Name);
            }

            // リストの内容を改行で連結して1つの文字列にする
            string message = string.Join("\n", wallTypeNames);

            // TaskDialogを使用して壁の種類を表示
            TaskDialog.Show("Available Wall Types", message);
        }
        catch (Exception ex)
        {
            // 例外が発生した場合、TaskDialogを使用してエラーメッセージを表示
            TaskDialog.Show("Error", "An error occurred: " + ex.Message);
        }
    }
}
