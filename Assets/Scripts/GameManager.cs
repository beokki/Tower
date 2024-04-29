using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] private List<GameObject> enemyPrefab;
    [SerializeField] TMP_Text waveText;
    [SerializeField] TMP_Text enemyText;
    [SerializeField] TMP_Text killText;
    [SerializeField] TMP_Text hpText;

    GameTile[,] tiles;
    public GameTile spawnTile;
    const int column = 20;
    const int row = 10;

    public GameTile TargetTile { get; internal set; }
    readonly List<GameTile> pathEnd = new List<GameTile>();

    public int initEnemyCount = 5;
    public int currentWave = 1;
    public int killedCount = 0;

    public static GameManager instance;

    private List<GameTile> activeTurrets = new List<GameTile>();

    public GameObject skillTreeRoot;
    public GameObject gameplayRoot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        tiles = new GameTile[column, row];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var spawnPos = new Vector3(x, y, 0);
                var tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                tiles[x, y] = tile.GetComponent<GameTile>();
                tiles[x, y].GM = this;
                tiles[x, y].X = x;
                tiles[x, y].Y = y;
                if ((x + y) % 2 == 0)
                {
                    tile.GetComponent<GameTile>().TurnGray();
                }
            }
        }

        spawnTile = tiles[1, 8];
        spawnTile.SetEnemySpawn();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && TargetTile != null)
        {
            foreach (var t in tiles)
            {
                t.SetPath(false);
            }

            var path = Pathfinding(spawnTile, TargetTile);
            var tile = TargetTile;
            
            while (tile != null)
            {
                pathEnd.Add(tile);
                tile.SetPath(true);
                tile = path[tile];
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightShift))
        {
            StartCoroutine(SpawnEnemyWave());
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ShowSkillTree();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ShowGameplay();
        }
    }

    public void ShowGameplay()
    {
        gameplayRoot.SetActive(true);
        skillTreeRoot.SetActive(false);
    }

    public void ShowSkillTree()
    {
        skillTreeRoot.SetActive(true);
        gameplayRoot.SetActive(false);
    }

    private Dictionary<GameTile, GameTile> Pathfinding(GameTile srcTile, GameTile targetTile)
    {
        var dist = new Dictionary<GameTile, int>();
        var prev = new Dictionary<GameTile, GameTile>();
        var Q = new List<GameTile>();

        foreach (var v in tiles)
        {
            dist.Add(v, 9999);
            prev.Add(v, null);
            Q.Add(v);
        }
        dist[srcTile] = 0;

        while(Q.Count > 0)
        {
            GameTile u = null;
            int minDist = int.MaxValue;

            foreach (var v in Q)
            {
                if (dist[v] < minDist)
                {
                    minDist = dist[v];
                    u = v;
                }
            }

            Q.Remove(u);

            foreach (var v in FindNeighbor(u))
            {
                if (!Q.Contains(v) || v.IsBlocked)
                {
                    continue;
                }

                int alt = dist[u] + 1;

                if (alt < dist[v])
                {
                    dist[v] = alt;
                    prev[v] = u;
                }
            }
        }

        return prev;
    }

    private List<GameTile> FindNeighbor(GameTile u)
    {
        var r = new List<GameTile>();

        if (u.X - 1 >= 0)
            r.Add(tiles[u.X - 1, u.Y]);
        if (u.X + 1 < column)
            r.Add(tiles[u.X + 1, u.Y]);

        if (u.Y - 1 >= 0)
            r.Add(tiles[u.X, u.Y - 1]);
        if (u.Y + 1 < row)
            r.Add(tiles[u.X, u.Y + 1]);
        return r;
    }

    //IEnumerator SpawnEnemyCoroutine()
    //{
    //    while (true)
    //    {
    //        yield return StartCoroutine(SpawnEnemyWave());
    //        yield return new WaitUntil(() => Enemy.enemies.Count == 0);
    //        currentWave++;
    //        UpdateWaveText(currentWave);
    //        yield return new WaitForSeconds(3f);
    //    }
    //}

    public void StartNextWave()
    {
        if (Enemy.enemies.Count == 0)
        {
            StartCoroutine(SpawnEnemyWave());
            currentWave++;
            UpdateWaveText(currentWave);
            //CheckNodes();
        }
    }

    IEnumerator SpawnEnemyWave()
    {
        Difficulty();

        int currentEnemyCount = Mathf.CeilToInt(initEnemyCount * Mathf.Pow(1.25f, currentWave - 1));

        for (int i = 0; i < currentEnemyCount; i++)
        {
            yield return new WaitForSeconds(0.5f);
            GameObject enemyPrefabs = enemyPrefab[UnityEngine.Random.Range(0, enemyPrefab.Count)];
            var enemy = Instantiate(enemyPrefabs, spawnTile.transform.position, Quaternion.identity);
            var e = enemy.GetComponent<Enemy>();
            e.SetPath(pathEnd);
            e.hp = e.maxhp * (int)Mathf.Pow(1.25f, currentWave);
            //e.speed = e.speed * (int)Mathf.Pow(1.25f, currentWave);
            EnemySpawned();
        }
    }

    private void Difficulty()
    {
        if (Player.instance.health > Player.instance.health * 0.75)
        {
            initEnemyCount += 1;
        }
        else if (Player.instance.health < Player.instance.health * 0.5)
        {
            initEnemyCount = Mathf.Max(initEnemyCount - 1, 5);
        }
    }

    private void UpdateWaveText(int v)
    {
        if (waveText != null)
        {
            waveText.text = $"Wave: {v}";
        }
    }

    private void UpdateEnemyText()
    {
        if (enemyText != null)
            enemyText.text = $"Enemy: {Enemy.enemies.Count}";
    }

    private void UpdateKilledText(int v)
    {
        if (killText != null)
            killText.text = $"Killed: {v}";
    }

    public void EnemySpawned()
    {
        UpdateEnemyText();
    }

    public void EnemyDefeated()
    {
        killedCount++;
        UpdateKilledText(killedCount);
        UpdateEnemyText();
    }

    public void ClearEnemies()
    {
        foreach (var enemy in new HashSet<Enemy>(Enemy.enemies))
        {
            Destroy(enemy.gameObject);
        }
        Enemy.enemies.Clear();
    }

    public void Defeated()
    {
        Player.instance.ResetHealth();
        ClearEnemies();
        pathEnd.Clear();
        ResetWave();
    }

    public void ResetWave()
    {
        StopAllCoroutines();
        currentWave = 1;
        UpdateWaveText(currentWave);
        UpdateEnemyText();
        UpdateKilledText(0);
    }

    //public void CheckNodes()
    //{
    //    foreach (NodeButton node in FindObjectsOfType<NodeButton>())
    //    {
    //        if (node.currentState == NodeButton.NodeState.Unavailable)
    //        {
    //            node.SetState(NodeButton.NodeState.Available);
    //        }
    //    }
    //}

    public void RegisterTurret(GameTile turret)
    {
        if (!activeTurrets.Contains(turret))
        {
            activeTurrets.Add(turret);
        }
    }

    public void UnregisterTurret(GameTile turret)
    {
        activeTurrets.Remove(turret);
    }

    public void ApplyBonusToTurrets(Action<GameTile> apply)
    {
        foreach (GameTile turret in activeTurrets)
        {
            apply(turret);
        }
    }

    public void UpdateHealth(int amount)
    {
        Player.instance.health += amount;
        UpdateHPUI();
    }

    private void UpdateHPUI()
    {
        hpText.text = $"HP: {Player.instance.health += Player.bonusHP}";
    }
}
