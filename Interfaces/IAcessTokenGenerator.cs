namespace Auth.Api.Interfaces;

public interface IAcessTokenGenerator
{
    public string Generate(Guid userIdentifier);
}
