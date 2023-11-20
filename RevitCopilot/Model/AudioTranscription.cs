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

public class AudioTranscription : OpenAIAPIModel
{
    private WaveInEvent waveIn;
    private WaveFileWriter waveWriter;
    private readonly string wavPath = "C:\\temp\\test.wav";

    public void StartRecording()
    {
        int deviceNumber = 0;
        waveIn = new WaveInEvent
        {
            DeviceNumber = deviceNumber,
            WaveFormat = new WaveFormat(44100, WaveIn.GetCapabilities(deviceNumber).Channels)
        };

        waveWriter = new WaveFileWriter(wavPath, waveIn.WaveFormat);
        waveIn.DataAvailable += (_, e) => waveWriter?.Write(e.Buffer, 0, e.BytesRecorded);
        waveIn.RecordingStopped += (_, __) => waveWriter?.Dispose();
        waveIn.StartRecording();
        MessageBoxResult result = MessageBox.Show("ChatGPTに話しかけましょう！", "録音中", MessageBoxButton.OKCancel, MessageBoxImage.Information);
        waveIn?.StopRecording();
        waveIn?.Dispose();
        waveIn = null;

        waveWriter?.Close();
        waveWriter = null;
    }

    public async Task<string> TranscribeAudioAsync()
    {
        var url = "https://api.openai.com/v1/audio/transcriptions";
        var model = "whisper-1";
        var language = "ja"; 

        using (var httpClient = new HttpClient())
        {
            var fileStream = File.OpenRead(wavPath);

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StreamContent(fileStream), "file", Path.GetFileName(wavPath));
                formData.Add(new StringContent(model), "model");
                formData.Add(new StringContent(language), "language");

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("multipart/form-data"));

                var response = await httpClient.PostAsync(url, formData);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var whisperResponse = JsonSerializer.Deserialize<WhisperResponse>(responseContent);
                    return whisperResponse?.Text;
                }
            }
        }

        return null;
    }

    public class WhisperResponse
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }
}
