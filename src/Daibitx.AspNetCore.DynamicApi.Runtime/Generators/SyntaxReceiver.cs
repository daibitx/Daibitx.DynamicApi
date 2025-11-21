using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Daibitx.AspNetCore.DynamicApi.Runtime.Generators
{
    /// <summary>
    /// Lists all interface declarations
    /// </summary>
    public class SyntaxReceiver : ISyntaxReceiver
    {
        public List<InterfaceDeclarationSyntax> CandidateInterfaces { get; } = new List<InterfaceDeclarationSyntax>();
        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is InterfaceDeclarationSyntax ids)
            {
                CandidateInterfaces.Add(ids);
            }
        }
    }
}
