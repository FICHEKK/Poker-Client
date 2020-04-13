namespace Table.ResponseProcessors
{
    public interface IServerResponseProcessor
    {
        /// <summary>Reads the payload data from the server response.</summary>
        void ReadPayloadData();
        
        /// <summary>Processes the response by raising an event with the previously read payload data.</summary>
        void ProcessResponse(ServerConnectionHandler handler);
    }
}