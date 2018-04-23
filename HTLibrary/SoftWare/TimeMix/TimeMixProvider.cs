using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace User.SoftWare.TimeMix
{
    public class TimeMixProvider
    {
        DispatcherTimer timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
        string timeTableFileName;
        bool istimeTableLoaded = false;
        TimeTable analyse;
        public event TimeMixChangedEventHandler TimeMixChanged;

        public TimeMixProvider(string folder)
        {
            timer.Tick += Timer_Tick;
            DataFolder = folder;
        }
        private async void Timer_Tick(object sender, EventArgs e)
        {
            TimeMixChangedEventArgs args = new TimeMixChangedEventArgs();
            DateTime timee = DateTime.Now + TimeSpan.FromSeconds(Offset);
            if (!istimeTableLoaded)
            {
                await Task.Run(() =>
                {
                    if (File.Exists(DataFolder + TimeTableFileName + ".xml"))
                    {
                        analyse = TimeTable.Load(DataFolder + TimeTableFileName + ".xml").ToAnalyse();
                        istimeTableLoaded = true;
                    }
                });
            }
            if (istimeTableLoaded)
            {
                var layer = analyse.Select(TimeTableDayOfWeek);
                List<TimeSection> sections = layer.ToList();
                bool ishandled = false;
                for (int i = 0; i < sections.Count; i++)
                {
                    if (timee < sections[i].Time)
                    {
                        args.NextSection = sections[i];
                        if (i > 0)
                        {
                            args.CurrentSection = sections[i - 1];
                            TimeSpan t = (args.NextSection.Time - args.CurrentSection.Time);
                            TimeSpan r = (timee.TimeOfDay - args.CurrentSection.Time.TimeOfDay);
                            args.Percent = r.TotalSeconds / t.TotalSeconds;
                        }
                        else
                        {
                            args.CurrentSection = analyse.Select(TimeTableDayOfWeek.Pre()).Last();
                            TimeSpan t = (args.NextSection.Time - args.CurrentSection.Time + TimeSpan.FromDays(1));
                            TimeSpan r = (timee.TimeOfDay - args.CurrentSection.Time.TimeOfDay + TimeSpan.FromDays(1));
                            args.Percent = r.TotalSeconds / t.TotalSeconds;
                        }
                        ishandled = true;
                        break;
                    }
                }
                if (!ishandled)
                {
                    args.CurrentSection = analyse.Select(TimeTableDayOfWeek).Last();
                    args.NextSection = analyse.Select(TimeTableDayOfWeek.Next()).First();
                    TimeSpan t = (args.NextSection.Time - args.CurrentSection.Time + TimeSpan.FromDays(1));
                    TimeSpan r = (timee.TimeOfDay - args.CurrentSection.Time.TimeOfDay + TimeSpan.FromDays(1));
                    args.Percent = r.TotalSeconds / t.TotalSeconds;

                }
                NewestTimeMixChangedEventargs = args;
                TimeMixChanged?.Invoke(this, args);
            }
        }
        public string DataFolder { get; set; }
        public int Offset { get; set; }
        public DayOfWeek TimeTableDayOfWeek { get; set; } = DateTime.Now.DayOfWeek;
        public string TimeTableFileName
        {
            get => timeTableFileName; set
            {
                timeTableFileName = value;
                istimeTableLoaded = false;
            }
        }
        public bool IsEnabled { get => timer.IsEnabled; set => timer.IsEnabled = value; }
        public TimeMixChangedEventArgs NewestTimeMixChangedEventargs { get; private set; }
    }
    public class TimeMixChangedEventArgs : EventArgs
    {
        internal TimeMixChangedEventArgs()
        {
        }
        internal TimeMixChangedEventArgs(TimeSection currentSection, TimeSection nextSection)
        {
            CurrentSection = currentSection;
            NextSection = nextSection;
        }

        public TimeSection CurrentSection { get; internal set; }
        public TimeSection NextSection { get; internal set; }
        public double Percent { get; internal set; }
    }
    public delegate void TimeMixChangedEventHandler(object sender, TimeMixChangedEventArgs e);
}
