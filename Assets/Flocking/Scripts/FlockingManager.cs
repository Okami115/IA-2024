using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public Transform target;
    public int boidCount = 50;
    public Boid boidPrefab;
    private List<Boid> boids = new List<Boid>();

    private void Start()
    {
        for (int i = 0; i < boidCount; i++)
        {
            GameObject boidGO = Instantiate(boidPrefab.gameObject, new Vector3(UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10), UnityEngine.Random.Range(-10, 10)), Quaternion.identity);
            Boid boid = boidGO.GetComponent<Boid>();
            boid.Init(Alignment, Cohesion, Separation, Direction);
            boids.Add(boid);
        }
    }

    public Vector3 Alignment(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;
        foreach (Boid b in insideRadiusBoids)
        {
            avg += b.transform.forward.normalized;
        }
        avg /= insideRadiusBoids.Count;
        avg.Normalize();
        return avg;
    }

    public Vector3 Cohesion(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;
        foreach (Boid b in insideRadiusBoids)
        {
            avg += b.transform.position;
        }
        avg /= insideRadiusBoids.Count;
        return (avg - boid.transform.position).normalized;
    }

    public Vector3 Separation(Boid boid)
    {
        List<Boid> insideRadiusBoids = GetBoidsInsideRadius(boid);
        Vector3 avg = Vector3.zero;

        int count = 0;

        if (insideRadiusBoids.Count == 0)
            return avg;


        foreach (Boid b in insideRadiusBoids)
        {
            if (b == boid)
                continue;

            float distance = Vector3.Distance(boid.transform.position, b.transform.position);

            if (distance < boid.separationRadius)
            {
                Vector3 otherBoidToCurrBoid = boid.transform.position - b.transform.position;
                Vector3 dirToTravel = otherBoidToCurrBoid.normalized;

                dirToTravel /= distance;

                avg += dirToTravel;

                count++;
            }
        }

        if (count == 0)
            return Vector3.zero;

        avg /= count;

        avg *= boid.separationFactor;

        return avg;
    }

    public Vector3 Direction(Boid boid)
    {
        return (target.position - boid.transform.position).normalized;
    }

    public List<Boid> GetBoidsInsideRadius(Boid boid)
    {
        List<Boid> insideRadiusBoids = new List<Boid>();

        foreach (Boid b in boids)
        {
            if (Vector3.Distance(boid.transform.position, b.transform.position) < boid.detectionRadious)
            {
                insideRadiusBoids.Add(b);
            }
        }

        return insideRadiusBoids;
    }
}