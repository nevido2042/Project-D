using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class ScoreEntry
{
    public int score;
    // We can add player name here later if needed: public string playerName;

    public ScoreEntry(int score)
    {
        this.score = score;
    }
}

public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance { get; private set; }

    private const string PREFS_KEY = "TetrisTopScores";
    public int MaxScores = 5; // How many top scores to keep

    public List<ScoreEntry> TopScores { get; private set; } = new List<ScoreEntry>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadScores();
            DontDestroyOnLoad(gameObject); // Optional: Keep it alive across scenes if needed
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Attempt to add a new score. Returns true if it made it to the top list.
    public bool AddScore(int newScore)
    {
        // Don't add 0 scores unless you want to
        if (newScore <= 0) return false;

        TopScores.Add(new ScoreEntry(newScore));

        // Sort descending
        TopScores = TopScores.OrderByDescending(s => s.score).ToList();

        // Keep only the top 'MaxScores'
        if (TopScores.Count > MaxScores)
        {
            TopScores.RemoveRange(MaxScores, TopScores.Count - MaxScores);
        }

        SaveScores();

        // Check if the newly added score is in the top list (this simple logic works because we just added it and it wasn't culled, or it's within the count)
        return TopScores.Any(s => s.score == newScore);
    }

    private void SaveScores()
    {
        // Convert the list of scores to a simple comma-separated string
        // E.g., "1000,800,500,100"
        string data = string.Join(",", TopScores.Select(s => s.score.ToString()));
        PlayerPrefs.SetString(PREFS_KEY, data);
        PlayerPrefs.Save();
        Debug.Log("Scores Saved: " + data);
    }

    private void LoadScores()
    {
        TopScores.Clear();
        if (PlayerPrefs.HasKey(PREFS_KEY))
        {
            string data = PlayerPrefs.GetString(PREFS_KEY);
            if (!string.IsNullOrEmpty(data))
            {
                string[] scoreStrings = data.Split(',');
                foreach (string s in scoreStrings)
                {
                    if (int.TryParse(s, out int scoreVal))
                    {
                        TopScores.Add(new ScoreEntry(scoreVal));
                    }
                }
            }
        }
    }
    
    // Helper to format scores for UI
    public string GetRankingText()
    {
        if (TopScores == null || TopScores.Count == 0)
        {
            return "No Scores Yet";
        }

        string result = "";
        for (int i = 0; i < TopScores.Count; i++)
        {
            result += $"{i + 1}. {TopScores[i].score}\n";
        }
        return result;
    }
}
