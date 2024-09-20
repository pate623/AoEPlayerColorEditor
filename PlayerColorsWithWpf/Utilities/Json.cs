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

    }
}
