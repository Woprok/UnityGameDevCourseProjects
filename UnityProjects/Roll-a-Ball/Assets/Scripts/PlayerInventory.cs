using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerInventory : MonoBehaviour
    {
        private int score;

        public int WinScore = 9;
        public int BaseGoldScore = 4;
        public int BasePurpleScore = 1;

        public int GoldCount
        {
            get => goldCount;
            set
            {
                score += BaseGoldScore;
                goldCount = value;
            }
        }

        public int PurpleCount
        {
            get => purpleCount;
            set
            {
                purpleCount = value;
                score = score * (BasePurpleScore * purpleCount);
            }
        }

        public int ObjectCount = 0;
        private int purpleCount;
        private int goldCount;
        public int ItemsLeft() => ObjectCount - PurpleCount - GoldCount;

        public int CalculateScore() => score;
    }
}