// Ignore Spelling: Json Deserialize deserialized Deserializes

using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace PlayerColorEditor.Utilities
{
    /// <summary>
    /// Helper class for saving and reading JSON files.
    /// </summary>
    public class Json
    {
        /// <summary>
        /// Deserializes JSON objects from text.<br/>
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="fileLocation">The JSON file to read</param>
        /// <returns>The deserialized objects</returns>
        public static IEnumerable<T> DeserializeObjects<T>(string fileLocation)
        {
            JsonSerializer serializer = new();
            using StringReader strReader = new(fileLocation);
            using JsonTextReader jsonReader = new(strReader);
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                yield return serializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Deserializes single JSON object from text.<br/>
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="fileLocation">The JSON file to read</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeObject<T>(string fileLocation)
        {
            T deserializedJson = JsonConvert.DeserializeObject<T>(fileLocation);
            return deserializedJson;
        }

        /// <summary>
        /// Writes all objects into a file.<br/>
        /// If the file exists replaces it.<br/>
        /// </summary>
        /// <typeparam name="T">Object type to write into the file.</typeparam>
        /// <param name="dataContent">List of object to write</param>
        /// <param name="fileLocation">File to write into</param>
        /// <param name="writeIndented">whether or not to write indented JSON text.</param>
        public static void SaveToDisk<T>(List<T> dataContent, string fileLocation, bool writeIndented = true)
        {
            System.Text.Json.JsonSerializerOptions options = new() { WriteIndented = writeIndented };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < dataContent.Count; i++)
            {
                jsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(dataContent[i], options);
            }

            File.WriteAllText(fileLocation, jsonTextToWriteInTheFile);
        }

        /// <summary>
        /// Writes an objects into a file.<br/>
        /// If the file exists replaces it.<br/>
        /// </summary>
        /// <typeparam name="T">Object type to write into the file.</typeparam>
        /// <param name="dataContent">The object to write</param>
        /// <param name="fileLocation">File to write into</param>
        /// <param name="writeIndented">whether or not to write indented JSON text.</param>
        public static void SaveToDisk<T>(T dataContent, string fileLocation, bool writeIndented = true)
        {
            SaveToDisk([dataContent], fileLocation, writeIndented);
        }
    }
}
