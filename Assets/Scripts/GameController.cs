using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject hazard;
    public Vector3 spawnValues;
    public int hazardCount;
    public float spawnWait;
    public float startWait;
    public float waveWait;

    public List<Hazard> hazards;
    private bool gameOver;
    private bool restart;
    private int score;

    private Canvas canvas;

    private void Start()
    {
        gameOver = false;
        restart = false;
        hazards = new List<Hazard>();
        score = 0;
        StartCoroutine(SpawnAsteroids(hazards));
    }

    private void Update()
    {
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    IEnumerator SpawnAsteroids(List<Hazard> hazards)
    {
        yield return new WaitForSeconds(startWait);
        while (true)
        {
            for (int i = 0; i < hazardCount; i++)
            {
                hazards.Add(new Hazard
                {
                    Type = HazardType.Asteroid,
                    X = Random.Range(-spawnValues.x, spawnValues.x),
                    Y = Random.Range(-spawnValues.z, spawnValues.z)
                });
                Vector3 spawnPosition = new Vector3(hazards[i].X, spawnValues.y, hazards[i].Y);
                Quaternion spawnRotation = Quaternion.identity;
                Instantiate(hazard, spawnPosition, spawnRotation);
                yield return new WaitForSeconds(spawnWait);
            }
            yield return new WaitForSeconds(waveWait);

            if (gameOver)
            {
                restart = true;
                break;
            }
        }
    }

    public void AddScore(int scoreValue)
    {
        score += scoreValue;
        UpdateScore();
    }

    public void GameOver()
    {
        gameOver = true;
    }

    void UpdateScore()
    {
    }
}
