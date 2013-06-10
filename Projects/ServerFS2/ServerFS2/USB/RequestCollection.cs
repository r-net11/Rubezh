using System.Collections.Generic;
using System.Linq;

namespace ServerFS2
{
	public class RequestCollection
	{
		public List<Request> Requests { get; private set; }
		object locker = new object();

		public RequestCollection()
		{
			Requests = new List<Request>();
		}

		public void Clear()
		{
			lock (locker)
			{
				//Requests.RemoveAll(x => (DateTime.Now - x.StartTime).TotalSeconds > 20);
				Requests.Clear();
			}
		}

		public void AddRequest(Request request)
		{
			lock (locker)
			{
				Requests.Add(request);
			}
		}

		public Request GetFirst()
		{
			lock (locker)
			{
				return Requests.FirstOrDefault();
			}
		}

		public Request GetById(uint id)
		{
			lock (locker)
			{
				return Requests.FirstOrDefault(x => x.Id == id);
			}
		}

		public void RemoveById(uint id)
		{
			lock (locker)
			{
				Requests.RemoveAll(x => x.Id == id);
			}
		}

		public int Count()
		{
			lock (locker)
			{
				return Requests.Count();
			}
		}
	}
}