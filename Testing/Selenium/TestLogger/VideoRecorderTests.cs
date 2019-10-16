using NUnit.Framework;
using Selenium.testLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Testing.Selenium.TestLogger
{
    [TestFixture]
    public class VideoRecorderTests
    {
        [Test]
        public void FullScreenshot_TakesImageOfFullScreen()
        {
            VideoRecorder rec = new VideoRecorder();

            string outputDir = @"C:\Users\MANNN\Desktop";

            rec.SetDirectory(outputDir);
            rec.SetContinousRecording(true);
            rec.SetFps(1);
            Thread testThread = new Thread(() => rec.StartRecording());


            testThread.Start();

            Thread.Sleep(30000);
            rec.StopRecording();

            Console.WriteLine("Recording Stopped");
            
            rec.SaveVideoFile();

            while (!rec.SaveCompleted) { }
            testThread.Abort();

        }
    }
}
