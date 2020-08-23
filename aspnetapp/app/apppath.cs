using System.IO;
using System.Reflection;

namespace App
{
    public class AppPath
    {
        public static string GetAppPath()
        {
            //return System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Directory.GetCurrentDirectory();
        }

        public static string GetStaticPath()
        {
            return GetAppPath() + "/data/ui/static";
        }
    }
}