using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyList", menuName = "ScriptableObjects/EnemyList", order = 1)]
public class EnemyList : ScriptableObject
{
	[SerializeField] private List<Enemy> enemies;

	public List<Enemy> Enemies { get => enemies; set => enemies = value; }


	private void OnEnable()
	{
		enemies = new List<Enemy>();
	}
}