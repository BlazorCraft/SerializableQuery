using BlazorCraft.SerializableQuery.ExpressionConverter;
using BlazorCraft.SerializableQuery.ExpressionConverter.Base;
using BlazorCraft.SerializableQuery.Extension;
using BlazorCraft.SerializableQuery.Result;
using Microsoft.EntityFrameworkCore;

namespace BlazorCraft.SerializableQuery;

public class BcSerializableQueryExecutor
{
    private readonly List<IBcExpressionConverter> _resolvers;

    public BcSerializableQueryExecutor()
    {
        _resolvers =
        [
            new BcBoolExpressionConverter(),
            new BcDateTimeExpressionConverter(),
            new BcDecimalExpressionConverter(),
            new BcEnumExpressionConverter(),
            new BcGuidExpressionConverter(),
            new BcIntegerExpressionConverter(),
            new BcStringExpressionConverter(),
            new BcListExpressionConverter(),
        ];
    }
        
    public BcSerializableQueryExecutor(IEnumerable<IBcExpressionConverter> resolvers)
    {
        _resolvers = resolvers.ToList();
    }
        
    public BcSerializableQueryResult<TEntity> ResolveDataSourceRequest<TEntity>(BcSerializableQuery opts, IQueryable<TEntity> query)
        => ResolveDataSourceRequest<TEntity, TEntity>(opts, query, list => list);

    public BcSerializableQueryResult<TResult> ResolveDataSourceRequest<TEntity, TResult>(BcSerializableQuery opts, IQueryable<TEntity> query, Func<List<TEntity>, List<TResult>> projector)
    {
        var filteredQuery = query
            .Where(_resolvers, opts);

        var finalQuery = filteredQuery
            .OrderBy(opts)
            .Skip((opts.Page - 1) * opts.PageSize)
            .Take(opts.PageSize);

        var list = finalQuery.ToList();
        List<TResult> projected = projector(list);

        return new BcSerializableQueryResult<TResult>()
        {
            Data = projected,
            TotalDataCount = query.Count(),
            FilteredDataCount = filteredQuery.Count()
        };
    }

    public async Task<IBcSerializableQueryResult<TEntity>> ResolveDataSourceRequestAsync<TEntity>(BcSerializableQuery opts, IQueryable<TEntity> entities)
        => await ResolveDataSourceRequestAsync<TEntity, TEntity>(opts, entities, list => list);

    public async Task<BcSerializableQueryResult<TResult>> ResolveDataSourceRequestAsync<TEntity, TResult>(BcSerializableQuery opts, IQueryable<TEntity> query,
        Func<List<TEntity>, List<TResult>> projector)
    {
        var filteredQuery = query
            .Where(_resolvers, opts);

        var finalQuery = filteredQuery
            .OrderBy(opts)
            .Skip((opts.Page - 1) * opts.PageSize)
            .Take(opts.PageSize);

        var list = await finalQuery.ToListAsync();

        return new BcSerializableQueryResult<TResult>()
        {
            Data = projector(list),
            TotalDataCount = query.Count(),
            FilteredDataCount = filteredQuery.Count()
        };
    }
}