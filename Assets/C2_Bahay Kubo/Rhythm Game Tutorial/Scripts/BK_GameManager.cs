using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class BK_GameManager : MonoBehaviour
{

    public AudioSource theMusic;

    public bool startPlaying;

    public BeastScroller theBS;

    public static BK_GameManager instance;

    public int currentScore;
    public int scorePerNote = 100;

    public int currentMultiplier;
    public int multiplierTracker;
    public int[] multiplierThresholds;


    public Text scoreText;
    public Text multiText;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;

        scoreText.text = "Score: 0 ";
        currentMultiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (!startPlaying)
        {
            if (Input.anyKeyDown)
            {
                startPlaying = true;
                theBS.hasStarted = true;

                theMusic.Play();
            }

        }
    }

    public void NoteHit()
    {
        Debug.Log("Hit on time");

        currentScore += scorePerNote;
        scoreText.text = "Score: " + currentScore * currentMultiplier;
    }

    public void NoteMissed()
    {
        Debug.Log("Missed Note");
    }
}
