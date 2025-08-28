namespace BGJ_14
{
    public class GameSessionService : IService
    {
        private PlayerProgress _playerProgress;

        public PlayerProgress playerProgress => _playerProgress;

        public void StartNewGameSession()
        {
            _playerProgress = new PlayerProgress();
        }
    }
}
