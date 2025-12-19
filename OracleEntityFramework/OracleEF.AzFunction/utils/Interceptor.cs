using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OracleEF.AzFunction.utils
{
    public class QueryTimingInterceptor : DbCommandInterceptor
    {
        private readonly ILogger<QueryTimingInterceptor> _logger;

        public QueryTimingInterceptor(ILogger<QueryTimingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<DbDataReader> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await base.ReaderExecutingAsync(command, eventData, cancellationToken);
            stopwatch.Stop();

            _logger.LogInformation($"Query took {stopwatch.ElapsedMilliseconds} ms: {command.CommandText}");
            return result;
        }
    }

}
