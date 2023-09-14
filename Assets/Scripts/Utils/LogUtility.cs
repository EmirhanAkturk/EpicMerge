using UnityEngine;

namespace Utils
{
    public static class LogUtility
    {
        public static void PrintLog(string message, GameObject go = null)
        {
            if(!CanPrintLog()) return;

            if(go == null)
                Debug.Log(message);
            else
                Debug.Log(message, go);
        }
        
        public static void PrintLog(object obj, GameObject go = null)
        {
            if(!CanPrintLog()) return;

            PrintLog(obj.ToString(), go);
        }
    
        public static void PrintColoredLog(string message, LogColor logColor)
        {
            if(!CanPrintLog()) return;
        
            switch (logColor)
            {
                case LogColor.Red:
                    PrintLog($"<color=red> {message} </color>");
                    break;
                case LogColor.Green:
                    PrintLog($"<color=green> {message} </color>");
                    break;
                case LogColor.Blue:
                    PrintLog($"<color=blue> {message} </color>");
                    break;
            }
        }

        public static void PrintColoredError(string message)
        {
            if(!CanPrintLog()) return;
            PrintColoredLog(message, LogColor.Red);
        }

        private static bool CanPrintLog()
        {
#if UNITY_EDITOR
            return true;
#endif
            // if (SRDebug.Instance != null && SRDebug.Instance.Settings != null)
            //     return SRDebug.Instance.Settings.IsEnabled;
            // else
            //     return false;
        }
    }

    public enum LogColor
    {
        Red = 1,
        Green = 2,
        Blue = 3,
    }
}