using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static GameManager Instance { get; private set; }

    public Board board; // 보드 참조
    public int score; // 현재 점수

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

    // 줄이 제거될 때 점수 추가
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
        Debug.Log("현재 점수: " + score);
    }

    // 게임 종료 처리
    public void GameOver()
    {
        if (board != null)
        {
            board.enabled = false;
        }
        Debug.Log("게임 오버!");
    }
}
