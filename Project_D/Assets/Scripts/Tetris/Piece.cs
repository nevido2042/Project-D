using UnityEngine;
using UnityEngine.InputSystem;

public class Piece : MonoBehaviour
{
    public Board board { get; private set; } // 보드 참조
    public TetrominoData data { get; private set; } // 테트리미노 데이터
    public Vector3Int position { get; private set; } // 현재 위치
    public int rotationIndex { get; private set; } // 회전 상태 인덱스
    public Vector3Int[] cells { get; private set; } // 현재 형태의 셀들
    public Color color { get; private set; } // 블록 색상

    private Transform[] blockTransforms; // 화면에 보여줄 4개의 블록 트랜스폼

    // 새로운 Input System 액션 참조
    private InputAction moveAction;
    private InputAction dropAction;
    private InputAction rotateAction;
    private InputAction hardDropAction;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip moveSound;
    public AudioClip rotateSound;
    public AudioClip lockSound;

    [Header("Timers")]
    public float stepDelay = 1f; // 기본 하강 속도
    public float lockDelay = 0.5f; // 바닥에 닿은 뒤 고정되기 전 대기 시간

    private float stepTime;
    private float lockTime;

    // 초기화 클래스
    public void Initialize(Board board, Vector3Int position, TetrominoData data, Color color)
    {
        this.board = board;
        this.position = position;
        this.data = data;
        this.color = color;
        this.rotationIndex = 0;
        this.stepTime = 0f;
        this.lockTime = 0f;

        // 원본 데이터를 손상시키지 않기 위해 셀 배열 복사
        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
            // 중요: 배열 원소가 부모 중심에 존재하도록 보드에서 offset 설정
        }

        // 오디오 소스 설정
        audioSource = GetComponent<AudioSource>();

        // 비주얼 블록 생성 및 설정
        if (blockTransforms == null)
        {
            blockTransforms = new Transform[data.cells.Length];
            for (int i = 0; i < data.cells.Length; i++)
            {
                GameObject block = Instantiate(board.blockPrefab, transform);
                blockTransforms[i] = block.transform;
            }
        }

        for (int i = 0; i < cells.Length; i++)
        {
            blockTransforms[i].localPosition = cells[i];
            blockTransforms[i].GetComponent<SpriteRenderer>().color = this.color;
        }
        
