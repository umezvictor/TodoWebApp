namespace Application.Interfaces
{
    public interface IDateTimeService
    {
        DateTime NowUtc { get; }
        DateTime EpocToDateTime(string date);
        string DateTimeToEpoc(DateTime date);
    }
}
