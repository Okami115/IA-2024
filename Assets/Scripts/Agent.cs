using System;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private FSM<Behaivours,Flags> fsm;

    [SerializeField] private Transform target;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;
    [SerializeField] private float chaseDistance;

    private void Start()
    {
        fsm = new FSM<Behaivours, Flags>();

        fsm.AddBehaviour<ChaseState>(Behaivours.Chase,
            onTickParameters: () => { return new object[] { this.transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<PatrolState>(Behaivours.Patrol,
            onTickParameters: () => { return new object[] { this.transform, wayPoints[0], wayPoints[1], target, speed, chaseDistance }; });

        fsm.AddBehaviour<ExplodeState>(Behaivours.Explode,
            onTickParameters: () => { return new object[] { this.transform, target, speed }; });

        fsm.SetTrasnsition(Behaivours.Patrol, Flags.OnTargetNear, Behaivours.Chase, () => { Debug.Log("CAGASTE"); });
        fsm.SetTrasnsition(Behaivours.Chase, Flags.OnTargetReach, Behaivours.Explode, () => { Debug.Log("*Procede a explotar*"); });
        fsm.SetTrasnsition(Behaivours.Chase, Flags.OnTargetLost, Behaivours.Patrol, () => { Debug.Log("Verga, se fue"); });

        fsm.ForceState(Behaivours.Patrol);
    }


    private void Update()
    {
        fsm.Tick();
    }

}
public enum Behaivours
{
    Chase,
    Patrol,
    Explode
}

public enum Flags
{
    OnTargetReach,
    OnTargetLost,
    OnTargetNear,
}
