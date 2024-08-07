using FrameValueReporter.Models;
using FrameValueReporter.ValueReporter;
using System.Runtime.InteropServices;

namespace FrameValueReporter.FrameCallback
{
    public class FrameCallback : IFrameCallback
    {
        readonly IValueReporter valueReporter;
        public delegate void FrameUpdateHandler(Frame frame);
        public event FrameUpdateHandler OnFrameUpdated;

        public FrameCallback(IValueReporter valueReporter, params FrameUpdateHandler[] delegates)
        {
            this.valueReporter = valueReporter;
            OnFrameUpdated = Report;
            AddDelegatesToOnFrameUpdated(delegates);
        }

        public void FrameReceived(IntPtr pFrame, int width, int height)
        {
            byte[] buffer = new byte[width * height];
            // https://stackoverflow.com/questions/5486938/c-sharp-how-to-get-byte-from-intptr
            Marshal.Copy(pFrame, buffer, 0, width * height);
            Frame bufferedFrame = new Frame
            {
                RawData = buffer
            };
            OnFrameUpdated(bufferedFrame);
        }

        public void AddDelegatesToOnFrameUpdated(params FrameUpdateHandler[] delegates)
        {
            foreach (var d in delegates)
            {
                OnFrameUpdated += d;
            }
        }

        public void RemoveDelegatesFromOnFrameUpdated(params FrameUpdateHandler[] delegates)
        {
            foreach (var d in delegates)
            {
                OnFrameUpdated -= d;
            }
        }

        void Report(Frame frame)
        {
            double sum = frame.RawData.Aggregate(0, (double sum, byte next) => sum + next);
            valueReporter.Report(sum / frame.RawData.Length);
        }
    }
}
