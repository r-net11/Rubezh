using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.GK
{
    public class GkEvents
    {
        
        #region Construction
        public GkEvents()
        {
            db = new GkJournalDatabaseEntities();
        }
        #endregion

        #region Fields
        GkJournalDatabaseEntities db;
        #endregion

        #region Methods
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
        public void Emptify()
        {
            foreach (gkEvent t in db.gkEvents)
                db.gkEvents.DeleteObject(t);
            db.SaveChanges();
        }
        public List<gkEvent> Select_Items()
        {
            var query =
                from t in db.gkEvents
                orderby t.GKNo
                select t;
            return query.ToList();
        }
        public List<gkEvent> Select_Items(uint start_no, uint end_no, DateTime start_dt, DateTime end_dt)
        {
            var query =
                from t in db.gkEvents
                where t.GKNo>= start_no && t.GKNo<=end_no
                where t.Date >= start_dt && t.Date <= end_dt
                orderby t.GKNo
                select t;
            return query.ToList();
        }
        #endregion
    }

}
