namespace _Game.Scripts.Systems.IndicatorSystem
{
    public interface IIndicator
    {
        bool IsShowingIndicator { get; }
        void UpdateIndicatorState(bool showState);
    }
}
