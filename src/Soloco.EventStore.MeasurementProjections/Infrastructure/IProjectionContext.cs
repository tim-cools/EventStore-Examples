namespace Soloco.EventStore.Test.MeasurementProjections.Infrastructure
{
    public interface IProjectionContext
    {
        void EnableProjection(string name);
        
        void EnsureProjection(string name, string source);

        T GetState<T>(string projectionName);
    }
}