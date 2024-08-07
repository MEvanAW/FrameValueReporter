namespace FrameValueReporter.FrameCallback
{
    public interface IFrameCallback
    {
        public void FrameReceived(IntPtr pFrame, int width, int height);
    }
}