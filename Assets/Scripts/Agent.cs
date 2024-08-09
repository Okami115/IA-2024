using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private FSM fsm;

    [SerializeField] private Transform target;
    [SerializeField] private Transform[] wayPoints;
    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;
    [SerializeField] private float chaseDistance;

    private void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Behaivours)).Length, Enum.GetValues(typeof(Flags)).Length);

        fsm.AddBehaviour<ChaseState>((int)Behaivours.Chase,
            onTrikParameters: () => { return new object[] { this.transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<PatrolState>((int)Behaivours.Patrol,
            onTrikParameters: () => { return new object[] { this.transform, wayPoints[0], wayPoints[1], target, speed, chaseDistance }; });

        fsm.AddBehaviour<ExplodeState>((int)Behaivours.Explode,
            onTrikParameters: () => { return new object[] { this.transform, target, speed }; });

        fsm.SetTrasnsition((int)Behaivours.Chase, (int)Flags.OnTargetReach, (int)Behaivours.Explode);
        fsm.SetTrasnsition((int)Behaivours.Chase, (int)Flags.OnTargetLost, (int)Behaivours.Patrol);

        fsm.SetTrasnsition((int)Behaivours.Patrol, (int)Flags.OnTargetNear, (int)Behaivours.Chase);
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
