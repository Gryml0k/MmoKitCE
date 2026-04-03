namespace MultiplayerARPG
{
    [System.Serializable]
    public partial struct EquipWeapons
    {
        public CharacterItem rightHand;
        public CharacterItem leftHand;
        
        public EquipWeapons Clone(bool generateNewId = false)
        {
            return new EquipWeapons()
            {
                rightHand = rightHand.Clone(generateNewId),
                leftHand = leftHand.Clone(generateNewId),
            };
        }
    }
}
