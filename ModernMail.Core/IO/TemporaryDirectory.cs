using System;
using System.IO;

namespace ModernMail.Core.IO
{
    public class TemporaryDirectory
    {
        public TemporaryDirectory(string fullPath = null)
        {
            FullPath = fullPath ?? GenerateTempPath();
            Directory.CreateDirectory(FullPath);
        }

        public string FullPath { get; private set; }

        public string[] Files
        {
            get
            {
                return Directory.GetFiles(FullPath, "*", SearchOption.AllDirectories);
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(FullPath))
                Directory.Delete(FullPath, true);
        }

        private static string GenerateTempPath()
        {
            return Path.GetTempPath() + Guid.NewGuid().ToString();
        }
    }
}
