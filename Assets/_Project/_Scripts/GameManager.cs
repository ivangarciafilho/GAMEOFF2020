using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[System.Serializable]
public enum GameState
{
	MENU = 0,
	GAMEPLAY = 1,
	PAUSE = 2,
	GAMEEND = 3
}

public class GameManager : Service<GameManager>
{
	public GameState gameState;
	
	public Level[] levels;
	
	[Header("HUD")]
	public GameObject mainMenu;
	public GameObject levelSelection;
	public GameObject endScreen;
	public GameObject pauseMenu;
	
	
	[Header("Gameplay")]
	public Ball ball;
	public PlayerInput playerInput;
	public GameObject spaceship;
	public GameObject character;
	public Rigidbody2D characterRb;
	public Transform worldAnchor;
	
	public float delayBallIdle = 0.5f;
	float delayBallIdleElapsed = 0.0f;
	bool ballMoving = false;
	
	int currentLevel = 0;
	
	void Start()
	{
		SetState(GameState.MENU);		
	}
	
	public void SetState(int state)
	{
		SetState((GameState)state);
	}
	
	public void SetState(GameState state)
	{
		gameState = state;
		
		switch(gameState)
		{
		case GameState.MENU:
			Time.timeScale = 1;
		
			mainMenu.SetActive(true);
			levelSelection.SetActive(false);
			endScreen.SetActive(false);
			pauseMenu.SetActive(false);
		
			ball.gameObject.SetActive(false);
			playerInput.gameObject.SetActive(false);
			
			
			foreach(var level in levels)
			{
				level.gameObject.SetActive(false);
			}
		
			break;
			
		case GameState.GAMEPLAY:
		
			Time.timeScale = 1;
		
			mainMenu.SetActive(false);
			levelSelection.SetActive(false);
			endScreen.SetActive(false);
			pauseMenu.SetActive(false);
		
			ball.gameObject.SetActive(true);
			playerInput.gameObject.SetActive(true);			
			
			break;
		case GameState.PAUSE:
		
			playerInput.gameObject.SetActive(false);
			pauseMenu.SetActive(true);
			
			Time.timeScale = 0;
		
		
			break;
			
		case GameState.GAMEEND:
		
			playerInput.gameObject.SetActive(false);
			endScreen.SetActive(true);
		
		
			break;
		}
	}
	
	void Update()
	{
		switch(gameState)
		{
		case GameState.MENU:
		
			ball.rb2D.velocity = Vector2.zero;
			
			
			break;
			
		case GameState.GAMEPLAY:
		
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				SetState(GameState.PAUSE);
			}
		
		
			if(ballMoving)
			{
				if(ball.rb2D.velocity.magnitude < 0.1f && delayBallIdleElapsed < Time.time)
				{
					ball.ResetPosition();
					ballMoving = false;
				
					ResumeInput();
				}			
			}
			
			break;
		case GameState.PAUSE:
		
			if(Input.GetKeyDown(KeyCode.Escape))
			{
				SetState(GameState.GAMEPLAY);
			}		
		
			break;
			
		case GameState.GAMEEND:
		
		
			break;
		}
	}
	
	public void ResumeInput()
	{
		playerInput.gameObject.SetActive(true);
	}
	
	public void SignalThrow()
	{
		delayBallIdleElapsed = Time.time + delayBallIdle;
		ballMoving = true;
	}
	
	public void StartLevel(int index)
	{
		currentLevel = index;
		
		character.gameObject.SetActive(false);
		spaceship.transform.position = new Vector3(-15, -15, 0);
		
		SetState(GameState.GAMEPLAY);
		
		levels[index].Setup();
		levels[index].gameObject.SetActive(true);
		
		StartCoroutine(StartLevelRoutine());
		
		
	}
	
	IEnumerator StartLevelRoutine()
	{
		playerInput.gameObject.SetActive(false);
		
		while(ball.body.attractor == null) yield return null;

		MakeWorldAnchorLookAt(ball.transform.position, 0.0f, 2.5f);
		yield return null;

		Vector3 dir = (worldAnchor.GetChild(0).position + worldAnchor.GetChild(0).up * 5) * 2 - spaceship.transform.position;
		
		var seq = DOTween.Sequence();
		seq.Append(spaceship.transform.DOMove(worldAnchor.GetChild(0).position + worldAnchor.GetChild(0).up * 5, 1.5f));
		seq.Join(spaceship.transform.DORotateQuaternion(Quaternion.Euler(0, 0, (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90.0f), 1.0f));
		

		seq.Append(spaceship.transform.DOMove(worldAnchor.GetChild(0).position, 0.75f));
		seq.Join(spaceship.transform.DORotateQuaternion(worldAnchor.GetChild(0).rotation, 1.0f));
	
		seq.AppendCallback(() => 
		{
			character.transform.position = spaceship.transform.position;
			
			character.SetActive(true);
			character.transform.SetParent(worldAnchor.GetChild(0));
			character.transform.localPosition = Vector3.zero;
			character.transform.localEulerAngles = Vector3.zero;
			
			MakeWorldAnchorLookAt(character.transform.position, 0.0f, 2.5f);
			
			
			playerInput.gameObject.SetActive(true);
		});
	}
	
	public void MakeWorldAnchorLookAt(Vector3 target, float time = 0.0f, float zOffset = 0.0f)
	{
		var seq = DOTween.Sequence();
		
		Vector3 worldAnchorToCharacter = target - worldAnchor.position;
		float zRot = (Mathf.Atan2(worldAnchorToCharacter.y, worldAnchorToCharacter.x) * Mathf.Rad2Deg) - 90f;
		Quaternion targetRot = Quaternion.Euler(0, 0, zRot + zOffset);
		
		seq.Append(worldAnchor.DORotateQuaternion(targetRot, time));
	}
	
	public void NextLevel()
	{
		currentLevel++;
		if(currentLevel >= levels.Length)
		{
			SetState(GameState.MENU);
		}
		else 
		{
			StartLevel(currentLevel);
		}
	}
	
	public void BallHitAFlag(Transform flagT)
	{
		ball.rb2D.velocity = Vector3.zero;
		
		ballMoving = false;
		playerInput.gameObject.SetActive(false);
				
		Vector3 midPoint = GameUtils.MidPoint(character.transform.position, flagT.position);
		Vector3 dirToMidPoint = midPoint - ball.body.attractor.transform.position;
		Vector3 point = ball.body.attractor.transform.position + dirToMidPoint * 2.3f;
		
		Vector3 ballToAttractor = (flagT.transform.position - ball.body.attractor.transform.position).normalized;
		Vector3 dir = (flagT.transform.position + ballToAttractor * 10);
		
		var seq = DOTween.Sequence();
		seq.Append(character.transform.DOLocalMoveY(5.0f, 1.8f).SetEase(Ease.Linear));
		seq.Append(character.transform.DOLocalMoveY(0.0f, 1.2f));
		seq.AppendCallback(() => 
		{
			playerInput.gameObject.SetActive(true);
		});
		
		MakeWorldAnchorLookAt(ball.transform.position, 3.0f, 1.25f);
	}
	
	public void QuitGame()
	{
		Application.Quit();
		
	}
}
