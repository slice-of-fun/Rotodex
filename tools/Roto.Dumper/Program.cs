using System;
using System.IO;
using System.Text.Json;
using Roto.Core;

namespace Roto.Dumper
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Initializing Roto.Dumper...");
            
            // The goal is to dump files to the 'resources' folder at the root of the repo.
            // Since this runs in tools/Roto.Dumper/bin/Debug/net8.0, we traverse up.
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var resourceDir = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", "..", "..", "resources"));
            
            if (!Directory.Exists(resourceDir))
            {
                Directory.CreateDirectory(resourceDir);
            }

            Console.WriteLine($"Output Directory: {resourceDir}");

            // Execute Extractors
            EncounterExtractor.Extract(resourceDir);
            LearnsetExtractor.Extract(resourceDir);
            PersonalDataExtractor.Extract(resourceDir);

            Console.WriteLine("Resource extraction complete!");
        }
    }
}
