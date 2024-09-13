using System;
using System.Collections.Concurrent;
using System.Collections.Generic;


namespace BehaivioursActions
{
    public struct BehaivioursAction
    {
        private Dictionary<int, ICollection<Action>> mainThreadsBehaiviours;
        private ConcurrentDictionary<int, ConcurrentBag<Action>> multiThreadsBehaiviours;
        private Action transitionBehaiviours;
    
        public void AddMainThreadBehaviours(int executionOrder, Action behaviour)
        {
            if (mainThreadsBehaiviours == null)
                mainThreadsBehaiviours = new Dictionary<int, ICollection<Action>>();
    
            if (!mainThreadsBehaiviours.ContainsKey(executionOrder))
                mainThreadsBehaiviours.Add(executionOrder, new List<Action>());
    
            mainThreadsBehaiviours[executionOrder].Add(behaviour);
        }
    
        public void AddMultiThreadsBehaviours(int executionOrder, Action behaviour)
        {
            if (multiThreadsBehaiviours == null)
                multiThreadsBehaiviours = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
    
            if (!multiThreadsBehaiviours.ContainsKey(executionOrder))
                multiThreadsBehaiviours.TryAdd(executionOrder, new ConcurrentBag<Action>());
    
            multiThreadsBehaiviours[executionOrder].Add(behaviour);
        }
    
        public void SetTransition(Action behaviours)
        {
            transitionBehaiviours = behaviours;
        }
    
        public Dictionary<int, ICollection<Action>> MainThreadBahaviours => mainThreadsBehaiviours;
        public ConcurrentDictionary<int, ConcurrentBag<Action>> MultiThreadBehaviours => multiThreadsBehaiviours;
        public Action TransitionBehaviours => transitionBehaiviours;
    }
}

