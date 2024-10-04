using UnityEngine;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    const float DISTANCE_BETWEEN_OBSTACLES = 4.5f;
    const float HEIGHT_RANDOM = 3f;
    const int MIN_COUNT = 3;
    public GameObject prefab;
    Vector3 pos = new Vector3(DISTANCE_BETWEEN_OBSTACLES, 0, 0);

    List<Coin> coins = new List<Coin>();

    private static CoinManager instance = null;

    public static CoinManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<CoinManager>();

            return instance;
        }
    }

    private void Awake()
    {
        instance = this;
    }

    public void Reset()
    {
        for (int i = 0; i < coins.Count; i++)
            Destroy(coins[i].gameObject);

        coins.Clear();

        pos.x = 0;

        InstantiateObstacle();
        InstantiateObstacle();
    }

    public Coin GetNextCoin(Vector3 pos)
    {
        for (int i = 0; i < coins.Count; i++)
        {
            if (pos.x < coins[i].transform.position.x + 2f)
                return coins[i];
        }

        return null;
    }

    public bool IsColliding(Vector3 pos)
    {
        Collider2D collider = Physics2D.OverlapBox(pos, new Vector2(0.3f, 0.3f), 0);

        if (collider != null)
            return true;

        return false;
    }

    public void CheckAndInstatiate()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            coins[i].CheckToDestroy();
        }

        while (coins.Count < MIN_COUNT)
            InstantiateObstacle();
    }

    void InstantiateObstacle()
    {
        pos.x += DISTANCE_BETWEEN_OBSTACLES;
        pos.y = Random.Range(-HEIGHT_RANDOM, HEIGHT_RANDOM);
        GameObject go = GameObject.Instantiate(prefab, pos, Quaternion.identity);
        go.transform.SetParent(this.transform, false);
        Coin coin = go.GetComponent<Coin>();
        coin.OnDestroy += OnCoinDestroy;
        coins.Add(coin);
    }

    void OnCoinDestroy(Coin coin)
    {
        coin.OnDestroy -= OnCoinDestroy;
        coins.Remove(coin);
    }
}
