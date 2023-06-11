using System;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace boilerPlate.Domain.Helpers
{
	public static class TextHelpers
	{
        public static string TakeTillLength(this string valToChange, int index)
        {
            if (string.IsNullOrEmpty(valToChange))
            {
                return string.Empty;
            }
            else if (valToChange.Length <= index)
            {
                return valToChange;
            }
            else
            {
                return valToChange.Substring(index);
            }
        }
        public static string SerializeToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static Hashtable ToHashTable<T>(this T obj)
        {
            var jsonString = obj.SerializeToJson();
            
            return ConvertJsonToHashTable(jsonString);
        }
        static Hashtable ConvertJsonToHashTable(string jsonString)
        {
            // Parse the JSON string into a JsonDocument
            JsonDocument jsonDocument = JsonDocument.Parse(jsonString);

            Hashtable hashtable = new Hashtable();

            // Convert the JSON properties to key-value pairs in the Hashtable
            foreach (JsonProperty property in jsonDocument.RootElement.EnumerateObject())
            {
                // Convert property value to a type supported by Hashtable
                object value = ConvertJsonValueToHashtableValue(property.Value);

                // Add key-value pair to the Hashtable
                hashtable.Add(property.Name, value);
            }

            return hashtable;
        }
        static object ConvertJsonValueToHashtableValue(JsonElement jsonElement)
        {
            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Object:
                    // Convert nested object recursively
                    return ConvertJsonToHashTable(jsonElement.GetRawText());

                case JsonValueKind.Array:
                    // Convert array to ArrayList recursively
                    ArrayList arrayList = new ArrayList();
                    foreach (JsonElement element in jsonElement.EnumerateArray())
                    {
                        arrayList.Add(ConvertJsonValueToHashtableValue(element));
                    }
                    return arrayList;

                case JsonValueKind.String:
                    return jsonElement.GetString();

                case JsonValueKind.Number:
                    // Convert number to int or double
                    return jsonElement.ValueKind == JsonValueKind.Number ? (object)jsonElement.GetInt32() : jsonElement.GetDouble();

                case JsonValueKind.True:
                    return true;

                case JsonValueKind.False:
                    return false;

                case JsonValueKind.Null:
                default:
                    return null;
            }
        }
    }
}

