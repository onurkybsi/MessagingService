using System;
using System.Collections.Generic;
using MessagingService.Infrastructure;
using Newtonsoft.Json;

namespace MessagingService.Model
{
    public class User : MongoDBEntity
    {
        public string Username { get; set; }
        [JsonIgnore]
        public string HashedPassword { get; set; }
        public string Token { get; set; }
        public DateTime SystemEntryDate { get; }
        public List<string> BlockedUsers { get; set; }
    }
}