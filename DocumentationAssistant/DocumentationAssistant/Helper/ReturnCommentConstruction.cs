using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DocumentationAssistant.Settings;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DocumentationAssistant.Helper
{
    /// <summary>
    ///   The return comment construction.
    /// </summary>
    public class ReturnCommentConstruction
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="ReturnCommentConstruction" /> class.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        public ReturnCommentConstruction(TypeSyntax returnType)
        {
            if (returnType is PredefinedTypeSyntax)
            {
                this.Comment = GeneratePredefinedTypeComment(returnType as PredefinedTypeSyntax);
            }
            else if (returnType is IdentifierNameSyntax)
            {
                this.Comment = GenerateIdentifierNameTypeComment(returnType as IdentifierNameSyntax);
            }
            else if (returnType is QualifiedNameSyntax)
            {
                this.Comment = GenerateQualifiedNameTypeComment(returnType as QualifiedNameSyntax);
            }
            else if (returnType is GenericNameSyntax)
            {
                this.Comment = GenerateGenericTypeComment(returnType as GenericNameSyntax);
            }
            else if (returnType is ArrayTypeSyntax)
            {
                this.Comment = this.GenerateArrayTypeComment(returnType as ArrayTypeSyntax);
            }
            else
            {
                this.Comment = GenerateGeneralComment(returnType.ToFullString());
            }
        }

        /// <summary>
        ///   Generates the comment.
        /// </summary>
        public string Comment { get; }

        /// <summary>
        ///   Generates predefined type comment.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string GeneratePredefinedTypeComment(PredefinedTypeSyntax returnType)
        {
            return DetermineStartedWord(returnType.Keyword.ValueText) + " " + returnType.Keyword.ValueText + ".";
        }

        /// <summary>
        ///   Generates identifier name type comment.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string GenerateIdentifierNameTypeComment(IdentifierNameSyntax returnType)
        {
            return GenerateGeneralComment(returnType.Identifier.ValueText);
        }

        /// <summary>
        ///   Generates qualified name type comment.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string GenerateQualifiedNameTypeComment(QualifiedNameSyntax returnType)
        {
            return GenerateGeneralComment(returnType.ToString());
        }

        /// <summary>
        ///   Generates array type comment.
        /// </summary>
        /// <param name="arrayTypeSyntax"> The array type syntax. </param>
        /// <returns> The comment. </returns>
        private string GenerateArrayTypeComment(ArrayTypeSyntax arrayTypeSyntax)
        {
            return "An array of " + DetermineSpecificObjectName(arrayTypeSyntax.ElementType);
        }

        /// <summary>
        ///   Generates generic type comment.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string GenerateGenericTypeComment(GenericNameSyntax returnType)
        {
            // this will return the full generic Ex. Task<Request>- which then will get added to a CDATA
            if (BridgedOptions.Options?.UseNaturalLanguageForReturnNode != true)
            {
                return returnType.ToFullString();
            }

            string genericTypeStr = returnType.Identifier.ValueText;
            if (genericTypeStr.Contains("ReadOnlyCollection"))
            {
                return "A read only collection of " + DetermineSpecificObjectName(returnType.TypeArgumentList.Arguments.First());
            }

            // IEnumerable IList List
            if (genericTypeStr == "IEnumerable" || genericTypeStr.Contains("List") || genericTypeStr.Contains("Collection"))
            {
                return "A list of " + DetermineSpecificObjectName(returnType.TypeArgumentList.Arguments.First());
            }

            if (genericTypeStr.Contains("Dictionary"))
            {
                if (returnType.TypeArgumentList.Arguments.Count == 2)
                {
                    return $"A dictionary with a key of type {returnType.TypeArgumentList.Arguments.FirstOrDefault()} and a value of type {returnType.TypeArgumentList.Arguments.LastOrDefault()}";
                }
                return GenerateGeneralComment(genericTypeStr);
            }

            return GenerateGeneralComment(genericTypeStr);
        }

        //TODO: test this and implement. This would replace the above strings in GenerateGenericTypeComment, because that does not account for generics of generics
        private static string LookupNaturalStringByType(Type type)
        {
            if (type.IsAssignableFrom(typeof(IReadOnlyCollection<>)))
            {
                var baseStr = "A read only collection of ";
                if (type.IsGenericType)
                {
                    foreach (var item in type.GenericTypeArguments)
                    {
                        baseStr += LookupNaturalStringByType(item);
                    }
                }
                return baseStr += type.Name;
            }

            if (type.IsAssignableFrom(typeof(IDictionary)))
            {
                if (type.GenericTypeArguments.Count() == 2)
                {
                    var key = LookupNaturalStringByType(type.GetGenericArguments().First());
                    var value = LookupNaturalStringByType(type.GetGenericArguments().Last());
                    return $"A dictionary with a key of type {key} and a value of type {value}";
                }
            }
            if (type.IsAssignableFrom(typeof(IEnumerable)))
            {
                var baseStr = "A list of ";
                if (type.IsGenericType)
                {
                    foreach (var item in type.GenericTypeArguments)
                    {
                        baseStr += LookupNaturalStringByType(item);
                    }
                }
                return baseStr += type.Name;
            }
            return DetermineStartedWord(type.Name);
        }

        /// <summary>
        ///   Generates general comment.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string GenerateGeneralComment(string returnType)
        {
            return DetermineStartedWord(returnType) + " " + returnType + ".";
        }

        /// <summary>
        ///   Determines specific object name.
        /// </summary>
        /// <param name="specificType"> The specific type. </param>
        /// <returns> The comment. </returns>
        private static string DetermineSpecificObjectName(TypeSyntax specificType)
        {
            string result = null;
            if (specificType is IdentifierNameSyntax identifierNameSyntax)
            {
                result = Pluralizer.Pluralize((identifierNameSyntax).Identifier.ValueText);
            }
            else if (specificType is PredefinedTypeSyntax predefinedTypeSyntax)
            {
                result = (predefinedTypeSyntax).Keyword.ValueText;
            }
            else if (specificType is GenericNameSyntax genericNameSyntax)
            {
                result = (genericNameSyntax).Identifier.ValueText;
            }
            else
            {
                result = specificType.ToFullString();
            }
            return result + ".";
        }

        /// <summary>
        ///   Determines started word.
        /// </summary>
        /// <param name="returnType"> The return type. </param>
        /// <returns> The comment. </returns>
        private static string DetermineStartedWord(string returnType)
        {
            var vowelChars = new List<char>() { 'a', 'e', 'i', 'o', 'u' };
            if (vowelChars.Contains(char.ToLower(returnType[0])))
            {
                return "An";
            }
            else
            {
                return "A";
            }
        }
    }
}
