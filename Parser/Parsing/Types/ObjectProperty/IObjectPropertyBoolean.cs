namespace Parser
{
    public interface IObjectPropertyBoolean : IObjectPropertyIndex
    {
        bool IsClosingPopup { get; }
        void SetIsClosingPopup();
    }
}
