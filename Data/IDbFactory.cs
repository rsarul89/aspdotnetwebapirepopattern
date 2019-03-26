namespace Data
{
    public interface IDbFactory
    {
        WebApiDbContext Init();
    }
}
