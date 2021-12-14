using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // -------------------------
    // - - - - - FIELD - - - - -
    // -------------------------

    public GameObject[] enemyPrefabs;





    // ---------------------------
    // - - - - - METHODS - - - - -
    // ---------------------------

    /// <summary>
    /// Spawn enemies at object's position.
    /// </summary>
    /// <param name="count">Number of enemies to spawn</param>
    public void Spawn(int count, int type)
    {
        // For loop based on input count
        for (int i = 0; i < count; i++)
            // Instantiate an enemy
            Instantiate(enemyPrefabs[type], transform.position, Quaternion.identity);
    }

    /// <summary>
    /// Spawn enemies at a specified position.
    /// </summary>
    /// <param name="count">Number of enemies to spawn</param>
    /// <param name="position">The spawn position</param>
    public void Spawn(int count, Vector3 position, int type)
    {
        // For loop based on input count
        for (int i = 0; i < count; i++)
            // Instantiate an enemy
            Instantiate(enemyPrefabs[type], position, Quaternion.identity);
    }

    /// <summary>
    /// Spawn enemies within a rectangle.
    /// </summary>
    /// <param name="count">Number of enemies to spawn</param>
    /// <param name="rectangle">The spawn area (x1, x2, y1, y2)</param>
    public void Spawn(int count, Vector4 rectangle, int type)
    {
        // Make a position vector
        Vector3 position = transform.position;

        // For loop based on input count
        for (int i = 0; i < count; i++)
        {
            // Randomize coordinates
            position.x = Random.Range(rectangle[0], rectangle[1]);
            position.y = Random.Range(rectangle[2], rectangle[3]);

            // Instantiate an enemy
            Instantiate(enemyPrefabs[type], position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Spawn enemies on a rectangle's borders.
    /// </summary>
    /// <param name="count">Number of enemies to spawn</param>
    /// <param name="rectangle">The spawn area (x1, x2, y1, y2)</param>
    public Enemy[] BorderSpawn(int count, Vector4 rectangle, int type)
    {
		// Create the array to return
		Enemy[] enemies = new Enemy[count];

        // Make a position vector
        Vector3 position = transform.position;

        // For loop based on input count
        for (int i = 0; i < count; i++)
        {
            // Randomize coordinates
            if (Random.value > .5)
            {
                position.x = Random.Range(rectangle[0], rectangle[1]);      // x value is anywhere in range
                position.y = rectangle[2 + Mathf.RoundToInt(Random.value)]; // y value is either min or max
            }
            else
            {
                position.x = rectangle[Mathf.RoundToInt(Random.value)]; // x value is either min or max
                position.y = Random.Range(rectangle[2], rectangle[3]);  // y value is anywhere in range
            }

            // Instantiate an enemy
            enemies[i] = Instantiate(enemyPrefabs[type], position, Quaternion.identity).GetComponent<Enemy>();
        }

		return enemies;
    }
}
