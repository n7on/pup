using System.Collections.Generic;
using System.Management.Automation;
using System.Text;
using System.Text.Json;

namespace Pup.Common
{
    public static class JsonHelper
    {

        /// <summary>
        /// Converts a JsonElement to C# objects (Dictionary, array, primitives) for use in PowerShell.
        /// </summary>
        public static object ConvertJsonElement(JsonElement element)
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.Object:
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                    {
                        dict[prop.Name] = ConvertJsonElement(prop.Value);
                    }
                    return new PSObject(dict);

                case JsonValueKind.Array:
                    var list = new List<object>();
                    foreach (var item in element.EnumerateArray())
                    {
                        list.Add(ConvertJsonElement(item));
                    }
                    return list.ToArray();

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Number:
                    if (element.TryGetInt64(out var longVal))
                        return longVal;
                    return element.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                case JsonValueKind.Undefined:
                default:
                    return null;
            }
        }

        /// <summary>
        /// Converts a JsonElement object to a Dictionary.
        /// </summary>
        public static Dictionary<string, object> ConvertJsonElementToDict(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
                return null;

            var dict = new Dictionary<string, object>();
            foreach (var prop in element.EnumerateObject())
            {
                dict[prop.Name] = ConvertJsonElement(prop.Value);
            }
            return dict;
        }

        /// <summary>
        /// Pretty-prints a JsonElement as a formatted string for human display.
        /// Handles indentation, truncates large arrays/objects.
        /// </summary>
        public static string PrettyPrint(JsonElement element, int indent = 0)
        {
            var indentStr = new string(' ', indent * 2);

            switch (element.ValueKind)
            {
                case JsonValueKind.Undefined:
                    return "undefined";

                case JsonValueKind.Null:
                    return "null";

                case JsonValueKind.True:
                    return "true";

                case JsonValueKind.False:
                    return "false";

                case JsonValueKind.Number:
                    return element.GetRawText();

                case JsonValueKind.String:
                    return element.GetString();

                case JsonValueKind.Array:
                    return PrettyPrintArray(element, indent, indentStr);

                case JsonValueKind.Object:
                    return PrettyPrintObject(element, indent, indentStr);

                default:
                    return element.GetRawText();
            }
        }

        private static string PrettyPrintArray(JsonElement element, int indent, string indentStr)
        {
            var items = new List<string>();
            foreach (var item in element.EnumerateArray())
            {
                items.Add(PrettyPrint(item, indent + 1));
            }

            if (items.Count == 0)
                return "[]";

            if (items.Count <= 5 && string.Join(", ", items).Length < 80)
                return $"[{string.Join(", ", items)}]";

            var sb = new StringBuilder("[\n");
            var maxItems = 20;
            for (int i = 0; i < System.Math.Min(items.Count, maxItems); i++)
            {
                sb.Append($"{indentStr}  {items[i]}");
                if (i < items.Count - 1) sb.Append(',');
                sb.Append('\n');
            }
            if (items.Count > maxItems)
            {
                sb.Append($"{indentStr}  ... ({items.Count - maxItems} more items)\n");
            }
            sb.Append($"{indentStr}]");
            return sb.ToString();
        }

        private static string PrettyPrintObject(JsonElement element, int indent, string indentStr)
        {
            var props = new List<string>();
            foreach (var prop in element.EnumerateObject())
            {
                props.Add($"{prop.Name}: {PrettyPrint(prop.Value, indent + 1)}");
            }

            if (props.Count == 0)
                return "{}";

            if (props.Count <= 3 && string.Join(", ", props).Length < 60)
                return $"{{ {string.Join(", ", props)} }}";

            var sb = new StringBuilder("{\n");
            var maxProps = 30;
            for (int i = 0; i < System.Math.Min(props.Count, maxProps); i++)
            {
                sb.Append($"{indentStr}  {props[i]}");
                if (i < props.Count - 1) sb.Append(',');
                sb.Append('\n');
            }
            if (props.Count > maxProps)
            {
                sb.Append($"{indentStr}  ... ({props.Count - maxProps} more properties)\n");
            }
            sb.Append($"{indentStr}}}");
            return sb.ToString();
        }

    }
}
