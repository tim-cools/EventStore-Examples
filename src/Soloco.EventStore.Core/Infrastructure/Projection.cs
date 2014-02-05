namespace Soloco.EventStore.Core.Infrastructure
{
    public class Projection
    {
        public string Name { get; private set; }
        public string Status { get; private set; }

        public Projection(string name, string status)
        {
            Name = name;
            Status = status;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}, Status: {1}", Name, Status);
        }
    }
}