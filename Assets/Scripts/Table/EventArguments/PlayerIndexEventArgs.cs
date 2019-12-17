using System;

namespace Table.EventArguments {
    public class PlayerIndexEventArgs : EventArgs {
        public int Index { get; }

        public PlayerIndexEventArgs(int index) {
            Index = index;
        }
    }
}