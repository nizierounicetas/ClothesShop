namespace OnlineShop.Utility
{
    public class MutexSynchronizer
    {
        public Mutex ItemImageMutex { get; set; }

        public MutexSynchronizer()
        {
            ItemImageMutex = new Mutex();
        }
    }
}
