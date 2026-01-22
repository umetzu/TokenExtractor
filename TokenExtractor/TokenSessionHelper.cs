using OpenQA.Selenium;

internal static class TokenSessionHelper
{
    internal static string? WaitForToken(IWebDriver driver, string tokenName, string? successUrlPart = null)
    {
        string? extractedToken = null;

        while (extractedToken == null)
        {
            Thread.Sleep(1000);

            if (!IsBrowserOpen(driver)) return null;

            if (!string.IsNullOrWhiteSpace(successUrlPart) && !driver.Url.Contains(successUrlPart))
            {
                Console.WriteLine($"Waiting for user to reach {successUrlPart}...");
                continue;
            }

            var cookie = driver.Manage().Cookies.GetCookieNamed(tokenName);
            if (cookie != null)
            {
                extractedToken = cookie.Value;
            }
        }

        return extractedToken;
    }

    internal static void SaveToken(string tokenValue, string fileName)
    {
        File.WriteAllText(fileName, tokenValue);
        Console.WriteLine($"Token saved to {fileName}");
    }

    private static bool IsBrowserOpen(IWebDriver driver)
    {
        try
        {
            var _ = driver.Title;
            return true;
        }
        catch (WebDriverException)
        {
            return false;
        }
    }
}
