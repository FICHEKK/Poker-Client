namespace Table.ResponseProcessors
{
    public interface IServerResponseProcessor
    {
        /// <summary>
        /// Indicates whether the processing of this response can wait.
        /// If it can wait and connection handler is in the waiting state, processing of this
        /// response will be added to the queue of postponed actions.
        /// If it can't wait, the processing will always be performed immediately.
        /// </summary>
        bool CanWait { get; }
        
        /// <summary>
        /// Reads the payload data from the server response.
        /// </summary>
        void ReadPayloadData();
        
        /// <summary>
        /// Processes the response by raising an event with the previously read payload data.
        /// </summary>
        void ProcessResponse(ServerConnectionHandler handler);
    }
}