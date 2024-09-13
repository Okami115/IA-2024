using ECS;
using System.Collections.Generic;
using System.Threading.Tasks;

public sealed class RotationSystem : ECSSystem
{
    private ParallelOptions parallelOptions;

    private IDictionary<uint, RotationComponent> rotationComponents;
    private IDictionary<uint, VelocityComponent> velocityComponents;
    private IEnumerable<uint> queryedEntities;

    public override void Initialize()
    {
        parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 32 };
    }

    protected override void PreExecute(float deltaTime)
    {
        rotationComponents ??= ECSManager.GetComponents<RotationComponent>();
        velocityComponents ??= ECSManager.GetComponents<VelocityComponent>();
        queryedEntities ??= ECSManager.GetEntitiesWhitComponentTypes(typeof(PositionComponent), typeof(VelocityComponent));
    }

    protected override void Execute(float deltaTime)
    {
        Parallel.ForEach(queryedEntities, parallelOptions, i =>
        {
            rotationComponents[i].X += velocityComponents[i].velocity * deltaTime * 1000;
            rotationComponents[i].Y += velocityComponents[i].velocity * deltaTime * 1000;
            rotationComponents[i].Z += velocityComponents[i].velocity * deltaTime * 1000;
        });
    }

    protected override void PostExecute(float deltaTime)
    {
    }
}
