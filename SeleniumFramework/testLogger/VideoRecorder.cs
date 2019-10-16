using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Accord.Math;
using Accord.Video;
using Accord.Video.FFMPEG;

namespace Selenium.testLogger
{
    public class VideoRecorder
    {//will need to launch this as a seperate thread to stop race conditions
        public Dictionary<TimeSpan, byte[]> ImageBuffer { get; private set; } = new Dictionary<TimeSpan, byte[]>();
        public double Fps { get; private set; } = 10;
        public int BufferSizeInFrames { get; private set; } = 150;
        public bool ContinuousRecording { get; private set; } = false;
        public bool Record { get; private set; } = false;//when set to true the recording starts, when false it stops
        private System.Timers.Timer timer = new System.Timers.Timer { Enabled = true };
        public bool SaveCompleted { get; private set; } = false;
        public string SaveDirectory { get; private set; }
        public bool Saving { get; private set; } = false;
        public DateTime RecordingStartTime { get; private set; }
        public VideoFileWriter videoWriter { get; private set; } = new VideoFileWriter();
        public int ScreenWidth { get; private set; } = Screen.PrimaryScreen.Bounds.Width;
        public int ScreenHeight { get; private set; } = Screen.PrimaryScreen.Bounds.Height;

        //-------------------------------------------------------
        public void SetContinousRecording(bool cont)
        {
            this.ContinuousRecording = cont;
        }
        public void SetBufferInSeconds(int seconds)
        {
            this.BufferSizeInFrames = (int)Math.Round(this.Fps * seconds);
        }
        public void SetBufferInFrames(int buffer)
        {
            this.BufferSizeInFrames = buffer;
        }
        public void SetFps(int fps)
        {
            this.Fps = fps;
        }
        public void SetDirectory(String dir)
        { this.SaveDirectory = dir; }

        //-------------------------------------------------------


        //-------------------------------------------------------


        public void StartRecording()
        {
            this.Record = true;
            //start stopwatch
            timer.Elapsed += new ElapsedEventHandler(Timer_Tick);

            //milliseconds per frame
            double frameTime = 1000 / Fps;

            videoWriter.FrameRate = new Rational(frameTime);
            videoWriter.VideoCodec = VideoCodec.H265;
            videoWriter.VideoOptions["crf"] = "18"; // visually lossless
            videoWriter.VideoOptions["preset"] = "veryfast";
            videoWriter.VideoOptions["tune"] = "zerolatency";
            videoWriter.VideoOptions["x264opts"] = "no-mbtree:sliced-threads:sync-lookahead=0";
            videoWriter.Width = ScreenWidth;
            videoWriter.Height = ScreenHeight;

            timer.Interval = frameTime; // in miliseconds   
            
            if(ContinuousRecording)
            {
                videoWriter.Open(SaveDirectory + "\\file.mp4");
            }

            RecordingStartTime = DateTime.Now;
            timer.Start();
        }

        public void Timer_Tick(object sender, EventArgs e)
        {
            if (Record)
            {
                TakeFullScreenShot();
            }
        }

        public void StopRecording()
        {
            this.Record = false;//stops the buffer loop
            this.timer.Stop();
            if(ContinuousRecording)
            {
                videoWriter.Close();
            }
        }

        public void SaveVideoFile()
        {
            if (!Saving && !ContinuousRecording)
            {
                Saving = true; 

                videoWriter.Open(SaveDirectory + "\\file.mp4");

                var orderedTimeStamps = (ImageBuffer.Keys).OrderBy(o => o.Ticks).ToList();

                foreach (var frame in orderedTimeStamps)
                {
                    var adjustedFrame = frame - orderedTimeStamps[0];

                    videoWriter.WriteVideoFrame((Bitmap)Image.FromStream(new MemoryStream(ImageBuffer[frame])), adjustedFrame);
                }
                videoWriter.Close();

                SaveCompleted = true;
                Saving = false;
            }
        }

        public void TakeFullScreenShot()
        {
            if (Record && !Saving)
            {
                try
                {
                    Bitmap bmp = new Bitmap(ScreenWidth,ScreenHeight);
                    using (Graphics g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                    }
                    var frameTime = DateTime.Now;
                    byte[] result = null;
                    using (MemoryStream stream = new MemoryStream())
                    {
                        bmp.Save(stream, ImageFormat.Png);
                        result = stream.ToArray();
                    }

                    TimeSpan timeStamp = frameTime - RecordingStartTime;

                    if (!ContinuousRecording)
                    {
                        PlaceImageInBuffer(timeStamp, result);
                    }else
                    {
                        ContinuousRecordingSave(bmp, timeStamp);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public void ContinuousRecordingSave(Bitmap bmp, TimeSpan timeStamp)
        {
            //TODO: throwing exceptions, I think because race conditions. Need to use a buffer to store frames while others are written. might need an extra thread.
            videoWriter.WriteVideoFrame(bmp, timeStamp);
        }
        
        public void PlaceImageInBuffer(TimeSpan frameTime, byte[] frame)
        {
            //check if all frames are taken
            if ((ImageBuffer.Count() < BufferSizeInFrames) || ContinuousRecording)//Look at Screencapture example to set up Continuous recording
            { ImageBuffer.Add(frameTime, frame); }
            else
            {
                //StopRecording();
                ClearSectionOfBuffer();
                ImageBuffer.Add(frameTime, frame);

                //Console.WriteLine("buffer Full");
            }
        }

        public void ClearSectionOfBuffer()
        {//shift all the images along one second and place new image.
            foreach (var k in ImageBuffer.Keys.OrderBy(k => k).Take((int)Math.Round(Fps))) ImageBuffer.Remove(k);
        }

    }
}
