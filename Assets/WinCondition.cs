using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

class WinCondition: MonoBehaviour
{
    private int _numberOfPeople = 0;

    [SerializeField]
    private int _numberToWin = 1;

    [SerializeField]
    private ProgressBarPro _progress;

    public static WinCondition s_instance;

    private List<BuildingSpawner> m_spawners;

    void Start()
    {
        s_instance = this;
        m_spawners = FindObjectsOfType<BuildingSpawner>().ToList();
        m_spawners.Shuffle();
    }

    public void NewPersonChained()
    {
        _numberOfPeople++;
        UpdateScore();
        
        if (_numberOfPeople >= _numberToWin)
        {
            SceneManager.LoadScene("WinScreen");
        }
    }

    public void PersonDied()
    {
        _numberOfPeople--;
        UpdateScore();
    }

    public void UpdateScore()
    {
        float toFlip =Mathf.Ceil( m_spawners.Count * (_numberOfPeople / (1.0f * _numberToWin)));

        foreach(BuildingSpawner spawner in m_spawners)
        {
            if (toFlip > 0.0f)
            {
                spawner.Convert(true);
            }
            else
            {
                break;
            }
            toFlip--;
        }

        _progress.SetValue(_numberOfPeople, _numberToWin);
    }
}