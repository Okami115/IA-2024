using UnityEngine;

public class Tank : TankBase
{
    float multiplier = 1f;
    protected override void OnReset()
    {
        multiplier = 1f;
    }

    protected override void OnThink(float dt)
    {
        Vector3 dirToMine = GetDirToMine(nearMine);

        inputs[0] = dirToMine.x;
        inputs[1] = dirToMine.z;
        inputs[2] = transform.forward.x;
        inputs[3] = transform.forward.z;
        inputs[4] = nearMine.layer == 6 ? 1.0f : -1.0f;

        float[] output = brain.Synapsis(inputs);

        float dif = output[0] - inputs[1];

        if(dif < 0.5f && dif > -0.5f)
            multiplier *= 0.1f * dif;

        SetForces(output[0], output[1], dt);

        Mathf.Clamp(multiplier, 0f, 2f);

        genome.fitness *= 1 * multiplier;
    }

    protected override void OnTakeMine(GameObject mine)
    {

        if(mine.layer == 6)
        {
            multiplier *= 1.1f;
        }
        else if(mine.layer == 7)
        {
            multiplier *= 0.9f;
        }
    }
}
