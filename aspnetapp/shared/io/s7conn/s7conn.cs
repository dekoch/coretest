using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using S7.Net;

namespace Shared
{

    public class S7Conn
    {
        private struct connection
        {
            public string Ip;
            public CpuType Cpu;
            public int Port;
            public Int16 Rack;
            public Int16 Slot;
            public int MaxCnt;
            public List<PlcDriver> Drivers;
        }

        private List<connection> connections = new List<connection>();

        private readonly object connLock = new object();

        public Error AddPLC(CpuType cpu, string ip, int port, Int16 rack, Int16 slot, int maxcnt)
        {
            Error err = new Error();

            lock (connLock)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].Ip == ip)
                    {
                        connection item = connections[i];
                        item.MaxCnt = maxcnt;
                        err = open(ref item);
                        connections[i] = item;

                        return err;
                    }
                }

                connection conn = new connection();
                conn.Ip = ip;
                conn.Cpu = cpu;
                conn.Port = port;
                conn.Rack = rack;
                conn.Slot = slot;
                conn.MaxCnt = maxcnt;
                conn.Drivers = new List<PlcDriver>();

                err = open(ref conn);
                if (err.HasError())
                {
                    return err;
                }

                connections.Add(conn);
            }

            return err;
        }

        private Error open(ref connection conn)
        {
            Error err = new Error();

            for (int i = conn.Drivers.Count; i < conn.MaxCnt; i++)
            {
                PlcDriver driver = new PlcDriver();
                driver.SetIsActive(false);
                err = driver.Open(conn.Cpu, conn.Ip, conn.Port, conn.Rack, conn.Slot);
                if (err.HasError())
                {
                    return err;
                }

                conn.Drivers.Add(driver);
            }

            return err;
        }

        public Error Close(string ip)
        {
            Error err = new Error();

            lock (connLock)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].Ip == ip)
                    {
                        connection item = connections[i];
                        err = close(ref item);
                        connections[i] = item;

                        return err;
                    }
                }
            }

            return err;
        }

        private Error close(ref connection conn)
        {
            Error err = new Error();

            for (int i = 0; i < conn.Drivers.Count; i++)
            {
                PlcDriver driver = conn.Drivers[i];
                driver.SetIsActive(false);
                err = driver.Close();
                if (err.HasError())
                {
                    return err;
                }
            }

            return err;
        }

        public (byte[], Error) ReadBytes(DataType dataType, int db, int startByteAdr, int count, string ip)
        {
            Error err = new Error();
            byte[] ret = new byte[] { };

            PlcDriver driver = new PlcDriver();
            bool isFree = false;
            SpinWait spin = new SpinWait();

            while (isFree == false)
            {
                lock (connLock)
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].Ip == ip)
                        {
                            for (int ii = 0; ii < connections[i].Drivers.Count; ii++)
                            {
                                if (isFree == false)
                                {
                                    driver = connections[i].Drivers[ii];
                                    if (driver.GetIsActive() == false)
                                    {
                                        isFree = true;
                                        driver.SetIsActive(true);
                                        connections[i].Drivers[ii] = driver;
                                    }
                                }
                            }
                        }
                    }
                }

                if (isFree == false)
                {
                    spin.SpinOnce();
                }
            }

            (ret, err) = driver.ReadBytes(dataType, db, startByteAdr, count);

            lock (connLock)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].Ip == ip)
                    {
                        for (int ii = 0; ii < connections[i].Drivers.Count; ii++)
                        {
                            driver = connections[i].Drivers[ii];
                            driver.SetIsActive(false);
                            connections[i].Drivers[ii] = driver;
                        }
                    }
                }
            }

            return (ret, err);
        }

        public Error WriteBytes(DataType dataType, int db, int startByteAdr, byte[] value, string ip)
        {
            Error err = new Error();

            PlcDriver driver = new PlcDriver();
            bool isFree = false;
            SpinWait spin = new SpinWait();

            while (isFree == false)
            {
                lock (connLock)
                {
                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].Ip == ip)
                        {
                            for (int ii = 0; ii < connections[i].Drivers.Count; ii++)
                            {
                                if (isFree == false)
                                {
                                    driver = connections[i].Drivers[ii];
                                    if (driver.GetIsActive() == false)
                                    {
                                        isFree = true;
                                        driver.SetIsActive(true);
                                        connections[i].Drivers[ii] = driver;
                                    }
                                }
                            }
                        }
                    }
                }

                if (isFree == false)
                {
                    spin.SpinOnce();
                }
            }

            err = driver.WriteBytes(dataType, db, startByteAdr, value);

            lock (connLock)
            {
                for (int i = 0; i < connections.Count; i++)
                {
                    if (connections[i].Ip == ip)
                    {
                        for (int ii = 0; ii < connections[i].Drivers.Count; ii++)
                        {
                            driver = connections[i].Drivers[ii];
                            driver.SetIsActive(false);
                            connections[i].Drivers[ii] = driver;
                        }
                    }
                }
            }

            return err;
        }
    }
}