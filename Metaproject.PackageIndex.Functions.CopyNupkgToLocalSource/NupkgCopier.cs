using System.IO;
using System.Linq;
using static System.Environment;
using static System.IO.Directory;
using static System.IO.File;
using static System.IO.Path;
using static System.String;

namespace Metaproject.PackageIndex.Functions.CopyNupkgToLocalSource
{
    public static class NupkgCopier
    {
        private const char dot = '.';
        private const string EnvironmentVariableName = "LOCAL_NUGET_SOURCE_PATH";
        private const string Nupkg = "*.nupkg";

        public static void CopyNupkg(string nupkgDirectory, string localNuGetSourcePath = null)
        {
            if (IsNullOrEmpty(localNuGetSourcePath))
                localNuGetSourcePath = GetEnvironmentVariable(EnvironmentVariableName);
            CreateDirectory(localNuGetSourcePath);
            var files = EnumerateFiles(nupkgDirectory, Nupkg, SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var name = new FileInfo(file).Name;
                DeletePreviousVersions(localNuGetSourcePath, name);
                Copy(file, Combine(localNuGetSourcePath, name));
            }
        }

        private static void DeletePreviousVersions(string directory, string name)
        {
            var parts = name.Split(dot);
            var partsLength = parts.Length;
            var nameParts = parts.Take(partsLength - 4);
            foreach (var file in EnumerateFiles(directory,
                $"{Join(dot.ToString(), nameParts)}{dot}{Nupkg}"))
            {
                if (file.Split(dot).Length != partsLength) continue;
                File.Delete(file);
            }
        }
    }
}
