﻿using System.Collections.Generic;
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
        _camera.orthographicSize = Mathf.Lerp(_camera.orthographicSize, Mathf.Lerp(5, 20, _completionRatio), Time.deltaTime);
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
        float completionRatio = (_numberOfPeople / (1.0f * _numberToWin));
        float toFlip = Mathf.Ceil( m_spawners.Count * completionRatio);
        _completionRatio = completionRatio;

        List<BuildingSpawner> orderedBuildingsToConvert = new List<BuildingSpawner>();
        List<BuildingSpawner> buildingsLeft = new List<BuildingSpawner>();

        int numBuildingsConverted = 0;

        if (this == null) return;

        Vector3 centerPos = this.transform.position;

        
        foreach (BuildingSpawner thisBuilding in m_spawners)
        {

            if (thisBuilding == null) break;
            Vector3 thisBuildingPos = thisBuilding.transform.position;
            float thisBuildingDistance = Vector3.Distance(thisBuildingPos, centerPos);

            if (thisBuilding.isConverted)
            {
                numBuildingsConverted++;
            }
            else
            {
                if (orderedBuildingsToConvert.Count == 0) orderedBuildingsToConvert.Add(thisBuilding);
                else
                {
                    int i = 0;
                    foreach(BuildingSpawner refBuilding in orderedBuildingsToConvert)
                    {
                        Vector3 refBuildingPos = refBuilding.transform.position;
                        float refBuildingDistance = Vector3.Distance(refBuildingPos, centerPos);


                        if (thisBuildingDistance <= refBuildingDistance)
                        {
                            orderedBuildingsToConvert.Insert(i, thisBuilding);
                            break;
                        }

                        i++;
                    }
                }
            }
        }

        toFlip -= numBuildingsConverted;

        foreach (BuildingSpawner spawner in orderedBuildingsToConvert)
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