using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Represents the different types of grunts in the game
public enum GruntType
{
	MELEE = 0,
	RANGED = 1
}

public class WaveManager : MonoBehaviour
{
	[SerializeField] private EnemySpawner spawner;
	[SerializeField] private EnemyList enemies;
	[SerializeField] private GameObject miniBossPrefab;
	[SerializeField] private GameObject bossPrefab;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject[] ammoObjects;
	[SerializeField] private GameObject[] teleportPoints;
	//[SerializeField] private GameObject menuButton;
	//[SerializeField] private GameObject winText;
	[SerializeField] private int waveNum = 1;
	private int meleeNum = 3;
	private int rangedNum = 3;
	private Vector4 spawnRectangle = new Vector4(-12, 12, -10, 10);

	// For the audio cues
	private bool alreadyWon = false;

	// Start is called before the first frame update
	void Start()
    {
		enemies.Enemies.AddRange(spawner.BorderSpawn(meleeNum, spawnRectangle, (int)GruntType.MELEE));
		enemies.Enemies.AddRange(spawner.BorderSpawn(rangedNum, spawnRectangle, (int)GruntType.RANGED));
		//menuButton.SetActive(false);
		//winText.SetActive(false);

		// Music Cues
		AkSoundEngine.StopAll();
		AkSoundEngine.PostEvent("Combat", gameObject);
		AkSoundEngine.PostEvent("DresdenStart", gameObject);
	}

    // Update is called once per frame
    void Update()
    {
		for (int i = 0; i < enemies.Enemies.Count; i++)
		{
			if (enemies.Enemies[i] == null)
			{
				enemies.Enemies.RemoveAt(i);
			}
		}
		if (enemies.Enemies.Count == 0)
		{
			waveNum++;
			switch (waveNum)
			{
				case 2:
				case 4:
					// Spawn grunts
					meleeNum += 3;
					rangedNum += 1;
					enemies.Enemies.AddRange(spawner.BorderSpawn(meleeNum, spawnRectangle, (int)GruntType.MELEE));
					enemies.Enemies.AddRange(spawner.BorderSpawn(rangedNum, spawnRectangle, (int)GruntType.RANGED));
					break;
				case 3:
					// Spawn Mini Boss
					GameObject miniBoss = Instantiate(miniBossPrefab, Vector2.zero, Quaternion.identity);
					enemies.Enemies.Add(miniBoss.GetComponent<Enemy>());

					miniBoss.GetComponent<MinibossScript>().Player = player;

					enemies.Enemies.AddRange(spawner.BorderSpawn(3, spawnRectangle, (int)GruntType.MELEE));
					enemies.Enemies.AddRange(spawner.BorderSpawn(1, spawnRectangle, (int)GruntType.RANGED));

					for (int i = 0; i < ammoObjects.Length; i++)
					{
						ammoObjects[i].SetActive(true);
					}
					break;
				case 5:
					// Spawn Boss
					GameObject boss = Instantiate(bossPrefab, Vector2.zero, Quaternion.identity);
					enemies.Enemies.Add(boss.GetComponent<Enemy>());

					boss.GetComponent<BossScript>().Player = player;
					boss.GetComponent<BossScript>().TeleportPoints = teleportPoints;
					break;
				default:
					// Player has won
					meleeNum = 0;
					rangedNum = 0;
					Cursor.visible = true;
					Debug.Log("EEEEEEEEEEEE");
					// Music Cues
					if (!alreadyWon)
                    {
						AkSoundEngine.StopAll();
						AkSoundEngine.PostEvent("Win", gameObject);
						SceneManager.LoadScene("Win");
						alreadyWon = true;
					}
					break;
			}
		}
	}

	public void LoadMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
