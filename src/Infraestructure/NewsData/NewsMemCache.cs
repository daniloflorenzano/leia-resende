﻿using System.Linq.Expressions;
using Core.News;
using Microsoft.Extensions.Caching.Memory;

namespace Infraestructure.NewsData;

public sealed class NewsMemCache : INewsRepository
{
    // REFERENCIA: https://learn.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-8.0#use-setsize-size-and-sizelimit-to-limit-cache-size
    public MemoryCache Cache { get; } = new MemoryCache(
        new MemoryCacheOptions
        {
            SizeLimit = 1024
        });

    public Task Create(News news)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<News>> Read(Expression<Func<News, bool>>? where = null)
    {
        throw new NotImplementedException();
    }
}