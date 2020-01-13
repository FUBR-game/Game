using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLevelDungeon : MonoBehaviour
{
    public int seed;
    public bool rollNewSeedOnPlay;
    public LevelPrefs[] levels;

    // Start is called before the first frame update
    void Start()
    {
        if (rollNewSeedOnPlay) seed = System.Environment.TickCount;
        UnityEngine.Random.InitState(seed);
    }

    [System.Serializable]
    public struct LevelPrefs
    {
        public string name;
        public int levelSize;
        [Range(0, 5)] public int splitSize;
    }
}
