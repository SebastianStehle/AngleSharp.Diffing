using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using AngleSharp.Dom;

namespace AngleSharp.Diffing.Extensions
{
    public static class ElementExtensions
    {
        public static bool TryGetAttrValue(this IElement element, string attributeName, out bool result)
        {
            return TryGetAttrValue(element, attributeName, ParseBoolAttribute, out result);

            static bool ParseBoolAttribute(string boolValue) => string.IsNullOrWhiteSpace(boolValue) || bool.Parse(boolValue);
        }

        public static bool TryGetAttrValue(this IElement element, string attributeName, [NotNullWhen(true)]out string result)
        {
            return TryGetAttrValue(element, attributeName, GetStringAttrValue, out result);

            static string GetStringAttrValue(string value) => value;
        }

        public static bool TryGetAttrValue<T>(this IElement element, string attributeName, out T result) where T : System.Enum
        {
            return TryGetAttrValue(element, attributeName, ParseEnum, out result);

            static T ParseEnum(string enumValue) => (T)Enum.Parse(typeof(T), enumValue, true);
        }

        public static bool TryGetAttrValue<T>(this IElement element, string attributeName, Func<string, T> resultFunc, [NotNullWhen(true)] out T result)
        {
            if (element is null) throw new ArgumentNullException(nameof(element));
            if (resultFunc is null) throw new ArgumentNullException(nameof(resultFunc));

            if (element.Attributes[attributeName] is IAttr optAttr)
            {
                result = resultFunc(optAttr.Value);
                return true;
            }
            else
            {
#pragma warning disable CS8653 // A default expression introduces a null value for a type parameter.
                result = default;
#pragma warning restore CS8653 // A default expression introduces a null value for a type parameter.
                return false;
            }
        }

        public static TEnum GetInlineOptionOrDefault<TEnum>(this IElement startElement, string optionName, TEnum defaultValue)
            where TEnum : System.Enum => GetInlineOptionOrDefault(startElement, optionName, x => x.Parse<TEnum>(), defaultValue);

        public static bool GetInlineOptionOrDefault(this IElement startElement, string optionName, bool defaultValue)
            => GetInlineOptionOrDefault(startElement, optionName, x => string.IsNullOrWhiteSpace(x) || bool.Parse(x), defaultValue);

        public static T GetInlineOptionOrDefault<T>(this IElement startElement, string optionName, Func<string, T> resultFunc, T defaultValue)
        {
            if (startElement is null) throw new ArgumentNullException(nameof(startElement));
            if (resultFunc is null) throw new ArgumentNullException(nameof(resultFunc));

            var element = startElement;

            while (element is { })
            {
                if (element.Attributes[optionName] is IAttr attr)
                {
                    return resultFunc(attr.Value);
                }
                element = element.ParentElement;
            }

            return defaultValue;
        }

        public static bool TryGetNodeIndex(this INode node, [NotNullWhen(true)]out int index)
        {
            index = -1;

            if (node.ParentElement is null) return false;

            var parentElement = node.ParentElement;

            for (int i = 0; i < parentElement.ChildNodes.Length; i++)
            {
                if (parentElement.ChildNodes[i].Equals(node))
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<INode> GetParents(this INode node)
        {
            var parent = node.Parent;
            while (parent is { })
            {
                yield return parent;
                parent = parent.Parent;
            }
        }
    }
}