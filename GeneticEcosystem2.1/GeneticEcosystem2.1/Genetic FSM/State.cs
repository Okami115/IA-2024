using BehaivioursActions;
using System;

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehaivioursAction GetOnEnterBehaviours(params object[] parameters);
    public abstract BehaivioursAction GetOnExitBehaviours(params object[] parameters);
    public abstract BehaivioursAction GetTickBehaviours(params object[] parameters);
}

