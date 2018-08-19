using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CodeAnalysis.CSharp;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace UniRx
{
    public static class TriggerFileGenerator
    {
        /*
                Generate liket following text...

#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace UniRx.Async.Triggers
{
    [DisallowMultipleComponent]
    public class AsyncAnimatorTrigger : AsyncTriggerBase
    {
        AsyncTriggerPromise<int> onAnimatorIK;
        AsyncTriggerPromiseDictionary<int> onAnimatorIKs;
        AsyncTriggerPromise<AsyncUnit> onAnimatorMove;
        AsyncTriggerPromiseDictionary<AsyncUnit> onAnimatorMoves;

        protected override IEnumerable<ICancelablePromise> GetPromises()
        {
            return Concat(onAnimatorIK, onAnimatorIKs, onAnimatorMove, onAnimatorMoves);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        void OnAnimatorIK(int layerIndex)
        {
            TrySetResult(onAnimatorIK, onAnimatorIKs, layerIndex);
        }

        /// <summary>Callback for setting up animation IK (inverse kinematics).</summary>
        public UniTask<int> OnAnimatorIKAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onAnimatorIK, ref onAnimatorIKs, cancellationToken);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        void OnAnimatorMove()
        {
            TrySetResult(onAnimatorMove, onAnimatorMoves, AsyncUnit.Default);
        }

        /// <summary>Callback for processing animation movements for modifying root motion.</summary>
        public UniTask OnAnimatorMoveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetOrAddPromise(ref onAnimatorMove, ref onAnimatorMoves, cancellationToken);
        }
    }
}

#endif

        */
        class TypeInfo
        {
            public string TypeName { get; set; }
            public MethodInfo[] Methods { get; set; }

            public override string ToString()
            {
                return TypeName + ", " + string.Join(" | ", Methods.Select(x => x.ToString()));
            }
        }

        class MethodInfo
        {
            public string MethodName { get; set; }
            public ParameterListSyntax Args { get; set; }
            public bool IsPublic { get; set; }

            public override string ToString()
            {
                return (IsPublic ? "public " : "") + MethodName + Args;
            }
        }

        static string ToCamelCase(string s)
        {
            return Char.ToLower(s[0]) + s.Substring(1, s.Length - 1);
        }

        public static void GenerateAsyncTrigger(string rootDir, string outputDir)
        {
            var typeInfos = new List<TypeInfo>();

            // parse code

            foreach (var item in Directory.EnumerateFiles(rootDir))
            {
                if (Path.GetFileNameWithoutExtension(item).EndsWith("Trigger"))
                {
                    var file = File.ReadAllText(item);

                    var tree = CSharpSyntaxTree.ParseText(file, new CSharpParseOptions(preprocessorSymbols: new[]
                    {
                        "CSHARP_7_OR_LATER"
                    }));

                    var type = tree.GetCompilationUnitRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First();

                    var methods = type.DescendantNodes()
                        .OfType<MethodDeclarationSyntax>()
                        .Select(x => new MethodInfo
                        {
                            MethodName = x.Identifier.Text,
                            Args = x.ParameterList,
                            IsPublic = x.Modifiers.Any(y => y.Value.ToString() == "public")
                        })
                        .Where(x => x.MethodName != "RaiseOnCompletedOnDestroy")
                        .ToArray();

                    typeInfos.Add(new TypeInfo { TypeName = type.Identifier.Text, Methods = methods });
                }
            }

            // generate text

            var sb = new StringBuilder();
            foreach (var item in typeInfos)
            {
                // if (item.TypeName == "AsyncStateMachineTrigger") continue;

                var methodText = string.Join(" | ", item.Methods.Select(x => x.ToString()));

                List<(string returnType, string fieldName)> fieldList = new List<(string returnType, string fieldName)>();
                foreach (var method in item.Methods.Where(x => !x.IsPublic))
                {
                    var argsList = method.Args.Parameters.Select(x => x.Type.ToString()).ToArray();
                    var returnTypeName = (argsList.Length == 0) ? "AsyncUnit"
                                       : (argsList.Length == 1) ? argsList[0]
                                       : "(" + string.Join(", ", argsList) + ")";
                    fieldList.Add((returnTypeName, ToCamelCase(method.MethodName)));
                }

                var promiseList = string.Join(", ", fieldList.SelectMany(x => new[] { x.fieldName, x.fieldName + "s" }));
                var fieldTemplate = new StringBuilder();
                foreach (var field in fieldList)
                {
                    fieldTemplate.AppendLine($"        AsyncTriggerPromise<{field.returnType}> {field.fieldName};");
                    fieldTemplate.AppendLine($"        AsyncTriggerPromiseDictionary<{field.returnType}> {field.fieldName}s;");
                }

                var methodTemplate = new StringBuilder();
                foreach (var method in item.Methods)
                {
                    if (!method.IsPublic)
                    {
                        var argsList = method.Args.Parameters.Select(x => x.Identifier.ToString()).ToArray();
                        var parameterName = (argsList.Length == 0) ? "AsyncUnit.Default"
                                           : (argsList.Length == 1) ? argsList[0]
                                           : "(" + string.Join(", ", argsList) + ")";

                        var m = ToCamelCase(method.MethodName);

                        methodTemplate.AppendLine($@"
        void {method.MethodName}{method.Args}
        {{
            TrySetResult({m}, {m}s, {parameterName});
        }}
");
                    }
                    else
                    {
                        var m = ToCamelCase(method.MethodName.Replace("AsObservable", ""));

                        methodTemplate.AppendLine($@"
        public UniTask {method.MethodName.Replace("AsObservable", "Async")}(CancellationToken cancellationToken = default(CancellationToken))
        {{
            return GetOrAddPromise(ref {m}, ref {m}s, cancellationToken);
        }}
");
                    }
                }

                var template = $@"
#if CSHARP_7_OR_LATER
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UniRx.Async.Triggers
{{
    [DisallowMultipleComponent]
    public class {item.TypeName.Replace("Observable", "Async")} : AsyncTriggerBase
    {{
{fieldTemplate}

        protected override IEnumerable<ICancelablePromise> GetPromises()
        {{
            return Concat({promiseList});
        }}

{methodTemplate}
    }}
}}

#endif
";

                if (item.TypeName == "ObservableMouseTrigger")
                {
                    sb.AppendLine();
                    sb.AppendLine("#if !(UNITY_IPHONE || UNITY_ANDROID || UNITY_METRO)");
                }

                sb.AppendLine(template);

                if (item.TypeName == "ObservableMouseTrigger")
                {
                    sb.AppendLine();
                    sb.AppendLine("#endif");
                }

                // gen

                var fileName = item.TypeName.Replace("Observable", "Async");
                if (fileName == "AsyncDestroyTrigger"
                 || fileName == "AsyncStateMachineTrigger")
                {
                    sb.Clear();
                    continue;
                }

                var outputPath = Path.Combine(outputDir, fileName + ".cs");
                var code = sb.ToString();

                File.WriteAllText(outputPath, code);
                sb.Clear();
            }
        }
    }
}
