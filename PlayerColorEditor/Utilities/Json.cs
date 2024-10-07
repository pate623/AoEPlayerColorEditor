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
        private static readonly Logger Log = new(typeof(Json));

        /// <summary>
        /// Deserializes JSON objects from text.<br/>
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="jsonFile">The JSON file to read</param>
        /// <returns>The deserialized objects</returns>
        public static IEnumerable<T> DeserializeObjects<T>(FileInfo jsonFile)
        {
            Log.Trace($"Reading file {jsonFile.Name}");
            string jsonAsTest = File.ReadAllText(jsonFile.FullName);
            JsonSerializer serializer = new();
            using StringReader strReader = new(jsonAsTest);
            using JsonTextReader jsonReader = new(strReader);
            jsonReader.SupportMultipleContent = true;

            while (jsonReader.Read())
            {
                yield return serializer.Deserialize<T>(jsonReader);
            }
            Log.Trace($"File {jsonFile} read.");
        }

        /// <summary>
        /// Deserializes single JSON object from file.<br/>
        /// </summary>
        /// <typeparam name="T">The type of object to deserialize</typeparam>
        /// <param name="JsonFile">The JSON file to read</param>
        /// <returns>The deserialized object</returns>
        public static T DeserializeObject<T>(FileInfo JsonFile)
        {
            Log.Trace($"Reading file {JsonFile.Name}");
            string jsonAsTest = File.ReadAllText(JsonFile.FullName);
            T deserializedJson = JsonConvert.DeserializeObject<T>(jsonAsTest);
            Log.Trace($"File {JsonFile} read.");
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
        public static void SaveToDisk<T>(List<T> dataContent, FileInfo jsonFile, bool writeIndented = true)
        {
            Log.Trace($"Saving JSON file {jsonFile.Name}");
            System.Text.Json.JsonSerializerOptions options = new() { WriteIndented = writeIndented };

            string jsonTextToWriteInTheFile = "";

            for (int i = 0; i < dataContent.Count; i++)
            {
                jsonTextToWriteInTheFile += System.Text.Json.JsonSerializer.Serialize(dataContent[i], options);
            }

            File.WriteAllText(jsonFile.FullName, jsonTextToWriteInTheFile);
            Log.Trace($"JSON file {jsonFile.Name} saved.");
        }

        /// <summary>
        /// Writes an objects into a file.<br/>
        /// If the file exists replaces it.<br/>
        /// </summary>
        /// <typeparam name="T">Object type to write into the file.</typeparam>
        /// <param name="dataContent">The object to write</param>
        /// <param name="jsonFile">File to write into</param>
        /// <param name="writeIndented">whether or not to write indented JSON text.</param>
        public static void SaveToDisk<T>(T dataContent, FileInfo jsonFile, bool writeIndented = true)
        {
            SaveToDisk([dataContent], jsonFile, writeIndented);
        }
    }
}
