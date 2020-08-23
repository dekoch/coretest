using System.Diagnostics;

namespace Shared
{
    public class Error
    {
        private System.Exception ex;
        StackTrace stackTrace;

        public void Set(string err)
        {
            ex = new System.InvalidOperationException(err);

            //stackTrace = new StackTrace(); 
            //ex = new System.InvalidOperationException(err + " " + stackTrace.GetFrame(0).ToString());
        }

        public void Set(System.Exception err)
        {
            ex = err;
        }

        public System.Exception Get()
        {
            return ex;
        }

        public bool HasError()
        {
            if (ex == null)
            {
                return false;
            }

            if (ex.ToString().Length > 0)
            {
                return true;
            }

            return false;
        }
    }
}