namespace Infrastructure
{
    public interface ISecurityService
    {
        bool Connect();
        bool ReConnect();
        bool Validate();
    }
}