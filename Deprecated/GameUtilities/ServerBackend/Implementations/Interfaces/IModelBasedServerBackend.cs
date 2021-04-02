namespace GameUtilities.ServerBackend.Implementations
{
    public interface IModelBasedServerBackend<TModel>
    {
        TModel Model { get; }
    }
}