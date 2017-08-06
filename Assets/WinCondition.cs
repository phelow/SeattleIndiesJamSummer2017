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

    public AudioClip newFollowerSound;
    public AudioClip followerDiedSound;

    [SerializeField]
    private Camera _camera;

    private float _completionRatio;

    void Start()
    {
        s_instance = this;
        m_spawners = FindObjectsOfType<BuildingSpawner>().ToList();
        m_spawners.Shuffle();
        UpdateScore();
    }

    private void Update()
    {
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, Mathf.Lerp(8, 20, _completionRatio), Time.deltaTime);
    }

    public void NewPersonChained()
    {
        _numberOfPeople++;
        UpdateScore();
        
        if (_numberOfPeople >= _numberToWin)
        {
            SceneManager.LoadScene("WinScreen");
        }

        SoundSystem.s_soundSystem.PlaySound(newFollowerSound);
    }

    public void PersonDied()
    {
        _numberOfPeople--;
        UpdateScore();

        SoundSystem.s_soundSystem.PlaySound(followerDiedSound);
    }

    public void UpdateScore()
    {
        float completionRatio = (_numberOfPeople / (1.0f * _numberToWin));
        float toFlip = Mathf.Ceil( m_spawners.Count * completionRatio);
        _completionRatio = completionRatio;

        foreach (BuildingSpawner spawner in m_spawners)
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