        transform.position = this.position;
    }

    private void Start()
    {
        // Unity Input System 설정. PlayerInput 컴포넌트나 Action 자산을 사용하지 않고 코드에서 직접 구성
        var actionMap = new InputActionMap("Tetris");

        // 좌/우 및 하강 이동 (WASD 및 화살표 지원)
        moveAction = actionMap.AddAction("Move");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/s")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/a")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/d")
            .With("Right", "<Keyboard>/rightArrow");

        // 회전
        rotateAction = actionMap.AddAction("Rotate");
        rotateAction.AddBinding("<Keyboard>/q");     // 반시계
        rotateAction.AddBinding("<Keyboard>/e");     // 시계
        rotateAction.AddBinding("<Keyboard>/upArrow"); // 방향키 위 (빠른 시계 맵핑)

        // 하드 드롭 (한번에 떨어지기)
        hardDropAction = actionMap.AddAction("HardDrop");
        hardDropAction.AddBinding("<Keyboard>/space");

        actionMap.Enable();

        // 입력 이벤트 등록
        moveAction.performed += OnMove;
        rotateAction.performed += OnRotate;
        hardDropAction.performed += OnHardDrop;
    }

    private void OnDestroy()
    {
        // 스크립트 파괴 시 메모리 누수 방지
        if (moveAction != null) moveAction.performed -= OnMove;
        if (rotateAction != null) rotateAction.performed -= OnRotate;
        if (hardDropAction != null) hardDropAction.performed -= OnHardDrop;
    }

    private void Update()
    {
        // 게임 상태가 Playing일 때만 로직 실행 (보드가 활성화된 상태)
        if (board == null || !board.enabled || GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) {
            return;
        }

        stepTime += Time.deltaTime;

        // 잠금 대기 시간 로직 및 자동 하강
        if (board.IsValidPosition(this, position + Vector3Int.down))
        {
            lockTime = 0f; // 떨어질 수 있으면 잠금 타이머 초기화
            if (stepTime >= stepDelay)
            {
                stepTime = 0f;
                Move(Vector2Int.down);
            }
        }
        else
        {
            // 바닥에 닿은 상태
            lockTime += Time.deltaTime;
            if (lockTime >= lockDelay)
            {
                Lock();
                return; // 고정된 후에는 아래 로직 스킵
            }
        }

        // Piece 오브젝트의 실제 위치와 하위 블록 위치 업데이트
        transform.position = this.position;
        for (int i = 0; i < cells.Length; i++)
        {
            blockTransforms[i].localPosition = cells[i];
        }
    }

    public bool MoveLeft()
    {
        return Move(Vector2Int.left);
    }

    public bool MoveRight()
    {
        return Move(Vector2Int.right);
    }

    public bool MoveDown()
    {
        if (Move(Vector2Int.down))
        {
            stepTime = 0f;
            return true;
        }
        return false;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        // 현재 게임 상태 확인
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;

        Vector2 direction = context.ReadValue<Vector2>();

        // 좌우 이동
        if (direction.x > 0.5f)
        {
            MoveRight();
        }
        else if (direction.x < -0.5f)
        {
            MoveLeft();
        }

        // 아래로 부드러운 하강 (소프트 드롭)
        if (direction.y < -0.5f)
        {
            MoveDown();
        }
    }

    public void RotateClockwise()
    {
        Rotate(1);
    }

    private void OnRotate(InputAction.CallbackContext context)
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
        RotateClockwise();
    }

    public void TriggerHardDrop()
    {
        HardDrop();
    }

    private void OnHardDrop(InputAction.CallbackContext context)
    {
        if (GameManager.Instance == null || GameManager.Instance.State != GameState.Playing) return;
        TriggerHardDrop();
    }

    // 지정된 방향으로 이동 시도
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        // 새 위치가 유효한지 검사
        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            PlaySFX(moveSound);
            lockTime = 0f; // 이동 성공 시 잠금 딜레이 초기화
        }

        return valid;
    }

    // 회전 로직 (단순 구현, 월 킥 제외)
    private void Rotate(int direction)
    {
        // 1. 현재 회전 상태 저장
        int originalRotation = rotationIndex;
        
        // 2. 회전 행렬 적용
        rotationIndex = (rotationIndex + direction + 4) % 4;
        ApplyRotationMatrix(direction);

        // 3. 충돌 검사
        if (!board.IsValidPosition(this, position))
        {
            // 충돌 시 원래 상태로 복구
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction); 
        }
        else
        {
            PlaySFX(rotateSound);
            lockTime = 0f; // 회전 성공 시 잠금 딜레이 초기화
        }
    }

    // 블록의 셀들을 90도 회전
    private void ApplyRotationMatrix(int direction)
    {
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // I와 O 블록은 회전 중심이 .5 오프셋을 가짐 (이번 예제에서는 중앙 기준 단순화 처리)
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * Data.RotationMatrix[0] * direction) + (cell.y * Data.RotationMatrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * Data.RotationMatrix[2] * direction) + (cell.y * Data.RotationMatrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // 즉시 바닥으로 하강 (하드 드롭)
    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

        Lock();
    }

    // 바닥이나 다른 블록에 닿았을 때 고정
    public void Lock()
    {
        board.Set(this); // 현재 위치에 블록 고정
        board.ClearLines(); // 완성된 줄 확인 및 삭제
        PlaySFX(lockSound);
        board.SpawnPiece(); // 새로운 블록 생성 (이 과정에서 오버플로우 체크)
    }

    // 효과음 재생 도우미 메서드
    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
