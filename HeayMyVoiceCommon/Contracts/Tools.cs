

using System.Globalization;
using System.Resources;

namespace HeayMyVoiceCommon.Contracts
{
    public class Tools
    {
        public static string GetResource(string resource)
        {
            var rm = new ResourceManager(typeof(Properties.Resources));
            return rm.GetString(resource, CultureInfo.InvariantCulture);

        }
    }
}
