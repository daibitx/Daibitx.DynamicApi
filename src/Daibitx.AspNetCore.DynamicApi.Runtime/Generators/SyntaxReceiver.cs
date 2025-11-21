using System.Collections.Generic;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Daibitx.AspNetCore.DynamicApi.Runtime.Generators
{
    /// <summary>
    /// 列出所有的接口声明
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
