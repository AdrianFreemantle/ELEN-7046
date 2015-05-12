﻿using System.Linq;

namespace MobilePoll.Infrastructure.Persistence
{
    public interface IRepository<TEntity> : IQueryable<TEntity> where TEntity : class
    {
        TEntity Get(int id);
        void Add(TEntity newEntity);
        void Remove(TEntity entity);
    }
}