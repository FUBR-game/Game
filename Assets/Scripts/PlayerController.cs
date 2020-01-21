using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using BeardedManStudios.Forge.Networking.Unity;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInventory))]
public class PlayerController : PlayerControllerBehavior, DamageAble
{
    public const int maxHealth = 1000;
    public const int maxMana = 1000;

    public int health = maxHealth;
    public int mana = maxMana;

    public GameObject spellSpawnPoint;
    public Camera playerCamera;

    public float walkSpeed = 5.0f;
    public float sprintSpeed = 10.0f;
    public float gravity = 9.8f;

    public float jumpHeight = 5.0f;
//    public float airControl = 2.0f;

    public float sensitivityX = 15f;
    public float sensitivityY = 15f;

    public float minimumX = -360f;
    public float maximumX = 360f;

    public float minimumY = -60f;
    public float maximumY = 60f;

    private PlayerInventory inventory;

    private bool canMove = true;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;

    private float rotationX;
    private float rotationY;

    private Quaternion originalRotationPlayer;
    private Quaternion originalRotationCamera;
    private Quaternion originalRotationSpellSpawnPoint;
    
    private LogHandler _logger;
    
    private RectTransform healthBar;
    private RectTransform healthBarBack;
    private Text healthText;
    
    private RectTransform manaBar;
    private RectTransform manaBarBack;
    private Text manaText;
    
    void Start()
    {
        _logger = LogHandler.Instance;
        controller = GetComponent<CharacterController>();
        inventory = GetComponent<PlayerInventory>();
        originalRotationPlayer = transform.localRotation;
        originalRotationCamera = playerCamera.transform.localRotation;
        originalRotationSpellSpawnPoint = spellSpawnPoint.transform.localRotation;
        
        healthBarBack = GameObject.Find("HealthBarBack").GetComponent<RectTransform>();
        healthBar = healthBarBack.Find("HealthBarFront").GetComponent<RectTransform>();
        healthText = healthBarBack.Find("HealthBarText").GetComponent<Text>();

        manaBarBack = GameObject.Find("ManaBarBack").GetComponent<RectTransform>();
        manaBar = manaBarBack.Find("ManaBarFront").GetComponent<RectTransform>();
        manaText = manaBarBack.Find("ManaBarText").GetComponent<Text>();
    }

    protected override void NetworkStart()
    {
        base.NetworkStart();

        if (!networkObject.IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            playerCamera.gameObject.GetComponent<AudioListener>().enabled = false;
            canMove = false;
        }
    }

    void Update()
    {
        if (!networkObject.IsOwner)
        {
            transform.position = networkObject.position;
            transform.rotation = networkObject.rotation;
            health = networkObject.healt;
            return;
        }

        if (canMove)
        {
            Move();
            MouseMove();

            if (Input.GetButtonUp("CastSpell"))
            {
                networkObject.SendRpc(RPC_CAST_SPELL, Receivers.All);
            }

            if (Input.GetKeyDown(KeyCode.Z))
            {
                inventory.NextSpell();
            }

            networkObject.position = transform.position;
            networkObject.rotation = transform.rotation;
            networkObject.healt = health;
        }

        // Health bar
        healthBar.sizeDelta = new Vector2(Mathf.Lerp(0.0f, healthBarBack.sizeDelta.x, (float)health/maxHealth), healthBarBack.sizeDelta.y);
        healthText.text = health.ToString();

        // When you die
        if (health <= 0)
        {
            healthText.text = "You're dead!";
        }
        // Health bar
        healthBar.sizeDelta = new Vector2(Mathf.Lerp(0.0f, healthBarBack.sizeDelta.x, (float)health/maxHealth), healthBarBack.sizeDelta.y);
        healthText.text = health.ToString();

        // When you die
        if (health <= 0)
        {
            healthText.text = "You're dead!";
        }
        else
        {
            healthText.text = health.ToString();
        }

        // Mana bar
        manaBar.sizeDelta = new Vector2(Mathf.Lerp(0.0f, manaBarBack.sizeDelta.x, (float)mana / maxMana), manaBarBack.sizeDelta.y);
        manaText.text = mana.ToString();
    }

    private void Move()
    {
        if (controller.isGrounded)
        {
            if (Input.GetButton("Sprint"))
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= sprintSpeed;
            }
            else
            {
                moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
                moveDirection = transform.TransformDirection(moveDirection);
                moveDirection *= walkSpeed;
            }

            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y += jumpHeight;
            }
        }
//        else
//        {
//            moveDirection.x = Input.GetAxis("Horizontal") * airControl;
//            moveDirection.z = Input.GetAxis("Vertical") * airControl;
//            moveDirection = transform.TransformDirection(moveDirection);
//        }

        moveDirection.y -= gravity * Time.deltaTime;

        controller.Move(moveDirection * Time.deltaTime);
    }

    private void MouseMove()
    {
        rotationX += Input.GetAxis("Mouse X") * sensitivityX;
        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

        rotationX = ClampAngle(rotationX, minimumX, maximumX);
        rotationY = ClampAngle(rotationY, minimumY, maximumY);

        Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
        Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, Vector3.left);

        transform.localRotation = originalRotationPlayer * xQuaternion;
        playerCamera.transform.localRotation = originalRotationCamera * yQuaternion;
        spellSpawnPoint.transform.localRotation = originalRotationSpellSpawnPoint * yQuaternion;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f)
            angle += 360f;
        if (angle > 360f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    //RCPs
    public override void CastSpell(RpcArgs args)
    {
        MainThreadManager.Run(() =>
        {
            var spell = inventory.spellBook[inventory.currentSpell];

            var castedSpell = Instantiate(spell.gameObject, spellSpawnPoint.transform.position, Quaternion.identity);

            castedSpell.transform.rotation = spellSpawnPoint.transform.rotation;
            castedSpell.GetComponent<Rigidbody>().AddForce(spellSpawnPoint.transform.forward * spell.speed);

            _logger.Log(LogTag.Attack, new []{Time.time.ToString(), "test"});
        });
    }

    public override void Die(RpcArgs args)
    {
        networkObject.Destroy();
    }

    //IDamageAble
    public void takeDamage(int amount)
    {
        if (networkObject.IsOwner)
        {
            health -= amount;
            if (health <= 0)
            {
                networkObject.SendRpc(RPC_DIE, Receivers.Others);

                var provider = GameObject.Find("NetworkProvider");
                var networkProvider = provider.GetComponent<NetworkProvider>();
                networkProvider.Disconnect();
                Application.Quit();
            }
        }
    }

    public void recoverDamage(int amount)
    {
        health += amount;
    }
}