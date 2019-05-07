using System.Collections.Generic;
using UnityEngine;

namespace Dmm.Log
{
    public class MyLog : MonoBehaviour
    {
        #region Singleton

        private static MyLog _instance;

        private static MyLog Instance()
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<MyLog>();
            }

            return _instance;
        }

        #endregion

        #region Enable

        public MyLogConfig Config;

        private static MyLogConfig GetConfig()
        {
            var instance = Instance();
            if (instance == null)
            {
                return null;
            }

            return instance.Config;
        }

        public static bool EnableDebug
        {
            get
            {
                var config = GetConfig();
                if (config == null)
                {
                    return false;
                }

                return !config.DisableAll && config.EnableDebug;
            }
        }

        public static bool EnableInfo
        {
            get
            {
                var config = GetConfig();
                if (config == null)
                {
                    return false;
                }

                return !config.DisableAll && config.EnableInfo;
            }
        }

        public static bool EnableWarn
        {
            get
            {
                var config = GetConfig();
                if (config == null)
                {
                    return false;
                }

                return !config.DisableAll && config.EnableWarning;
            }
        }

        public static bool EnableError
        {
            get
            {
                var config = GetConfig();
                if (config == null)
                {
                    return false;
                }

                return !config.DisableAll && config.EnableError;
            }
        }

        #endregion

        #region Raw log

        public static void AssertError(string description, string expect, string actual)
        {
            if (EnableError)
            {
                UnityEngine.Debug.LogError("[" + description + "]：\n---->预期值：" + expect + "\n---->实际值：" + actual);
            }
        }

        public static void Info(string tag, string content)
        {
            if (EnableInfo)
            {
                UnityEngine.Debug.Log(tag + " -> " + content);
            }
        }

        public static void Debug(string tag, string content)
        {
            if (EnableDebug)
            {
                UnityEngine.Debug.Log(tag + " -> " + content);
            }
        }

        public static void Warn(string tag, string content)
        {
            if (EnableWarn)
            {
                UnityEngine.Debug.LogWarning(tag + " -> " + content);
            }
        }

        public static void Error(string tag, string content)
        {
            if (EnableError)
            {
                UnityEngine.Debug.LogError(tag + " -> " + content);
            }
        }

        #endregion

        #region Log with frame

        public static void InfoWithFrame(string tag, string content)
        {
            Info(tag, "|Frame: " + Time.frameCount + "| " + content);
        }

        public static void DebugWithFrame(string tag, string content)
        {
            Debug(tag, "|Frame: " + Time.frameCount + "| " + content);
        }

        public static void ErrorWithFrame(string tag, string content)
        {
            Error(tag, "|Frame: " + Time.frameCount + "| " + content);
        }

        public static void WarnWithFrame(string tag, string content)
        {
            Warn(tag, "|Frame: " + Time.frameCount + "| " + content);
        }

        #endregion

        #region Log Async

        private readonly Queue<LogItem> _logItems = new Queue<LogItem>();

        public static void InfoAsync(string tag, string content)
        {
            var ins = Instance();
            if (!ins) return;

            var item = new LogItem();
            item.Tag = tag;
            item.Content = content;
            item.Level = LogLevel.Info;
            item.WithFrame = false;

            ins._logItems.Enqueue(item);
        }

        public static void DebugAsync(string tag, string content)
        {
            var ins = Instance();
            if (!ins) return;

            var item = new LogItem();
            item.Tag = tag;
            item.Content = content;
            item.Level = LogLevel.Debug;
            item.WithFrame = false;

            ins._logItems.Enqueue(item);
        }

        public static void WarnAsync(string tag, string content)
        {
            var ins = Instance();
            if (!ins) return;

            var item = new LogItem();
            item.Tag = tag;
            item.Content = content;
            item.Level = LogLevel.Warn;
            item.WithFrame = false;

            ins._logItems.Enqueue(item);
        }

        public static void ErrorAsync(string tag, string content)
        {
            var ins = Instance();
            if (!ins) return;

            var item = new LogItem();
            item.Tag = tag;
            item.Content = content;
            item.Level = LogLevel.Error;
            item.WithFrame = false;

            ins._logItems.Enqueue(item);
        }

        public void LateUpdate()
        {
            if (_logItems.Count > 0)
            {
                var item = _logItems.Dequeue();
                switch (item.Level)
                {
                    case LogLevel.Info:
                        if (item.WithFrame)
                            InfoWithFrame(item.Tag, item.Content);
                        else
                            Info(item.Tag, item.Content);
                        break;

                    case LogLevel.Warn:
                        if (item.WithFrame)
                            WarnWithFrame(item.Tag, item.Content);
                        else
                            Warn(item.Tag, item.Content);
                        break;

                    case LogLevel.Debug:
                        if (item.WithFrame)
                            DebugWithFrame(item.Tag, item.Content);
                        else
                            Debug(item.Tag, item.Content);
                        break;

                    case LogLevel.Error:
                        if (item.WithFrame)
                            ErrorWithFrame(item.Tag, item.Content);
                        else
                            Error(item.Tag, item.Content);
                        break;
                }
            }
        }

        #endregion
    }

    public class LogItem
    {
        public string Tag;
        public string Content;
        public bool WithFrame;
        public LogLevel Level;
    }

    public enum LogLevel
    {
        Info,
        Warn,
        Debug,
        Error
    }
}