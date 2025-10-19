public interface ICoordinator<T>
    where T : IScreenView
{
    void Attach(T view); // subscribe to view events
    void Detach(); // unsubscribe
}
