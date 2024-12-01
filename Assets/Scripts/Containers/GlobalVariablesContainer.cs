namespace DefaultNamespace
{
    public static class GlobalVariablesContainer
    {
        private static bool isAbilityActive = false;
        
        public static bool IsAbilityActive
        {
            get => isAbilityActive;
            set => isAbilityActive = value;
        }
    }
}