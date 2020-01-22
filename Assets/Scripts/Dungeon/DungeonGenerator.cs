using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Assets.Scripts;
using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;

public class DungeonGenerator : DungeonGeneratorBehavior
{
    [Range(0.0f, 0.25f)] public float maxSplitOffset;
    [Range(0, 10)] public int splitDepth;
    public int levelWidth;
    public int levelHeight;

    public List<Vector2> PlayerSpawnPoints;
    
    public Loot lootBox;
    public List<Loot> lootList;
    
    public int seed;

    // Lower = more randomness, higher = less randomless, more fairness
    [Range(0.0f, 1.0f)] public float minRoomRatio;

    public bool buildBlocks;

    public GameObject destroyableWallBlock;
    public GameObject invincibleWallBlock;

    [Range(0, 6)] public int maxWallOffset;
    [Range(0, 1)] public float roomChance = 0.2f;

    List<GameObject> splitCubes;

    Assets.BlockGrid grid;
    LevelSplit[] splits;

    private SpellManager spellManager;

    public void Start()
    {
        spellManager = GameObject.Find("SpellManager").GetComponent<SpellManager>();
    }

    public void CallGenerate(int seed)
    {
        UnityEngine.Random.InitState(seed);

        grid = new Assets.BlockGrid(levelWidth, levelHeight,
            new Vector2((destroyableWallBlock.transform.localScale.x * levelWidth),
                (destroyableWallBlock.transform.localScale.z * levelHeight)));

        splits = SplitLevels();

        if (buildBlocks)
        {
            BuildBlocks(splits);
            MakePlayerSpawns(splits);
            SpawnLoot(splits);
        }
    }

    private LevelSplit[] SplitLevels()
    {
        LevelSplit[] splits = new LevelSplit[]
            {new LevelSplit {start = new Vector2Int(0, 0), delta = new Vector2Int(levelWidth, levelHeight)}};
        for (int splitLevel = 1; splitLevel <= splitDepth; splitLevel++)
        {
            int newSplitNo = splits.Length * 2;
            LevelSplit[] newSplits = new LevelSplit[newSplitNo];

            splitCubes = new List<GameObject>();

            int currentIndex = 0;
            foreach (LevelSplit split in splits)
            {
                bool splitHorizontal = DoISplitHorizontally(split);
                int splitRange = 0;
                if (splitHorizontal)
                {
                    splitRange = Mathf.RoundToInt(split.delta.x * maxSplitOffset);
                }
                else
                {
                    splitRange = Mathf.RoundToInt(split.delta.y * maxSplitOffset);
                }

                LevelSplit[] newSplit = SplitLevel(split, UnityEngine.Random.Range(-splitRange, splitRange),
                    splitHorizontal);
                newSplits[currentIndex] = newSplit[0];
                currentIndex++;
                newSplits[currentIndex] = newSplit[1];
                currentIndex++;
            }

            splits = newSplits;
        }

        return splits;
    }

    private struct LevelSplit
    {
        public Vector2Int start;
        public Vector2Int delta;
    }

    private void BuildBlocks(LevelSplit[] splits)
    {
        // edge walls (there's probably a better way to do this)
        var edgeWallWest = Instantiate(invincibleWallBlock);
        var edgeGridPos = grid.GridPosToRealPos(new Vector2Int(0, levelHeight / 2));
        var edgeGridScale = grid.GetRealLength(new Vector2Int(levelWidth, levelHeight));
        edgeWallWest.transform.position = new Vector3(edgeGridPos.x, 4, edgeGridPos.y);
        edgeWallWest.transform.localScale = new Vector3(edgeWallWest.transform.localScale.x, edgeWallWest.transform.localScale.y, edgeGridScale.y);

        var edgeWallEast = Instantiate(invincibleWallBlock);
        edgeGridPos = grid.GridPosToRealPos(new Vector2Int(levelWidth, levelHeight / 2));
        edgeWallEast.transform.position = new Vector3(edgeGridPos.x, 4, edgeGridPos.y);
        edgeWallEast.transform.localScale = new Vector3(edgeWallEast.transform.localScale.x, edgeWallEast.transform.localScale.y, edgeGridScale.y);

        var edgeWallNorth = Instantiate(invincibleWallBlock);
        edgeGridPos = grid.GridPosToRealPos(new Vector2Int(levelWidth / 2, 0));
        edgeWallNorth.transform.position = new Vector3(edgeGridPos.x, 4, edgeGridPos.y);
        edgeWallNorth.transform.localScale = new Vector3(edgeGridScale.x, edgeWallNorth.transform.localScale.y, edgeWallNorth.transform.localScale.z);

        var edgeWallSouth = Instantiate(invincibleWallBlock);
        edgeGridPos = grid.GridPosToRealPos(new Vector2Int(levelWidth / 2, levelHeight));
        edgeWallSouth.transform.position = new Vector3(edgeGridPos.x, 4, edgeGridPos.y);
        edgeWallSouth.transform.localScale = new Vector3(edgeGridScale.x, edgeWallSouth.transform.localScale.y, edgeWallSouth.transform.localScale.z);

        foreach (LevelSplit split in splits)
        {
            int maxHorOffSet = maxWallOffset;
            int maxVerOffSet = maxWallOffset;
            int topOffset = UnityEngine.Random.Range(0, maxHorOffSet);
            maxHorOffSet = -topOffset;
            int bottomOffset = UnityEngine.Random.Range(0, maxHorOffSet);
            int leftOffset = UnityEngine.Random.Range(0, maxVerOffSet);
            maxVerOffSet -= leftOffset;
            int rightOffset = UnityEngine.Random.Range(0, maxVerOffSet);

            for (int x = 0; x < split.delta.x; x++)
            {
                for (int y = 0; y < split.delta.y; y++)
                {
                    int coordX = split.start.x + x;
                    int coordY = split.start.y + y;

                    if (coordX == 0 || coordY == 0 || coordX == levelWidth || coordY == levelHeight) continue;

                    Vector2 gridPos = grid.GridPosToRealPos(new Vector2Int(coordX, coordY));
                    GameObject cube;

                    if (x <= topOffset || split.delta.x - x <= bottomOffset || y <= leftOffset || split.delta.y - y <= rightOffset) // wall
                    {
                        cube = Instantiate(destroyableWallBlock, transform);
                        cube.transform.localPosition = new Vector3(gridPos.x, 4, gridPos.y);
                    }
                }

            }
        }
    }

