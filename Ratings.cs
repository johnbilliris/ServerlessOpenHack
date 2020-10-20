using System;
using System.Text.Json.Serialization;

namespace Team1
{
    public class RatingClass
    {
        public string Id {get;set;}
        public string UserId {get;set;}
        public string ProductId {get;set;}
        public string LocationName {get;set;}
        
        public int Rating {get;set;}
        
        public string UserNotes {get;set;}
        public DateTime TimeStamp {get;set;}
    }
}