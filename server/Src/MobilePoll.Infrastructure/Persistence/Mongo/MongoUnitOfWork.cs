using System.Collections.Generic;
using MobilePoll.Persistence;
using MongoDB.Driver;

namespace MobilePoll.Infrastructure.Persistence.Mongo
{
    public class MongoUnitOfWork : IUnitOfWork, IRepositoryFactory
    {
        private const string ConnectionString = "mongodb://localhost:27017";
        public static bool DropDatabaseOnStartup = false;
        private readonly MongoServer server;
        private readonly MongoDatabase mongoDatabase;
        private readonly List<IMongoCollectionUnitOfWork> mongoUnitsOfWork = new List<IMongoCollectionUnitOfWork>();

        static MongoUnitOfWork()
        {
            if (DropDatabaseOnStartup)
            {
                MongoClient client = new MongoClient(ConnectionString);
                MongoServer server = client.GetServer();
                var mongoDatabase = server.GetDatabase("MobilePoll");
                mongoDatabase.Drop();
            }
        }

        public MongoUnitOfWork()
        {
            MongoClient client = new MongoClient(ConnectionString);
            server = client.GetServer();
            mongoDatabase = server.GetDatabase("MobilePoll");
        }

        public void Commit()
        {
            foreach (var uow in mongoUnitsOfWork)
            {
                uow.Commit();
            }

            server.Disconnect();
        }

        public void Rollback()
        {
            server.Disconnect();
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            MongoCollection<TEntity> mongoCollection = mongoDatabase.GetCollection<TEntity>(typeof(TEntity).Name);

            var repository = new MongoRepository<TEntity>(mongoCollection);
            mongoUnitsOfWork.Add(repository);

            return repository;
        }
    }
}
