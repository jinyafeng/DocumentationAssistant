﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant.Helper
{
	/// <summary>
	/// A checker to check whether a member is private.
	/// </summary>
	public class PrivateMemberChecker
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

			// If the member is public, we still need to check whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(ConstructorDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to check whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(PropertyDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to check whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}

		public static bool IsPrivateMember(MethodDeclarationSyntax node)
		{
			if (!node.Modifiers.Any(SyntaxKind.PublicKeyword))
			{
				return true;
			}

			// If the member is public, we still need to check whether its parent class is a private class.
			// Since we don't want show warnings for public members within a private class.
			return IsPrivateMember(node.Parent as ClassDeclarationSyntax);
		}
	}
}
