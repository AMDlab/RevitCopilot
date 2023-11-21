using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using RevitCopilot;
using System.CodeDom.Compiler;
using Autodesk.Revit.DB;

public class RoslynCompiler
{
    public void CompileAndLoad(string sourceCode, Document revitDocument)
    {
        // C# コンパイル用のオプションを設定
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        // 実行アセンブリのディレクトリからすべての DLL ファイルの参照を取得
        var references = GetDllFileNames()
            .Select(fileName => MetadataReference.CreateFromFile(fileName))
            .ToList();

        // 基本的な .NET アセンブリの参照を追加
        var assemblyDirectoryPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
        //references.Add(MetadataReference.CreateFromFile($"{assemblyDirectoryPath}/System.dll"));
        references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
        references.Add(MetadataReference.CreateFromFile(typeof(Exception).Assembly.Location));


        var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);
        var compilation = CSharpCompilation.Create("DynamicAssembly", new[] { syntaxTree }, references, compilationOptions);

        // コンパイルとアセンブリの生成
        using (var ms = new MemoryStream())
        {
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                // コンパイルエラーの処理
                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                    diagnostic.IsWarningAsError ||
                    diagnostic.Severity == DiagnosticSeverity.Error);

                string exMessage = string.Empty;
                foreach (Diagnostic diagnostic in failures)
                {
                    exMessage += ("{0}: {1}", diagnostic.Id, diagnostic.GetMessage()) + Environment.NewLine;
                }
                throw new Exception(exMessage);
            }
            else
            {
                // コンパイルされたアセンブリから、クラスとメソッドを取得する
                Assembly assembly = Assembly.Load(ms.ToArray());
                var classTypes = assembly.GetTypes().Where(x => x.Name is "MyClass");
                if (classTypes.Count() != 1)
                {
                    throw new Exception($"コンパイル用クラスが見つからない。");
                }
                var classType = classTypes.First();
                var instance = Activator.CreateInstance(classType, null);
                var methods = classType.GetMethods().Where(x => x.Name is "MyMethod");
                if (methods.Count() != 1)
                {
                    throw new Exception($"コンパイル用メソッドが見つからない。");
                }
                // メソッドを呼び出す
                var method = methods.First();
                method.Invoke(instance, new object[] { revitDocument });
            }
        }

    }

    private string[] GetDllFileNames()
    {
        string executingAssemblyPath = Assembly.GetExecutingAssembly().Location;
        string directoryPath = Path.GetDirectoryName(executingAssemblyPath);
        string[] fileNames = Directory.GetFiles(directoryPath, "*.dll");
        return fileNames;
    }
}
