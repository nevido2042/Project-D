using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject blockPrefab;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Transform[,] grid;

    public Piece activePiece;
    public TetrominoData[] tetrominos;
    public Color[] colors;

    public Vector2Int spawnPosition = new Vector2Int(4, 18);

    private void Awake()
    {
        grid = new Transform[boardSize.x, boardSize.y];

        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        if (!enabled) return;

        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        Color color = colors != null && colors.Length > 0 ? colors[random % colors.Length] : Color.white;

        activePiece.Initialize(this, spawnPosition, data, color);

        if (!IsValidPosition(activePiece, spawnPosition))
        {
            GameManager.Instance?.GameOver();
        }
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int pos = piece.cells[i] + piece.position;
            
            if (pos.x >= 0 && pos.x < boardSize.x && pos.y >= 0 && pos.y < boardSize.y)
            {
                GameObject block = Instantiate(blockPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
                block.GetComponent<SpriteRenderer>().color = piece.color;
                grid[pos.x, pos.y] = block.transform;
            }
        }
    }

    public void ClearLines()
    {
        int row = 0;
        int linesCleared = 0;

        while (row < boardSize.y)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
                linesCleared++;
            }
            else
            {
                row++;
            }
        }

        if (linesCleared > 0)
        {
            GameManager.Instance?.AddScore(linesCleared);
        }
    }

    private bool IsLineFull(int row)
    {
        for (int col = 0; col < boardSize.x; col++)
        {
            if (grid[col, row] == null)
            {
                return false;
            }
        }
        return true;
    }

    private void LineClear(int row)
    {
        for (int col = 0; col < boardSize.x; col++)
        {
            Destroy(grid[col, row].gameObject);
            grid[col, row] = null;
        }

        while (row < boardSize.y - 1)
        {
            for (int col = 0; col < boardSize.x; col++)
            {
                grid[col, row] = grid[col, row + 1];

                if (grid[col, row] != null)
                {
                    grid[col, row].position += Vector3.down;
                }

                grid[col, row + 1] = null;
            }
            row++;
        }
    }

    public bool IsValidPosition(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int pos = piece.cells[i] + position;

            if (pos.x < 0 || pos.x >= boardSize.x || pos.y < 0)
            {
                return false;
            }

            if (pos.y < boardSize.y && grid[pos.x, pos.y] != null)
            {
                return false;
            }
        }

        return true;
    }
}
