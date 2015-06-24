using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace UniRxAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class HandleObservableAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "HandleObservable";

        internal const string Title = "IObservable<T> does not handled.";
        internal const string MessageFormat = "This call does not handle IObservable<T>.";
        internal const string Description = "IObservable<T> should be handled(assign, subscribe, chain operator).";
        internal const string Category = "Usage";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            var invocationExpressions = context.Node
                .DescendantNodes(descendIntoChildren: x => !(x is InvocationExpressionSyntax))
                .OfType<InvocationExpressionSyntax>();

            // in lambda expression
            var inlambdaInvocationExpressions = context.Node.DescendantNodes()
                .OfType<LambdaExpressionSyntax>()
                .SelectMany(x => x.DescendantNodes(descendIntoChildren: y => !(y is InvocationExpressionSyntax)))
                .OfType<InvocationExpressionSyntax>();

            foreach (var expr in invocationExpressions.Concat(inlambdaInvocationExpressions))
            {
                var type = context.SemanticModel.GetTypeInfo(expr).Type;
                // UniRx.IObservable? System.IObservable?
                if (new[] { type }.Concat(type.AllInterfaces).Any(x => x.Name == "IObservable"))
                {
                    // Okay => x = M(), var x = M(), return M(), from x in M(), (bool) ? M() : M()
                    if (expr.Parent.IsKind(SyntaxKind.SimpleAssignmentExpression)) continue;
                    if (expr.Parent.IsKind(SyntaxKind.EqualsValueClause) && expr.Parent.Parent.IsKind(SyntaxKind.VariableDeclarator)) continue;
                    if (expr.Parent.IsKind(SyntaxKind.ReturnStatement)) continue;
                    if (expr.Parent.IsKind(SyntaxKind.FromClause)) continue;
                    if (expr.Parent.IsKind(SyntaxKind.ConditionalExpression)) continue;

                    // Okay => M().M()
                    if (expr.DescendantNodes().OfType<InvocationExpressionSyntax>().Any()) continue;

                    // Report Warning
                    var diagnostic = Diagnostic.Create(Rule, expr.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}