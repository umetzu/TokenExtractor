using System.Security.Principal;

const string defaultLoginUrl = "https://cfg.aramarklabor.com/workforcesoftware/air";
const string defaultSuccessUrlPart = "/air";
const string defaultTokenName = "wfm_token";
const string defaultEdgePath = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe";
const string defaultDebugPort = "9222";

var identity = WindowsIdentity.GetCurrent();
var isAdmin = identity != null && new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);

if (isAdmin)
{
	ConsoleUi.Warning("Running elevated. Restart as a standard user to use mode 2.\n");
}

ConsoleUi.Header("=== Token Extractor ===");
ConsoleUi.Info("Select a mode:");
ConsoleUi.Step("1) Start a NEW browser session");
ConsoleUi.Step("2) Use an EXISTING browser session");
ConsoleUi.Prompt("Enter choice (1 or 2): ");

var selection = Console.ReadLine();

switch (selection)
{
    case "1":
        RunNewSession();
        break;
    case "2":
        RunExistingSession();
        break;
    default:
        ConsoleUi.Warning("Invalid selection. Exiting.");
        break;
}

void RunExistingSession()
{
    string edgePath = defaultEdgePath;
    string debugPort = defaultDebugPort;
    string loginUrl = defaultLoginUrl;
    string successUrlPart = defaultSuccessUrlPart;
    string tokenName = defaultTokenName;

    if (ShouldChangeDefaults())
    {
        edgePath = PromptForValue("edgePath", defaultEdgePath);
        debugPort = PromptForValue("debugPort", defaultDebugPort);
        loginUrl = PromptForValue("loginUrl", defaultLoginUrl);
        successUrlPart = PromptForValue("successUrlPart", defaultSuccessUrlPart);
        tokenName = PromptForValue("tokenName", defaultTokenName);
    }

    var existingSession = new ExistingSession(edgePath, debugPort, loginUrl, successUrlPart, tokenName, isAdmin);
    existingSession.Execute();
}

void RunNewSession()
{
    string loginUrl = defaultLoginUrl;
    string successUrlPart = defaultSuccessUrlPart;
    string tokenName = defaultTokenName;

    if (ShouldChangeDefaults())
    {
        loginUrl = PromptForValue("loginUrl", defaultLoginUrl);
        successUrlPart = PromptForValue("successUrlPart", defaultSuccessUrlPart);
        tokenName = PromptForValue("tokenName", defaultTokenName);
    }

    var session = new NewSession(loginUrl, successUrlPart, tokenName);
    session.Execute();
}

string PromptForValue(string name, string currentValue)
{
    ConsoleUi.Prompt($"Enter {name} (Press Enter to keep current: {currentValue}): ");
    var input = Console.ReadLine();
    return string.IsNullOrWhiteSpace(input) ? currentValue : input;
}

bool ShouldChangeDefaults()
{
    ConsoleUi.Info("\nCurrent defaults:");
    ConsoleUi.Step($"- Edge path: {defaultEdgePath}");
    ConsoleUi.Step($"- Debug port: {defaultDebugPort}");
    ConsoleUi.Step($"- Login URL: {defaultLoginUrl}");
    ConsoleUi.Step($"- Success URL part: {defaultSuccessUrlPart}");
    ConsoleUi.Step($"- Token name: {defaultTokenName}");
    ConsoleUi.Prompt("Would you like to change these? (y/N): ");
    var input = Console.ReadLine();
    return input?.Trim().Equals("y", System.StringComparison.OrdinalIgnoreCase) == true;
}
