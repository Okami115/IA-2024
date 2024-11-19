using UnityEngine;

public class Hervivoro : MonoBehaviour
{
    public GrapfView grapfView;

    public Node<Vector3> currentNode;

    private FSM<BehaivoursHevivoro, FlagsHervivoro> fsm;

    private void OnEnable()
    {
        fsm = new FSM<BehaivoursHevivoro, FlagsHervivoro>();

        fsm.AddBehaviour<HervivoroMoveToFood>(BehaivoursHevivoro.MoveToFood);

        fsm.AddBehaviour<HervivoroMoveAwayEnemyState>(BehaivoursHevivoro.MoveAwayEnemy);

        fsm.AddBehaviour<HervivoroEatState>(BehaivoursHevivoro.Eat);

        fsm.SetTrasnsition(BehaivoursHevivoro.MoveToFood, FlagsHervivoro.OnReadyToEat, BehaivoursHevivoro.Eat, () => { Debug.Log("*Procede a comer*"); });
        fsm.SetTrasnsition(BehaivoursHevivoro.MoveToFood, FlagsHervivoro.OnSearchFood, BehaivoursHevivoro.MoveAwayEnemy, () => { Debug.Log("*Procede a correr*"); });

        fsm.SetTrasnsition(BehaivoursHevivoro.MoveAwayEnemy, FlagsHervivoro.OnReadyToScape, BehaivoursHevivoro.MoveToFood, () => { Debug.Log("*Procede a buscar*"); });

        fsm.SetTrasnsition(BehaivoursHevivoro.Eat, FlagsHervivoro.OnReadyToScape, BehaivoursHevivoro.MoveAwayEnemy, () => { Debug.Log("*Procede a correr*"); });
        fsm.SetTrasnsition(BehaivoursHevivoro.Eat, FlagsHervivoro.OnSearchFood, BehaivoursHevivoro.MoveToFood, () => { Debug.Log("*Procede a buscar*"); });


    }
}

public enum BehaivoursHevivoro
{
    MoveToFood,
    MoveAwayEnemy,
    Eat
}

public enum FlagsHervivoro
{
    OnReadyToEat,
    OnSearchFood,
    OnReadyToScape
}