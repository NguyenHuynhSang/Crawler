using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{


    public class CUrl
    {
        public Dictionary<string, string> Header { set; get; }
        public string FormContent { set; get; }
        public string BaseURL { set; get; }

        public CUrl()
        {
            Header = new Dictionary<string, string>();
        }


        public void ReadFile(string path)
        {
          
            string[] lines = System.IO.File.ReadAllLines(path);
            object obj;
            foreach (var item in lines)
            {
                if (item.TrimStart().StartsWith("-H"))
                {
                    var content = item.Replace("-H '", "").Replace("' \\", "").TrimStart();


                    string header_name = GetStringBetweenCharacters(content, null, ':');
                    string header_value = GetStringBetweenCharacters(content, ' ', null);
                    this.Header.Add(header_name, header_value);

                }
                else if (item.TrimStart().Contains("curl"))
                {
                    var content = item.Replace("curl '", "").Replace("' \\", "").TrimStart();

                    this.BaseURL = content;

                }
                else if (item.TrimStart().Contains("--data-raw"))
                {
                    var content = item.Replace("--data-raw '", "").Replace("' \\", "").TrimStart();
                    this.FormContent = content;
                }

            }

          



        }
        private string GetStringBetweenCharacters(string input, char? charFrom, char? charTo)
        {
            int posFrom = 0;
            if (charFrom != null)
            {
                posFrom = input.IndexOf(charFrom.Value);

            }


            if (posFrom != -1) //if found char
            {
                int posTo = charTo != null ? input.IndexOf(charTo.Value, posFrom + 1) : input.Length;
                if (posTo != -1) //if found char
                {
                    return charFrom != null ? input.Substring(posFrom + 1, posTo - posFrom - 1) : input.Substring(posFrom, posTo - posFrom);
                }
            }

            return string.Empty;
        }


    }

}
