using UnityEngine;

// Повесить на префаб зомби. target можно не задавать - найдётся по тегу "Player".
// В инспекторе выбери detectionType:
//   Sight   - видит игрока через луч (targetLayer) и стреляет издалека
//   Hearing - слышит игрока в радиусе (сквозь стены), идёт к нему и бьёт вблизи
//   Both    - сначала пробует увидеть, иначе - слышит и идёт
public class Zombie : MonoBehaviour
{
    public enum DetectionType { Sight, Hearing, Both }
    public DetectionType detectionType = DetectionType.Sight;

    public Transform target;

    public float health = 50f;

    [Header("Зрение (через луч, как у Player)")]
    public LayerMask targetLayer = new LayerMask(); // сюда: слои игрока И стен
    public float viewDistance = 15f;
    public float shootDamage = 10f;
    public float fireRate = 1f;
    float nextFireTime = 0f;

    [Header("Слух (погоня + ближний бой)")]
    public float hearingRadius = 8f;
    public float moveSpeed = 3f;
    public float meleeRange = 1.5f;
    public float meleeDamage = 20f;
    public float meleeRate = 1f;
    float nextMeleeTime = 0f;

    RaycastHit hitData;
    float hitDistance;
    Vector3 hitPos;
    GameObject hitObj;
    string hitTag;

    void Start()
    {
        if (target == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    void FixedUpdate()
    {
        if (target == null) return;
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver) return;

        bool seesPlayer = false;

        if (detectionType == DetectionType.Sight || detectionType == DetectionType.Both)
        {
            seesPlayer = CanSeePlayer();

            if (seesPlayer)
            {
                transform.LookAt(target);
                ShootAt(target);
            }
        }

        if (!seesPlayer && (detectionType == DetectionType.Hearing || detectionType == DetectionType.Both))
        {
            if (CanHearPlayer())
            {
                ChaseAndMelee();
            }
        }
    }

    // Луч в сторону игрока: если первым попадёт стена - игрок не виден
    bool CanSeePlayer()
    {
        Vector3 lookDir = target.position - transform.position;
        Ray ray = new Ray(transform.position, lookDir);
        hitData = HitRay(ray);

        if (hitObj == null) return false;
        if (hitTag != "Player") return false;

        return hitDistance <= viewDistance;
    }

    // Сквозь стены, просто по дистанции
    bool CanHearPlayer()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        return distance <= hearingRadius;
    }

    void ChaseAndMelee()
    {
        float distance = Vector3.Distance(transform.position, target.position);
        transform.LookAt(target);

        if (distance > meleeRange)
        {
            Vector3 dir = (target.position - transform.position).normalized;
            dir.y = 0f;
            transform.position += dir * moveSpeed * Time.fixedDeltaTime;
        }
        else if (Time.time >= nextMeleeTime)
        {
            nextMeleeTime = Time.time + meleeRate;

            Player player = target.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(meleeDamage);
            }
        }
    }

    void ShootAt(Transform t)
    {
        if (Time.time < nextFireTime) return;
        nextFireTime = Time.time + fireRate;

        Player player = t.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage(shootDamage);
        }
    }

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
        if (GameManager.Instance != null)
            GameManager.Instance.EnemyKilled();

        Destroy(gameObject);
    }

    RaycastHit HitRay(Ray ray)
    {
        RaycastHit data;

        if (Physics.Raycast(ray, out data, Mathf.Infinity, targetLayer))
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
}