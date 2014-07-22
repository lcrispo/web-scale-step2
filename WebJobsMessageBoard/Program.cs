using Microsoft.Azure.Jobs;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebJobsMessageBoard.Models;

namespace WebJobsMessageBoard
{
    // To learn more about Windows Azure WebJobs start here http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connectionstring values in app.config
        // AzureJobsRuntime and AzureJobsData
        static void Main()
        {
            JobHost host = new JobHost();
            host.RunAndBlock();
        }

        public static void ProcessQueueMessage([QueueTrigger("incomingmessages")] string messagetext, [Table("MessageBoard")] CloudTable messagestable)
        {
            messagestable.Execute(TableOperation.InsertOrReplace( new ScaleMessage 
            {
                message = messagetext,
                RowKey = messagetext,
                PartitionKey = "default",
                Timestamp = DateTime.Now
            }));
            //writer.WriteLine(inputText);
        }
    }
}
