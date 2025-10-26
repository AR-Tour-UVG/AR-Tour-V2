public interface ICoordinator<T>
    where T : IScreenView
{
    void Attach(T view);
    void Detach();
}
