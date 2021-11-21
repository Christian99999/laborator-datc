using System;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Company.Function
{
    public class CreateStudent
    {
        private readonly ILogger _logger;

        public CreateStudent(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CreateStudent>();
        }

        [Function("CreateStudent")]
        public void Run([QueueTrigger("myqueue-items", Connection = "azurestoragetema4datc_STORAGE")] string myQueueItem)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
