using System.Diagnostics;
using Spectre.Console;


namespace Cava
{
    public class CavaReader
    {
        public event Action<int[]>? OnFrame;
        public void Start()
        {
            var proc = new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = "cava",
                    Arguments = "-p raw",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };
             proc.OutputDataReceived += (s, e) =>
            {
                if (e.Data == null) return;
            };

            proc.Start();

            // Cava RAW: reads bytes continuously
            var buffer = new byte[128]; // enough for ~64 bars
            while (true)
            {
                int read = proc.StandardOutput.BaseStream.Read(buffer, 0, buffer.Length);
                if (read <= 0) break;

                int bars = read;
                int[] values = new int[bars];

                for (int i = 0; i < bars; i++)
                    values[i] = buffer[i];

                OnFrame?.Invoke(values);
            }   
        }
    }
    public class CavaPanel
    {
        private int[] _bars = Array.Empty<int>();

        public CavaPanel(CavaReader reader)
        {
            reader.OnFrame += data => _bars = data;
        }

        public Panel GetPanel()
        {
            var canvas = new Canvas(_bars.Length, 20);

            for (int x = 0; x < _bars.Length; x++)
            {
                int height = Math.Clamp(_bars[x] / 5, 0, 19); // scale

                for (int y = 19; y >= 20 - height; y--)
                    canvas.SetPixel(x, y, Color.Green);
            }

            return new Panel(canvas)
            {
                Header = new PanelHeader("Audio Visualizer"),
                Border = BoxBorder.Rounded
            };
        }
    }
}