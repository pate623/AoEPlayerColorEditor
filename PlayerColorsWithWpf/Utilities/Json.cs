// Ignore Spelling: Json Deserialize

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PlayerColorEditor.Utilities
{
    public class Json
    {
        public static IEnumerable<T> DeserializeObjects<T>(string input)
        {
            JsonSerializer serializer = new();
            using StringReader strReader = new(input);
            using JsonTextReader jsonReader = new(strReader);
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                yield return serializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Deserialize single JSON object from text.<br/>
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="jsonAsText">The JSON object in text format</param>
        /// <returns>The deserialize object</returns>
        public static T DeserializeObject<T>(string jsonAsText)
        {
            T deserializedJson = JsonConvert.DeserializeObject<T>(jsonAsText);
            return deserializedJson;
        }

    }
}
