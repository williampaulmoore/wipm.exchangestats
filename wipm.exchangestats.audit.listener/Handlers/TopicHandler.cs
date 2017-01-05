namespace wipm.exchangestats.audit.listener {

    // todo:  move this into a library

    /// <summary>
    /// Common interface that all topic handlers conform to 
    /// </summary>
    public interface TopicHandler {

        string Name { get; }

        void Start();
        void Stop();
    }
}
