using UnityEngine;
using TMPro;

public enum GameState
{
    Menu,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    public Board board; // 보드 참조
    public int score; // 현재 점수

    public GameState State { get; private set; } = GameState.Menu;

    [Header("UI")]
    public TextMeshProUGUI scoreText; // 점수 표시 UI
    public GameObject startUI; // 시작 화면 UI
    public GameObject gameOverUI; // 게임 오버 UI

    [Header("Audio")]
    public AudioSource bgmSource; // 배경음악 소스
    public AudioSource sfxSource; // 효과음 소스
    public AudioClip lineClearSound; // 줄 삭제 효과음
    public AudioClip gameOverSound; // 게임 오버 효과음

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 게임 시작 시 메뉴 상태로 초기화
        ChangeState(GameState.Menu);
    }

    // 게임 상태 변경 및 처리
    public void ChangeState(GameState newState)
    {
        State = newState;

        // 상태에 따른 UI 활성화/비활성화
        if (startUI != null) startUI.SetActive(State == GameState.Menu);
        if (gameOverUI != null) gameOverUI.SetActive(State == GameState.GameOver);

        if (State == GameState.Playing)
        {
            // 게임 시작: 점수 초기화, 보드 활성화, BGM 재생, 새 블록 생성
            score = 0;
            UpdateScoreUI();
            board.ClearAllLines();
            board.enabled = true;
            
            if (bgmSource != null)
            {
                bgmSource.Play();
            }

            board.SpawnPiece();
        }
        else
        {
            // 게임 오버 또는 메뉴 상태: 보드 비활성화, BGM 정지
            board.enabled = false;
            if (bgmSource != null)
            {
                bgmSource.Stop();
            }
        }
    }

    // UI 버튼에서 호출될 게임 시작 함수
    public void StartGame()
    {
        ChangeState(GameState.Playing);
    }

    // 점수 추가 메서드
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
    }

    // 점수 UI 업데이트
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // 게임 오버 처리
    public void CheckGameOver()
    {
        ChangeState(GameState.GameOver);
        PlaySFX(gameOverSound);
    }

    // 효과음 재생 도우미 메서드
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
