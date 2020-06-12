using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inverter.homeassistant.MQTT
{
    public static class Logging
    {
        public static void WriteLog(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = "";// "C:\\Logs\\";
            logFilePath = logFilePath + "Log-" + System.DateTime.Today.ToString("MM-dd-yyyy") + "." + "txt";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.Append);
            }
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }

        public static void WriteStatus(string strLog)
        {
            StreamWriter log;
            FileStream fileStream = null;
            DirectoryInfo logDirInfo = null;
            FileInfo logFileInfo;

            string logFilePath = "";// "C:\\Logs\\";
            logFilePath = logFilePath + "Status.html";
            logFileInfo = new FileInfo(logFilePath);
            logDirInfo = new DirectoryInfo(logFileInfo.DirectoryName);
            if (!logDirInfo.Exists) logDirInfo.Create();
            fileStream = new FileStream(logFilePath, FileMode.OpenOrCreate);
            /*if (!logFileInfo.Exists)
            {
                fileStream = logFileInfo.Create();
            }
            else
            {
                fileStream = new FileStream(logFilePath, FileMode.OpenOrCreate);
            }*/
            log = new StreamWriter(fileStream);
            log.WriteLine(strLog);
            log.Close();
        }

        public static void DebugWrite(string type, string data)
        {
            if (Settings.isDebug == true && true)
            {
                Logging.WriteLog("\r\n\r\nDebug - " + DateTime.Now.ToString() + " - \r\n:" + data);
                Console.WriteLine(data);
            }
            else
            {
                Logging.WriteLog("\r\n\r\nError - " + DateTime.Now.ToString() + " - \r\n:" + data);
                Console.WriteLine(data);
            }
        }


    }
}
