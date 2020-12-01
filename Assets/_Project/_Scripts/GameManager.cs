using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
	public Material defaultSkybox;
	public Cinemachine.CinemachineTargetGroup targetGroup;
	
	[Header("Audio")]
	public AudioClip throwBallClip;
	public AudioClip spaceshipLandClip;
	public AudioClip hitFlagClip;
	
	public float delayBallIdle = 0.5f;
	float delayBallIdleElapsed = 0.0f;
	bool ballMoving = false;
	
	int currentLevel = 0;
	bool initializingLevel;
	GravityAttractor oldAttractor;
	
	static int goLevel = -1;
	
	void Start()
	{
		SetState(GameState.MENU);		
		if(goLevel != -1)
		{
			StartLevel(goLevel);
			goLevel = -1;
		}
	}
	
	public void SetState(int state)
	{
		SetState((GameState)state);
	}
	
	public void ReloadMenu()
	{
		goLevel = -1;
		SceneManager.LoadScene("Main", LoadSceneMode.Single);
	}
	
	public void SetState(GameState state)
	{
		gameState = state;
		
		switch(gameState)
		{
		case GameState.MENU:
			Time.timeScale = 1;
			
			RenderSettings.skybox = defaultSkybox;
		
			spaceship.SetActive(false);
			
			character.gameObject.SetActive(false);
			character.transform.SetParent(null);
		
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
			if(!ballMoving) playerInput.gameObject.SetActive(true);			
			
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
		
			if(Input.GetKeyDown(KeyCode.Escape) && !initializingLevel)
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
		
		AudioMaster.instance.PlaySfx(throwBallClip, 0.35f);
	}
	
	public void StartLevel(int index)
	{
		initializingLevel = true;
		currentLevel = index;
		
		character.gameObject.SetActive(false);
		spaceship.SetActive(true);
		spaceship.transform.position = new Vector3(-15, -15, 0);
		
		SetState(GameState.GAMEPLAY);
		
		levels[index].Setup();
		levels[index].gameObject.SetActive(true);
		
		character.transform.SetParent(null);
		playerInput.gameObject.SetActive(false);
		
		worldAnchor.position = levels[index].firstWorld.position;
		character.transform.position = worldAnchor.position;
		
		MakeWorldAnchorLookAt(levels[index].firstBallPosition.position, 0.0f, 2.5f);
		
		StartCoroutine(StartLevelRoutine());		
		
	}
	
	IEnumerator StartLevelRoutine()
	{
				
		while(ball.body.attractor == null) yield return null;
		
		yield return null;
		
		//Debug.DrawLine(worldAnchor.position, ball.transform.position, Color.red, 10.0f);

		//MakeWorldAnchorLookAt(ball.transform.position, 0.0f, 2.5f);
		yield return null;
		
		oldAttractor = ball.body.attractor;

		Vector3 dir = (worldAnchor.GetChild(0).position + worldAnchor.GetChild(0).up * 5) * 2 - spaceship.transform.position;
		
		var seq = DOTween.Sequence();
		seq.Append(spaceship.transform.DOMove(worldAnchor.GetChild(0).position + worldAnchor.GetChild(0).up * 5, 1.5f));
		seq.Join(spaceship.transform.DORotateQuaternion(Quaternion.Euler(0, 0, (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90.0f), 1.0f));
		

		seq.Append(spaceship.transform.DOMove(worldAnchor.GetChild(0).position, 0.75f));
		seq.Join(spaceship.transform.DORotateQuaternion(worldAnchor.GetChild(0).rotation, 1.0f));
	
		seq.AppendCallback(() => 
		{
			AudioMaster.instance.PlaySfx(spaceshipLandClip, 1.0f);
			
			character.transform.position = spaceship.transform.position;
			
			character.SetActive(true);
			character.transform.SetParent(worldAnchor.GetChild(0));
			character.transform.localPosition = Vector3.zero;
			character.transform.localEulerAngles = Vector3.zero;
			
			MakeWorldAnchorLookAt(character.transform.position, 0.0f, 2.5f);
			
			initializingLevel = false;
			playerInput.gameObject.SetActive(true);
		});
	}
	
	public void MakeWorldAnchorLookAt(Vector3 target, float time = 0.0f, float zOffset = 0.0f)
	{
		var seq = DOTween.Sequence();
		
		Vector3 worldAnchorToCharacter = target - worldAnchor.position;
		float zRot = (Mathf.Atan2(worldAnchorToCharacter.y, worldAnchorToCharacter.x) * Mathf.Rad2Deg) - 90f;
		Quaternion targetRot = Quaternion.Euler(0, 0, zRot + zOffset);
		
		if(time == 0.0f)
			worldAnchor.rotation = targetRot;
		else 
			seq.Append(worldAnchor.DORotateQuaternion(targetRot, time));
	}
	
	public void NextLevel()
	{
		
		levels[currentLevel].gameObject.SetActive(false);
		
		currentLevel++;
		if(currentLevel >= levels.Length)
		{
			//character.gameObject.SetActive(false);
			//spaceship.SetActive(true);
			//spaceship.transform.position = new Vector3(-15, -15, 0);
		
			//character.transform.SetParent(null);
			//playerInput.gameObject.SetActive(false);
			
			//SetState(GameState.MENU);
			
			ReloadMenu();
		}
		else 
		{
			//StartLevel(currentLevel);
			goLevel = currentLevel;
			SceneManager.LoadScene("Main", LoadSceneMode.Single);
		}
	}
	
	public void BallHitAFlag(Transform flagT)
	{
		initializingLevel = true;
		ball.rb2D.velocity = Vector3.zero;
		
		ballMoving = false;
		playerInput.gameObject.SetActive(false);
				
		Vector3 midPoint = GameUtils.MidPoint(character.transform.position, flagT.position);
		Vector3 dirToMidPoint = midPoint - ball.body.attractor.transform.position;
		Vector3 point = ball.body.attractor.transform.position + dirToMidPoint * 2.3f;
		
		Vector3 ballToAttractor = (flagT.transform.position - ball.body.attractor.transform.position).normalized;
		Vector3 dir = (flagT.transform.position + ballToAttractor * 10);
		
		var seq = DOTween.Sequence();
		if(oldAttractor != ball.body.attractor)
		{
			oldAttractor = ball.body.attractor;
			character.transform.SetParent(null);
			
			worldAnchor.position = ball.body.attractor.transform.position;
			MakeWorldAnchorLookAt(ball.transform.position, 0.0f, 1.5f);
			
			character.transform.SetParent(worldAnchor.GetChild(0));
			
			seq.Append(character.transform.DOLocalMove(Vector3.zero, 1.8f).SetEase(Ease.Linear));
			seq.Join(character.transform.DOLocalRotate(Vector3.zero, 1.2f)); 
		}
		else 
		{
			seq.Append(character.transform.DOLocalMoveY(5.0f, 1.8f).SetEase(Ease.Linear));
			seq.Append(character.transform.DOLocalMoveY(0.0f, 1.2f));
			MakeWorldAnchorLookAt(ball.transform.position, 3.0f, 1.25f);
		}
		
		seq.AppendCallback(() => 
		{
			initializingLevel = false;
			playerInput.gameObject.SetActive(true);
			AudioMaster.instance.PlaySfx(spaceshipLandClip, 1.0f);
		});
		
		AudioMaster.instance.PlaySfx(hitFlagClip);
	}
	
	public void QuitGame()
	{
		Application.Quit();
		
	}
}
