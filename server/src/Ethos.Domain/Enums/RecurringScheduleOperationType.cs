namespace Ethos.Domain.Enums
{
    /// <summary>
    /// Describe how to handle operations on recurring schedules.
    /// </summary>
    public enum RecurringScheduleOperationType
    {
        /// <summary>
        /// Execute operation only on the single specified instance.
        /// </summary>
        Instance = 1,

        /// <summary>
        /// Execute operation only on future instances.
        /// </summary>
        Future = 2,
    }
}
