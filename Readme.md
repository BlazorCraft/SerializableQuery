# BlazorCraft Serializable Query Library

The BlazorCraft Serializable Query Library provides a flexible way to apply dynamic filtering, sorting, and paging to data collections in Blazor applications via a REST API. It translates JSON-based filter definitions into LINQ expressions, making it easy to build rich data-driven UIs.

## Key Features

* **Dynamic Filtering**: Combine multiple filter expressions (AND, OR, single filters) on DTO properties.
* **Paging & Sorting**: Built-in support for page size, page number, and multi-column sorting.
* **Data Type Support**: Includes converters for strings, booleans, enums, DateTime, decimals, and integers.
* **Sync & Async Execution**: Works with both LINQ-to-Objects and EF Core (`async`/`await`).

## Library Structure

* **Fundamentals/ExpressionConverter**: Base classes and concrete converters for each data type.
* **BcSerializableQueryExecutor**: Core class that executes queries against `IQueryable<T>` sources.
* **BcSerializableQuery & BcSerializableQueryExpression**: DTOs that describe paging, sorting, and filter criteria.
* **BcSerializableQueryResult<T>**: Wrapper containing the result list, total record count, and filtered record count.

## Prerequisites

* .NET 6.0 or later
* (Optional) Entity Framework Core for database-backed scenarios
* Blazor Server or WebAssembly

## Installation

Install via NuGet (once published):

```powershell
Install-Package BlazorCraft.SerializableQuery
```

Or add as a project reference:

```xml
<ItemGroup>
  <ProjectReference Include="..\BlazorCraft.SerializableQuery\BlazorCraft.SerializableQuery.csproj" />
</ItemGroup>
```

## Usage

### 1. Dependency Injection Registration

```csharp
builder.Services.AddScoped<BcSerializableQueryExecutor>();

// or you can initialize it on premise
BcSerializableQueryExecutor Executor = new();
```

### 2. Define a Query Payload

Example JSON to filter for entities where `Name` contains "Blazor" and `IsActive` equals `true`, sorted by `Created` descending:

```json
{
  "page": 1,
  "pageSize": 10,
  "SerializableQuery": {
    "expressionType": "AndExpression",
    "expressions": [
      {
        "expressionType": "FilterExpression",
        "propertyExpression": "Name",
        "propertyType": "System.String",
        "filterAction": "Contains",
        "propertyValue": "Blazor"
      },
      {
        "expressionType": "FilterExpression",
        "propertyExpression": "IsActive",
        "propertyType": "System.Boolean",
        "filterAction": "Equals",
        "propertyValue": "True"
      }
    ]
  },
  "sortOptions": [
    {
      "propertyName": "Created",
      "descending": true
    }
  ]
}
```

### 3. Implementing an API Endpoint

In your ASP.NET Core controller or service, inject `BcSerializableQueryExecutor` and your `DbContext`:

```csharp
[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly BcSerializableQueryExecutor _executor;
    private readonly DbContext _db;

    public ArticlesController(BcSerializableQueryExecutor executor, DbContext db)
    {
        _executor = executor;
        _db = db;
    }

    [HttpPost("query")]
    public async Task<BcSerializableQueryResult<ArticleDto>> Query([FromBody] BcSerializableQuery query)
    {
        var baseQuery = _db.Set<Article>().AsQueryable();
        return await _executor.ResolveDataSourceRequestAsync<Article, ArticleDto>(
            query,
            baseQuery,
            list => list.Select(e => new ArticleDto { /* map fields */ }).ToList()
        );
    }
}
```

### 4. Calling from Blazor Client

Inject `HttpClient` and post your query payload:

```csharp
@inject HttpClient Http

private List<ArticleDto> Articles;
private int TotalCount, FilteredCount;

private async Task LoadDataAsync()
{
    var query = new BcSerializableQuery
    {
        Page = 1,
        PageSize = 20,
        // set SerializableQuery and SortOptions...
    };

    var response = await Http.PostAsJsonAsync("api/articles/query", query);
    var result = await response.Content.ReadFromJsonAsync<BcSerializableQueryResult<ArticleDto>>();

    Articles = result.Data;
    TotalCount = result.TotalDataCount;
    FilteredCount = result.FilteredDataCount;
}
```

## Extending the Library

* **Custom Converters**: Inherit from `BcExpressionConverterBase<T>` to add support for new data types or custom logic.
* **Advanced Projection**: Use the generic overload `ResolveDataSourceRequest<TEntity, TResult>(..., projector)` to project entities into custom DTOs.

## Unit Tests

A suite of xUnit tests validates filtering, sorting, and paging behavior. See the `UnitTests` folder for examples of `FilterExpression`, `AndExpression`, `OrExpression`, and more.

## License

This project is licensed under the [MIT](LICENSE).

## Support me

If you want to support me, buy a [Coffee](https://ko-fi.com/leeroy_manea)

Thank you