    private void SpawnLoot(LevelSplit[] splits)
    {
        foreach (LevelSplit split in splits)
        {
            var size = (split.delta.x - 2) * (split.delta.y - 2);

            var numLoot = 0;

            var postTaken = new List<Vector2Int>();

            if (size >= 120)
                numLoot++;
            if (size >= 80)
                numLoot++;
            if (size >= 50)
                numLoot++;
            if (size >= 35)
                numLoot++;
            if (size >= 20)
                numLoot++;
            if (size >= 8)
                numLoot++;

            if (UnityEngine.Random.value > 0.8f)
                numLoot++;

            for (int i = 0; i < numLoot; i++)
            {
                retryLoot:
                var lootX = (int) Mathf.Lerp(1, split.delta.x - 1, UnityEngine.Random.value);
                var lootY = (int) Mathf.Lerp(1, split.delta.y - 1, UnityEngine.Random.value);
                var lootVector = new Vector2Int(lootX, lootY);

                if (!postTaken.Contains(lootVector))
                {
                    var lootCoords =
                        grid.GridPosToRealPos(new Vector2Int(split.start.x + lootX, split.start.y + lootY));
                    var lootObject = Instantiate(lootBox, new Vector3(lootCoords.x, 1, lootCoords.y), new Quaternion());
                    postTaken.Add(lootVector);

                    lootObject.item = spellManager.getNewSpell(UnityEngine.Random.Range(0, spellManager.madeSpells.Count));
                    lootObject.index = lootList.Count;
                    
                    lootList.Add(lootObject);
                    lootObject.UpdateColor();
                }
                else goto retryLoot;
            }
        }
    }

    private void MakePlayerSpawns(LevelSplit[] splits)
    {
        foreach (LevelSplit split in splits)
        {
            var size = (split.delta.x - 2) * (split.delta.y - 2);

            if (size <= 16)
            {
                continue;
            }

            var spawnX = (int) Mathf.Lerp(1, split.delta.x - 1, UnityEngine.Random.value);
            var spawnY = (int) Mathf.Lerp(1, split.delta.y - 1, UnityEngine.Random.value);
            var spawnVector = new Vector2Int(spawnX, spawnY);
            var spawnCoords = grid.GridPosToRealPos(new Vector2Int(split.start.x + spawnX, split.start.y + spawnY));
            PlayerSpawnPoints.Add(spawnCoords);
        }

    }

    private LevelSplit[] SplitLevel(LevelSplit levelSplit, int splitOffset, bool horizontal)
    {
        LevelSplit[] splitResult = new LevelSplit[2];

        if (horizontal) // Horizontal split (so in width/x)
        {
            int newWidth = Mathf.RoundToInt(levelSplit.delta.x / 2) + splitOffset;
            splitResult[0] = new LevelSplit
                {start = levelSplit.start, delta = new Vector2Int(newWidth, levelSplit.delta.y)};
            splitResult[1] = new LevelSplit
            {
                start = new Vector2Int(levelSplit.start.x + newWidth, levelSplit.start.y),
                delta = new Vector2Int(levelSplit.delta.x - newWidth, levelSplit.delta.y)
            };
        }
        else // Vertical split (so in height/y)
        {
            int newHeigth = Mathf.RoundToInt(levelSplit.delta.y / 2) + splitOffset;
            splitResult[0] = new LevelSplit
                {start = levelSplit.start, delta = new Vector2Int(levelSplit.delta.x, newHeigth)};
            splitResult[1] = new LevelSplit
            {
                start = new Vector2Int(levelSplit.start.x, levelSplit.start.y + newHeigth),
                delta = new Vector2Int(levelSplit.delta.x, levelSplit.delta.y - newHeigth)
            };
        }

        return splitResult;
    }

    private bool DoISplitHorizontally(LevelSplit split)
    {
        if (split.delta.x / split.delta.y < minRoomRatio)
        {
            return false;
        }
        else if (split.delta.y / split.delta.x < minRoomRatio)
        {
            return true;
        }
        else return UnityEngine.Random.Range(0, 2) == 1;
    }

    //RCP
    public override void GenerateWorld(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            UnityEngine.Random.InitState(args.GetNext<int>());

            grid = new Assets.BlockGrid(levelWidth, levelHeight,
                new Vector2((destroyableWallBlock.transform.localScale.x * levelWidth),
                    (destroyableWallBlock.transform.localScale.z * levelHeight)));

            splits = SplitLevels();

            if (buildBlocks)
            {
                BuildBlocks(splits);
                SpawnLoot(splits);
            }
        });
    }
}