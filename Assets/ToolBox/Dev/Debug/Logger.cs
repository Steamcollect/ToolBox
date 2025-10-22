namespace ToolBox.Debug
{
    using UnityEngine;

    public static class Logger
    {
        private static string s_LogFilePath;
        public static string LogFilePath => s_LogFilePath;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
#if !UNITY_EDITOR
        // Documents/<NomProjet>/
        string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string projectFolder = Path.Combine(documentsPath, Application.productName);

        if (!Directory.Exists(projectFolder))
        {
            Directory.CreateDirectory(projectFolder);
        }

        // Chemin complet du fichier
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        s_LogFilePath = Path.Combine(projectFolder, $"SignalLostLog_v"+ Application.version +"_"+timestamp+".txt");

        try
        {
            File.WriteAllText(s_LogFilePath, $"=== Log d�marr� le {DateTime.Now} ===\n");
        }
        catch (Exception e)
        {
            Debug.LogError("Impossible de cr�er le fichier log: " + e.Message);
        }

        Application.logMessageReceived += HandleLog;
#endif
        }



        public static void Log(string message, bool isVerbose = true)
        {
            if (!isVerbose) return;
            Debug.Log(message);
        }

        public static void LogWarning(string message, bool isVerbose = true)
        {
            if (!isVerbose) return;
            Debug.LogWarning(message);
        }

        public static void LogError(string message, bool isVerbose = true)
        {
            if (!isVerbose) return;
            Debug.LogError(message);
        }

#if !UNITY_EDITOR
    private static void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{DateTime.Now:HH:mm:ss} [{type}] {logString}\n";

        if (type == LogType.Error || type == LogType.Exception)
            logEntry += stackTrace + "\n";

        try
        {
            File.AppendAllText(s_LogFilePath, logEntry);
        }
        catch (Exception e)
        {
            Debug.LogError("Erreur lors de l��criture du log: " + e.Message);
        }
    }
#endif
    }
}