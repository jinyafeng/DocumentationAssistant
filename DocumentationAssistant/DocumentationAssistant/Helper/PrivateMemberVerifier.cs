using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentationAssistant.Helper
{
	/// <summary>
	/// Verifies whether a member is private.
	/// </summary>
	public class PrivateMemberVerifier
	{
		public static bool IsPrivateMember(ClassDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			return false;
		}

		public static bool IsPrivateMember(FieldDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to verify whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(ConstructorDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to verify whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(PropertyDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to verify whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(MethodDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to verify whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}
	}
}
