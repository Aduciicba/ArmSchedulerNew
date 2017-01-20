using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodeksArmScheduler
{
    public partial class EventLog
    {

        #region Event

        private Event _self_event;
        [NotMapped]
        public Event Self_event
        {
            get
            {
                if (_self_event == null)
                {
                    _self_event = CustomApplicationContext.Db.Events.FirstOrDefault(x => x.id_event == fid_event);
                }
                return _self_event;
            }
        }

        #endregion
        [NotMapped]
        public string Event_type_name
        {
            get
            {
                return Self_event.EventGroup.name;
            }
        }

        [NotMapped]
        public DateTime EventTime
        {
            get
            {
                DateTime temp;
                bool parseResult = DateTime.TryParseExact(event_time, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out temp);
                return parseResult ? temp : DateTime.Now;
            }
            set
            {
                event_time = value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }


    }
}
