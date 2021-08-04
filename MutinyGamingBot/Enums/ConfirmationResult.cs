namespace MutinyBot.Enums
{
    /// <summary>
    /// Represents possible outcomes when asking a user for confirmation.
    /// </summary>
    public enum ConfirmationResult
    {
        /// <summary>
        /// Indicates the user confirmed the action.
        /// </summary>
        Confirmed,

        /// <summary>
        /// Indicates the user denied the action.
        /// </summary>
        Denied,

        /// <summary>
        /// Indicates that the interaction timed out.
        /// </summary>
        TimedOut
    }
}
