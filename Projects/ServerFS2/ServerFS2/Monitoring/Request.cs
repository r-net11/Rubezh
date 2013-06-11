//using System;
//using System.Collections.Generic;
//namespace ServerFS2.Monitor
//{
//    public class Request
//    {
//        public DateTime StartTime { get; private set; }
//        public int Id { get; set; }
//        public List<byte> Bytes { get; set; }
//        public RequestTypes RequestType { get; set; }

//        public Request(int id, RequestTypes requestType)
//        {
//            StartTime = DateTime.Now;
//            Id = id;
//            RequestType = requestType;
//        }

//        public Request(int id, RequestTypes requestType, List<byte> bytes)
//            : this(id, requestType)
//        {
//            Bytes = bytes;
//        }
//    }
//}