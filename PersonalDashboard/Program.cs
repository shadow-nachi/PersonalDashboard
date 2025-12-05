using Spectre.Console;
using Hardware.Info;
using System.Runtime.InteropServices;
using Cava;
using System.Diagnostics;

static void Programm()
{
    var layout = CreateLayout();

    // Create static panels
    var mainPanel = CreateMainPanel();
    var calendarPanel = CreateCalendarPanel();

    // Create Cava reader and panel
    var reader = new CavaReader();
    var cavaPanel = new CavaPanel(reader);
    Task.Run(() => reader.Start());

    // Live dashboard loop
    AnsiConsole.Live(layout).Start(ctx =>
    {
        while (true)
        {
            RenderDashboard(layout, mainPanel, calendarPanel, cavaPanel.GetPanel());
            ctx.Refresh();
            Thread.Sleep(33); // ~30 FPS
        }
    });
}
Programm();

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
    
    return new Panel(ReturnSystemInfo())
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]"+ welcomeMessage +"[/]")
        .Expand();
}

static Panel CreateCalendarPanel()
{
    return new Panel("[bold blue]Calendar Placeholder[/]")
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]Calendar[/]")
        .Expand();
}

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

static TimeSpan GetSystemUptime()
{
    return TimeSpan.FromMilliseconds(Environment.TickCount64);
}