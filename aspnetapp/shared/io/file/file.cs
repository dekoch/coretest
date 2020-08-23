namespace Shared
{
    public class File
    {
        public static (byte[], Error) Read(string path)
        {
            Error err = new Error();
            byte[] ret = { };

            try
            {
                System.IO.DirectoryInfo dir = System.IO.Directory.GetParent(path);

                if (System.IO.Directory.Exists(dir.FullName) == false)
                {
                    err.Set("directory not found");
                    return (ret, err);
                }

                ret = System.IO.File.ReadAllBytes(path);
            }
            catch (System.Exception ex)
            {
                err.Set(ex);
            }

            return (ret, err);
        }
    }
}