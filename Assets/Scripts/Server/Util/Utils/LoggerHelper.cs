using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

namespace Almond.Util
{
    [Flags]
    public enum LogLevel
    {
        NONE = 0,
        DEBUG = 1,
        INFO = 2,
        WARNING = 4,
        ERROR = 8,
        EXCEPT = 16,
        CRITICAL = 32,
    }

    public class LoggerHelper
    {
        public static LogLevel CurrentLogLevels = LogLevel.DEBUG | LogLevel.INFO | LogLevel.WARNING | LogLevel.ERROR | LogLevel.CRITICAL | LogLevel.EXCEPT;
        private const bool SHOW_STACK = true;
        private static LogWriter m_logWriter;
        public static string DebugFilterStr = string.Empty;

        static LoggerHelper()
        {
            m_logWriter = new LogWriter();
            Application.logMessageReceived += ProcessExceptionReport;
            //Application.RegisterLogCallback(new Application.LogCallback(ProcessExceptionReport));
        }

        public static void Release()
        {
            if (null != m_logWriter)
                m_logWriter.Release();
        }

        public static void UploadLogFile()
        {
            if (null != m_logWriter)
                m_logWriter.UploadTodayLog();
        }

        static ulong index = 0;

        public static void Debug(object message, bool isShowStack = SHOW_STACK, int user = 0)
        {
            if (DebugFilterStr != "") return;

            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message, " Index = ", index++), LogLevel.DEBUG);
        }

        public static void Debug(string filter, object message, bool isShowStack = SHOW_STACK)
        {

            if (DebugFilterStr != "" && DebugFilterStr != filter) return;
            if (LogLevel.DEBUG == (CurrentLogLevels & LogLevel.DEBUG))
            {
                Log(string.Concat(" [DEBUG]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.DEBUG);
            }

        }

        public static void Info(object message, bool isShowStack = SHOW_STACK)
        {
            if (LogLevel.INFO == (CurrentLogLevels & LogLevel.INFO))
                Log(string.Concat(" [INFO]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.INFO);
        }

        public static void Warning(object message, bool isShowStack = SHOW_STACK)
        {
            if (LogLevel.WARNING == (CurrentLogLevels & LogLevel.WARNING))
                Log(string.Concat(" [WARNING]: ", isShowStack ? GetStackInfo() : "", message), LogLevel.WARNING);
        }

        public static void Error(object message, bool isShowStack = SHOW_STACK)
        {
            if (LogLevel.ERROR == (CurrentLogLevels & LogLevel.ERROR))
                Log(string.Concat(" [ERROR]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.ERROR);
        }

        public static void Critical(object message, bool isShowStack = SHOW_STACK)
        {
            if (LogLevel.CRITICAL == (CurrentLogLevels & LogLevel.CRITICAL))
                Log(string.Concat(" [CRITICAL]: ", message, '\n', isShowStack ? GetStacksInfo() : ""), LogLevel.CRITICAL);
        }

        public static void Except(Exception ex, object message = null)
        {
            if (LogLevel.EXCEPT == (CurrentLogLevels & LogLevel.EXCEPT))
            {
                Exception innerException = ex;
                while (innerException.InnerException != null)
                {
                    innerException = innerException.InnerException;
                }
                Log(string.Concat(" [EXCEPT]: ", message == null ? "" : message + "\n", ex.Message, innerException.StackTrace), LogLevel.CRITICAL);
            }
        }

        private static string GetStacksInfo()
        {
            StringBuilder sb = new StringBuilder();
            StackTrace st = new StackTrace();
            var sf = st.GetFrames();
            for (int i = 2; i < sf.Length; i++)
            {
                sb.AppendLine(sf[i].ToString());
            }

            return sb.ToString();
        }

        private static void Log(string message, LogLevel level, bool writeEditorLog = true)
        {
            var msg = string.Concat(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff"), message);
            if (null != m_logWriter)
                m_logWriter.WriteLog(msg, level, writeEditorLog);
            //Debugger.Log(0, "TestRPC", message);
        }

        private static string GetStackInfo()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(2);
            var method = sf.GetMethod();
            return string.Format("{0}.{1}(): ", method.ReflectedType.Name, method.Name);
        }

        private static void ProcessExceptionReport(string message, string stackTrace, LogType type)
        {
            var logLevel = LogLevel.DEBUG;
            switch (type)
            {
                case LogType.Assert:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Error:
                    logLevel = LogLevel.ERROR;
                    break;
                case LogType.Exception:
                    logLevel = LogLevel.EXCEPT;
                    break;
                case LogType.Log:
                    logLevel = LogLevel.DEBUG;
                    break;
                case LogType.Warning:
                    logLevel = LogLevel.WARNING;
                    break;
                default:
                    break;
            }

            if (logLevel == (CurrentLogLevels & logLevel))
                Log(string.Concat(" [SYS_", logLevel, "]: ", message, '\n', stackTrace), logLevel, false);
        }
    }

    public class LogWriter
    {
        private string m_logPath = UnityEngine.Application.persistentDataPath + "/log/";
        private string m_logFileName = "log_{0}.txt";
        private string m_logFilePath;
        private FileStream m_fs;
        private StreamWriter m_sw;
        private Action<string, LogLevel, bool> m_logWriter;
        private readonly static object m_locker = new object();

        public LogWriter()
        {
            if (!Directory.Exists(m_logPath))
                Directory.CreateDirectory(m_logPath);
            m_logFilePath = string.Concat(m_logPath, string.Format(m_logFileName, DateTime.Today.ToString("yyyyMMdd")));
            try
            {
                m_logWriter = Write;
                m_fs = new FileStream(m_logFilePath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                m_sw = new StreamWriter(m_fs);

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex.Message);
            }
        }

        public void Release()
        {
            lock (m_locker)
            {
                if (m_sw != null)
                {
                    m_fs.GetType();
                    m_sw.Close();
                    m_sw.Dispose();
                    m_sw = null;
                }

                if (m_fs != null)
                {
                    m_fs.Close();
                    m_fs.Dispose();
                    m_fs = null;
                }
            }
        }

        public void UploadTodayLog()
        {
            /*
            lock (m_locker)
            {
                using (var fs = new FileStream(m_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        var content = sr.ReadToEnd();
                        var fn = Utils.GetFileName(m_logFilePath);//.Replace('/', '\\')
                        //  if (MogoWorld.theAccount != null)
                        {
                       //     fn = string.Concat(MogoWorld.theAccount.name, "_", fn);
                        }
                        DownLoadManager.Instance.UploadLogFile(fn, content);
                    }
                }
            }*/
        }

        public void WriteLog(string msg, LogLevel level, bool writeEditorLog)
        {
            if (null == m_logWriter)
            {
                return;
            }
#if UNITY_IPHONE
            m_logWriter(msg, level, writeEditorLog);
#else
            m_logWriter.BeginInvoke(msg, level, writeEditorLog, null, null);
#endif
        }

        private void Write(string msg, LogLevel level, bool writeEditorLog)
        {
#if UNITY_EDITOR
            lock (m_locker)
               try
                {
                    if (writeEditorLog)
                    {

                        switch (level)
                        {
                            case LogLevel.DEBUG:
                            case LogLevel.INFO:
                                UnityEngine.Debug.LogWarning(msg);
                                break;
                            case LogLevel.WARNING:
                                UnityEngine.Debug.LogWarning(msg);
                                break;
                            case LogLevel.ERROR:
                            case LogLevel.EXCEPT:
                            case LogLevel.CRITICAL:
                                UnityEngine.Debug.LogError(msg);
                                break;
                            default:
                                break;
                        }
                    }
                    if (m_sw != null)
                    {
                        m_sw.WriteLine(msg);
                        m_sw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    if(m_sw != null && m_sw.BaseStream != null)
                    {
                        UnityEngine.Debug.LogError(ex.Message);
                        m_sw.WriteLine(ex.Message); 
                        m_sw.Flush();
                    }
                }
#endif
        }
    }
}
