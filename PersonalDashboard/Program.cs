using Spectre.Console;
using Hardware.Info;

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

static Panel CreateCavaPanel()
{
    return new Panel("[bold blue]Cava Placeholder[/]")
        .Border(BoxBorder.Rounded)
        .Header("[bold yellow]Cava Visualization[/]")
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

static Tree BuildSystemInfoTree(HardwareInfo hardwareInfo)
{   
    string machineName = Environment.MachineName;
    var osInfo = new Tree(machineName);
    var software = osInfo.AddNode("Software");
    
    var os = hardwareInfo.OperatingSystem;
    software.AddNode("OS: " + os.Name);
    software.AddNode("Kernel Version: " + Environment.Version.ToString());
    software.AddNode("Uptime: " + GetSystemUptime().ToString(@"dd\.hh\:mm\:ss"));
    software.AddNode("Shell: " + (Environment.GetEnvironmentVariable("SHELL") ?? "N/A"));

    var hardware = osInfo.AddNode("Hardware");
    hardware.AddNode("Cpu: " + hardwareInfo.CpuList[0].Name);
    hardware.AddNode("Ram: " + (hardwareInfo.MemoryStatus.TotalPhysical / (1024 * 1024 * 1024)) + " GB");
    hardware.AddNode("Gpu: " + Markup.Escape(hardwareInfo.VideoControllerList[0].Name));
    hardwareInfo.RefreshDriveList();
    foreach (var drive in hardwareInfo.DriveList)
    {
        var Drive = hardware.AddNode("Drive: " + drive.Name + " (" + (drive.Size / (1024 * 1024 * 1024)) + " GB)");
        foreach (var partition in drive.PartitionList)
        {
            Drive.AddNode("Partition: " + partition.Name + " (" + (partition.Size / (1024 * 1024 * 1024)) + " GB)");
        }
    }
    
    return osInfo;
}

static TimeSpan GetSystemUptime()
{
    return TimeSpan.FromMilliseconds(Environment.TickCount64);
}