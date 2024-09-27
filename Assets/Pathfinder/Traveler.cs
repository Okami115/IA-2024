using System;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public GrapfView grapfView;

    public Node<Vector3> currentNode;

    [SerializeField] public Inventory inventory;

    public float minningSpeed;
    public float speed;

    private bool isRunning = false;

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
        isRunning = true;
        currentNode = grapfView.urbanCenter;

        fsm.AddBehaviour<MoveState>(Behaivours.Move,
            onEnterParameters: () => { return new object[] { currentNode, grapfView, inventory }; },
            onTickParameters: () => { return new object[] { transform, grapfView.OffsetPublic, speed, this }; });

        fsm.AddBehaviour<MiningState>(Behaivours.Mining,
            onEnterParameters: () => { return new object[] { currentNode, inventory, grapfView.mines, this }; },
            onTickParameters: () => { return new object[] { minningSpeed, Time.deltaTime, grapfView.mines, grapfView }; },
            onExitParameters: () => { return new object[] { this }; });

        fsm.AddBehaviour<WaitingFoodState>(Behaivours.Piquete,
            onEnterParameters: () => { return new object[] { currentNode, grapfView.mines }; },
            onTickParameters: () => { return new object[] { grapfView }; });

        fsm.AddBehaviour<WaitMinesState>(Behaivours.Wait,
            onEnterParameters: () => { return new object[] { grapfView, this }; });

        fsm.SetTrasnsition(Behaivours.Move, Flags.OnReadyToMine, Behaivours.Mining, () => { Debug.Log("*Procede a minar*"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.IsAlert, Behaivours.Move, () => { Debug.Log("*Procede a correr*"); });
        fsm.SetTrasnsition(Behaivours.Move, Flags.IsInHome, Behaivours.Wait, () => { Debug.Log("*Procede a esperar*"); });

        fsm.SetTrasnsition(Behaivours.Mining, Flags.OnReadyToBack, Behaivours.Move, () => { Debug.Log("*Procede a caminar*"); });
        fsm.SetTrasnsition(Behaivours.Mining, Flags.OnReadyToEat, Behaivours.Piquete, () => { Debug.Log("*Procede a hacer piquete*"); });
        fsm.SetTrasnsition(Behaivours.Mining, Flags.IsAlert, Behaivours.Move, () => { Debug.Log("*Procede a correr*"); });

        fsm.SetTrasnsition(Behaivours.Piquete, Flags.OnReadyToMine, Behaivours.Mining, () => { Debug.Log("*Procede a minar sin hambre*"); });
        fsm.SetTrasnsition(Behaivours.Piquete, Flags.IsAlert, Behaivours.Move, () => { Debug.Log("*Procede a correr*"); });

        fsm.SetTrasnsition(Behaivours.Wait, Flags.OnReadyToBack, Behaivours.Move, () => { Debug.Log("*Procede a caminar*"); });

        Vector3 aux = new Vector3(grapfView.OffsetPublic * currentNode.GetCoordinate().x, grapfView.OffsetPublic * currentNode.GetCoordinate().y, grapfView.OffsetPublic * currentNode.GetCoordinate().z);

        transform.position = aux;

        fsm.ForceState(Behaivours.Move);
    }

    private void Update()
    {
        fsm.Tick();
    }

    private void OnDrawGizmos()
    {
        /*
        if (!Application.isPlaying || !isRunning)
            return;


        foreach (Node<Vector3> node in grapfView.grapf.nodes)
        {
            if(node.Equals(startNode))
                Gizmos.color = Color.blue;
            else if(node.Equals(destinationNode))
                Gizmos.color = Color.black;
            else if(path.Contains(node))
                Gizmos.color = Color.yellow;
            else if(grapfView.mines.Contains(node) && !node.Equals(destinationNode))
                Gizmos.color = Color.green;
            else
                Gizmos.color = Color.white;

            Vector3 nodeCordinates = new Vector3(grapfView.OffsetPublic * node.GetCoordinate().x, grapfView.OffsetPublic * node.GetCoordinate().y);
            Gizmos.DrawWireSphere(nodeCordinates, 0.2f);
        }
         */
    }
}

[Serializable]
public class Inventory
{
    public int food = 0;
    public int gold = 0;

    Inventory(int food = 0, int gold = 0)
    {
        this.food = food;
        this.gold = gold;
    }
}

public enum Behaivours
{
    Move,
    Mining,
    Piquete,
    Wait,
    GiveFood
}

public enum Flags
{
    OnReadyToMine,
    OnReadyToBack,
    OnReadyToEat,
    OnWaitingOrders,
    OnReadyToTravel,
    OnReadyToGiveFood,
    IsAlert,
    IsInHome
}
