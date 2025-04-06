using log4net.Config;
using System.Reflection;
namespace Test.Infrastructure.Logging
{
    public static class Logger
    {
        public static void ConfigureLogging()
        {
            log4net.Util.LogLog.InternalDebugging = true;
            try
            {
                // Get Log4Net Config file
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var configFilePath = Path.Combine(assemblyDirectory, "Logging/Log4Net.config");

                if (!File.Exists(configFilePath))
                {
                    Console.WriteLine("Log4Net config file not found at: " + configFilePath);
                    return;
                }

                string currentDir = Directory.GetCurrentDirectory();
                string logFolderRelativePath = "logs";
                string logFolderPath = Path.Combine(currentDir, logFolderRelativePath);

                if (!Directory.Exists(logFolderPath))
                {
                    Directory.CreateDirectory(logFolderPath);
                    Console.WriteLine("Created log folder at: " + logFolderPath);
                }

                XmlConfigurator.Configure(new FileInfo(configFilePath));
                Console.WriteLine($"Log4Net configured using {configFilePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring log4net: {ex.Message}");
            }
        }
    }
}
