using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace NSuggest
{
    public sealed class RegistrySuggestions : IProvideSuggestions
    {
        private readonly List<RegistryKey> _rootKeys = new List<RegistryKey>
        {
            Registry.ClassesRoot,
            Registry.CurrentUser,
            Registry.LocalMachine,
            Registry.Users,
            Registry.CurrentConfig
        };

        public IEnumerable<string> For(string prefix)
        {
            var result = new List<string>();
            var parts = prefix.Split('\\');
            var rootStr = parts[0];

            if (parts.Length == 1)
            {
                result.AddRange(from key in _rootKeys
                                where key.Name.StartsWith(rootStr, StringComparison.OrdinalIgnoreCase)
                                select key.Name);
                return result;
            }
            var rootKey = _rootKeys.FirstOrDefault(key => key.Name.Equals(rootStr, StringComparison.OrdinalIgnoreCase));
            if (rootKey == null)
                return null;

            var subKey = rootKey;
            var sb = new StringBuilder();
            for (int n = 1; n < parts.Length - 1; n++)
                sb.Append(parts[n]).Append('\\');

            var middlePath = sb.ToString();
            if (middlePath.Length != 0)
            {
                try
                {
                    subKey = subKey.OpenSubKey(middlePath);
                }
                catch
                {
                    // SecurityException may be thrown
                    return result;
                }
            }

            var lastStr = parts[parts.Length - 1];
            // ReSharper disable InvocationIsSkipped
            // ReSharper disable PossibleNullReferenceException
            Debug.Assert(subKey != null, "subKey != null");
            var subKeyNames = subKey.GetSubKeyNames();
            // ReSharper restore InvocationIsSkipped
            // ReSharper restore PossibleNullReferenceException

            foreach (var subKeyName in 
                subKeyNames.Where(subKeyName => subKeyName.StartsWith(lastStr, StringComparison.OrdinalIgnoreCase)))
            {
                sb.Length = 0;
                sb.Append(rootKey.Name).Append('\\').Append(middlePath).Append(subKeyName);
                result.Add(sb.ToString());
            }
            subKey.Close();

            return result;
        }
    }
}