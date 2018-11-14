namespace ApplicationCore.Interfaces
{
    public interface IUniqueIDGenerator
    {
        long GetNextIdForUser();
        long GetNextIdForMessage();
    }
}
