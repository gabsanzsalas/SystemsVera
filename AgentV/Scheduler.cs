using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgentV
{
    public class Scheduler
    {
        private List<SchedulerItem> scheduler;

        public Scheduler()
        {
            InitSchedulerList();
        }

        private void InitSchedulerList()
        {
            using (StreamReader file = new StreamReader("schedule.json"))
            {
                string json = file.ReadToEnd();
                scheduler = JsonConvert.DeserializeObject<List<SchedulerItem>>(json);
            }
            if (scheduler == null)
                scheduler = new List<SchedulerItem>();
        }

        public void AddScheduleItem(int id, int interval)
        {
            scheduler.Add(new SchedulerItem() { Id = id, Interval = interval, LastDate = DateTime.Now });
            WriteToFile();
        }

        public void ChangeLastDate(int id)
        {
            scheduler.Where(x => x.Id == id).FirstOrDefault().LastDate = DateTime.Now;
            WriteToFile();
        }

        private void WriteToFile()
        {
            using (StreamWriter file = File.CreateText("schedule.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, scheduler);
            }
        }

        public List<SchedulerItem> GetScheduler()
        {
            return scheduler;
        }
    }

    public class SchedulerItem
    {
        public int Id { get; set; }
        public int Interval { get; set; }
        public DateTime LastDate { get; set; }
    }
}