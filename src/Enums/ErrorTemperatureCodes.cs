namespace Backend.Enums
{
    public enum ErrorTemperatureCodes
    {
        Success = 0,
        DuplicateTemperature = -1,
        TemperatureNotCreated = -2,
        TemperatureNotUpdated = -3,
        TownNotFound = -4,
    }
}
