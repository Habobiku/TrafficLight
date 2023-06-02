namespace Traffic
{
    public enum LightColor
    {
        Red,
        Yellow,
        Green
    }

    public class TrafficLight
    {
        private readonly int _redTime;
        private readonly int _maxGreenTime;
        private readonly int _greenTime;
        private const int YellowTime = 5;
        public int TempGreenLight;
        public DateTime GreenStartTime;

        private List<(Dictionary<LightColor, bool> colors, int duration)> _stateTransitions = new();
        private int _currentStateIndex;
        private Dictionary<LightColor, bool> _currentState = new();

        private CancellationTokenSource _cancellationTokenSource = new();

        public TrafficLight(int redTime, int minGreenTime, int maxGreenTime)
        {
            _redTime = redTime;
            _maxGreenTime = maxGreenTime;
            _greenTime = new Random().Next(minGreenTime, maxGreenTime);
            _currentStateIndex = 0;
            SetState();
        }

        private void SetState()
        {
            _stateTransitions = new List<(Dictionary<LightColor, bool> colors, int duration)>()
            {
                (new Dictionary<LightColor, bool> { { LightColor.Red, true }, { LightColor.Yellow, false }, { LightColor.Green, false } },
                    _redTime),
                (new Dictionary<LightColor, bool> { { LightColor.Red, true }, { LightColor.Yellow, true }, { LightColor.Green, false } },
                    YellowTime),
                (new Dictionary<LightColor, bool> { { LightColor.Red, false }, { LightColor.Yellow, false }, { LightColor.Green, true } },
                    _greenTime),
                (new Dictionary<LightColor, bool> { { LightColor.Red, false }, { LightColor.Yellow, true }, { LightColor.Green, false } },
                    YellowTime)
            };
            _currentState = _stateTransitions[_currentStateIndex].colors;
        }

        public async Task StartAsync()
        {
            ListenForPedestrianButtonAsync();

            while (true)
            {
                DisplayCurrentColor();

                if (_currentState[LightColor.Green])
                    GreenStartTime = DateTime.Now;

                try
                {
                    await Task.Delay(GetCurrentStateDuration() * 1000, _cancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Button pressed");
                    await Task.Delay(TempGreenLight * 1000);
                }

                ChangeState();
            }
        }

        private void DisplayCurrentColor()
        {
            Console.Clear();
            Console.WriteLine("TRAFFIC LIGHT");
            Console.WriteLine("-------------");
            Console.WriteLine($"RED:    {(_currentState[LightColor.Red] ? "ON" : "OFF")}");
            Console.WriteLine($"YELLOW: {(_currentState[LightColor.Yellow] ? "ON" : "OFF")}");
            Console.WriteLine($"GREEN:  {(_currentState[LightColor.Green] ? "ON" : "OFF")}");
        }

        public int GetCurrentStateDuration()
        {
            return _stateTransitions[_currentStateIndex].duration;
        }

        public void ChangeState()
        {
            _currentStateIndex = (_currentStateIndex + 1) % _stateTransitions.Count;
            _currentState = _stateTransitions[_currentStateIndex].colors;
        }

        public void ExtendGreenTime()
        {
            DateTime currentTime = DateTime.Now;
            TimeSpan remainingGreenTime = TimeSpan.FromSeconds(_greenTime) - (currentTime - GreenStartTime);
            TimeSpan additionalGreenTime = TimeSpan.FromSeconds(30);
            int remainingMaxGreenTime = _maxGreenTime - remainingGreenTime.Seconds;
            
            TempGreenLight = additionalGreenTime <= TimeSpan.FromSeconds(remainingMaxGreenTime)
                ? additionalGreenTime.Seconds
                : remainingMaxGreenTime;
        }

        private void ListenForPedestrianButtonAsync()
        {
            Task.Run(ListenForPedestrianButton);
        }
        
        private void ListenForPedestrianButton()
        {
            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Spacebar)
                {
                    if (_stateTransitions[_currentStateIndex].colors[LightColor.Green])
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource = new CancellationTokenSource();
                        ExtendGreenTime();
                    }
                }
            }
        }
    }

    class Program
    {
        static async Task Main()
        {
            int redTime = 120;
            int minGreenTime = 120;
            int maxGreenTime = 360;

            TrafficLight trafficLight = new(redTime, minGreenTime, maxGreenTime);
            await trafficLight.StartAsync();
        }
    }
}
