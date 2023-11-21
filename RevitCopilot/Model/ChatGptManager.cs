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
using System.Text;
using System.Collections.Generic;

namespace RevitCopilot.Model
{
    public class RevitCopilotManager : OpenAIAPIModel
    {
        public async Task<string> GetCsMethodByChatgpt(string content)
        {
            var response = await InqueryChatgpt(content);
            var csMethod = GetCsMethodFromResponse(response);
            return csMethod;
        }

        public async Task<string> InqueryChatgpt(string content)
        {
            var url = "https://api.openai.com/v1/chat/completions";
            var model = "gpt-4-1106-preview";
            var messages = new[]
            {
                new { role = "system", content = "あなたはRevitアドインの開発者です。" +
                "ユーザーの問い合わせに返答するためのC#クラスとメソッドを作成してください。" +
                "以下のルールを必ず守ってください。" +
                "・クラスは1つのみ作成し、クラス名は[MyClass]とする" +
                "・クラスのコンストラクタは記述しない" +
                "・メソッドは、クラスメソッドとして1つのみ作成し、メソッド名は[MyMethod]とする" +
                "・メソッドの引数は(Autodesk.Revit.DB.Document document)とする" +
                "・メソッドはtry-catchで例外処理を行い、例外が発生した場合はメッセージとして表示する" +
                "・メッセージを表示する際はTaskDialogを利用する" +
                "・static修飾子は使用しない"
                },
                new { role = "user", content }
            };

            using (var httpClient = new HttpClient())
            {
                var requestData = new
                {
                    model,
                    messages
                };

                var jsonContent = JsonSerializer.Serialize(requestData);
                using (var stringContent = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apikey);

                    var response = await httpClient.PostAsync(url, stringContent);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        var gptResponse = JsonSerializer.Deserialize<Gpt4Response>(responseContent);
                        return gptResponse?.Choices[0]?.Message?.Content;
                    }
                }
            }

            return null;
        }
        private string GetCsMethodFromResponse(string response)
        {
            if (string.IsNullOrEmpty(response)) return null;
            var rowList = response.Split('\n');
            bool isTarget = false;
            int staCounter = 0, endCounter = 0;
            var res = string.Empty;
            foreach (var row in rowList)
            {
                if (row.StartsWith("```"))
                {
                    isTarget = !isTarget;
                    continue;
                }
                if (isTarget)
                {
                    if (row.Contains("{")) staCounter += CharCount(row, '{');
                    if (row.Contains("}")) endCounter += CharCount(row, '}');
                    res += row + "\n";
                    if (staCounter > 0 && staCounter == endCounter)
                    {
                        // コードブロックが終わったら終了
                        break;
                    }
                }
            }
            return res;
        }
        private int CharCount(string text, char target)
        {
            int count = 0;
            foreach (char c in text)
            {
                if (c == target)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public class Gpt4Response
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonPropertyName("system_fingerprint")]
        public string SystemFingerprint { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("index")]
        public int Index { get; set; }

        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

}