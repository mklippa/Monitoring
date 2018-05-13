using System.IO;
using Microsoft.Extensions.Configuration;

namespace Emulator
{
    public class EmulatorSettings
    {
        private static readonly IConfigurationRoot Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        private EmulatorSettings()
        {
        }

        public string ApiBaseUrl => Configuration[nameof(ApiBaseUrl)];

        public string RequestUrl => Configuration[nameof(RequestUrl)];

        public int ErrorsLimit => int.Parse(Configuration[nameof(ErrorsLimit)]);

        public int DispatchPeriod => int.Parse(Configuration[nameof(DispatchPeriod)]);

        public static EmulatorSettings Instance => InstanceHolder.Instance;

        private static class InstanceHolder
        {
            static InstanceHolder()
            {
            }

            internal static readonly EmulatorSettings Instance = new EmulatorSettings();
        }
    }
}