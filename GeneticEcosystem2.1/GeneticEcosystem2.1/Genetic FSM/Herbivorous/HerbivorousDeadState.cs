using BehaivioursActions;

namespace herbivorous
{
    public class HerbivorousDeadState : State
    {
        private int lives;

        public override BehaivioursAction GetOnEnterBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaivioursAction GetOnExitBehaviours(params object[] parameters)
        {
            return default;
        }

        public override BehaivioursAction GetTickBehaviours(params object[] parameters)
        {
            BehaivioursAction behaviour = new BehaivioursAction();

            lives = (int)parameters[0];

            behaviour.SetTransition(() =>
            {
                if (lives <= 0)
                {
                    OnFlag.Invoke(HerbivoreFlags.ToCorpse);
                }
            });

            return behaviour;
        }
    }

}