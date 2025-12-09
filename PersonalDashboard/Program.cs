using Spectre.Console;
using Hardware.Info;
using System.Runtime.InteropServices;

// [ Rendering ]

var layout = CreateLayout();
RenderDashboard(layout, CreateMainPanel(), CreateCalendarPanel(), CreateCavaPanel());

static Layout CreateLayout()
{
    return new Layout()
        .SplitColumns(
            new Layout("left"),
            new Layout("right")
                .SplitRows(
                    new Layout("top-right"),
                    new Layout("bottom-right")
                )
        );
}

static void RenderDashboard(Layout layout, Panel mainPanel, Panel calendarPanel, Panel cavaPanel)
{
    layout["left"].Update(mainPanel);
    layout["right"]["top-right"].Update(calendarPanel);
    layout["right"]["bottom-right"].Update(cavaPanel);
    
    AnsiConsole.Clear();
    AnsiConsole.Write(layout);
}

static Panel CreateMainPanel()
{
    string userName = Environment.UserName;
    string welcomeMessage = $"Welcome back, {userName}!";

    var systemInfoPanel = new Panel(ReturnSystemInfo())
        .Border(BoxBorder.Rounded)
        .Header("[green]System Info[/]");

    var motdPanel = CreateMessageOfTheDayPanel()
        .Border(BoxBorder.Rounded)
        .Header("[cyan]Message of the Day[/]");

    var grid = new Grid();
    grid.AddColumn();
    grid.AddColumn();
    grid.AddRow(systemInfoPanel);
    grid.AddRow(motdPanel);

    var mainPanel = new Panel(grid)
        .Border(BoxBorder.Rounded)
        .Header($"[bold yellow]{welcomeMessage}[/]")
        .Expand();

    return mainPanel;
}

static Panel CreateCalendarPanel()
{
    return new Panel(CreateCalander())
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]Calendar[/]")
        .Expand();
}

// [ Side Pannels ]

static Calendar CreateCalander()
{
    var calender = new Calendar(DateTime.Now.Year, DateTime.Now.Month);
        calender.AddCalendarEvent(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
        calender.HighlightStyle("bold red");
    return calender;
}

static Panel CreateCavaPanel()
{
    return new Panel("[bold blue]Cava Placeholder[/]")
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]Cava Visualization[/]")
        .Expand();
}

// [ Fun Pannels ]

static Panel CreateMessageOfTheDayPanel()
{
    int Today = DateTime.Today.DayOfYear;

    int Christmas = (new DateTime(Today > new DateTime(DateTime.Today.Year, 12, 25).DayOfYear 
        ? DateTime.Today.Year + 1 : DateTime.Today.Year, 12, 25) - DateTime.Today).Days;

    int Halloween = (new DateTime(Today > new DateTime(DateTime.Today.Year, 10, 31).DayOfYear 
        ? DateTime.Today.Year + 1 : DateTime.Today.Year, 10, 31) - DateTime.Today).Days;

    string[][] motd =
    [
        // General
        [
            "Keep your Head up!!", 
            "You are doing great!", 
            "Hang in there!", 
            "Believe in yourself!", 
            "It's Almost over", 
            "<3",
            "Stay Hydrated!",
            "Take breaks!",
            "Listen to your favorite music!",
            "Today will be a good day!"
            
        ],
        // Spring
        [
            "Spring is here!", 
            "Time for new beginnings!", 
            "Fresh start ahead!"
        ],
        // Summer
        [
            "Enjoy the sunshine!", 
            "Summer vibes!", 
            "Make the most of it!", 
            "Sweat bother time!"
        ],
        // Fall
        [
            "Fall is beautiful!", 
            "Cozy season ahead!", 
            "Teatime!", 
            $"Spooky season in {Halloween} days!"
        ],
        // Winter
        [
            $"Just {Christmas} days until Christmas!", 
            "Stay warm!", 
            "Warm Chocolate time!", 
            "Maybe we'll see snow today"
        ]
    ];

    Random randMessage = new Random();
    float seasnalMessageChance = 0.3f;
    bool isSeasonalMessage = randMessage.NextDouble() < seasnalMessageChance;
    string selectedMessage;
    if (isSeasonalMessage)
    {
        int month = DateTime.Now.Month;
        int seasonIndex = month switch
        {
            3 or 4 or 5 => 1, // Spring
            6 or 7 or 8 => 2, // Summer
            9 or 10 or 11 => 3, // Fall
            12 or 1 or 2 => 4, // Winter
            _ => 0 // General
        };

        var seasonalMessages = motd[seasonIndex];
        selectedMessage = seasonalMessages[randMessage.Next(seasonalMessages.Length)];
    }
    else
    {
        var generalMessages = motd[0];
        selectedMessage = generalMessages[randMessage.Next(generalMessages.Length)];
    }
    if(randMessage.NextDouble() < 0.01f) // Vent time :3
    {
        selectedMessage = "Okay I am done being nice now, do you know how fucking hard it was to implement this shit?! How the world is going to shit and all you think of is to run a Fetch script every day? Well I am done, fuck you go fuck yourself I am done...";
    }
    return new Panel("[bold green]" + selectedMessage + "[/]")
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]Message of the Day[/]");
};

// [ Hardware Info Part ]

static Tree ReturnSystemInfo()
{
    var hardwareInfo = GetHardwareInfo();
    var osInfo = BuildSystemInfoTree(hardwareInfo);
    
    return osInfo;
}

static HardwareInfo GetHardwareInfo()
{
    var hardwareInfo = new HardwareInfo();
    hardwareInfo.RefreshAll();
    return hardwareInfo;
}

static string DetectDisplay()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        return "Windows";

    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        return "macOS";

    var sessionType = Environment.GetEnvironmentVariable("XDG_SESSION_TYPE")?.ToLowerInvariant();

    switch (sessionType)
    {
        case "wayland":
            return "Wayland";
        case "x11":
            return "X11";
    }

    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WAYLAND_DISPLAY")))
        return "Wayland";

    if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DISPLAY")))
        return "X11";

    return "Unknown";
}

static Tree BuildSystemInfoTree(HardwareInfo hardwareInfo)
{   
    string machineName = Environment.MachineName;
    var osInfo = new Tree(machineName);
    var software = osInfo.AddNode("Software");
    
    var os = hardwareInfo.OperatingSystem;
    software.AddNode("OS: " + os.Name);
    software.AddNode("Kernel Version: " + File.ReadAllText("/proc/version").Trim());
    software.AddNode("Uptime: " + GetSystemUptime().ToString(@"dd\.hh\:mm\:ss"));
    software.AddNode("Shell: " + (Environment.GetEnvironmentVariable("SHELL") ?? "N/A"));
    software.AddNode("Display Server: " + DetectDisplay());

    var hardware = osInfo.AddNode("Hardware");
    hardware.AddNode("Cpu: " + hardwareInfo.CpuList[0].Name);
    hardware.AddNode("Ram: " + (hardwareInfo.MemoryStatus.TotalPhysical / (1024 * 1024 * 1024)) + " GB");
    hardware.AddNode("Gpu: " + Markup.Escape(hardwareInfo.VideoControllerList[0].Name));
    
    return osInfo;
}

//  [ Utility Functions ]

static TimeSpan GetSystemUptime()
{
    return TimeSpan.FromMilliseconds(Environment.TickCount64);
}