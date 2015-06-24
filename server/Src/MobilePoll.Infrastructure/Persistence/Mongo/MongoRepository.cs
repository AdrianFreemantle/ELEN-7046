using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MobilePoll.Persistence;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace MobilePoll.Infrastructure.Persistence.Mongo
{
    public class MongoRepository<TEntity> : IMongoCollectionUnitOfWork, IRepository<TEntity> where TEntity : class
    {
        private readonly List<TEntity> updatedEntities = new List<TEntity>();        
        private readonly MongoCollection<TEntity> mongoCollection;

        public Expression Expression { get { return mongoCollection.AsQueryable().Expression; } }
        public Type ElementType { get { return mongoCollection.AsQueryable().ElementType; } }
        public IQueryProvider Provider { get { return mongoCollection.AsQueryable().Provider; } }

        public MongoRepository(MongoCollection<TEntity> mongoCollection)
        {
            this.mongoCollection = mongoCollection;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            return mongoCollection.AsQueryable().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public TEntity Get(Guid id)
        {
            return mongoCollection.FindOneById(id);
        }

        public TEntity Get(int id)
        {
            return mongoCollection.FindOneById(id);
        }

        public TEntity Add(TEntity newEntity)
        {
            updatedEntities.Add(newEntity);
            return newEntity;
        }

        public TEntity Update(TEntity entity)
        {
            updatedEntities.Add(entity);
            return entity;
        }

        void IMongoCollectionUnitOfWork.Commit()
        {
            foreach (var updatedEntity in updatedEntities)
            {
                mongoCollection.Save(updatedEntity);
            }
        }

        public void Remove(TEntity entity)
        {
            throw new NotImplementedException();
        }
    }
}