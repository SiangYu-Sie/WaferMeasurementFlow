using System;

namespace WaferMeasurementFlow.Services
{
    public class LogSystem
    {
        public void LogInfo(string message)
        {
            Console.WriteLine($"[INFO] {DateTime.Now}: {message}");
            // Here you could also write to a file or database
        }

        public void LogError(string message)
        {
            Console.WriteLine($"[ERROR] {DateTime.Now}: {message}");
        }

        public void LogDebug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[DEBUG] {DateTime.Now}: {message}");
        }
    }
}
