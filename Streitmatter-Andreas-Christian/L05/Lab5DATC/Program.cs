using System.Reflection.Emit;
using System.Data;
using System.IO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Models;

namespace L05
{
    class Program
    {
        private static CloudTable _studentsTable;
        private static CloudTable _raportTable;

        static async Task Main(string[] args)
        {
            await InitializeRaportTable();
            OpenStudentTable();

            List<StudentEntity> students = await GetAllStudents();

            var mapStud = new Dictionary<string, int>();
            int cntGeneral = 0;
            foreach(StudentEntity s in students)
            {
                if(mapStud.ContainsKey(s.PartitionKey))
                    mapStud[s.PartitionKey]++;
                else
                    mapStud[s.PartitionKey] = 1;
                
                cntGeneral++;
            }

            Console.Write(DateTime.Now.ToString("HH:mm:ss") + ": ");
            foreach(KeyValuePair<string, int> s in mapStud)
            {
                RaportEntity raportEntity = new RaportEntity(s.Key, s.Value);
                await CreateRaport(raportEntity);
                Console.Write(s.Key + "->" + s.Value + ";  ");
            }
            RaportEntity raportEntity2 = new RaportEntity("General", cntGeneral);
            await CreateRaport(raportEntity2);
            Console.WriteLine("General->" + cntGeneral.ToString());
        }


        public static async Task<List<StudentEntity>> GetAllStudents()
        {
            var students = new List<StudentEntity>();

            TableQuery<StudentEntity> query = new TableQuery<StudentEntity>();

            TableContinuationToken token = null;

            do
            {
                TableQuerySegment<StudentEntity> resultSegment = await _studentsTable.ExecuteQuerySegmentedAsync(query, token);
                token = resultSegment.ContinuationToken;

                students.AddRange(resultSegment.Results);
            }while (token != null);

            return students;
        }

        public static async Task CreateRaport(RaportEntity raport)
        {
            var insertOperation = TableOperation.Insert(raport);

            await _raportTable.ExecuteAsync(insertOperation);
        }
        private static void OpenStudentTable()
        {
            string _connectionString = "DefaultEndpointsProtocol=https;"
            + "AccountName=azurestoragetema4datc;"
            + "AccountKey=XGTJJL7PgqQChx2WeNYzlWuxZqfeCRX6BNu7wwAkKJu8XG0sHcNlzmDhadiQGUBwc8J57rlBmT6ZzRa001VjMg==;"
            + "EndpointSuffix=core.windows.net";

            _studentsTable = CloudStorageAccount.Parse(_connectionString).CreateCloudTableClient().GetTableReference("studenti");
        }
        private static async Task InitializeRaportTable()
        {
            string _connectionString = "DefaultEndpointsProtocol=https;AccountName=azurestoragetema4datc;AccountKey=XGTJJL7PgqQChx2WeNYzlWuxZqfeCRX6BNu7wwAkKJu8XG0sHcNlzmDhadiQGUBwc8J57rlBmT6ZzRa001VjMg==;EndpointSuffix=core.windows.net";
            
            _raportTable = CloudStorageAccount.Parse(_connectionString).CreateCloudTableClient().GetTableReference("rapoarte");

            await _raportTable.CreateIfNotExistsAsync();
        }
    }
}