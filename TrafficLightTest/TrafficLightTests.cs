namespace TrafficLightTest
{
    public class TrafficLightTests
    {
        private TrafficLight? _trafficLight;

        [SetUp]
        public void Setup()
        {
            _trafficLight = new TrafficLight(10, 5, 15);
        }

        [Test]
        [TestCase(6, 6, 6)]
        public void TrafficLightStartAsyncDisplaysCorrectOutput(int redTime, int minGreenTime, int maxGreenTime,
            int yellowTime = 5)
        {
            _trafficLight = new(redTime, minGreenTime, maxGreenTime);
            var output = new StringWriter();
            Console.SetOut(output);
            Task startTask = _trafficLight.StartAsync();

            var expectedOutput = "TRAFFIC LIGHT" +
                                 "\n-------------\n" +
                                 "RED:    ON\nYELLOW: OFF\nGREEN:  OFF\n";
            Assert.That(output.ToString(), Is.EqualTo(expectedOutput));
        }

        [Test]
        [TestCase(6, 6, 6)]
        public async Task TrafficLightStartAsyncCyclesThroughStates(int redTime, int minGreenTime, int maxGreenTime, int yellowTime = 5)
        {
            _trafficLight = new(redTime, minGreenTime, maxGreenTime);

            Task startTask = _trafficLight.StartAsync();
            await Task.Delay(TimeSpan.FromSeconds(redTime + minGreenTime + yellowTime));

            Assert.Multiple(() =>
            {
                Assert.That(startTask.IsFaulted, Is.False, "The StartAsync method should not throw an exception.");
                Assert.That(startTask.IsCanceled, Is.False, "The StartAsync method should not be canceled.");
                Assert.That(startTask.IsCompleted, Is.False, "The StartAsync method should run indefinitely.");
            });
        }

        [Test]
        [TestCase(6, 120, 360)]
        [TestCase(10, 200, 250)]
        [TestCase(6, 81, 90)]
        public void TrafficLightPedestrianButtonPressedAddingTime(int redTime, int minGreenTime, int maxGreenTime,
            int yellowTime = 5)
        {
            int expectedTime = 30;

            _trafficLight = new(redTime, minGreenTime, maxGreenTime)
            {
                GreenStartTime = DateTime.Now.AddSeconds(-yellowTime)
            };
            _trafficLight.ExtendGreenTime();

            Assert.That(_trafficLight.TempGreenLight, Is.EqualTo(expectedTime));
        }

        [Test]
        [TestCase(6, 15, 15)]
        [TestCase(6, 20, 25)]
        [TestCase(6, 10, 19)]
        public void TrafficLightPedestrianButtonPressedNoAddingTime(int redTime, int minGreenTime, int maxGreenTime, int yellowTime = 5)
        {
            int notExpected=30;
            _trafficLight = new(redTime, minGreenTime, maxGreenTime)
            {
                GreenStartTime = DateTime.Now.AddSeconds(-5)
            };
           
            _trafficLight.ExtendGreenTime();

            Assert.That(_trafficLight.TempGreenLight, Is.Not.EqualTo(notExpected));
        }

        [Test]
        [TestCase(1, 2, 2)]
        [TestCase(1, 5, 5)]
        [TestCase(10, 7, 7)]
        public void TestGetCurrentStateDuration(int redTime, int minGreenTime, int maxGreenTime, int yellowTime = 5)
        {
            _trafficLight = new(redTime, minGreenTime, maxGreenTime);
            var resultRed = _trafficLight.GetCurrentStateDuration();
            _trafficLight.ChangeState();
            var resultYellow = _trafficLight.GetCurrentStateDuration();
            _trafficLight.ChangeState();
            var resultGreen = _trafficLight.GetCurrentStateDuration();
            Assert.Multiple(() =>
            {
                Assert.That(yellowTime, Is.EqualTo(resultYellow));
                Assert.That(minGreenTime, Is.EqualTo(resultGreen));
                Assert.That(resultRed, Is.EqualTo(redTime));
            });
        }
    }
}
