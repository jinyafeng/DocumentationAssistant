using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentationAssistant.Helper
{
    public static class CommentHelper
    {
        public static string GetClassComment(string name)
        {
            return GetCommonComment(name);
        }

        public static string GetFieldComment(string name)
        {
            return GetCommonComment(name);
        }

        public static string GetConstructorComment(string name)
        {
            return $" Initializes a new instance of the <see cref=\"{name}\"/> class.";
        }

        public static string GetPropertyComment(string name,bool isBoolean,bool hasSetter)
        {
            string comment = "Gets";
            if (hasSetter)
            {
                comment += " or sets";
            }

            if (isBoolean)
            {
                comment += GetPropertyBooleanPart(name);
            }
            else
            {
                comment += GetCommonComment(name);
            }
            return comment;
        }

        public static string GetMethodComment(string name)
        {
            var parts = SpilitNameAndToLower(name);
            parts[0]= new Pluralize.NET.Core.Pluralizer().Pluralize(parts[0]);
            return string.Join(" ", parts) + ".";
        }

        public static string GetParameterComment(string name)
        {
            return GetCommonComment(name);
        }

        public static string GetReturnComment(string name)
        {
            return "The " + name;
        }

        private static string GetPropertyBooleanPart(string name)
        {
            var booleanPart = " a value indicating whether";

            var parts = SpilitNameAndToLower(name).ToList();

            var isWord = parts.FirstOrDefault(o=>o=="is");
            if (isWord!=null)
            {
                parts.Remove(isWord);
                parts.Insert(parts.Count-2,isWord);
            }

            booleanPart += string.Join(" ",parts);
            return booleanPart;
        }

        private static string GetCommonComment(string name) => $"The {string.Join(" ", SpilitNameAndToLower(name))}.";

        private static string[] SpilitNameAndToLower(string name)
        {
            Regex splitByUpper = new Regex(@"([A-Z])(?<=[a-z]\1|[A-Za-z]\1(?=[a-z]))");
            var str= splitByUpper.Replace(name, " $1");
            str = str.Replace('_',' ');
            var parts = str.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i].ToLower();
            }
            return parts;
        }
    }
}
