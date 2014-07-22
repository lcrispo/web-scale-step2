using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebJobsMessageBoard.Models
{
    public class ScaleMessage: TableEntity
    {
        public string message { get; set; }
    }
}