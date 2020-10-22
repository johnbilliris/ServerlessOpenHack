using System;
using System.Text.Json.Serialization;

namespace Team1.Models
{
    public class RatingClass
    {
        public string id {get;set;}
        public string userId {get;set;}
        public string productId {get;set;}
        public string locationName {get;set;}
        public int rating {get;set;}
        public string userNotes {get;set;}
        public DateTime timeStamp {get;set;}
    }
}