using UnityEngine;

public class Caravana : MonoBehaviour
{
    public GrapfView grapfView;

    public Node<Vector2Int> currentNode;

    public Node<Vector2Int> targetNode;

    public float speed;
    public int food;

    private FSM<Behaivours, Flags> fsm;

    private void OnEnable()
    {
        grapfView.ChangePathFinder += StartPathFinder;

        fsm = new FSM<Behaivours, Flags>();
    }

    private void OnDestroy()
    {
        grapfView.ChangePathFinder -= StartPathFinder;
    }

    void StartPathFinder(int startIndexNode, int endIndexNode)
    {
        currentNode = grapfView.urbanCenter;

        fsm.AddBehaviour<MoveCaravanaState>(Behaivours.Move,
            onEnterParameters: () => { return new object[] { currentNode, grapfView }; },
            onTickParameters: () => { return new object[] { transform, speed, this }; });

        fsm.AddBehaviour<WaitOrdersState>(Behaivours.Wait,
            onEnterParameters: () => { return new object[] { grapfView, this }; },
            onTickParameters: () => { return new object[] { transform, speed, this }; });

        fsm.AddBehaviour<GiveFoodState>(Behaivours.GiveFood,
            onEnterParameters: () => { return new object[] { currentNode, grapfView }; },
            onTickParameters: () => { return new object[] { this }; });


        fsm.SetTrasnsition(Behaivours.Move, Flags.OnWaitingOrders, Behaivours.Wait, () => { Debug.Log("*Procede a esperar*"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.OnReadyToGiveFood, Behaivours.GiveFood, () => { Debug.Log("*Procede a dar comida*"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.IsAlert, Behaivours.Move, () => { Debug.Log("*Procede a correr*"); });

        fsm.SetTrasnsition(Behaivours.GiveFood, Flags.IsAlert, Behaivours.Move, () => { Debug.Log("*Procede a correr*"); });
        fsm.SetTrasnsition(Behaivours.GiveFood, Flags.OnReadyToTravel, Behaivours.Move, () => { Debug.Log("*Procede a viajar*"); });

        fsm.SetTrasnsition(Behaivours.Wait, Flags.OnReadyToTravel, Behaivours.Move, () => { Debug.Log("*Procede a viajar*"); });

        Vector3 aux = new Vector3(grapfView.OffsetPublic * currentNode.GetCoordinate().x, grapfView.OffsetPublic * currentNode.GetCoordinate().y);

        transform.position = aux;

        fsm.ForceState(Behaivours.Wait);
    }

    private void Update()
    {
        fsm.Tick();
    }
}
