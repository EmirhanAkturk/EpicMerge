namespace _Game.Scripts.Systems.IndicationSystem
{
    public interface IIndicatorController
    {
        bool IsShowingIndicator { get; }
        void UpdateIndicatorState(bool isMergeable);
    }
}
