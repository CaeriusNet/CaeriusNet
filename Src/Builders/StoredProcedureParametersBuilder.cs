﻿using static CaeriusNet.Utilities.CacheType;

namespace CaeriusNet.Builders;

/// <summary>
///     Builds the parameters for a stored procedure call, including support for Table-Valued Parameters (TVPs).
/// </summary>
public sealed record StoredProcedureParametersBuilder(string ProcedureName, int Capacity = 1)
{
    private TimeSpan? _cacheExpiration;
    private string? _cacheKey;
    private CacheType? _cacheType;

    /// <summary>
    ///     Gets the list of TSQL parameters to be used in the stored procedure call.
    /// </summary>
    private List<SqlParameter> Parameters { get; } = [];

    /// <summary>
    ///     Adds a parameter to the stored procedure call.
    /// </summary>
    /// <param name="parameter">The name of the parameter.</param>
    /// <param name="value">The value of the parameter.</param>
    /// <param name="dbType">The TSQL data type of the parameter. Use <see cref="SqlDbType" /> enumeration.</param>
    /// <returns>The <see cref="StoredProcedureParametersBuilder" /> instance for chaining.</returns>
    public StoredProcedureParametersBuilder AddParameter(string parameter, object value, SqlDbType dbType)
    {
        var itemParameter = new SqlParameter(parameter, dbType) { Value = value };
        Parameters.Add(itemParameter);
        return this;
    }

    /// <summary>
    ///     Adds a Table-Valued Parameter (TVP) to the stored procedure call.
    /// </summary>
    /// <typeparam name="T">The type of the object that maps to the TVP.</typeparam>
    /// <param name="parameterName">The name of the TVP parameter.</param>
    /// <param name="tvpName">The name of the TVP type in SQL Server.</param>
    /// <param name="items">The collection of items to map to the TVP using the <see cref="ITvpMapper{T}" /> interface.</param>
    /// <returns>The <see cref="StoredProcedureParametersBuilder" /> instance for chaining.</returns>
    /// <exception cref="ArgumentException">Thrown when the items collection is empty.</exception>
    public StoredProcedureParametersBuilder AddTvpParameter<T>(string parameterName, string tvpName,
        IEnumerable<T> items)
        where T : class, ITvpMapper<T>
    {
        var tvpMappers = items.ToList();
        if (tvpMappers.Count == 0)
            throw new ArgumentException("No items found in the collection to map to a Table-Valued Parameter.");
        var dataTable = tvpMappers[0].MapAsDataTable(tvpMappers);
        var parameter = new SqlParameter(parameterName, SqlDbType.Structured)
        {
            TypeName = tvpName,
            Value = dataTable
        };

        Parameters.Add(parameter);
        return this;
    }

    /// <summary>
    ///     Adds caching support to the stored procedure call.
    /// </summary>
    /// <param name="cacheKey">The unique key for the cache.</param>
    /// <param name="expiration">Optional expiration time for the cache. Defaults to null for no expiration.</param>
    /// <param name="cacheType">The type of cache strategy to use. Defaults to <see cref="CacheType.InMemory" />.</param>
    /// <returns>The <see cref="StoredProcedureParametersBuilder" /> instance for chaining.</returns>
    public StoredProcedureParametersBuilder AddCache(string cacheKey, TimeSpan? expiration = null,
        CacheType cacheType = InMemory)
    {
        _cacheType = cacheType;
        _cacheKey = cacheKey;
        _cacheExpiration = expiration;
        return this;
    }

    /// <summary>
    ///     Configures the stored procedure parameters to use an in-memory cache with a specified key and expiration time.
    /// </summary>
    /// <param name="cacheKey">The unique key used to identify the cache entry.</param>
    /// <param name="expiration">The duration after which the cache will expire.</param>
    /// <returns>The <see cref="StoredProcedureParametersBuilder" /> instance for chaining.</returns>
    public StoredProcedureParametersBuilder AddInMemoryCache(string cacheKey, TimeSpan expiration)
    {
        _cacheKey = cacheKey;
        _cacheType = InMemory;
        _cacheExpiration = expiration;
        return this;
    }

    /// <summary>
    ///     Adds a frozen cache to the stored procedure call.
    /// </summary>
    /// <param name="cacheKey">The unique key used to identify the frozen cache.</param>
    /// <returns>The <see cref="StoredProcedureParametersBuilder" /> instance for chaining.</returns>
    public StoredProcedureParametersBuilder AddFrozenCache(string cacheKey)
    {
        _cacheKey = cacheKey;
        _cacheType = Frozen;
        _cacheExpiration = null;
        return this;
    }

    /// <summary>
    ///     Builds and returns a StoredProcedureParameters object containing all configured parameters.
    /// </summary>
    /// <returns>
    ///     A <see cref="StoredProcedureParameters" /> instance containing the stored procedure name, capacity,
    ///     parameters, and optional caching settings.
    /// </returns>
    public StoredProcedureParameters Build()
    {
        return new StoredProcedureParameters(ProcedureName, Capacity, Parameters, _cacheKey, _cacheExpiration,
            _cacheType);
    }
}