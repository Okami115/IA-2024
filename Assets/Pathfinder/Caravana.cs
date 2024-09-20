using UnityEngine;

public class Caravana : MonoBehaviour
{
    public GrapfView grapfView;

    public Node<Vector2Int> currentNode;

    public float speed;

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
        StopAllCoroutines();
        currentNode = grapfView.urbanCenter;

        fsm.AddBehaviour<MoveCaravanaState>(Behaivours.Move,
            onEnterParameters: () => { return new object[] { currentNode, grapfView}; },
            onTickParameters: () => { return new object[] { transform, speed, this }; });


        //fsm.SetTrasnsition(Behaivours.Move, Flags.OnReadyToMine, Behaivours.Mining, () => { Debug.Log("*Procede a minar*"); });

        Vector3 aux = new Vector3(grapfView.OffsetPublic * currentNode.GetCoordinate().x, grapfView.OffsetPublic * currentNode.GetCoordinate().y);

        transform.position = aux;

        fsm.ForceState(Behaivours.Move);
    }

    private void Update()
    {
        fsm.Tick();
    }
}
