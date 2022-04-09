namespace Ethos.Domain.UnitTest
{
    public static class CronTestExpressions
    {
        /// <summary>
        /// Every week day (MON-FRI) at 9am
        /// </summary>
        public const string EveryWeekDayAt9 = "0 09 * * MON-FRI";


        /// <summary>
        /// Every monday at 14
        /// </summary>
        public const string EveryMondayAt14 = "0 14 * * MON";
    }
}
