using ECS;

public class Layer
{
    public int neuronCount;
    public float[,] weights;

    public Layer(int neuronCount, float[,] weights)
    {
        this.neuronCount = neuronCount;
        this.weights = weights;
    }
    public Layer(int neuronCount)
    {
        this.neuronCount = neuronCount;
    }
}

public class InputLayerComponent : ECSComponent
{
    public Layer layer;
    public int inputCount;
    public InputLayerComponent(Layer layer)
    {
        this.layer = layer;
    }
}

public class HiddenLayerComponent : ECSComponent
{
    public Layer[] hiddenLayers;

    public HiddenLayerComponent(Layer[] hiddenLayers)
    {
        this.hiddenLayers = hiddenLayers;
    }
}

public class OutputLayerComponent : ECSComponent
{
    public Layer layer;

    public OutputLayerComponent(Layer layer)
    {
        this.layer = layer;
    }
}

public class OutputComponent : ECSComponent
{
    public float[] outputs;

    public OutputComponent(float[] outputs)
    {
        this.outputs = outputs;
    }
}
public class InputComponent : ECSComponent
{
    public float[] inputs;

    public InputComponent(float[] inputs)
    {
        this.inputs = inputs;
    }
}

public class BiasComponent : ECSComponent
{
    public float Bias;

    public BiasComponent(float Bias)
    {
        this.Bias = Bias;
    }
}

public class SigmoidComponent : ECSComponent
{
    public float Sigmoid;

    public SigmoidComponent(float Sigmoid)
    {
        this.Sigmoid = Sigmoid;
    }
}
