using System;
using System.Collections.Generic;
using System.Timers;
using S7.Net;

namespace Shared
{

    public class LiveList
    {
        public struct Item
        {
            public string UUID;
            public string Name;
            public string Ip;
            public DataType DataType;
            public int DB;
            public int StartByteAdr;
            public int Count;
            public byte[] Value;
            public int Interval;
            public DateTime Updated;
        }

        private static List<Item> items = new List<Item>();
        private static S7Conn conn = new S7Conn();
        private static System.Timers.Timer aTimer;

        public Error Startup(ref S7Conn s7conn)
        {
            Error err = new Error();

            conn = s7conn;

            aTimer = new System.Timers.Timer(50);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;

            return err;
        }

        private static void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            aTimer.Stop();

            Error err = update();
            if (err.HasError())
            {
                Console.WriteLine(err);
            }

            aTimer.Start();
        }

        private static Error update()
        {
            Error err = new Error();
            
            for (int i = 0; i < items.Count; i++)
            {
                Item item = items[i];

                DateTime dt = item.Updated;
                dt = dt.AddMilliseconds(item.Interval);

                if (dt.Ticks > DateTime.Now.Ticks)
                {
                    continue;
                }

                (item.Value, err) = conn.ReadBytes(item.DataType, item.DB, item.StartByteAdr, item.Count, item.Ip);
                if (err.HasError())
                {
                    return err;
                }

                item.Updated = DateTime.Now;
                items[i] = item;
            }

            return err;
        }

        public Error AddItem(Item item)
        {
            Error err = new Error();

            items.Add(item);

            return err;
        }

        public (byte[], Error) GetValue(string uuid)
        {
            Error err = new Error();
            byte[] ret = new byte[] { };

            ret = items.Find(x => x.UUID == uuid).Value;
            if (ret == null)
            {
                err.Set("item not found");
            }

            return (ret, err);
        }
    }
}