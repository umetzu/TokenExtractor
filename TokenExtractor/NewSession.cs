using OpenQA.Selenium.Edge;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

internal class NewSession
{
    private readonly string _loginUrl;
    private readonly string _successUrlPart;
    private readonly string _tokenName;

    internal NewSession(string loginUrl, string successUrlPart, string tokenName)
    {
        _loginUrl = loginUrl;
        _successUrlPart = successUrlPart;
        _tokenName = tokenName;
    }

    internal void Execute()
    {
        ConsoleUi.Header("\n=== NEW SESSION ===");
        ConsoleUi.Info("[1/4] Setting up Edge driver (this may take a moment)...");

        new DriverManager().SetUpDriver(new EdgeConfig(), VersionResolveStrategy.MatchingBrowser);

        var options = new EdgeOptions();

        using EdgeDriver driver = new(options);
        try
        {
            ConsoleUi.Info("[2/4] Launching Edge and navigating to login page...");
            driver.Navigate().GoToUrl(_loginUrl);

            ConsoleUi.Step("[3/4] Complete the login in the Edge window.");
            ConsoleUi.Info("[4/4] Monitoring browser for token cookie...");

            var extractedToken = TokenSessionHelper.WaitForToken(driver, _tokenName, _successUrlPart);

            if (extractedToken == null)
            {
                ConsoleUi.Warning("Browser closed before token was captured.");
                return;
            }

            ConsoleUi.Success("\n*** TOKEN CAPTURED ***");
            ConsoleUi.Success(extractedToken);

            TokenSessionHelper.SaveToken(extractedToken, $"{_tokenName}.txt");

        }
        catch (Exception ex)
        {
            ConsoleUi.Error("Error: " + ex.Message);
        }
        finally
        {
            driver.Quit();
        }
    }
}
