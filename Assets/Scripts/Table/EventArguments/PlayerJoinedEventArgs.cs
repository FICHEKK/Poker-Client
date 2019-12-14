using System;

namespace Table.EventArguments {
    public class PlayerJoinedEventArgs : EventArgs {
        public string Username { get; }
        public int ChipCount { get; }

        public PlayerJoinedEventArgs(string username, int chipCount) {
            Username = username;
            ChipCount = chipCount;
        }
    }
}