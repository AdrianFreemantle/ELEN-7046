namespace MobilePoll.Infrastructure.Persistence.Mongo
{
    internal interface IMongoCollectionUnitOfWork
    {
        void Commit();
    }
}