using System;

namespace Table.EventArguments {
    public class TableNotEmptyEventArgs : EventArgs {
        public string Username { get; }
        public int ChipCount { get; }

        public TableNotEmptyEventArgs(string username, int chipCount) {
            Username = username;
            ChipCount = chipCount;
        }
    }
}