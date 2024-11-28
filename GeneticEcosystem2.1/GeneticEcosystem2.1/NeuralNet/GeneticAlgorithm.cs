using NeuralNet.Network;
using System;
using System.Collections.Generic;

namespace NeuralNet.Components
{
    public static class GeneticAlgorithm
    {
        private static Random random = new Random();

        enum EvolutionType
        {
            None = 0,
            AddNeurons,
            AddLayer
        }

        public static List<Genome> population = new List<Genome>();
        static List<Genome> newPopulation = new List<Genome>();

        private static int newNeuronToAddQuantity;
        private static int randomLayer = 0;
        private static List<NeuronLayer> neuronLayers;


        public static Genome[] GetRandomGenomes(int count, int genesCount)
        {
            Genome[] genomes = new Genome[count];

            for (int i = 0; i < count; i++)
            {
                genomes[i] = new Genome(genesCount);
            }

            return genomes;
        }

        public static float RandomRangeFloat(float min, float max)
        {
            return (float)(random.NextDouble() * (max - min) + min);
        }

        public static Genome[] Epoch(Genome[] oldGenomes, GeneticAlgorithmData data)
        {
            float currentTotalFitness = 0;

            population.Clear();
            newPopulation.Clear();

            population.AddRange(oldGenomes);
            population.Sort(HandleComparison);

            GeneticAlgorithmData backUpData = new GeneticAlgorithmData(data);
            foreach (Genome g in population)
            {
                currentTotalFitness += g.fitness;
            }


            if (currentTotalFitness < data.totalFitness)
            {
                data.generationStalled++;
                if (data.generationStalled >= data.maxStalledGenerationsUntilEvolve)
                {
                    data.generationStalled = 0;
                }
            }

            data.totalFitness = currentTotalFitness;
            CalculateNeuronsToAdd(data.brainStructure);


            SelectElite(data.eliteCount);
            while (newPopulation.Count < population.Count)
            {
                Crossover(data);
            }

            data.mutationChance = backUpData.mutationChance;
            data.mutationRate = backUpData.mutationRate;
            return newPopulation.ToArray();
        }

        private static void CalculateNeuronsToAdd(NeuralNetwork brain)
        {
            Random random = new Random();
            newNeuronToAddQuantity = random.Next(1, 3);
            randomLayer = random.Next(1, brain.layers.Count - 1);
            neuronLayers = brain.layers;
        }


        static void SelectElite(int eliteCount)
        {
            for (int i = 0; i < eliteCount && newPopulation.Count < population.Count; i++)
            {
                newPopulation.Add(population[i]);
            }
        }

        static void Crossover(GeneticAlgorithmData data)
        {
            Genome mom = RouletteSelection(data.totalFitness);
            Genome dad = RouletteSelection(data.totalFitness);

            Genome child1;
            Genome child2;

            Crossover(data, mom, dad, out child1, out child2);

            newPopulation.Add(child1);
            newPopulation.Add(child2);
        }

        static void Crossover(GeneticAlgorithmData data, Genome mom, Genome dad,
            out Genome child1,
            out Genome child2)
        {
            child1 = new Genome();
            child2 = new Genome();

            child1.genome = new float[mom.genome.Length];
            child2.genome = new float[mom.genome.Length];

            int pivot = random.Next(0, mom.genome.Length);

            for (int i = 0; i < pivot; i++)
            {
                child1.genome[i] = mom.genome[i];

                if (ShouldMutate(data.mutationChance))
                    child1.genome[i] += RandomRangeFloat(-data.mutationRate, data.mutationRate);

                child2.genome[i] = dad.genome[i];

                if (ShouldMutate(data.mutationChance))
                    child2.genome[i] += RandomRangeFloat(-data.mutationRate, data.mutationRate);
            }


            for (int i = pivot; i < mom.genome.Length; i++)
            {
                child2.genome[i] = mom.genome[i];

                if (ShouldMutate(data.mutationChance))
                    child2.genome[i] += RandomRangeFloat(-data.mutationRate, data.mutationRate);

                child1.genome[i] = dad.genome[i];

                if (ShouldMutate(data.mutationChance))
                    child1.genome[i] += RandomRangeFloat(-data.mutationRate, data.mutationRate);
            }
        }

        static bool ShouldMutate(float mutationChance)
        {
            return RandomRangeFloat(0.0f, 1.0f) < mutationChance;
        }

        static int HandleComparison(Genome x, Genome y)
        {
            return x.fitness > y.fitness ? 1 : x.fitness < y.fitness ? -1 : 0;
        }

        public static Genome RouletteSelection(float totalFitness)
        {
            float rnd = RandomRangeFloat(0, MathF.Max(totalFitness, 0));

            float fitness = 0;

            for (int i = 0; i < population.Count; i++)
            {
                fitness += MathF.Max(population[i].fitness, 0);
                if (fitness >= rnd)
                    return population[i];
            }

            return null;
        }
    }

}