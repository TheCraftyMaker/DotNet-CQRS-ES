namespace DomainDriven.Infrastructure.Projection
{
    public interface IProjectionRepository<TReadModel>
    {
        Task<TReadModel?> Find(string id);
        
        Task InsertProjection(TReadModel model);

        Task UpdateProjection(TReadModel model);
    }
}