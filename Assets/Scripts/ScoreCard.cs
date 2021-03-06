﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ScoreCard : MonoBehaviour
{
    #region Variables
    [SerializeField] private int m_SongIndex = 0; // Used to identify player pref for comparison
    Dictionary<int, List<Score>> m_SongScores = new Dictionary<int, List<Score>>();

    int m_targetHighscore = 0;      // Highest score for the selected song
    int m_displayedScore = 0;       // Score displayed to the player through the m_scoreText
    int m_actualScore = 0;          // The real score

    [SerializeField] InputField m_playerName;

    [SerializeField] Text m_scoreText;
    [SerializeField] Text[] m_Top10Text;

    private bool m_canScore = false;

    public void CanScore(bool b)
    {
        m_canScore = b;
    }

    public static ScoreCard instance
    {
        get
        {
            return FindObjectOfType<ScoreCard>();
        }
    }

    public int ActualScore      // public field
    {
        set
        {
            if(m_canScore)
                m_actualScore = value;

            if (m_displayedScore != m_actualScore)
                StartCoroutine(ScoreTick());
        }
        get { return m_actualScore; }
    }

    int DisplayScore
    {
        set
        {
            m_displayedScore = value;
            m_scoreText.text = m_displayedScore.ToString();
        }
    }
    #endregion

    private void Start()
    {
        List <Score> ls1 =  new List<Score>();
        m_SongScores.Add(0, ls1);
        List<Score> ls2 = new List<Score>();
        m_SongScores.Add(1, ls2);
        List<Score> ls3 = new List<Score>();
        m_SongScores.Add(2, ls3);
    }

    public void ScoreDelta(int a_deltaScore)
    {
        ActualScore += a_deltaScore;
    }

    public void StartNewRound(int index)
    {
        m_actualScore = 0;
        m_displayedScore = 0;

        ActualScore = 0;
        DisplayScore = 0;
        m_SongIndex = index;
        m_playerName.text = null;
    }

    public void SelfAddScore()
    {
        AddScore(m_playerName.text, ActualScore);
    }

    //[ContextMenu("print")]
    public void UpdateScores()
    {

        string t = "";
        foreach (Score s in m_SongScores[m_SongIndex])
        {
            t += s.name + "\t\t" + s.score + "\n";
        }
        m_Top10Text[m_SongIndex].text = t;
    }

    // debug Function and var
    //int test = 5;
    //[ContextMenu("Addscore")]
    //public void TestScorePush()
    //{
    //    AddScore("Daniel", test);
    //    test += 5;
    //}
    
    public void AddScore(Score a_score)
    {
        if (a_score.name == "")
            a_score.name = "John Doe";

        m_SongScores[m_SongIndex].Add(a_score);
        m_SongScores[m_SongIndex] = SortScores(m_SongScores[m_SongIndex]);

        string t = "";
        foreach(Score s in m_SongScores[m_SongIndex])
        {
            t += s.name + " " + s.score + ", ";
        }
    }

    public void AddScore(string a_name, int a_score)
    {
        AddScore(new Score(a_name, a_score));
    }

    List<Score> SortScores(List<Score> a_scoreList)
    {
        List<Score> r = new List<Score>();  // Return value

        while (a_scoreList.Count > 0)    
        {
            Score highest = a_scoreList[0];
            foreach(Score s in a_scoreList)
            {
                if (s.score > highest.score)
                    highest = s;
            }
            a_scoreList.Remove(highest);
            r.Add(highest);
        }

        var v = r.Take(10); // only keep the fist 10 // Top 10
        r = v.ToList<Score>();

        return r;   // return 
    }

    IEnumerator ScoreTick()
    {
        while(m_displayedScore != m_actualScore)
        {
            if (m_displayedScore < m_actualScore)
                DisplayScore = m_displayedScore + 1;

            else
                DisplayScore = m_displayedScore - 1;

            yield return null;
        }

    }
}

public struct Score : IComparer <Score>
{
    public string name;
    public int score;

    public Score(string name, int score)
    {
        this.name = name;
        this.score = score;
    }

    public int Compare(Score x, Score y)
    {
        return x.score - y.score;
    }
}