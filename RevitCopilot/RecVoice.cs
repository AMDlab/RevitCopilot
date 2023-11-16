using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NAudio.Wave;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.IO;
using System.Text.Json;
using System.Windows;
using Newtonsoft.Json.Linq;
using RevitCopilot.Properties;
using System.Reflection;

namespace RevitCopilot
{
    /// <summary>
    /// ボタンに実装するコマンド
    /// </summary>
    [Transaction(TransactionMode.ReadOnly)]
    public class RecVoice : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //{               
            //  "Apikey": "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
            //}
            //
            //みたいにApikeyを書いたjsonをResoucesフォルダーに入れてください。

            var dllPath = Assembly.GetExecutingAssembly().Location;
            var dllFolder = Path.GetDirectoryName(dllPath);
            var resourcesFolder = Path.Combine(dllFolder, "Resources");
            var apikeyPath = Path.Combine(resourcesFolder, "Apikey.json");
            string json = File.ReadAllText(apikeyPath);
            JObject jsonObj = JObject.Parse(json);
            string apikey = jsonObj["Apikey"].ToString();

            // 録音デバイスを選択
            //Console.WriteLine("Available Recording Devices:");
            //for (int n = 0; n < WaveInEvent.DeviceCount; n++)
            //{
            //    var capabilities = WaveInEvent.GetCapabilities(n);
            //    Console.WriteLine($"{n}: {capabilities.ProductName}");
            //}

            //Console.WriteLine("Select a device number:");
            //var deviceNumber = int.Parse(Console.ReadLine());
            var deviceNumber = 0;

            // 録画処理を開始
            var waveIn = new WaveInEvent
            {
                DeviceNumber = deviceNumber = 0,
                WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(deviceNumber).Channels)
            };

            var wavPath = "C:\\temp\\test.wav";
            var waveWriter = new WaveFileWriter(wavPath, waveIn.WaveFormat);

            waveIn.DataAvailable += (_, ee) =>
            {
                waveWriter?.Write(ee.Buffer, 0, ee.BytesRecorded);
                waveWriter?.Flush();
            };
            waveIn.RecordingStopped += (_, __) =>
            {
                waveWriter?.Flush();
            };
            waveIn.StartRecording();
            MessageBoxResult result = MessageBox.Show("ChatGPTに話しかけましょう！", "録音中", MessageBoxButton.OKCancel, MessageBoxImage.Information);
            waveIn?.StopRecording();
            waveIn?.Dispose();
            waveIn = null;

            waveWriter?.Close();
            waveWriter = null;

            if (result == MessageBoxResult.Cancel) { return Result.Cancelled; }

            var url = "https://api.openai.com/v1/audio/transcriptions";
            var model = "whisper-1"; // 使用するモデル

            using (var httpClient = new HttpClient())
            {
                var fileStream = File.OpenRead(wavPath);

                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(new StreamContent(fileStream), "file", Path.GetFileName(wavPath));
                    formData.Add(new StringContent(model), "model");

                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                    // Whisper APIにリクエスト送信
                    var response = httpClient.PostAsync(url, formData);

                    if (response.Result.IsSuccessStatusCode)
                    {
                        // レスポンスをJSONとしてパースし、必要な情報を取得
                        var responseContent = response.Result.Content.ReadAsStringAsync();
                        var whisperResponse = JsonSerializer.Deserialize<WhisperResponse>(responseContent.Result);

                        MessageBox.Show(whisperResponse.Text, "お前が喋った内容", MessageBoxButton.OK);
                    }
                    else
                    {
                        MessageBox.Show("失敗しました", "失敗しました", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            return Result.Succeeded;
        }

        public class WhisperResponse
        {
            [JsonPropertyName("text")]
            public string Text { get; set; }
        }
    }
}