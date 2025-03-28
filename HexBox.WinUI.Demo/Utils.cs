using Windows.ApplicationModel;

namespace HexBox.WinUI.Demo
{
    public class Utils
    {
        public static bool IsPackaged()
        {
            try
            {
                var package = Package.Current;
                return package != null;
            }
            catch
            {
                return false;
            }
        }
    }
}
