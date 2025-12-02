using Spectre.Console;

var layout = new Layout()
    .SplitColumns(
        new Layout("left")
        ,
        new Layout("right")
            .SplitRows(
                new Layout("top-right"),
                new Layout("bottom-right")
            )
    );

string UserName = Environment.UserName;
string WelcomeMessage = $"Welcome back, {UserName}!";

var mainPanel = new Panel("[bold Green]" + WelcomeMessage + "[/]")
    .Border(BoxBorder.Rounded)
    .Header("[bold yellow]Personal Dashboard[/]")
    .Expand();
    
var calanderPanel = new Panel("[bold blue]Calendar Placeholder[/]")
    .Border(BoxBorder.Rounded)
    .Header("[bold yellow]Calendar[/]")
    .Expand();
    
var systemInfoPanel = new Panel("[bold blue]System Info Placeholder[/]");

var CavaPlaceholderPanel = new Panel("[bold blue]Cava Placeholder[/]")
    .Border(BoxBorder.Rounded)
    .Header("[bold yellow]Cava Visualization[/]")
    .Expand();
    
static void RenderDashboard(Layout layout, Panel mainPanel, Panel calanderPanel, Panel systemInfoPanel, Panel cavaPlaceholderPanel)
{
    layout["left"].Update(mainPanel);
    layout["right"]["top-right"].Update(calanderPanel);
    layout["right"]["bottom-right"].Update(cavaPlaceholderPanel);
    
    AnsiConsole.Clear();
    AnsiConsole.Write(layout);
}

RenderDashboard(layout, mainPanel, calanderPanel, systemInfoPanel, CavaPlaceholderPanel);
/* var calender = new Calendar(DateTime.Now.Year, DateTime.Now.Month);
var hardwareInfo = new HardwareInfo();
hardwareInfo.RefreshAll();

var os = hardwareInfo.OperatingSystem;
var cpu = hardwareInfo.CpuList.First();
var gpu = hardwareInfo.VideoControllerList.First();
var memory = hardwareInfo.MemoryList;
var drives = hardwareInfo.DriveList;
var nics = hardwareInfo.NetworkAdapterList;

string MachineName = Environment.MachineName;
string KernelVersion = System.Environment.Version.ToString();
string ShellInfo = Environment.GetEnvironmentVariable("SHELL") ?? "N/A";

var OsInfo = new Table();
OsInfo.AddColumn("Category");
OsInfo.AddColumn("Details");
OsInfo.AddRow(os.Version);
OsInfo.AddRow();
 */