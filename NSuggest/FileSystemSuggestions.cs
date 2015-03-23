using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using FILETIME = System.Runtime.InteropServices.ComTypes.FILETIME;

namespace NSuggest
{
    public sealed class FileSystemSuggestions : IProvideSuggestions, ISuggestSuffix
    {
        public bool IncludeFiles { get; set; }

        public IEnumerable<string> For(string prefix)
        {
            if (prefix.Length < 2 || prefix[1] != ':')
                yield break;

            var lastSlashPos = prefix.LastIndexOf('\\');
            if (lastSlashPos == -1)
                yield break;

            var fileNamePatternLength = prefix.Length - lastSlashPos - 1;
            var baseFolder = prefix.Substring(0, lastSlashPos + 1);
            Win32FindData fd;
            var hFind = FindFirstFile(prefix + "*", out fd);
            if (hFind.ToInt32() == InvalidHandleValue)
                yield break;

            do
            {
                if ((fd.cFileName[0] == '.')
                    || ((fd.dwFileAttributes & FileAttributes.Hidden) != 0)
                    || ((fd.dwFileAttributes & FileAttributes.Directory) == 0 && !IncludeFiles)
                    || (fileNamePatternLength > fd.cFileName.Length))
                {
                    continue;
                }

                yield return baseFolder + fd.cFileName;
            } while (FindNextFile(hFind, out fd));

            FindClose(hFind);
        }

        public string For(string prefix, string firstMatch)
        {
            return prefix.EndsWith("\\") ? null : firstMatch.Substring(prefix.Length);
        }

        #region Native Methods

        private const short InvalidHandleValue = -1;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindFirstFile(string lpFileName, out Win32FindData lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FindNextFile(IntPtr hFindFile, out Win32FindData lpFindFileData);

        [DllImport("kernel32.dll")]
        public static extern bool FindClose(IntPtr hFindFile);

        // ReSharper disable FieldCanBeMadeReadOnly.Local
        // ReSharper disable MemberCanBePrivate.Local
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct Win32FindData
        {
            public FileAttributes dwFileAttributes;
            public FILETIME ftCreationTime;
            public FILETIME ftLastAccessTime;
            public FILETIME ftLastWriteTime;
            public uint nFileSizeHigh;
            public uint nFileSizeLow;
            public uint dwReserved0;
            public uint dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)] public string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)] public string cAlternateFileName;
        }
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local

        #endregion
    }
}