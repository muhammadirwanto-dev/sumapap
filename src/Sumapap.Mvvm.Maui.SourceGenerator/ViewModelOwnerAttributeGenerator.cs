using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Sumapap.Mvvm.Maui.SourceGenerator
{
    [Generator(LanguageNames.CSharp)]
    public class ViewModelOwnerAttributeGenerator : IIncrementalGenerator
    {
        private const string AttributeNamespace = "Sumapap.Mvvm.Attributes.ViewModelOwnerAttribute";

        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (node, _) => IsClassWithAttribute(node),
                    transform: (syntaxContext, _) => GetSemanticTargetForGeneration(syntaxContext))
                .Where(classSymbol => classSymbol != null);

            context.RegisterSourceOutput(classDeclarations, (productionContext, classSymbol) =>
            {
                var sourceCode = GenerateSourceCode(classSymbol);

                productionContext.AddSource($"{classSymbol}.ViewModelOwner.g.cs", sourceCode);
            });
        }

        private static bool IsClassWithAttribute(SyntaxNode node)
        {
            return node is ClassDeclarationSyntax syntax && syntax.AttributeLists.Count > 0;
        }

        private static INamedTypeSymbol GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            foreach (var attributeList in classDeclaration.AttributeLists)
            {
                foreach (var attribute in attributeList.Attributes)
                {
                    var symbol = context.SemanticModel.GetSymbolInfo(attribute).Symbol?.ContainingType;
                    if (symbol != null)
                    {
                        var symbolName = symbol?.ToDisplayString();
                        if (symbolName.StartsWith(AttributeNamespace))
                        {
                            return context.SemanticModel.GetDeclaredSymbol(classDeclaration) as INamedTypeSymbol;
                        }
                    }
                }
            }

            return null;
        }

        private static string GenerateSourceCode(INamedTypeSymbol classSymbol)
        {
            // Retrieve the class name and attribute argument (ViewModel type)
            var className = classSymbol.Name;

            // Retrieve attribute class data
            var attributeData = classSymbol.GetAttributes()
                .First(attr => attr.AttributeClass?.ToDisplayString().StartsWith(AttributeNamespace) ?? false);

            // Full name with namespace
            var viewModelSymbol = attributeData.AttributeClass.IsGenericType
                ? attributeData.AttributeClass.TypeArguments[0] as INamedTypeSymbol
                : attributeData.ConstructorArguments[0].Value as INamedTypeSymbol;
            var viewModelName = viewModelSymbol?.ToDisplayString();

            // Retrieve the IsDefaultConstructor from the named arguments
            // Extract information from the attribute data
            var attributeProperties = attributeData.NamedArguments.ToDictionary(
                arg => arg.Key,
                arg => arg.Value.Value?.ToString() ?? "null"
            );

            // Check for specific properties in the attribute data
            var isDefaultConstructor =
                attributeProperties.ContainsKey("IsDefaultConstructor") &&
                bool.Parse(attributeProperties["IsDefaultConstructor"]);

            // Class namespace
            var namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

            if (isDefaultConstructor)
            {
                return CreateClassWithDefaultConstructor(namespaceName, className, viewModelName);
            }
            else
            {
                return CreateClass(namespaceName, className, viewModelName);
            }
        }

        private static string CreateClass(string namespaceStr, string className, string viewModelTypeName)
        {
            return
$@"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace {namespaceStr}
{{
    public partial class {className} : IViewModelOwner<{viewModelTypeName}>
    {{
        public {viewModelTypeName} ViewModel {{ get; private set; }} = default!;

        private void PostInitializeComponent()
        {{
            ViewModel = MauiServiceProvider.Current.GetRequiredService<{viewModelTypeName}>();
            BindingContext = ViewModel;
        }}
    }}
}}";
        }

        private static string CreateClassWithDefaultConstructor(string namespaceStr, string className, string viewModelTypeName)
        {
            return
$@"using Sumapap.Mvvm.Maui;
using Sumapap.Mvvm.Abstractions;

namespace {namespaceStr}
{{
    public partial class {className} : IViewModelOwner<{viewModelTypeName}>
    {{
        public {viewModelTypeName} ViewModel {{ get; }}

        public {className}()
        {{
            InitializeComponent();

            ViewModel = MauiServiceProvider.Current.GetRequiredService<{viewModelTypeName}>();
            BindingContext = ViewModel;
        }}
    }}
}}";
        }
    }
}
