﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AgentV
{
    public class Scheduler
    {
        private static List<SchedulerItem> scheduler;
        private static List<LaunchedItem> launched;

        public Scheduler()
        {
            using (var file = File.OpenText("schedule.json"))
            {
                string json = file.ReadToEnd();
                try
                {
                    scheduler = JsonConvert.DeserializeObject<List<SchedulerItem>>(json);
                }
                catch (Exception ex) {; }

            }
            if (scheduler == null)
                scheduler = new List<SchedulerItem>();

            using (var file = File.OpenText("launched.json"))
            {
                string json = file.ReadToEnd();
                try
                {
                    launched = JsonConvert.DeserializeObject<List<LaunchedItem>>(json);
                }
                catch (Exception ex) {; }

            }
            if (launched == null)
                launched = new List<LaunchedItem>();
        }

        public void AddScheduleItem(int id, int interval)
        {
            if (scheduler.Any(x => x.Id == id))
                scheduler.Remove(scheduler.Find(x => x.Id == id));

            scheduler.Add(new SchedulerItem() { Id = id, Interval = interval, CreationDate = DateTime.Now });

            WriteToScheduleFile();
        }

        public void CancelScheduleItem(int id)
        {
            scheduler.Remove(scheduler.Find(x => x.Id == id));
            WriteToScheduleFile();
        }
        public void CancelLaunchedItem(int id)
        {
            launched.Remove(launched.Find(x => x.Id == id));
            WriteToLaunchedFile();
        }

        public void AddLaunchedItem(int id, DateTime creationDate)
        {
            launched.Add(new LaunchedItem() { Id = id, LastDate = creationDate });
            WriteToLaunchedFile();
        }


        public void ChangeLastDate(int id)
        {
            launched.Where(x => x.Id == id).FirstOrDefault().LastDate = DateTime.Now;
            WriteToLaunchedFile();
        }

        private static void WriteToScheduleFile()
        {
            using (StreamWriter file = File.CreateText("schedule.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, scheduler);
            }
        }

        private static void WriteToLaunchedFile()
        {
            using (StreamWriter file = File.CreateText("launched.json"))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(file, launched);
            }
        }

        public static List<ScheduleStatus> GetScheduleStatus()
        {
            List<ScheduleStatus> ret = new List<ScheduleStatus>();
            foreach (SchedulerItem item in scheduler)
            {
                ret.Add(new ScheduleStatus() { Id = item.Id, Interval = item.Interval, CreationDate = item.CreationDate, LastDate = launched.Where<LaunchedItem>(x => x.Id == item.Id).FirstOrDefault().LastDate });
            }
            return ret;
        }

        public static List<SchedulerItem> GetScheduler()
        {
            return scheduler;
        }

        public static List<LaunchedItem> GetLaunched()
        {
            return launched;
        }
    }

    public class SchedulerItem
    {
        public int Id { get; set; }
        public int Interval { get; set; }
        public DateTime CreationDate { get; set; }
    }

    public class ScheduleStatus
    {
        public int Id { get; set; }
        public int Interval { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastDate { get; set; }
    }

    public class LaunchedItem
    {
        public int Id { get; set; }
        public DateTime LastDate { get; set; }
    }
}