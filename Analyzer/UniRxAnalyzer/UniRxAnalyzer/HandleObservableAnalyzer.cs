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
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();

            foreach (var expr in invocationExpressions)
            {
                var type = context.SemanticModel.GetTypeInfo(expr).Type;
                // UniRx.IObservable? System.IObservable?
                if (new[] { type }.Concat(type.AllInterfaces).Any(x => x.Name == "IObservable"))
                {
                    if (ValidateInvocation(expr)) continue;

                    // Report Warning
                    var diagnostic = Diagnostic.Create(Rule, expr.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        static bool ValidateInvocation(InvocationExpressionSyntax expr)
        {
            bool allAncestorsIsParenthes = true;
            foreach (var x in expr.Ancestors())
            {
                // scope is in lambda, method
                if (x.IsKind(SyntaxKind.SimpleLambdaExpression) || x.IsKind(SyntaxKind.ParenthesizedLambdaExpression) || x.IsKind(SyntaxKind.ArrowExpressionClause))
                {
                    // () => M()
                    if (allAncestorsIsParenthes) return true;
                    break;
                }
                if (x.IsKind(SyntaxKind.MethodDeclaration)) break;
                if (x.IsKind(SyntaxKind.PropertyDeclaration)) break;
                if (x.IsKind(SyntaxKind.ConstructorDeclaration)) break;

                // x = M()
                if (x.IsKind(SyntaxKind.SimpleAssignmentExpression)) return true;
                // var x = M()
                if (x.IsKind(SyntaxKind.VariableDeclarator)) return true;
                // return M()
                if (x.IsKind(SyntaxKind.ReturnStatement)) return true;
                // from x in M()
                if (x.IsKind(SyntaxKind.FromClause)) return true;
                // (bool) ? M() : M()
                if (x.IsKind(SyntaxKind.ConditionalExpression)) return true;
                // M(M())
                if (x.IsKind(SyntaxKind.InvocationExpression)) return true;
                // new C(M())
                if (x.IsKind(SyntaxKind.ObjectCreationExpression)) return true;

                // (((((M()))))
                if (!x.IsKind(SyntaxKind.ParenthesizedExpression))
                {
                    allAncestorsIsParenthes = false;
                }
            }

            // Okay => M().M()
            if (expr.DescendantNodes().OfType<InvocationExpressionSyntax>().Any()) return true;

            return false;
        }
    }
}