using OpenQA.Selenium.Edge;
using System.Diagnostics;

internal class ExistingSession(string edgePath, string debugPort, string loginUrl, string successUrlPart, string tokenName, bool isAdmin)
{
    internal void Execute()
    {
        ConsoleUi.Header("\n=== EXISTING SESSION ===");
        ConsoleUi.Info("We will restart Edge in debug mode to access your existing cookies.");
        ConsoleUi.Info("Steps:");
        ConsoleUi.Step("1) CLOSE all open Microsoft Edge windows.");
        ConsoleUi.Step("2) Return here and press any key when Edge is fully closed.");
        Console.ReadKey();

        try
        {
            var userDataDir = GetDefaultUserDataDir();

            if (!HasAccessToUserDataDir(userDataDir))
            {
                ConsoleUi.Error("\nAccess to the Edge user data folder is blocked (likely by Windows Ransomware Protection / Controlled Folder Access).");
                ConsoleUi.Info("To allow access:");
                ConsoleUi.Step("1) Open Windows Security > Virus & threat protection.");
                ConsoleUi.Step("2) Under Ransomware protection, choose 'Manage ransomware protection'.");
                ConsoleUi.Step("3) Select 'Allow an app through Controlled folder access' and add this application's executable.");
                ConsoleUi.Warning("After granting access, close Edge and rerun this option.");
                return;
            }

            var edgeProcess = new Process();
            edgeProcess.StartInfo.FileName = edgePath;
            edgeProcess.StartInfo.Arguments = $"--remote-debugging-port={debugPort} --user-data-dir=\"{userDataDir}\"";
            var p = edgeProcess.Start();

            ConsoleUi.Info("Launching Edge in debug mode...");
            Thread.Sleep(2000);

            if (edgeProcess.HasExited)
            {
                ConsoleUi.Warning($"Failed to launch edge with profile: {userDataDir}.");
                ConsoleUi.Warning($"{ (isAdmin ? "Running elevated. Restart as a standard user or use mode " : "Try mode ") }1.\n");

                return;
            }

            ConsoleUi.Info($"Connected to Edge on port {debugPort}. If the browser does not open, ensure it was fully closed and retry.");
            var options = new EdgeOptions
            {
                DebuggerAddress = "127.0.0.1:" + debugPort
            };

            using EdgeDriver driver = new (options);

            if (!driver.Url.Contains(loginUrl))
            {
                driver.Navigate().GoToUrl(loginUrl);
            }

            ConsoleUi.Info("Monitoring Edge for token cookie...");

            var token = TokenSessionHelper.WaitForToken(driver, tokenName, successUrlPart);

            if (token != null)
            {
                ConsoleUi.Success("\n*** TOKEN CAPTURED ***");
                ConsoleUi.Success(token);
                TokenSessionHelper.SaveToken(token, $"{tokenName}.txt");
            }
            else
            {
                ConsoleUi.Warning("Browser closed before token was captured.");
            }
        }
        catch (Exception ex)
        {
            ConsoleUi.Error($"Error: {ex.Message}");
            ConsoleUi.Warning("Ensure all Edge windows were closed before starting.");
        }

        ConsoleUi.Info("\nPress any key to exit...");
        Console.ReadKey();
    }

    static string GetDefaultUserDataDir()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, @"Microsoft\Edge\User Data");
    }

    static bool HasAccessToUserDataDir(string path)
    {
        try
        {
            Directory.CreateDirectory(path);
            var probe = Path.Combine(path, ".write-test.tmp");
            File.WriteAllText(probe, "probe");
            try
            {
                if (File.Exists(probe))
                {
                    File.Delete(probe);
                }
            }
            catch { }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}