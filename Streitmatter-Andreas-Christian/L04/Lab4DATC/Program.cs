using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace Lab4DATC
{
    class Program
    {
        private static CloudTableClient tableClient;
        private static CloudTable studentsTable;
        static void Main(string[] args)
        {
            Task.Run(async () => { await Initialize(); })
            .GetAwaiter()
            .GetResult();
        }

        static async Task Initialize()
        {
            string storageConnectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=azurestoragetema4datc"
            + ";AccountKey=XGTJJL7PgqQChx2WeNYzlWuxZqfeCRX6BNu7wwAkKJu8XG0sHcNlzmDhadiQGUBwc8J57rlBmT6ZzRa001VjMg=="
            + ";EndpointSuffix=core.windows.net";

            var account = CloudStorageAccount.Parse(storageConnectionString);
            tableClient = account.CreateCloudTableClient();

            studentsTable = tableClient.GetTableReference("studenti");

            await studentsTable.CreateIfNotExistsAsync();

            // await AddNewStudent();
            // await EditStudent();
             await GetAllStudents();

        }

        private static async Task GetAllStudents()
        {
            Console.WriteLine("Universtiate\tCNP\tNume\tEmail\tNumarTelefon\tAn");
            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token=null;
            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await studentsTable.ExecuteQuerySegmentedAsync(query , token);
                token = resultSegment.ContinuationToken;

                foreach(StudentEntity entity in resultSegment.Results)
                {
                    Console.WriteLine("{0}\t{1}\t{2} {3}\t{4}\t{5}\t{6}", entity.PartitionKey , entity.RowKey, entity.FirstName,entity.LastName, entity.Email, entity.PhoneNumber, entity.Year);
                }
            } while (token !=null);
        }
        private static async Task AddNewStudent()
        {
            var student = new StudentEntity("UPT", "1990513988743");
            
            student.FirstName = "Popa";
            student.LastName = "Gigel";
            student.Email = "popa.gigel@gmail.com";
            student.Year = 4;
            student.PhoneNumber = "0712345678";
            student.Faculty = "AC";

            var insertOperation = TableOperation.Insert(student);

            await studentsTable.ExecuteAsync(insertOperation);
        }
    }
}
