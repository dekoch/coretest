using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using S7.Net;
using Shared;

namespace App
{

    public class APIPLC
    {
        public class Response
        {
            public double Version { get; set; }
            public byte[] Data { get; set; }
            public bool[] Bit { get; set; }
            public string Status { get; set; }
        }

        private static LiveList liveList = new LiveList();
        private static readonly object writeLock = new object();

        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Error err = global.Conn.AddPLC(CpuType.S7300, "192.168.178.241", 102, 0, 2, 1);
            if (err.HasError())
            {
                Console.WriteLine(err);
                return;
            }

            LiveList.Item item = new LiveList.Item();
            item.UUID = "1234";
            item.Ip = "192.168.178.241";
            item.DataType = DataType.DataBlock;
            item.DB = 1000;
            //item.DB = 128;
            item.StartByteAdr = 1;
            item.Count = 1;
            //item.Count = 8695;
            item.Interval = 100;
            err = liveList.AddItem(item);
            if (err.HasError())
            {
                Console.WriteLine(err);
                return;
            }

            err = liveList.Startup(ref global.Conn);
            if (err.HasError())
            {
                Console.WriteLine(err);
                return;
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/api/plc/", async context =>
                {
                    await context.Response.WriteAsync(getResp(context));
                });
            });
        }

        private static string getResp(HttpContext context)
        {
            Response resp = new Response();
            resp.Version = 1.0;

            string par1 = context.Request.Query["par1"];

            if (par1 == "1")
            {
                Write(true);
            }
            else if (par1 == "0")
            {
                Write(false);
            }

            var (b, err) = liveList.GetValue("1234");
            if (err.HasError() == false)
            {
                if (b.Length > 0)
                {
                    resp.Data = b;

                    resp.Bit = new bool[8];
                    resp.Bit[0] = Conversion.SelectBit(resp.Data[0], 0);
                    resp.Bit[1] = Conversion.SelectBit(resp.Data[0], 1);
                    resp.Bit[2] = Conversion.SelectBit(resp.Data[0], 2);
                    resp.Bit[3] = Conversion.SelectBit(resp.Data[0], 3);
                    resp.Bit[4] = Conversion.SelectBit(resp.Data[0], 4);
                    resp.Bit[5] = Conversion.SelectBit(resp.Data[0], 5);
                    resp.Bit[6] = Conversion.SelectBit(resp.Data[0], 6);
                    resp.Bit[7] = Conversion.SelectBit(resp.Data[0], 7);
                }
            }

            return JsonSerializer.Serialize(resp);
            //byte[] b = JsonSerializer.Serialize(resp);
            //return Encoding.UTF8.GetString(b, 0, b.Length);
        }

        public static void Write(bool tf)
        {
            lock (writeLock)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();

                var (b, errRead) = global.Conn.ReadBytes(DataType.DataBlock, 1000, 1, 1, "192.168.178.241");

                Console.WriteLine("#####");
                Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString());

                if (b.Length > 0)
                {
                    if (tf)
                    {
                        b[0] = SetBit(b[0], 7, true);
                    }
                    else
                    {
                        b[0] = SetBit(b[0], 7, false);
                    }

                    global.Conn.WriteBytes(DataType.DataBlock, 1000, 1, b, "192.168.178.241");

                    Console.WriteLine(stopWatch.ElapsedMilliseconds.ToString());
                }
            }
        }

        public static byte SetBit(byte data, int bitInByteIndex, bool value)
        {
            byte mask = (byte)(1 << bitInByteIndex);

            if (value)
            {
                // set to 1
                data |= mask;
            }
            else
            {
                // Set to 0
                data &= (byte)(~mask);
            }

            return data;
        }
    }
}