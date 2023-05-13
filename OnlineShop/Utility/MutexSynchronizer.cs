namespace OnlineShop.Utility
{
    public class MutexSynchronizer
    {
        public Mutex ItemImageMutex { get; private set; }
        public Mutex OrderMutex { get; private set; }

        public MutexSynchronizer()
        {
            ItemImageMutex = new Mutex();
            OrderMutex = new Mutex();
        }
    }
}
