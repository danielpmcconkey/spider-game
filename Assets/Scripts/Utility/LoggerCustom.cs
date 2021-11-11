using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Assets.Scripts.Utility
{
    public enum LogLevel { DEGUG, INFO, ERROR }

    public static class LoggerCustom
    {
        private static bool _isDebugModeOn = false;
        private static int _frameCount;
        private static string _debugLogFileLocation = string.Empty; // C:\Users\Dan\AppData\LocalLow\SpiderController\log.txt
        private static StringBuilder _logStringBuilder;

        public static void Init(bool isDebugModeOn, string debugLogFileLocation, int frameCount)
        {
            _isDebugModeOn = isDebugModeOn;
            _debugLogFileLocation = Path.Combine(debugLogFileLocation, 
                string.Format("spiderLog-{0}.txt", DateTime.Now.ToString("yyyy-MM-dd.HH.mm.ss.fff")));
            _frameCount = frameCount;
            _logStringBuilder = new StringBuilder();
        }
        public static void SetFrameCount(int count)
        {
            _frameCount = count;
        }

        public static void DEBUG(string message)
        {
            if (_isDebugModeOn) _LogMessage(LogLevel.DEGUG, message);
        }
        public static void INFO(string message)
        {
            _LogMessage(LogLevel.INFO, message);
        }
        public static void ERROR(string message)
        {
            _LogMessage(LogLevel.ERROR, message);
        }
        public static void WriteLogToFile()
        {
            using (StreamWriter sw = new StreamWriter(_debugLogFileLocation, true))
            {
                sw.WriteLine(_logStringBuilder.ToString());
            }
        }

        private static void _LogMessage(LogLevel level, string message)
        {
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff");
            string formattedMessage = String.Format("{0}\t{1}\t{2}\t{3}",timestamp, _frameCount, level, message);
            _logStringBuilder.AppendLine(formattedMessage);
        }

    }
}
