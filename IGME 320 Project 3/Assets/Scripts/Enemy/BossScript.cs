using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossScript : Enemy
{
    // Enum that determines the bosses attack
    private enum BossAttack
    {
        Attack1,
        Attack2,
        Attack3,
        Attack4
    }

    // Fields
    [SerializeField] private int maxHealth;
    private int curHealth;
    [SerializeField] private int lowHealth1;
    [SerializeField] private int lowHealth2;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject[] teleportPoints;  // Make Zero Index the Center
    [SerializeField] private RectTransform hpTransform;
    private int teleportIndex = 0;
    private BossAttack curAttack = BossAttack.Attack1;
    private bool attackRunning = false;
    private GameObject bullet;
    private Vector2 velocity;
    private bool[] lowHealthAttack = new bool[2] { false, false };
    private bool centerAttack = false;

    // Properties

    public int MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public int CurHealth
    {
        get { return curHealth; }
        set { curHealth = value; }
    }

	public GameObject Player { get => player; set => player = value; }
	public GameObject[] TeleportPoints { get => teleportPoints; set => teleportPoints = value; }

	// Start is called on scene start
	private void Start()
    {
        curHealth = maxHealth;
    }

    // Update is called once per frame
    protected override void Update()
    {
        hpTransform.transform.localScale = new Vector3((curHealth / (float)maxHealth), 1f, 1f);
        if (!attackRunning)
        {
            if (centerAttack)
            {
                curAttack = BossAttack.Attack1;
                TeleportTo(0);
            }

            switch (curAttack)
            {
                // Big Attack From The Center
                case BossAttack.Attack1:
                    StartCoroutine("AttackPattern1");
                    attackRunning = true;
                    break;

                case BossAttack.Attack2:
                    StartCoroutine("AttackPattern2");
                    attackRunning = true;
                    break;

                case BossAttack.Attack3:
                    StartCoroutine("AttackPattern3");
                    attackRunning = true;
                    break;

                case BossAttack.Attack4:
                    StartCoroutine("AttackPattern4");
                    attackRunning = true;
                    break;
            }

            TakeDamage(5);
        }
    }

    /// <summary>
    /// Shoots out bullets in a spiraling pattern on two different sides
    /// </summary>
    private IEnumerator AttackPattern1()
    {
        centerAttack = false;

        for (int i = 0; i < 120; i++)
        {
            velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * (34.0f * i)), Mathf.Cos(Mathf.Deg2Rad * (34.0f * i)));
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = velocity * bulletSpeed;
            bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            bullet.GetComponent<Rigidbody2D>().velocity = -velocity * bulletSpeed;
            yield return new WaitForSeconds(0.05f);
        }

        yield return new WaitForSeconds(3.0f);

        TeleportTo(Random.Range(1,5));

        yield return new WaitForSeconds(3.0f);

        //RandomAttack();
        curAttack = BossAttack.Attack4;
        attackRunning = false;
    }
    
    /// <summary>
    /// Does a shotgun effect
    /// </summary>
    private IEnumerator AttackPattern2()
    {
        
        for (int i = 0; i < 20; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                velocity = (new Vector2(player.transform.position.x + Random.Range(-0.05f, 0.05f), player.transform.position.y + Random.Range(-0.05f, 0.05f))
                    - new Vector2(transform.position.x + Random.Range(-5.0f, 5.0f), transform.position.y + Random.Range(-5.0f, 5.0f))).normalized;
                bullet = Instantiate(bulletPrefab, teleportPoints[teleportIndex].transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = velocity * bulletSpeed;

            }

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(3.0f);

        TeleportTo(Random.Range(1, 5));

        yield return new WaitForSeconds(3.0f);

        RandomAttack();
        attackRunning = false;
    }

    /// <summary>
    /// Shoots bullets at the player from the unoccupied corners
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackPattern3()
    {
        for (int i = 0; i < 60; i++)
        {
            for (int j = 1; j < 5; j++)
            {
                if (teleportIndex != j)
                {
                    velocity = (new Vector2(player.transform.position.x, player.transform.position.y) - new Vector2(teleportPoints[j].transform.position.x, teleportPoints[j].transform.position.y)).normalized;
                    bullet = Instantiate(bulletPrefab, teleportPoints[j].transform.position, Quaternion.identity);
                    bullet.GetComponent<Rigidbody2D>().velocity = velocity * bulletSpeed;
                    //BulletScript bltScript = bullet.GetComponent<BulletScript>();
                    //bltScript.Velocity = velocity;
                    //bltScript.Speed = 0.1f;
                }
            }

            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(3.0f);

        TeleportTo(Random.Range(1, 5));

        yield return new WaitForSeconds(3.0f);

        RandomAttack();
        attackRunning = false;
    }

    /// <summary>
    /// Teleports to different corners and shots out a corner circle
    /// </summary>
    private IEnumerator AttackPattern4()
    {
        for (int i = 0; i < 20; i++)
        {
            float startAngle = 0.0f;
            switch (teleportIndex)
            {
                case 1:
                    startAngle = 90.0f;
                    break;

                case 2:
                    startAngle = 180.0f;
                    break;

                case 3:
                    startAngle = 270.0f;
                    break;

                case 4:
                    startAngle = 0.0f;
                    break;
            }

            for (int j = 0; j < 4; j++)
            {
                bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Rigidbody2D>().velocity = new Vector2(Mathf.Sin(Mathf.Deg2Rad * (startAngle + 30.0f * j)), Mathf.Cos(Mathf.Deg2Rad * (startAngle + 30.0f * j))) * bulletSpeed;
            }

            yield return new WaitForSeconds(0.25f);
            TeleportTo(Random.Range(1, 5));
        }

        yield return new WaitForSeconds(3.0f);

        TeleportTo(Random.Range(1, 5));

        yield return new WaitForSeconds(3.0f);

        //RandomAttack();
        curAttack = BossAttack.Attack1;
        attackRunning = false;

    }

    /// <summary>
    /// Teleports the boss to a certain corner
    /// </summary>
    private void TeleportTo(int index)
    {
        if (index < teleportPoints.Length && index > -1)
        {

            Vector3 currentPos = transform.position;
            currentPos.y = teleportPoints[index].transform.position.y;
            currentPos.x = teleportPoints[index].transform.position.x;
            transform.position = currentPos;
            teleportIndex = index;
        }
    }

    /// <summary>
    /// Selects a random attack based on the current attack
    /// </summary>
    private void RandomAttack()
    {
        float rand = Random.Range(0.0f, 1.0f);

        switch (curAttack)
        {
            case BossAttack.Attack1:
                if (rand < 0.33f) curAttack = BossAttack.Attack2;
                else if (rand < 0.67f) curAttack = BossAttack.Attack3;
                else curAttack = BossAttack.Attack4;
                break;

            case BossAttack.Attack2:
                if (rand < 0.1f) return;
                else if (rand < 5.5f) curAttack = BossAttack.Attack3;
                else curAttack = BossAttack.Attack4;
                break;

            case BossAttack.Attack3:
                if (rand < 0.33f) curAttack = BossAttack.Attack2;
                else curAttack = BossAttack.Attack4;
                break;

            case BossAttack.Attack4:
                if (rand < 0.50f) return;
                else if (rand < 0.75) curAttack = BossAttack.Attack2;
                else curAttack = BossAttack.Attack3;
                break;
        }
    }


    protected override IEnumerator Attack()
    {
        yield return null;
    }

    protected override Vector2 Seek(Vector2 targetPosition)
    {
        return Vector2.zero;
    }

    public override void TakeDamage(int amtDamage)
    {
        curHealth -= amtDamage;

        if (lowHealthAttack[0] == false && curHealth <= lowHealth1)
        {
            centerAttack = true;
            lowHealthAttack[0] = true;
        }
        else if (lowHealthAttack[1] == false && curHealth <= lowHealth2)
        {
            centerAttack = true;
            lowHealthAttack[1] = true;
        }

        if (curHealth <= 0)
		{
            //enemyList.Enemies.Remove(this);
            Destroy(this.gameObject);
		}
    }

}
