﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

	public class PlayerController : MonoBehaviour {
    public GameObject player;
	public GameObject ballPrefab;
    private Rigidbody rb;
	private GameController Controller;

    private int life;
	private int ballRemaining = 2;
    private float timer = 1f;
    private float moveH, moveV, moveSpeed;
    private float playerScale;

    public AudioSource loseLife, endSound, coin;

    public Text txtScore, txtLives, txtCenter;

    IEnumerator Start()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                moveSpeed = 1000f;
                playerScale = 3f;
                break;
            case 1:
                moveSpeed = 200f;
                playerScale = 3f;
                break;
            case 2:
                break;
            default:
                break;
        }
        rb = gameObject.GetComponent<Rigidbody>();
        player = GameObject.Find("Player").gameObject;
        player.transform.localScale = new Vector3(playerScale, 1f, 1f);

		Controller = (GameController)GameObject.Find ("Main").GetComponent("GameController");

        life = GameController.lives;

        //endSound = GameObject.Find("GameOver").gameObject.GetComponent<AudioSource>();

        yield return new WaitForSecondsRealtime(2);
    }

    public IEnumerator StartBox()   //called from NewLife()
    {
        yield return new WaitForSecondsRealtime(1);

        while (!(Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Space)))
        {
            yield return null;
        }
        Controller.SetCenter(" ");
        
        Time.timeScale = 1;
        player.transform.position = new Vector3(0f, 0f, 0f);
    }

    void Update () 
	{
		if (Input.GetMouseButtonDown (0)&& ballRemaining!=0) 
		{
			var ball = Instantiate (ballPrefab) as GameObject;
			ball.transform.position = (transform.position + new Vector3 (0f, 2f, 0f));
			ballRemaining -- ;
		}
        if (life != GameController.lives)
        {
            life = GameController.lives;
            LoseLife();
        }

        if (Input.GetMouseButtonDown(1))
        {
            GameController.lives--;
            LoseLife();
        }

		if (transform.position.x > 30f) transform.position = new Vector3(30f, transform.position.y, 0f);
		if (transform.position.x < -30f) transform.position = new Vector3(-30f, transform.position.y, 0f);
		//!!This is for restrict the posistion of the player, because the panel will eventually move out of the playing area under extream circumanstance.
		//If the scale of the panel changes, these two lines need to be fixed also.
    }

    void FixedUpdate()//I nearly rewrite this function, although I disabled the F and H key -- they can no longer move the pad. But the mouse now works better.
    {
        float mouseH = Input.GetAxis("Mouse X");
        float mouseV = Input.GetAxis("Mouse Y");
		float keyboardH = 0f;

        if (mouseH != 0f || mouseV != 0f)
        {
            Vector3 motion = new Vector3(mouseH * moveSpeed, 0f, 0f);
            rb.AddForce(motion * moveSpeed);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
        	keyboardH = -1f;
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
			keyboardH = 1f;
        }

		Vector3 keyboardMotion = new Vector3 (keyboardH, 0f, 0f);

		//rb.velocity = Vector3.zero;
		rb.AddForce (keyboardMotion * moveSpeed);


		if (!Input.GetKey (KeyCode.F) || !Input.GetKey (KeyCode.H) ) 
		{
			rb.velocity = Vector3.zero;
		}

    }

	private void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.tag.Equals("block"))
		{
			int points = (int)other.gameObject.GetComponent<Rigidbody> ().mass;
			Controller.AddScore(points);
			coin.Play ();
		}
	}

    public void LoseLife()
    {
        int lives = GameController.lives;// Shall we make this variable a global variable? --Yanbo

        if (lives < 1) { endSound.Play(); }
        else { loseLife.Play(); }

        Time.timeScale = 0;

        if (lives < 1)
        {
            Controller.SetCenter("Game Over\nYour Final Score is: " + GameController.score.ToString() + "\n\nPress (r) to try again");
            GameController.instance.PlayerDead();
        }
        else
        {
            Controller.SetCenter("\nYou dropped the ball.\nPress(r or space) to continue");
            NewLife();
        }
		ballRemaining = 2;
		//endSound.Play();// It is supposed to play the end sound, however it doesn't, I need to figure it out -- Yanbo
    }

    public void NewLife()
    {
        //var blocks = GameObject.FindGameObjectsWithTag("block"); //on next level
        //foreach (GameObject block in blocks) Destroy(block);
        StartCoroutine(StartBox());
    }
}
