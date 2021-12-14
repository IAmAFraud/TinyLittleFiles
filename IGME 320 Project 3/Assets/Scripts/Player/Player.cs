using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera nonImpulseCamera;
    [SerializeField] private Rigidbody2D playerRigidbody;
    [SerializeField] private GameObject gunPivot;
    [SerializeField] private GameObject gunTip;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject bulletManaPrefab;
    [SerializeField] private GameObject mFlashDisplay;
    [SerializeField] private GameObject plySprite;
    [SerializeField] private GameObject aimprev;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Cinemachine.CinemachineImpulseSource impulseSource;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private float gunRecoil = 30f;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private int gunDamage = 1;
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private GameObject[] manaBalls;
    [SerializeField] private GameObject gunSpr;
    [SerializeField] private GameObject rodSpr;


    private Vector3 worldPoint;
    private Vector2 inputVector;
    private Vector3 angleVector;
    private Vector3 gunPos;
    private Vector2 gunDir;

    private float currentAngle;

    // For the audio cues
    private bool alreadyDead = false;

    private int chamberAmount = 6;
    private int maxAmmo = 100;
    private int chamberAmmo = 6;
    private int revAmmo = 64;

    private int playerHP = 3;
    private int maxHP = 3;

    private float invulTime = 3f;
    private float inClock;
    private int mana = 0;
    private float manaInterval = 2f;
    private float manaClock = 0;
    private bool cWep;


    private float reloadTime = 1f;
    private float reloadClock = 0f;


    // Start is called before the first frame update
    void Start()
    {
        inClock = Time.timeSinceLevelLoad;
        manaClock = Time.timeSinceLevelLoad;

        gunPos = gunPivot.transform.localPosition;
        mFlashDisplay.SetActive(false);

        reloadClock = Time.timeSinceLevelLoad;


        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        gunSpr.SetActive(cWep);
        rodSpr.SetActive(!cWep);
        if (reloadClock > Time.timeSinceLevelLoad)
        {
            if (Mathf.Ceil(Time.timeSinceLevelLoad * 10f) % 2 == 0)
            {
                ammoText.gameObject.SetActive(true);
            }
            else
            {
                ammoText.gameObject.SetActive(false);
            }
        }
        else
		{
            ammoText.gameObject.SetActive(true);
        }

        if (manaClock < Time.timeSinceLevelLoad)
		{
            manaClock = Time.timeSinceLevelLoad + manaInterval;



            mana = Mathf.Clamp(mana + 1, 0, 6);

        }
        if (inClock > Time.timeSinceLevelLoad)
        {
            if (Mathf.Ceil(Time.timeSinceLevelLoad * 10f) % 2 == 0)
            {
                plySprite.SetActive(true);
            }
            else
            {
                plySprite.SetActive(false);
            }

        }
        else
		{
            plySprite.SetActive(true);
        }

        ammoText.text = chamberAmmo + "/6 \n (" + revAmmo + ")";
        hpText.text = "HP (" + playerHP + "/" + maxHP + ")";

        hearts[0].SetActive(false);
        hearts[1].SetActive(false);
        hearts[2].SetActive(false);
        for (int i = 0; i < playerHP; i++)
		{
            hearts[i].SetActive(true);
		}

        for (int i = 0; i < 6; i++)
        {
            manaBalls[i].SetActive(false);
        }
        for (int i = 0; i < mana; i++)
        {
            manaBalls[i].SetActive(true);
        }

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = nonImpulseCamera.farClipPlane * .5f;
        worldPoint = nonImpulseCamera.ScreenToWorldPoint(mousePos);


        Vector2 direction = (worldPoint - transform.position).normalized;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        gunDir = direction / direction.magnitude;

        if (angle != currentAngle && (angle + currentAngle) < 3f)
		{
            currentAngle = angle;
        }
        currentAngle = Mathf.Lerp(currentAngle, angle, Time.deltaTime * 30f);
        angleVector.z = currentAngle;

        gunPivot.transform.localRotation = Quaternion.Lerp(gunPivot.transform.localRotation, Quaternion.Euler(angleVector), Time.deltaTime * 30f);

        //gunPivot.transform.Rotate(Vector3.forward, angle);


        //transform.position = transform.position + (Vector3)(inputVector * speed * Time.deltaTime);

        Vector3 scale = gunPivot.transform.localScale;
        Vector3 currentPos = gunPivot.transform.localPosition;
        Vector3 plyScale = plySprite.transform.localScale;

        if (Mathf.Abs(angle) > 90)
		{
            currentPos.x = gunPos.x * -1;
            scale.y = -1;
            plyScale.x = -1;

        }
        else
		{
            currentPos.x = gunPos.x;
            scale.y = 1;
            plyScale.x = 1;

        }
        gunPivot.transform.localPosition = currentPos;

        gunPivot.transform.localScale = scale;

        plySprite.transform.localScale = plyScale;

        Vector3 aimPos = aimprev.transform.position;
        aimPos.x = worldPoint.x;
        aimPos.y = worldPoint.y;
        aimprev.transform.position = aimPos;


        //Cursor.visible = false;
    }

	private void FixedUpdate()
	{
        if (inputVector.magnitude > 0.01f)
        {
            playerRigidbody.velocity += inputVector * speed;
            playerRigidbody.velocity = Vector2.ClampMagnitude(playerRigidbody.velocity, speed);
        }
        else
        {
            playerRigidbody.velocity -= playerRigidbody.velocity * 0.9f;
        }
    }

    private void OnReload(InputValue value)
	{

        if (reloadClock > Time.timeSinceLevelLoad)
		{
            return;
		}
        
        revAmmo += chamberAmmo;
        chamberAmmo = 0;

        int take = Mathf.Clamp(revAmmo,0,chamberAmount);

        revAmmo -= take;
        reloadClock = Time.timeSinceLevelLoad + reloadTime;
        chamberAmmo += take;

        // Audio Cue
        AkSoundEngine.PostEvent("RevolverReload", gameObject);
    }

	private void OnMovement(InputValue value)
	{
        inputVector = value.Get<Vector2>();
    }
    private void ShootGun(InputValue value)
	{
        if (chamberAmmo <= 0)
        {
            OnReload(value);
        }
        if (reloadClock > Time.timeSinceLevelLoad)
        {
            return;
        }
        if (chamberAmmo <= 0) return;
        GameObject bullet = Instantiate(bulletPrefab);
        bullet.transform.position = gunTip.transform.position;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = playerRigidbody.velocity;
        bulletRb.AddForce(gunDir * bulletSpeed, ForceMode2D.Impulse);

        bullet.GetComponent<Bullet>().Damage = gunDamage;


        StartCoroutine(MuzzleFlash(0.05f));
        impulseSource.GenerateImpulse();

        if (Mathf.Abs(currentAngle) > 90)
        {
            currentAngle -= gunRecoil;
        }
        else
        {
            currentAngle += gunRecoil;
        }


        chamberAmmo--;

        // Audio Cue
        AkSoundEngine.PostEvent("Gunshot", gameObject);
    }
    private void ShootMana(InputValue value)
	{
        if (mana <= 0) return;
        manaClock = Time.timeSinceLevelLoad + manaInterval;
        mana--;
        GameObject bullet = Instantiate(bulletManaPrefab);
        bullet.transform.position = gunTip.transform.position;

        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.velocity = playerRigidbody.velocity;
        bulletRb.AddForce(gunDir * bulletSpeed, ForceMode2D.Impulse);

        bullet.GetComponent<Bullet>().ShouldDestroyOnCollide = false;

        // Audio Cue
        AkSoundEngine.PostEvent("Fuego",gameObject);
        AkSoundEngine.PostEvent("Fireball",gameObject);
    }

    private void OnSwap(InputValue value)
	{
        cWep = !cWep;
	}
    private void OnShoot(InputValue value)
	{
        if (Time.timeScale == 0) return;
        if (cWep)
		{
            ShootGun(value);
        }
        else
		{
            ShootMana(value);
		}
        
    }

    private IEnumerator MuzzleFlash(float seconds)
    {
        mFlashDisplay.SetActive(true);
        yield return new WaitForSeconds(seconds);
        mFlashDisplay.SetActive(false);
    }

    public bool TakeDamage(int dmg)
	{
        if (inClock > Time.timeSinceLevelLoad)
		{
            return false;
		}

        // Audio Cue
        AkSoundEngine.PostEvent("DresdenHit", gameObject);

        playerHP = Mathf.Clamp(playerHP - dmg, 0, maxHP);
        inClock = Time.timeSinceLevelLoad + invulTime;
		if (playerHP <= 0)
		{
			//menuButton.SetActive(true);
			//loseText.GetComponent<UnityEngine.UI.Text>().text = "You Lose!";
			//loseText.SetActive(true);

			Cursor.visible = true;

            // Music Cues
            if (!alreadyDead)
            {
                AkSoundEngine.StopAll();
                AkSoundEngine.PostEvent("Lose", gameObject);
                SceneManager.LoadScene("Loss");
                alreadyDead = true;
            }
            
		}
        return false;
	}


	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.CompareTag("Ammo"))
		{
            revAmmo = Mathf.Clamp(revAmmo + Random.Range(3,15), 0, maxAmmo);
            Destroy(collision.gameObject);
		}


        if (collision.gameObject.CompareTag("Enemy"))
        {

			TakeDamage(1);
        }
    }
}