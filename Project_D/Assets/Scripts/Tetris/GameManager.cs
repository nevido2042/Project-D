using UnityEngine;
using TMPro; // TextMeshPro를 사용하기 위해 추가

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    public Board board; // 보드 참조
    public int score; // 현재 점수
    public TextMeshProUGUI scoreText; // 점수 표시 UI

    [Header("Audio")]
    public AudioSource bgmSource; // 배경음악 오디오 소스
    public AudioSource sfxSource; // 효과음 오디오 소스
    public AudioClip lineClearSound; // 줄 제거 효과음
    public AudioClip gameOverSound; // 게임 오버 효과음

    private void Awake()
    {
        // 싱글톤 패턴 설정
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
        UpdateScoreUI();
        if (bgmSource != null && !bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }

    // 줄이 제거될 때 점수 추가 및 UI 업데이트
    public void AddScore(int linesCleared)
    {
        int points = 0;
        switch (linesCleared)
        {
            case 1: points = 100; break;
            case 2: points = 300; break;
            case 3: points = 500; break;
            case 4: points = 800; break;
        }
        score += points;
        UpdateScoreUI();
        
        // 줄 제거 효과음 재생
        if (sfxSource != null && lineClearSound != null)
        {
            sfxSource.PlayOneShot(lineClearSound);
        }
        
        Debug.Log("현재 점수: " + score);
    }

    // UI 텍스트 업데이트
    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    // 게임 종료 처리
    public void GameOver()
    {
        if (board != null)
        {
            board.enabled = false;
        }
        
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }

        if (sfxSource != null && gameOverSound != null)
        {
            sfxSource.PlayOneShot(gameOverSound);
        }

        Debug.Log("게임 오버!");
    }
}
