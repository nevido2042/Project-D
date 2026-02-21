using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } // 보드 참조
    public TetrominoData data { get; private set; } // 테트리미노 데이터
    public Vector2Int[] cells { get; private set; } // 현재 피스의 구성 블록 좌표들
    public Vector2Int position { get; private set; } // 현재 피스의 위치
    public int rotationIndex { get; private set; } // 현재 회전 상태 인덱스
    public Color color { get; private set; } // 피스 색상

    public float stepDelay = 1f; // 자동으로 아래로 떨어지는 간격
    public float lockDelay = 0.5f; // 바닥에 닿은 후 고정되기 전까지의 유예 시간

    private float stepTime;
    private float lockTime;

    private GameObject[] visualBlocks; // 시각적 블록 표현

    [Header("Audio")]
    public AudioClip moveSound; // 이동 효과음
    public AudioClip rotateSound; // 회전 효과음
    public AudioClip lockSound; // 고정 효과음
    private AudioSource audioSource; // 오디오 소스

    public void Initialize(Board board, Vector2Int position, TetrominoData data, Color color)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.color = color;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (cells == null)
        {
            cells = new Vector2Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = data.cells[i];
        }

        if (visualBlocks == null)
        {
            visualBlocks = new GameObject[cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                visualBlocks[i] = Instantiate(board.blockPrefab, transform);
            }
        }

        UpdateVisuals();
    }

    private void Update()
    {
        if (!board.enabled) return;

        lockTime += Time.deltaTime;

        // New Input System을 사용한 키보드 입력 처리
        if (Keyboard.current != null)
        {
            // 왼쪽 이동
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                if (Move(Vector2Int.left)) PlaySound(moveSound);
            }
            // 오른쪽 이동
            else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                if (Move(Vector2Int.right)) PlaySound(moveSound);
            }

            // 아래로 가속 (소프트 드롭)
            if (Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                if (Move(Vector2Int.down)) PlaySound(moveSound);
            }

            // 즉시 하강 (하드 드롭)
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                HardDrop();
            }

            // 시계 방향 회전
            if (Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                Rotate(1);
            }
        }

        // 자동 하강 타이머 체크
        if (Time.time >= stepTime)
        {
            Step();
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 한 칸 하강 시도
    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        // 바닥에 닿은 채로 유예 시간이 지났다면 고정
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    // 바닥까지 쭉 떨어뜨림
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    // 피스를 보드에 고정시키고 새로운 피스 생성 요청
    private void Lock()
    {
        PlaySound(lockSound);
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
        
        foreach(var block in visualBlocks) {
            block.SetActive(false);
        }
    }

    // 지정된 방향으로 이동 시도
    private bool Move(Vector2Int translation)
    {
        Vector2Int newPosition = position + translation;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            lockTime = 0f;
            UpdateVisuals();
        }

        return valid;
    }

    // 회전 시도
    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        // 회전 시 벽에 부딪히는 경우 Wall Kick(벽 차기) 보정 시도
        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
        else
        {
            PlaySound(rotateSound);
            UpdateVisuals();
        }
    }

    // 회전 행렬을 적용하여 세부 블록 좌표 변경
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector2 val = cells[i];
            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    val.x -= 0.5f;
                    val.y -= 0.5f;
                    x = Mathf.CeilToInt((val.x * Mathf.Cos(direction * Mathf.PI / 2f)) - (val.y * Mathf.Sin(direction * Mathf.PI / 2f)));
                    y = Mathf.CeilToInt((val.x * Mathf.Sin(direction * Mathf.PI / 2f)) + (val.y * Mathf.Cos(direction * Mathf.PI / 2f)));
                    break;

                default:
                    x = Mathf.RoundToInt((val.x * Mathf.Cos(direction * Mathf.PI / 2f)) - (val.y * Mathf.Sin(direction * Mathf.PI / 2f)));
                    y = Mathf.RoundToInt((val.x * Mathf.Sin(direction * Mathf.PI / 2f)) + (val.y * Mathf.Cos(direction * Mathf.PI / 2f)));
                    break;
            }
            cells[i] = new Vector2Int(x, y);
        }
    }

    // 벽 차기 보정 데이터 테스트
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(0); i++)
        {
            Vector2Int translation = data.wallKicks[i, wallKickIndex];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(1));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    // 피스의 시각적 위치 업데이트
    private void UpdateVisuals()
    {
        for (int i = 0; i < cells.Length; i++)
        {
            visualBlocks[i].SetActive(true);
            visualBlocks[i].transform.position = new Vector3(position.x + cells[i].x, position.y + cells[i].y, 0);
            visualBlocks[i].GetComponent<SpriteRenderer>().color = color;
        }
    }
}
