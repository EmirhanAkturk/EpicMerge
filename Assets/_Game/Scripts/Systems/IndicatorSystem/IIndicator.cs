namespace _Game.Scripts.Systems.IndicationSystem
{
    public interface IIndicator
    {
        bool IsShowingIndicator { get; }
        void UpdateIndicatorState(bool isMergeable);
    }
}
