using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Massena.Infrastructure.Core.Infrastructure.Helpers
{
    public static class StringHelper
    {
        public static string RemoveAccent(this string value)
        {
            return new string(value
               .Normalize(NormalizationForm.FormD)
               .Where(ch => char.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
               .ToArray());
        }

        public static IEnumerable<string> RegexKeyWords(string text)
        {
            Match match = null;

            Regex reg = new Regex(@"\<<.*?\>>");
            match = reg.Match(text);

            if (match.Success)
            {
                var valueList = new List<string>();
                for(var index = 0; match.Groups.Count < index; index++)
                {
                    valueList.Add(match.Groups[index].Value.Replace("<<", "").Replace(">>", ""));
                }

                return valueList;
            }

            return null;    
        }
    }
}
