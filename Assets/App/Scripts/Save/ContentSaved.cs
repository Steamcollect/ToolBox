namespace BT.Save
{
    [System.Serializable]
    public class ContentSaved
    {
        public int[] highscores = new int[3];
        public int lastScore;
        public int totalPeopleSaved;
        public int maxPeopleSaved;
        public int maxDriftCombo;
    }
}

