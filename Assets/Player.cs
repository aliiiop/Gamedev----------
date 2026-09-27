using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Повесить на объект Player: Rigidbody + Capsule Collider, тег "Player".
// В Input Actions Asset должны быть действия: Move, CameraControl, Attack.
// cam - дочерняя Main Camera. targetLayer - слои, по которым бьёт атака (враги).
public class Player : MonoBehaviour
{
    InputAction movementAction;
    InputAction cameraAction;
    InputAction attackAction;

    public Camera cam;
    float camH = 0f;
    float camV = 0f;

    public float health = 100f;
    public float maxHealth = 100f;
    public float damage = 25f;
    public float speed = 10f;
    public float attackRange = 100f; // лазер бьёт далеко

    public bool canFly = false;

    public LayerMask targetLayer;

    RaycastHit hitData;
    float hitDistance;
    Vector3 hitPos;
    GameObject hitObj;
    string hitTag;

    public TMP_Text tmp; // необязательно, можно оставить пустым

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        movementAction = InputSystem.actions.FindAction("Move");
        cameraAction = InputSystem.actions.FindAction("CameraControl");
        attackAction = InputSystem.actions.FindAction("Attack");

        Debug.Log("movementAction найден: " + (movementAction != null));

        movementAction.Enable();
        cameraAction.Enable();
        attackAction.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        health = maxHealth;
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
            return;

        if (health <= 0)
        {
            Die();
            return;
        }

        Movement();

        if (attackAction.triggered)
        {
            Attack();
        }
    }

    void Movement()
    {
        CameraControl();

        Vector3 camForward = cam.transform.forward;
        Vector3 camRight = cam.transform.right;

        if (canFly)
        {
            rb.useGravity = false;
        }
        else
        {
            camForward.y = 0f;
            camRight.y = 0f;
            rb.useGravity = true;
        }

        cam.transform.rotation = Quaternion.Euler(camV, camH, 0);

        Vector2 moveVector = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveVector.y += 1;
        if (Keyboard.current.sKey.isPressed) moveVector.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveVector.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveVector.x += 1;

        Debug.Log("moveVector(raw) = " + moveVector);
        Debug.Log("moveVector = " + moveVector);
        Vector3 moveDir = (camForward * moveVector.y) + (camRight * moveVector.x);

        transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
    }

    void CameraControl()
    {
        Vector2 cameraDelta = cameraAction.ReadValue<Vector2>();
        camH += cameraDelta.x;
        camV -= cameraDelta.y;
        camV = Mathf.Clamp(camV, -90f, 90f);
    }

    // "Лазер" - луч от камеры вперёд, бьёт по тому, во что попал (тег "Enemy")
    void Attack()
    {
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        hitData = HitRay(ray);

        if (hitObj != null && hitTag == "Enemy")
        {
            Zombie zombie = hitObj.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.TakeDamage(damage);
            }
        }
    }

    // Вызывается врагом (Zombie), когда он бьёт игрока
    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (tmp != null)
            tmp.text = "Game Over";

        if (GameManager.Instance != null)
            GameManager.Instance.PlayerDied();
    }

    RaycastHit HitRay(Ray ray)
    {
        RaycastHit data;

        if (Physics.Raycast(ray, out data, attackRange, targetLayer))
        {
            hitPos = data.point;
            hitDistance = data.distance;
            hitTag = data.collider.tag;
            hitObj = data.collider.gameObject;
        }
        else
        {
            hitPos = Vector3.zero;
            hitDistance = 0;
            hitTag = "";
            hitObj = null;
        }

        return data;
    }

    void OnTriggerEnter(Collider tr)
    {
        if (tr.gameObject.tag == "getFly")
            canFly = true;
    }
}