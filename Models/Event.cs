using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodeksArmScheduler
{
    public partial class Event
    {
        [NotMapped]
        public long IdEvent
        {
            get
            {
                return id_event;
            }
            set
            {
                id_event = value;
            }
        }

        [NotMapped]
        public long FidEventType
        {
            get
            {
                return fid_event_type;
            }
            set
            {
                fid_event_type = value;
            }
        }

        protected EventType _eventGroup;

        [NotMapped]
        public EventType EventGroup
        {
            get
            {
                if (_eventGroup == null)
                {
                    if (EventType != null)
                    {
                        if (!EventType.fid_event_type.HasValue)
                            _eventGroup = EventType;
                        else
                        {
                            _eventGroup = CustomApplicationContext.Db.EventTypes.FirstOrDefault(x => x.id_event_type == EventType.fid_event_type);
                        }
                    }
                }
                return _eventGroup;
            }
            set
            {
                _eventGroup = value;
            }
        }

        protected EventType _eventType;
        [NotMapped]
        public EventType EventType
        {
            get
            {
                if (_eventType == null)
                {
                    _eventType = CustomApplicationContext.Db.EventTypes.FirstOrDefault(x => x.id_event_type == FidEventType);
                }

                return _eventType;
            }
        }

        [NotMapped]
        public string StartWeekDays
        {
            get
            {
                return start_week_days;
            }
            set
            {
                start_week_days = value;
            }
        }

        [NotMapped]
        public DateTime StartTime
        {
            get
            {
                DateTime temp;
                bool parseResult = DateTime.TryParseExact(start_time, "yyyy-MM-dd HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out temp);
                return parseResult ? temp : DateTime.Now;
            }
            set
            {
                start_time = value.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        private List<EventEmail> _emailList = new List<EventEmail>();

        [NotMapped]
        public List<EventEmail> EmailList
        {
            get
            {
                return _emailList;
            }

            set
            {
                _emailList = value;
            }
        }



    }
}
