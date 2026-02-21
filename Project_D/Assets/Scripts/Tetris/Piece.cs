using UnityEngine;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector2Int[] cells { get; private set; }
    public Vector2Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public Color color { get; private set; }

    public float stepDelay = 1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float lockTime;

    private GameObject[] visualBlocks;

    public void Initialize(Board board, Vector2Int position, TetrominoData data, Color color)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.color = color;
        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        lockTime = 0f;

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

        if (UnityEngine.InputSystem.Keyboard.current != null)
        {
            if (UnityEngine.InputSystem.Keyboard.current.leftArrowKey.wasPressedThisFrame)
            {
                Move(Vector2Int.left);
            }
            else if (UnityEngine.InputSystem.Keyboard.current.rightArrowKey.wasPressedThisFrame)
            {
                Move(Vector2Int.right);
            }

            if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame)
            {
                Move(Vector2Int.down);
            }

            if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                HardDrop();
            }

            if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                Rotate(1);
            }
        }

        if (Time.time >= stepTime)
        {
            Step();
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
        
        foreach(var block in visualBlocks) {
            block.SetActive(false);
        }
    }

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

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
        else
        {
            UpdateVisuals();
        }
    }

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
