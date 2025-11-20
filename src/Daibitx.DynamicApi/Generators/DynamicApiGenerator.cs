using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daibitx.DynamicApi.Models;
using Daibitx.DynamicApi.Templates;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Daibitx.DynamicApi.Generators
{
    [Generator]
    public class DynamicApiGenerator : ISourceGenerator
    {
        private const string DynamicControllerInterfaceName = "Daibitx.DynamicApi.Interfaces.IDynamicController";
        private const string RoutePrefixAttributeName = "Daibitx.DynamicApi.Attributes.RoutePrefixAttribute";
        private const string ApiExplorerSettingsAttributeName = "Daibitx.DynamicApi.Attributes.ApiExplorerSettingsAttribute";
        private const string HttpMethodAttributeName = "Daibitx.DynamicApi.Attributes.HttpMethodAttribute";
        
        public void Execute(GeneratorExecutionContext context)
        {
            if (!ShouldGenerate(context))
            {
                return;
            }

            if (context.SyntaxReceiver is not SyntaxReceiver receiver)
            {
                return;
            }

            var interfaceSymbol = context.Compilation.GetTypeByMetadataName(DynamicControllerInterfaceName);
            if (interfaceSymbol == null)
            {
                return;
            }

            var routePrefixAttributeSymbol = context.Compilation.GetTypeByMetadataName(RoutePrefixAttributeName);
            var apiExplorerSettingsAttributeSymbol = context.Compilation.GetTypeByMetadataName(ApiExplorerSettingsAttributeName);
            var httpMethodAttributeSymbol = context.Compilation.GetTypeByMetadataName(HttpMethodAttributeName);
            
            foreach (var interfaceSyntax in receiver.CandidateInterfaces)
            {
                var model = context.Compilation.GetSemanticModel(interfaceSyntax.SyntaxTree);
                var symbol = model.GetDeclaredSymbol(interfaceSyntax) as INamedTypeSymbol;
                if (symbol == null || !symbol.AllInterfaces.Contains(interfaceSymbol))
                {
                    continue;
                }
                
                var controllerCode = GenerateController(interfaceSymbol, routePrefixAttributeSymbol, apiExplorerSettingsAttributeSymbol, httpMethodAttributeSymbol, context);
                
                // 将生成的代码添加到编译
                if (!string.IsNullOrEmpty(controllerCode))
                {
                    var controllerName = $"{interfaceSymbol.Name}Controller".TrimStart('I', 'i');
                    context.AddSource($"{controllerName}.g.cs", SourceText.From(controllerCode, Encoding.UTF8));
                }
            }
        }
        private string GenerateController(
           INamedTypeSymbol interfaceSymbol,
           INamedTypeSymbol routePrefixAttributeSymbol,
           INamedTypeSymbol apiExplorerSettingsAttributeSymbol,
           INamedTypeSymbol httpMethodAttributeSymbol,
           GeneratorExecutionContext context)
        {
            try
            {
                var interfaceName = interfaceSymbol.Name;
                var controllerName = $"{interfaceName}Controller".TrimStart('I', 'i');
                var namespaceName = interfaceSymbol.ContainingNamespace.ToString();
                var routePrefix = GetRoutePrefix(interfaceSymbol, routePrefixAttributeSymbol);
                var apiExplorerSettings = GetApiExplorerSettings(interfaceSymbol, apiExplorerSettingsAttributeSymbol);
                var methods = GenerateMethods(interfaceSymbol, context, httpMethodAttributeSymbol);
                var template = new ControllerTemplate();
                return template.Generate(
                    namespaceName,
                    controllerName,
                    interfaceName,
                    routePrefix,
                    apiExplorerSettings,
                    methods);
            }
            catch (Exception ex)
            {
                var diagnostic = Diagnostic.Create(
                   new DiagnosticDescriptor(
                       "DA001",
                       "Controller Generation Failed",
                       $"Failed to generate controller for {interfaceSymbol.Name}: {ex.Message}",
                       "DynamicApiGenerator",
                       DiagnosticSeverity.Warning,
                       isEnabledByDefault: true),
                   interfaceSymbol.Locations.FirstOrDefault());

                context.ReportDiagnostic(diagnostic);
                return string.Empty;
            }

        }
        private string GetRoutePrefix(INamedTypeSymbol interfaceSymbol, INamedTypeSymbol routePrefixAttributeSymbol)
        {
            var attribute = interfaceSymbol.GetAttributes()
                .FirstOrDefault(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, routePrefixAttributeSymbol));

            if (attribute != null && attribute.ConstructorArguments.Length > 0)
            {
                return attribute.ConstructorArguments[0].Value?.ToString() ?? string.Empty;
            }
            return $"/api/{interfaceSymbol.Name.ToLowerInvariant()}";
        }

        private ApiExplorerSettings GetApiExplorerSettings(INamedTypeSymbol interfaceSymbol, INamedTypeSymbol apiExplorerSettingsAttributeSymbol)
        {
            var settings = new ApiExplorerSettings();
            var attribute = interfaceSymbol.GetAttributes().FirstOrDefault(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, apiExplorerSettingsAttributeSymbol));
            if (attribute != null)
            {
                foreach (var namedArg in attribute.NamedArguments)
                {
                    if (namedArg.Key == "IgnoreApi" && namedArg.Value.Value is bool ignoreApi)
                    {
                        settings.IgnoreApi = ignoreApi;
                    }
                    else if (namedArg.Key == "GroupName" && namedArg.Value.Value is string groupName)
                    {
                        settings.GroupName = groupName;
                    }
                }
            }
            return settings;
        }
        private List<MethodInfo> GenerateMethods(INamedTypeSymbol interfaceSymbol, GeneratorExecutionContext context, INamedTypeSymbol httpMethodAttributeSymbol)
        {
            var methods = new List<MethodInfo>();
            foreach (var member in interfaceSymbol.GetMembers())
            {
                if (member is IMethodSymbol methodSymbol && methodSymbol.MethodKind == MethodKind.Ordinary)
                {
                    var attribute = methodSymbol.GetAttributes().FirstOrDefault(ad => SymbolEqualityComparer.Default.Equals(ad.AttributeClass, httpMethodAttributeSymbol));
                    string httpMethod;
                    if (attribute != null && attribute.ConstructorArguments.Length > 0 && attribute.ConstructorArguments[0].Value is Attributes.HttpMethod method)
                    {
                        httpMethod = HttpMethodResolver.Resolve(method);
                    }
                    else
                    {
                        httpMethod = HttpMethodResolver.Resolve(methodSymbol.Name);
                    }
                    
                    var methodInfo = new MethodInfo
                    {
                        Name = methodSymbol.Name,
                        ReturnType = methodSymbol.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                        HttpMethod = httpMethod
                    };
                    
                    // 生成参数信息
                    methodInfo.Parameters = GenerateParameters(methodSymbol, httpMethod);
                    
                    methods.Add(methodInfo);
                }
            }
            return methods;
        }

        private List<ParameterInfo> GenerateParameters(IMethodSymbol methodSymbol, string httpMethod)
        {
            var parameters = new List<ParameterInfo>();
            
            foreach (var param in methodSymbol.Parameters)
            {
                var paramInfo = new ParameterInfo
                {
                    Name = param.Name,
                    Type = param.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    BindingSource = ParameterBindingResolver.Resolve(param, httpMethod),
                    IsOptional = param.IsOptional,
                    DefaultValue = ParameterBindingResolver.GetDefaultValue(param)
                };
                
                parameters.Add(paramInfo);
            }
            
            return parameters;
        }
        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }


        private bool ShouldGenerate(GeneratorExecutionContext context)
        {
            if (context.ParseOptions.PreprocessorSymbolNames.Contains("PREVENT_DYNAMIC_API_GENERATION"))
                return false;
            else
                return true;
        }
    }
}
