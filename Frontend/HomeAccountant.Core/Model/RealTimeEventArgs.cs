namespace HomeAccountant.Core.Model
{
    public class RealTimeEventArgs<T> : RealTimeEventArgs
    {
        public T Value { get; private set; }

        public RealTimeEventArgs(T value)
        {
            Value = value;
        }
    }

    public class RealTimeEventArgs : EventArgs
    {
        public new static RealTimeEventArgs Empty => new RealTimeEventArgs();
        public RealTimeEventArgs() { }
    }
}
