namespace wipm.exchangestats.data.ingress.listener {


    /// <summary>
    /// Common interface that all topic handlers conform to 
    /// </summary>
    public interface TopicHandler {

        string Name { get; }

        void Start();
        void Stop();
    }
}
