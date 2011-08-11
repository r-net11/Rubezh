namespace Infrastructure
{
    public interface ISecurityService
    {
        bool Check();
        void ChangeUser();
    }
}