using UnityEngine;

public class Board : MonoBehaviour
{
    public GameObject blockPrefab; // 블록 프리팹
    public Vector2Int boardSize = new Vector2Int(10, 20); // 보드 크기 (10x20)
    public Transform[,] grid; // 보드 상의 블록들을 관리하는 2D 격자

    public Piece activePiece; // 현재 조작 중인 피스
    public TetrominoData[] tetrominos; // 테트리미노 데이터 배열
    public Color[] colors; // 테트리미노별 색상

    public Vector2Int spawnPosition = new Vector2Int(4, 18); // 피스 생성 초기 위치

    private void Awake()
    {
        grid = new Transform[boardSize.x, boardSize.y];

        // 각 테트리미노 데이터 초기화
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece(); // 게임 시작 시 첫 피스 생성
    }

    public void SpawnPiece()
    {
        if (!enabled) return;

        // 랜덤하게 다음 피스 선택
        int random = Random.Range(0, tetrominos.Length);
        TetrominoData data = tetrominos[random];

        Color color = colors != null && colors.Length > 0 ? colors[random % colors.Length] : Color.white;

        // 활성 피스 초기화
        activePiece.Initialize(this, spawnPosition, data, color);

        // 생성 위치부터 이미 블록이 있다면 게임 오버
        if (!IsValidPosition(activePiece, spawnPosition))
        {
            GameManager.Instance?.GameOver();
        }
    }

    // 피스가 바닥에 닿아 고정될 때 호출
    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int pos = piece.cells[i] + piece.position;
            
            if (pos.x >= 0 && pos.x < boardSize.x && pos.y >= 0 && pos.y < boardSize.y)
            {
                // 실제 프리팹을 생성하여 보드에 배치
                GameObject block = Instantiate(blockPrefab, new Vector3(pos.x, pos.y, 0), Quaternion.identity, transform);
                block.GetComponent<SpriteRenderer>().color = piece.color;
                grid[pos.x, pos.y] = block.transform;
            }
        }
    }

    // 꽉 찬 줄이 있는지 확인하고 제거
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

    // 특정 줄이 꽉 찼는지 확인
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

    // 특정 줄을 지우고 위의 블록들을 아래로 내림
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

    // 해당 위치가 이동 가능한지 충돌 체크
    public bool IsValidPosition(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int pos = piece.cells[i] + position;

            // 경계 밖 체크
            if (pos.x < 0 || pos.x >= boardSize.x || pos.y < 0)
            {
                return false;
            }

            // 이미 채워진 칸 체크
            if (pos.y < boardSize.y && grid[pos.x, pos.y] != null)
            {
                return false;
            }
        }

        return true;
    }
}
