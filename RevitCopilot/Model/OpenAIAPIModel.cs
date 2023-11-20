using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NAudio.Wave;
using System.Reflection;
using System.Text.Json.Serialization;
using Autodesk.Revit.UI;
using System.Windows;

public abstract class OpenAIAPIModel
{
    internal string apikey;
    internal OpenAIAPIModel()
    {
        LoadApikey();
    }
    private void LoadApikey()
    {
        var dllPath = Assembly.GetExecutingAssembly().Location;
        var dllFolder = Path.GetDirectoryName(dllPath);
        var resourcesFolder = Path.Combine(dllFolder, "Resources");
        var apikeyPath = Path.Combine(resourcesFolder, "Apikey.json");
        string json = File.ReadAllText(apikeyPath);
        JObject jsonObj = JObject.Parse(json);
        apikey = jsonObj["Apikey"].ToString();
    }
}
