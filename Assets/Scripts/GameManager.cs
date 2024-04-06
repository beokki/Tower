using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject enemyPrefab;

    GameTile[,] tiles;
    private GameTile spawnTile;
    const int column = 20;
    const int row = 10;

    private void Awake()
    {
        tiles = new GameTile[column, row];

        for (int x = 0; x < column; x++)
        {
            for (int y = 0; y < row; y++)
            {
                var spawnPos = new Vector3(x, y, 0);
                var tile = Instantiate(tilePrefab, spawnPos, Quaternion.identity);
                tiles[x, y] = tile.GetComponent<GameTile>();
                if ((x + y) % 2 == 0)
                {
                    tile.GetComponent<GameTile>().TurnGray();
                }
            }
        }

        spawnTile = tiles[1, 8];
        spawnTile.SetEnemySpawn();
        StartCoroutine(SpawnEnemyCoroutine());
    }

    IEnumerator SpawnEnemyCoroutine()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                yield return new WaitForSeconds(0.6f);
                Instantiate(enemyPrefab, spawnTile.transform.position, Quaternion.identity);
            }
            yield return new WaitForSeconds(2f);
        }
    }
}
