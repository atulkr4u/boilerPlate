using System;
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
    }
}

