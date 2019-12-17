using System;

namespace Table.EventArguments {
    public class PlayerJoinedEventArgs : EventArgs {
        public string Username { get; }
        public int Stack { get; }

        public PlayerJoinedEventArgs(string username, int stack) {
            Username = username;
            Stack = stack;
        }
    }
}