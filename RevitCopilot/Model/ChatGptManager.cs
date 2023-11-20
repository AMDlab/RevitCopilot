using ChatGPT.API.Framework;

namespace RevitCopilot.Model
{
    public class RevitCopilotManager : OpenAIAPIModel
    {
        public string GetCsMethodByChatgpt(string content)
        {
            var response = InqueryChatgpt(content);
            var csMethod = GetCsMethodFromResponse(response);
            return csMethod;
        }
        private string InqueryChatgpt(string content)
        {
            ChatGPTClient cgc = new ChatGPTClient(apikey);
            cgc.CreateCompletions("hoge", 
                "あなたはRevitアドインの開発者です。" +
                "ユーザーの問い合わせに返答するためのC#クラスとメソッドを作成してください。" +
                "以下のルールを必ず守ってください。" +
                "・クラスは1つのみ作成する" +
                "・メソッドは、クラスメソッドとして1つのみ作成する" +
                "・メソッドの引数は(Autodesk.Revit.DB.Document document)とする" +
                "・static修飾子は使用しない"
                );
            var res = cgc.Ask("hoge", content);
            if(res != null) { return res.GetMessageContent(); }
            return null;
        }
        private string GetCsMethodFromResponse(string response)
        {
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
}