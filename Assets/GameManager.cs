﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityStandardAssets.Characters.FirstPerson;

public class GameManager : MonoBehaviour
{
	[SerializeField] private bool dontDestroyOnLoad; // the object will move from one scene to another (you only need to add it once)


	public static GameManager instance = null;
	public static GameObject player_object;
	public static PlayerController player;
	public float pause_time = 1f;
	bool dead = false;

	public static string scene_to_load;

	public static int player_coins = 0;
	public static bool has_torch = false;
	public static bool has_sword = false;

	public static int saved_times = 0;
	public static int save_coint_count = 0;
	public static int save_healt = 20;
	public static bool save_has_torch = false;
	public static bool save_has_sword = false;
	public static string save_active_scene = "Level1";
	public static bool loading = false;

	public static float volume_slider = 0.3f;

	static float slow_time = 0.5f;

	private void OnEnable()
	{
		player_object = GameObject.FindGameObjectWithTag("Player");
		player = GameObject.FindObjectOfType<PlayerController>();
	}
	void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
		else if (instance != this)
		{
			Destroy(this);
		}
		player = GameObject.FindObjectOfType<PlayerController>();
	}

	void Setup()
	{
		player = GameObject.FindObjectOfType<PlayerController>();
	}

	public static void NextScene(int scene)
	{
		// Loading next scene implies game progress, don't overwrite coin count
		SceneManager.LoadScene(scene);
	}

	public static void LoadScene(string scene)
	{
		scene_to_load = scene;
		SceneManager.LoadScene("Loading");
		player_coins = 0;
	}

	public static void LoadSceneSaved()
	{
		// Choosing from menu resets coins count
		SceneManager.LoadScene(save_active_scene);
		loading = true;
		has_torch = false;
		has_sword = false;
	}
	private void Update()
	{
		_ = SceneManager.GetActiveScene().name.Equals("Level3") ? RenderSettings.fog = false : RenderSettings.fog = true;

		if (player == null) player = GameObject.FindObjectOfType<PlayerController>();

		player_coins = player.coins_count;

		if(player.life <= 0 && !dead)
		{
			if(Time.timeScale <= slow_time)
			{
				Time.timeScale = 1;
				dead = true;
				player_coins = player.coins_count;
				player.GetComponentInParent<FirstPersonController>().enabled = false;
				player.GetComponentInParent<Animator>().enabled = false;
				Debug.Log(Time.timeScale);
				StartCoroutine(GameOver(pause_time));
			}
			else
			{
				//Debug.Log(Time.timeScale);
				Time.timeScale -= Time.deltaTime * 0.5f;
			}
		}

		if (SceneManager.GetActiveScene().buildIndex == 4)
		{
			Screen.lockCursor = false;
			UnityEngine.Cursor.lockState = CursorLockMode.None;
			UnityEngine.Cursor.visible = true;
		}

		AudioListener.volume = GameManager.volume_slider;
	}

	public static void LoadState()
	{
		LoadSceneSaved();
	}


	IEnumerator GameOver(float pause_time)
	{
		yield return new WaitForSeconds(pause_time);
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		player.life = player.max_life;
		dead = false;
		player.coins_count = player_coins;
	}
}

