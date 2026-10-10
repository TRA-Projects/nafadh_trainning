using System;
using System.IO;
using NLog;
using NLog.Config;

namespace Bidding_API.Helpers
{
    /// <summary>
    /// IIS-safe NLog setup: log next to Bidding_API.dll (AppContext.BaseDirectory),
    /// not Directory.GetCurrentDirectory() which can be System32\inetsrv under in-process IIS.
    /// </summary>
    public static class NLogBootstrap
    {
        public static string Configure()
        {
            var appBase = AppContext.BaseDirectory?.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar) ?? Directory.GetCurrentDirectory();

            var nlogPath = Path.Combine(appBase, "nlog.config");
            if (!File.Exists(nlogPath))
            {
                nlogPath = Path.Combine(Directory.GetCurrentDirectory(), "nlog.config");
            }

            var logDir = Path.Combine(appBase, "logs");
            try
            {
                Directory.CreateDirectory(logDir);
                Directory.CreateDirectory(Path.Combine(logDir, "archives"));
            }
            catch (Exception ex)
            {
                TryWriteBootFailure(appBase, "CreateDirectory failed: " + ex);
            }

            if (!File.Exists(nlogPath))
            {
                TryWriteBootFailure(logDir, "nlog.config not found. looked=" + nlogPath);
                return logDir;
            }

            try
            {
                LogManager.Configuration = new XmlLoggingConfiguration(nlogPath);
                LogManager.Configuration.Variables["logDir"] = logDir;
                LogManager.ReconfigExistingLoggers();
                LogManager.GetCurrentClassLogger().Info(
                    "NLog boot. logDir={0} appBase={1} nlogPath={2}",
                    logDir,
                    appBase,
                    nlogPath);

                // Permission probe — if this fails, IIS App Pool cannot write logs.
                var probe = Path.Combine(logDir, "write-probe.txt");
                File.WriteAllText(probe, DateTime.UtcNow.ToString("o") + " OK" + Environment.NewLine);
            }
            catch (Exception ex)
            {
                TryWriteBootFailure(logDir, "NLog configure/write failed: " + ex);
            }

            return logDir;
        }

        private static void TryWriteBootFailure(string dir, string message)
        {
            try
            {
                Directory.CreateDirectory(dir);
                File.AppendAllText(
                    Path.Combine(dir, "nlog-boot-failure.txt"),
                    DateTime.UtcNow.ToString("o") + " " + message + Environment.NewLine);
            }
            catch
            {
                // Last resort: ignore — process may lack any write rights on site folder.
            }
        }
    }
}
