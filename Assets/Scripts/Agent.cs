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

        fsm.AddBehaviour<MoveState>(Behaivours.Move,
            onTickParameters: () => { return new object[] { this.transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<MiningState>(Behaivours.Mining,
            onTickParameters: () => { return new object[] { this.transform, wayPoints[0], wayPoints[1], target, speed, chaseDistance }; });

        fsm.AddBehaviour<WaitingFoodState>(Behaivours.Piquete,
            onTickParameters: () => { return new object[] { this.transform, target, speed }; });

        fsm.ForceState(Behaivours.Mining);
    }


    private void Update()
    {
        fsm.Tick();
    }

}

