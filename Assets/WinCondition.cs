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

    void Start()
    {
        s_instance = this;
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
        _progress.SetValue(_numberOfPeople, _numberToWin);
    }
}