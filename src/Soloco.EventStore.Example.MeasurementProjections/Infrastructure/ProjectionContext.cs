using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using EventStore.ClientAPI;

namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public class ProjectionContext
    {
        private readonly ProjectionsManager _projections;
        private readonly MeasurementConsole _console;

        private IEnumerable<Projection> _currentProjections;

        public ProjectionContext(ProjectionsManager projections, MeasurementConsole console)
        {
            if (projections == null) throw new ArgumentNullException("projections");
            if (console == null) throw new ArgumentNullException("console");

            _projections = projections;
            _console = console;            
        }

        public void Initialize()
        {
            if (_currentProjections != null) throw new InvalidOperationException("Context already initialized.");

            _currentProjections = GetCurrentProjections();

            //EnableProjection("$by_category");
            //EnableProjection("$by_event_type");
            //EnableProjection("$stream_by_category");
            //EnableProjection("$streams");

            //EnsureProjection("MeasurementRead");
            EnsureProjection("MeasurementReadCounter");
            //EnsureProjection("MeasurementReadAveragePerDay");
            //EnsureProjection("MeasurementReadRollingAveragePerWeekday");
            //EnsureProjection("MeterToDeviceType");
            //EnsureProjection("DeviceTypeRollingAveragePerWeekHour");    
        }

        private IEnumerable<Projection> GetCurrentProjections()
        {
            var all = _projections.ListAll(EventStoreCredentials.Default);
            var json = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(all);
            var projections = new List<Projection>();

            _console.Log("Current projections: ");

            foreach (var projection in json.projections)
            {
                var newProjection = new Projection(projection.name.Value, projection.status.Value);

                projections.Add(newProjection);
                _console.Log("> " + newProjection);
            }
            return projections;
        }

        private void EnableProjection(string name)
        {
            if (_currentProjections.Any(p => p.Name == name && p.Status != "Stopped")) return;

            _projections.Enable(name, EventStoreCredentials.Default);
        }

        public void EnsureProjection(string name)
        {
            var expectedQuery = ReadQueryFromEmbeddeResource(name);
            if (_currentProjections.Any(n => n.Name == name))
            {
                UpdateProjection(name, expectedQuery);
            }
            else
            {
                AddProjection(name, expectedQuery);
            }
        }

        private void AddProjection(string name, string expectedQuery)
        {
            _console.Log("Add projection: " + name);

            _projections.CreateContinuous(name, expectedQuery, EventStoreCredentials.Default);
        }

        private void UpdateProjection(string name, string expectedQuery)
        {
            _console.Log("Update existing projection: " + name);

            var currentQuery = _projections.GetQuery(name, EventStoreCredentials.Default);

            if (expectedQuery != currentQuery)
            {
                _projections.UpdateQuery(name, expectedQuery, EventStoreCredentials.Default);
            }
        }

        private static string ReadQueryFromEmbeddeResource(string name)
        {
            var fullName = string.Format("{0}.Projections.{1}.js", typeof(Program).Namespace, name);
            var assembly = typeof(ProjectionContext).Assembly;
            using (var stream = assembly.GetManifestResourceStream(fullName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}