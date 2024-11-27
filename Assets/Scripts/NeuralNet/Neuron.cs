using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Neuron
{
	private float[] weights;
	private float bias;
	private float p; 

	public int WeightsCount
	{
		get { return weights.Length; }
	}

	public Neuron(int weightsCount, float bias, float p)
	{
		weights = new float[weightsCount];

		for (int i = 0; i < weights.Length; i++)
		{
			weights[i] = Random.Range(-1.0f, 1.0f);
		}

		this.bias = bias;
		this.p = p;
	}
	
	public Neuron(byte[] data,ref int outputOffset)
	{
		int length = BitConverter.ToInt32(data, outputOffset);
		outputOffset += sizeof(int);
		weights = new float[length];
		for (int i = 0; i < length; i++)
		{
			weights[i] = BitConverter.ToSingle(data, outputOffset);
			outputOffset += sizeof(float);
		}
		bias = BitConverter.ToSingle(data, outputOffset);
		outputOffset += sizeof(float);
		p = BitConverter.ToSingle(data, outputOffset);
		outputOffset += sizeof(float);
        
	}
	public byte[] Serialize()
	{
		List<byte> bytes = new List<byte>();
        
		bytes.AddRange(BitConverter.GetBytes(weights.Length));
        
		foreach (float weight in weights)
		{
			bytes.AddRange(BitConverter.GetBytes(weight));
		}
        
		bytes.AddRange(BitConverter.GetBytes(bias));
        
		bytes.AddRange(BitConverter.GetBytes(p));

		return bytes.ToArray();
	}

	public float Synapsis(float[] input)
	{
		float a = 0;

		for (int i = 0; i < input.Length; i++)
		{
			a += weights[i] * input[i];
		}

		a += bias * weights[weights.Length - 1];

		return Sigmoid(a);
	}

	public int SetWeights(float[] newWeights, int fromId)
	{
		for (int i = 0; i < weights.Length; i++)
		{
			this.weights[i] = newWeights[i + fromId];
		}

		return fromId + weights.Length;
	}

	public float[] GetWeights()
	{
		return this.weights;
	}

	public float Sigmoid(float a)
	{
		return 1.0f / (1.0f + Mathf.Exp(-a / p));
	}
	
	public static float Sigmoid(float a, float p)
	{
		return 1.0f / (1.0f + Mathf.Exp(-a / p));
	}
}
