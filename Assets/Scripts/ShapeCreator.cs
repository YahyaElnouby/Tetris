using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShapeCreator : MonoBehaviour
{
    public GameObject[] allShapes;
    public Text scoreText;
    public static int score = 0;
    public GameObject restartButton;

    private void Awake()
    {
        CreateShape();
        restartButton.SetActive(false);
        score = 0;
        scoreText.text = "Score " + 0;
    }

    public void CreateShape()
    {
        
        if (FindObjectOfType<BlockScript>() == null || !FindObjectOfType<BlockScript>().GameOver())
        {
            Instantiate(allShapes[Random.Range(0, allShapes.Length)], transform.position, Quaternion.identity);
        }
        else if (FindObjectOfType<BlockScript>().GameOver())
        {
            Debug.Log("The game is " + FindObjectOfType<BlockScript>().GameOver());
            restartButton.SetActive(true);
        }
    }

    public void ScoreUpdate(bool lineCleared)
    {
        if (lineCleared)
        {
            score += 40;
            scoreText.text = "Score " + score;
        }
        else
        {
            score += 10;
            scoreText.text = "Score " + score;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Tetris Main");
        restartButton.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
