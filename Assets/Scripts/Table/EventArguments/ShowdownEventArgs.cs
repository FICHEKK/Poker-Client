using System;
using System.Collections.Generic;

namespace Table.EventArguments {
    public class ShowdownEventArgs : EventArgs {
        public List<int> WinnerIndexes { get; }
        
        public ShowdownEventArgs(List<int> winnerIndexes) {
            WinnerIndexes = winnerIndexes;
        }
    }
}