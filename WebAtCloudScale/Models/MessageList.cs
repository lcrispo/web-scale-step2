using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebAtScale.Models
{
    public class MessageList
    {
        public MessageList()
        {
            Messages = new List<string>();
        }
        
        public List<string> Messages { get; set; }
    }
}