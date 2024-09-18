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

        fsm.AddBehaviour<ExplodeState>(Behaivours.Explode,
            onTickParameters: () => { return new object[] { this.transform, target, speed }; });

        fsm.SetTrasnsition(Behaivours.Mining, Flags.OnTargetNear, Behaivours.Move, () => { Debug.Log("CAGASTE"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.OnReadyToMine, Behaivours.Explode, () => { Debug.Log("*Procede a explotar*"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.OnTargetLost, Behaivours.Mining, () => { Debug.Log("Verga, se fue"); });

        fsm.ForceState(Behaivours.Mining);
    }


    private void Update()
    {
        fsm.Tick();
    }

}
public enum Behaivours
{
    Move,
    Mining,
    Explode
}

public enum Flags
{
    OnReadyToMine,
    OnTargetLost,
    OnTargetNear,
}
