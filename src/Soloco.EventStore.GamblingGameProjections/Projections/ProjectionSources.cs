using System;
using System.IO;

namespace Soloco.EventStore.GamblingGameProjections.Projections
{
    public static class ProjectionSources
    {
        public static string Read(string name)
        {
            var fullName = string.Format("{0}.Sources.{1}.js", typeof(ProjectionSources).Namespace, name);

            using (var stream = ReadStream(fullName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static Stream ReadStream(string fullName)
        {
            var assembly = typeof(ProjectionSources).Assembly;
            var stream = assembly.GetManifestResourceStream(fullName);
            
            if (stream == null) throw new InvalidOperationException(string.Format("Stream '{0}' not found!", fullName));

            return stream;
        }
    }
}