using System;
using S7.Net;

namespace Shared
{
    public class PlcDriver
    {
        private string uuid;
        private Plc driver;
        private bool isActive;

        public Error Open(CpuType cpu, string ip, int port, Int16 rack, Int16 slot)
        {
            Error err = new Error();

            uuid = Guid.NewGuid().ToString();
            isActive = false;

            try
            {
                driver = new Plc(cpu, ip, port, rack, slot);
                driver.ReadTimeout = 200;
                driver.WriteTimeout = 200;
                driver.Open();
            }
            catch (System.Exception ex)
            {
                err.Set(ex);
            }

            return err;
        }

        public Error Close()
        {
            Error err = new Error();

            try
            {
                driver.Close();
                isActive = false;
            }
            catch (System.Exception ex)
            {
                err.Set(ex);
            }

            return err;
        }

        public string GetUUID()
        {
            return uuid;
        }

        public void SetIsActive(bool bo)
        {
            isActive = bo;
        }

        public bool GetIsActive()
        {
            return isActive;
        }

        public (byte[], Error) ReadBytes(DataType dataType, int db, int startByteAdr, int count)
        {
            Error err = new Error();
            byte[] ret = new byte[] { };

            try
            {
                ret = driver.ReadBytes(dataType, db, startByteAdr, count);
            }
            catch (System.Exception ex)
            {
                err.Set(ex);
            }

            return (ret, err);
        }

        public Error WriteBytes(DataType dataType, int db, int startByteAdr, byte[] value)
        {
            Error err = new Error();

            try
            {
                driver.WriteBytes(dataType, db, startByteAdr, value);
            }
            catch (System.Exception ex)
            {
                err.Set(ex);
            }

            return err;
        }
    }
}