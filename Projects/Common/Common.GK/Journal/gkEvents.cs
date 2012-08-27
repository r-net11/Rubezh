using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Common.GK
{
    public class GkEvents
    {
        GkJournalDatabaseEntities db;
        public GkEvents()
        {
            db = new GkJournalDatabaseEntities();
        }
        public void Add(JournalItem j)
        {
            if (db.gkEvents.FirstOrDefault(gke => gke.GKNo == j.GKNo) == null)
            {
                db.gkEvents.AddObject(new gkEvent
                {
                    GKNo = j.GKNo,
                    Date = DateTime.Parse(j.StringDate),
                    EventName = j.EventName,
                    EventDescription = j.EventDescription,
                    KAUNo = j.KAUNo,
                    KAUAddress = j.KAUAddress,
                    Code = j.Code,
                    ObjectNo = j.ObjectNo,
                    ObjectState = j.ObjectState,
                    GKObjectNo = j.GKObjectNo,
                    ObjectDeviceAddress = j.ObjectDeviceAddress,
                    ObjectDeviceType = j.ObjectDeviceType,
                    ObjectFactoryNo = j.ObjectFactoryNo

                });
                db.SaveChanges();
            }
        }
        public List<gkEvent> Select_Items()
        {
            var query =
                from t in db.gkEvents
                orderby t.GKNo
                select t;
            return query.ToList();
        }
    }

}
