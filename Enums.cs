namespace Auth.Api;

public class Enums
{
    public enum RegisterStatus
    {
        Success,
        Unsuccessful
    }

    public enum SearchUserBy
    {
        Id = 1,
        Name = 2,
        Email = 3
    }
}
