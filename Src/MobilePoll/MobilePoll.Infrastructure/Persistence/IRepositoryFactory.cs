﻿namespace MobilePoll.Infrastructure.Persistence
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
    }
}