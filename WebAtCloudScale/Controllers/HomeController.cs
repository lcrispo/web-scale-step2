using WebAtScale.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;
using System.Web.Configuration;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;


namespace WebAtScale.Controllers
{

    public class MessageEntity : TableEntity
    {
        public MessageEntity(string guid1, string guid2)
        {
            this.PartitionKey = guid1;
            this.RowKey = guid2;
        }

        public MessageEntity() { }

        public string Message { get; set; }
    }
    public class HomeController : Controller
    {
        private readonly DbConnectionContext db = new DbConnectionContext();
        CloudStorageAccount account = new CloudStorageAccount(new StorageCredentials(WebConfigurationManager.AppSettings["StorageAccountName"], WebConfigurationManager.AppSettings["StorageAccount"]), false);
        CloudStorageAccount account2 = new CloudStorageAccount(new StorageCredentials(WebConfigurationManager.AppSettings["StorageAccountName2"], WebConfigurationManager.AppSettings["StorageAccount2"]), false);
        



        public ActionResult Index()
        {
            CloudBlobClient client = account.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference("webatscale");
            container.CreateIfNotExists();
            container.SetPermissions( new BlobContainerPermissions {PublicAccess = BlobContainerPublicAccessType.Blob});

            var imagesModel = new ImageGallery();

            if (WebConfigurationManager.AppSettings["BlobStore"] == "local")
            {
                var imageFiles = Directory.GetFiles(Server.MapPath("~/Upload_Files/"));
                foreach (var item in imageFiles)
                {
                    imagesModel.ImageList.Add("~/Upload_Files/" + Path.GetFileName(item));
                }
            
            }
            else 
            {
                var blobs = container.ListBlobs();
                foreach (var item in blobs)
                 {
                     imagesModel.ImageList.Add(item.Uri.OriginalString);
                 }
            }
            return View(imagesModel);
        }

        [HttpGet]
        public ActionResult UploadImage()
        {
            return View();
        }

        
        public ActionResult Messages()
        {

            // Create the table client.
            CloudTableClient tableClient = account2.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("MessageBoard");
            table.CreateIfNotExists();

            List<MessageEntity> allEntities = new List<MessageEntity>();
            var MessagesModel = new MessageList();
            TableQuery<MessageEntity> query = new TableQuery<MessageEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "GeneralMessages"));

            // Print the fields for each customer.
            foreach (MessageEntity entity in table.ExecuteQuery(query))
            {
                MessagesModel.Messages.Add(entity.Message);
            }

            return View(MessagesModel);
        }


        [HttpGet]
        public ActionResult SubmitMessage()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImageMethod()
        {
            if (Request.Files.Count != 0)
            {
                for (int i = 0; i < Request.Files.Count; i++)
                {
                    HttpPostedFileBase file = Request.Files[i];
                    int fileSize = file.ContentLength;
                    string fileName = Request.Files.AllKeys[i];
                    if (WebConfigurationManager.AppSettings["BlobStore"] == "local")
                    {
                        file.SaveAs(Server.MapPath("~/Upload_Files/" + fileName));
                    
                    }
                    else
                    {
                        CloudBlobClient client = account.CreateCloudBlobClient();
                        CloudBlobContainer container = client.GetContainerReference("webatscale");
                        var contentType = "application/octet-stream";
                        switch (Path.GetExtension(fileName))
                        {
                            case "png": contentType = "image/png"; break;
                            case "jpg": contentType = "image/png"; break;
                        }

                        var blob = container.GetBlockBlobReference(fileName);
                        blob.Properties.ContentType = contentType;
                        blob.Properties.CacheControl = "public, max-age=3600";
                        blob.UploadFromStream(file.InputStream);
                        blob.SetProperties();


                    }
                }
                return Content("Success");
            }
            return Content("failed");
        }
    

           [HttpPost]
        public ActionResult SubmitMessageMethod()
        {

            CloudTableClient tableClient = account2.CreateCloudTableClient();

            // Create the table if it doesn't exist.
            CloudTable table = tableClient.GetTableReference("MessageBoard");

            MessageEntity message = new MessageEntity("GeneralMessages", WebConfigurationManager.AppSettings["MessageSource"]);
            message.Message = Request.Form["Message"];  

            // Create the TableOperation that inserts the customer entity.
            TableOperation insertOperation = TableOperation.Insert(message);

            // Execute the insert operation.
            table.Execute(insertOperation);
            return Content("Success");
        }
      }
    }


