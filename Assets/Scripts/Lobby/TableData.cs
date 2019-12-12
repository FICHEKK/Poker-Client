using UnityEngine;

namespace Lobby {
    public class TableData : MonoBehaviour {
        public string Title { get; set; }
        public int SmallBlind { get; set; }
        public int PlayerCount { get; set; }
        public int MaxPlayers { get; set; }

        public TableData(string title, int smallBlind, int playerCount, int maxPlayers) {
            Title = title;
            SmallBlind = smallBlind;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
        }

        public void Overwrite(TableData data) {
            Title = data.Title;
            SmallBlind = data.SmallBlind;
            PlayerCount = data.PlayerCount;
            MaxPlayers = data.MaxPlayers;
        }
    }
}