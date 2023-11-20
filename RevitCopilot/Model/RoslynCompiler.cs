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

public class RoslynCompiler
{
    public void CompileAndLoad(string sourceCode)
    {
        // C# コンパイル用のオプションを設定
        var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

        // 実行アセンブリのディレクトリからすべての DLL ファイルの参照を取得
        var references = GetDllFileNames()
            .Select(fileName => MetadataReference.CreateFromFile(fileName))
            .ToList();

        // 基本的な .NET アセンブリの参照を追加
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
                var classTypes = assembly.GetTypes().Where(x => !x.Name.Contains("Attribute"));
                if (classTypes.Count() != 1)
                {
                    throw new Exception($"クラスが{classTypes.Count()}個存在している。");
                }
                var classType = classTypes.First();
                var instance = Activator.CreateInstance(classType, null);
                var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                if (methods.Count() != 1)
                {
                    throw new Exception($"メソッドが{methods.Count()}個存在している。");
                }
                // メソッドを呼び出す
                var method = methods.First();
                method.Invoke(instance, new object[] { RevitDocuments.Doc });
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
