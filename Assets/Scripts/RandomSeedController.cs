using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSeedController : MonoBehaviour
{
    string currentSeed;
    int seedHash;

    public void GenerateRandomSeed()
    {
        string temp = System.DateTime.Now.Ticks.ToString();
        
        seedHash = int.Parse(temp.Substring(temp.Length - 8, 8));
        currentSeed = seedHash.ToString();
    }

    public void SetRandomSeed(string seed)
    {
        while(seed.Length < 8)
        {
            seed += "0";
        }

        currentSeed = seed.Substring(0, 8);

        seedHash = currentSeed.GetHashCode();
    }

    public string GetCurrentSeed()
    {
        return currentSeed;
    }

    public int GetCurrentSeedHash()
    {
        return seedHash;
    }
}
