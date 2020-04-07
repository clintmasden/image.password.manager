namespace SimplePasswordManager.States
{
    public static class StartupState
    {
        private static StartupForm _startupForm;
        internal static StartupForm StartupForm => _startupForm ?? (_startupForm = new StartupForm());
    }
}