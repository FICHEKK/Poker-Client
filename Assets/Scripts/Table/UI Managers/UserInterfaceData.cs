using UnityEngine;

namespace Table.UI_Managers {
    public class UserInterfaceData : MonoBehaviour {
        
        // Table information
        public int SmallBlind;
        public int SeatIndex;
        
        // Stack information
        public int MyStack;
        public int OpponentStack;
        
        // Round information
        public int MyCurrentBet;
        public int OpponentCurrentBet;
        public int CurrentPot;
        public int RequiredCallAmount;
        
        public Vector3 potPositionOnTable;
    }
}
