using Newtonsoft.Json.Linq;

namespace comunicaciones.email
{
    public class JSONMessageBuilder : IMessageBuilder
    {
        public const string STARTDELIMITERDOUBLEBRACE = "{{";
        public const string ENDDELIMITERDOUBLEBRACE = "}}";
        private List<string> GetTokens(string template, string startDelimiter, string endDelimiter)
        {

            if (string.IsNullOrEmpty(startDelimiter) || string.IsNullOrEmpty(startDelimiter)) throw new Exception("Non valid delimiters");
            if (string.IsNullOrEmpty(template)) return new List<string>();

            bool next = true;
            int currentPos = 0;
            List<string> tokens = new List<string>();
            while (next)
            {
                next = false;
                int start = template.IndexOf(startDelimiter, currentPos);
                if (start >= 0)
                {
                    start += startDelimiter.Length;
                    int end = template.IndexOf(endDelimiter, start);
                    if (end > 0)
                    {
                        tokens.Add(template[start..end].Trim());
                        currentPos = end;
                        next = true;
                    }
                }
            }

            return tokens;
        }

        private static string GetPropVal(string token, dynamic data)
        {
            List<string> parts = token.Split('.').ToList();

            if (parts.Count == 1)
            {
                return data[parts[0]] ?? "";
            }
            else
            {
                if (data[parts[0]] != null)
                {
                    int len = $"{parts[0]}.".Length;
                    return GetPropVal(token[len..], data[parts[0]]);
                }
                else
                {
                    return "";
                }
            }
        }

        public string FromTemplate(string template, string jsonData)
        {
            dynamic d;

            d = JObject.Parse(jsonData.Trim());

            List<string> tokens = GetTokens(template, STARTDELIMITERDOUBLEBRACE, ENDDELIMITERDOUBLEBRACE);
            foreach (string t in tokens)
            {
                template = template.Replace($"{STARTDELIMITERDOUBLEBRACE}{t}{ENDDELIMITERDOUBLEBRACE}", GetPropVal(t, d));
            }

            return template;
        }
    }

}
