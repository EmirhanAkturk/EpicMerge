namespace _Game.Scripts.Systems.IndicatorSystem
{
    public interface IIndicatorController
    {
        bool IsShowingIndicator { get; }
        void UpdateIndicatorState(bool isMergeable);
    }
}
