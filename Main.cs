using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    public int bitStringLenght = 100;
    public int populationSize = 30;
    public int GenerationSize = 30;
    public float mutationRate = 0.2f;
    public float eliteRate = 0.2f;
    public float crossoverRate = 0.2f;

    public float average = 0;
    public float best = 0;
    public float total = 0;

    public List<Chromosome> population = new List<Chromosome>();
    public List<Chromosome> elites = new List<Chromosome>();
    public List<Chromosome> parents = new List<Chromosome>();
    public List<Chromosome> offsprings = new List<Chromosome>();

    private int generationCount = 1;
    private int generationDivider;
    

    void Start()
    {
        generationDivider = GenerationSize / 10;
        population = GeneratePopulation(populationSize);

        while (generationCount != GenerationSize)
        {
            population = GenerateFitnessScore(population);
            population = population.OrderByDescending(x => x.fitnessScore).ToList();

            string result = string.Empty;
            for (int i = 0; i < population.Count/3; i++)
            {
                result += " " + population[i].fitnessScore;
            }
            Debug.Log($"Generation {generationCount} " + result);

            if (generationCount == GenerationSize-1)
            {
                for (int i = 0; i < populationSize; i++)
                {
                    total += population[i].fitnessScore;
                }

                average = total / populationSize;
                best = population[0].fitnessScore;
            }
           

            elites.Clear();
            elites = Elite(population, eliteRate);

            parents.Clear();
            parents = RandomRoulette(population);

            population.Clear();
            population.AddRange(elites);

            offsprings.Clear();
            offsprings = CrossOver(parents, 0.2f);
            offsprings = Mutation(offsprings, mutationRate);

            population.AddRange(offsprings);
            generationCount++;
        }

    }

    private List<Chromosome> Mutation(List<Chromosome> offsprings, float percentage)
    {
        List<Chromosome> mutatedOffSprrings = new List<Chromosome>();

        int percentageInt = (int)percentage * 100;
        for (int i = 0; i < offsprings.Count; i++)
        {
            string mutatedOffSpring = string.Empty;
            string currentOffSpring = offsprings[i].number;

            int randn = Random.Range(0, 100);
            if (randn<=percentage)
            {
                for (int j = 0; j < currentOffSpring.Length; j++)
                {
                    int rand = Random.Range(0, 100);
                    if (50 <= rand)
                    {
                        if (currentOffSpring[j].Equals('0'))
                        {
                            mutatedOffSpring += '1';
                        }
                        else
                        {
                            mutatedOffSpring += '0';
                        }

                    }
                    else
                    {
                        mutatedOffSpring += currentOffSpring[j];
                    }

                }
            }
            else
            {
                mutatedOffSpring = currentOffSpring;
            }

            

            Chromosome chromosome = new Chromosome();
            chromosome.number = mutatedOffSpring;

            mutatedOffSprrings.Add(chromosome);
        }

        return mutatedOffSprrings;

    }

    private List<Chromosome> CrossOver(List<Chromosome> parents, float rate)
    {
        List<Chromosome> offsprings = new List<Chromosome>();
        

        string chromosomeMale = parents[0].number;
        string chromosomeFemale = parents[1].number;

        int offSpringsSize = populationSize - population.Count;

        while (offsprings.Count != offSpringsSize)
        {

            int percentageInt = (int)rate * 100;
            int rands = Random.Range(0, 100);
            if (percentageInt <= rands)
            {
                int rand = Random.Range(3, chromosomeFemale.Length);
                int remaining = bitStringLenght - rand;

                string maleFirst = chromosomeMale.Substring(0, rand);
                string maleLast = chromosomeMale.Substring(rand, remaining);

                string femaleFirst = chromosomeFemale.Substring(0, rand);
                string femaleLast = chromosomeFemale.Substring(rand, remaining);

                Chromosome offspring1 = new Chromosome();
                offspring1.number = maleFirst + femaleLast;

                Chromosome offspring2 = new Chromosome();
                offspring2.number = femaleFirst + maleLast;





                int fitness1 = FitnessScore(offspring1.number);
                int fitness2 = FitnessScore(offspring2.number);

                if (fitness1 > fitness2)
                {
                    offsprings.Add(offspring1);
                }
                else
                {
                    offsprings.Add(offspring2);
                }
            }
            else
            {
                int rand = Random.Range(0, 2);
                if (rand == 1)
                {
                    offsprings.Add(parents[0]);
                }
                else
                {
                    offsprings.Add(parents[1]);
                }
                
            }
           
        }

        return offsprings;
    }

    private List<Chromosome> RandomRoulette(List<Chromosome> population)
    {

        List<Chromosome> parents = new List<Chromosome>();

        while (parents.Count != 2)
        {
            int randomIndex = Random.Range(0, population.Count);
            int fitness = population[randomIndex].fitnessScore;

            int chance = Random.Range(0, 101);
            if (chance <= fitness)
            {
                parents.Add(population[randomIndex]);
            }
        }

       
        return parents;


    }

    // Update is called once per frame
    void Update()
    {

    }
    public List<Chromosome> GeneratePopulation(int total)
    {
        List<Chromosome> population = new List<Chromosome>();

        for (int i = 0; i < total; i++)
        {
            Chromosome chromosome = new Chromosome();
            chromosome.number = GenerateBitString(bitStringLenght);
            population.Add(chromosome);
        }

        return population;

    }
    public string GenerateBitString(int bitStringLength)
    {
        string numberStr = string.Empty;

        while (numberStr.Length != bitStringLength)
        {
            int randomBit = Random.Range(0, 2);
            numberStr += randomBit;
        }

        return numberStr;
    }

    public List<Chromosome> GenerateFitnessScore(List<Chromosome> population)
    {
        List<Chromosome> newPopulation = new List<Chromosome>();

        for (int i = 0; i < population.Count; i++)
        {
            population[i].fitnessScore = FitnessScore(population[i].number);
            newPopulation.Add(population[i]);
        }

        return newPopulation;
    }

    public int FitnessScore(string number)
    {
        int counter = 0;
        for (int i = 0; i < number.Length; i++)
        {
            if (number[i].Equals('1'))
            {
                counter++;
            }
        }

        return counter;
    }

    public List<Chromosome> Elite(List<Chromosome> rankedPopulation, float percentage)
    {
        int counter = Convert.ToInt32(percentage * rankedPopulation.Count);
       
        List<Chromosome> elites = new List<Chromosome>();

        for (int i = 0; i < counter; i++)
        {
            elites.Add(rankedPopulation[i]);
        }

        for (int i = 0; i < counter; i++)
        {
            population.RemoveAt(i);

        }

        return elites;
    }



}


[System.Serializable]
public class Chromosome
{
    public string number = string.Empty;
    public int fitnessScore = 0;

}



