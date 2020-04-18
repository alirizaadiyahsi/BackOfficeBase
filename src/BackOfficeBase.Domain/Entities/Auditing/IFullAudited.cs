namespace BackOfficeBase.Domain.Entities.Auditing
{
    public interface IFullAudited : IAudited, IDeletionAudited
    {

    }

    public interface IFullAudited<TUser> : IFullAudited, IAudited<TUser>, IDeletionAudited<TUser>
    {

    }
}