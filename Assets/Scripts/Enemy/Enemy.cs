using System.Threading;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("General Variables")]

    #region Serialized
    [SerializeField] protected int _health;
    [SerializeField] protected float MovementSpeed;
    [SerializeField] protected float RotationSensibility;
    [SerializeField] protected float FieldVision;

    [Header("UI Components")]
    [SerializeField] protected Image HealthUI;
    [SerializeField] protected Gradient HealthGradient;
    [SerializeField] protected FollowBoat UiHealthFollowing;
    #endregion

    #region Non Serialized
    protected int DamageWhenHit = 35;
    protected bool _end;

    protected Vector2 Direction;

    protected Player Player;
    protected NavMeshAgent Agent;
    protected CancellationTokenSource TokenSource;
    #endregion

    private void OnEnable()
    {
        Events.onGameEnded += HandleEnd;
    }

    private void OnDisable()
    {
        Events.onGameEnded -= HandleEnd;
    }
    
    #region Abstract Methods
    public virtual void Move()
    {
        Agent.SetDestination(Player.transform.position);
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }

    public abstract void Damage(Vector3 hitPosition);

    protected virtual void HadleHealth(int valueToAdd)
    {
        _health += valueToAdd;
        HealthUISituation();
    }

    public virtual void Die(bool playerKilled)
    {
        Agent.speed = 0;

        int type = gameObject.name.Contains("Chaser") ? 1 : 0;
        if (playerKilled) Events.OnEnemyDefeated(type); 

        HealthUI.transform.parent.gameObject.SetActive(false);
    }
    #endregion

    public void Initialization()
    {
        TokenSource = new CancellationTokenSource();

        Player = FindObjectOfType<Player>();
        Agent = GetComponent<NavMeshAgent>();
        Agent.updateRotation = false;
        Agent.updateUpAxis = false;
        _end = false;

        UiHealthFollowing.Initiate(transform);

        HadleHealth(100);
    }

    private void HandleEnd() => _end = true;

    protected void RotateDirectlyToPlayer()
    {
        Direction = Player.transform.position - transform.position;
        transform.LookAt(Player.transform);
    }

    /// <summary>
    /// Depending on where the bullet hits the boat the damage caused is between a maximum or a minimum
    /// </summary>
    /// <param name="factor"></param>
    /// <param name="hitPosition"></param>
    public void GotHit(float factor, Vector3 hitPosition, bool playerShot)
    {
        if (_health < 0) return;

        if (playerShot) HadleHealth(-(int)(DamageWhenHit * factor));
        else HadleHealth(-(int)(_health * 0.1f));

        if (_health > 0)
        {
            Damage(hitPosition);
        }
        else Die(playerShot);
    }

    protected void SpreadWoodWhenDamaged(Vector3 hitPosition, Boat boat)
    {
        int quantityOfWoods = Random.Range(2, 6);

        for (int i = 0; i < quantityOfWoods; i++)
        {
            var wood = Instantiate(boat.WoodPrefab, hitPosition, Quaternion.identity);
            wood.Impulse();
        }
    }

    private void HealthUISituation()
    {
        if (HealthUI == null) return;

        float percentage = (float)_health / 100;

        HealthUI.fillAmount = percentage;
        HealthUI.color = HealthGradient.Evaluate(percentage);
    }

    public void DestroyObject() => Destroy(gameObject);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Ground"))
        {
            var damageFactor = Mathf.Abs(1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;

            GotHit(damageFactor * 0.6f, collision.contacts[0].point, false);
        }
    }
}
