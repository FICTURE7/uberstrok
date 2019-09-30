using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace UberStrok.WebServices.AspNetCore
{
    public static class NameHelpers
    {
        private static readonly Dictionary<string, Regex> _nameRegex;

        static NameHelpers()
        {
            _nameRegex = new Dictionary<string, Regex>
            {
                { "en-US", new Regex("^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?]{3,18}$", RegexOptions.Compiled) },
                { "ko-KR", new Regex("^[a-zA-Z0-9 .!_\\-<>{}~@#$%^&*()=+|:?\\p{IsHangulSyllables}]{3,18}$", RegexOptions.Compiled) }
            };
        }

        public static bool IsNameValid(string name, string locale)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;
            if (!_nameRegex.TryGetValue(locale, out Regex regex))
                // Fallback to en-US in case we don't handle the specified locale.
                regex = _nameRegex["en-US"];
            return regex.IsMatch(name);
        }
    }
}
