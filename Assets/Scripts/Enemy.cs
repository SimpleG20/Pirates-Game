using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [Header("General Variables")]

    #region Serialized
    [SerializeField] protected float MovementSpeed;
    [SerializeField] protected float RotationSensibility;
    [SerializeField] protected float FieldVision;

    [Header("UI Components")]
    [SerializeField] protected Image HealthUI;
    [SerializeField] protected Gradient HealthGradient;
    [SerializeField] protected FollowBoat UiHealthFollowing;

    [Header("Boat")]
    [SerializeField] protected Boat Boat = new Boat();
    #endregion

    #region Non Serialized
    private int _health;
    protected int _enemyLayer = 7;
    protected bool IsPlayerWithinSight;

    protected Vector2 Direction;

    protected CancellationTokenSource TokenSource;
    #endregion

    public void Initialization()
    {
        TokenSource = new CancellationTokenSource();

        UiHealthFollowing.Initiate(transform);

        ChangeHealthValue(100);
    }
    
    public abstract void Move();

    /// <summary>
    /// Depending on where the bullet hits the boat the damage caused is between a maximum or a minimum
    /// </summary>
    /// <param name="factor"></param>
    public void GotHit(float factor, Vector3 hitPosition)
    {
        if (_health < 0) return;

        ChangeHealthValue(-(int)(15 * factor));

        if (_health > 0) Damage(hitPosition);
        else Die();
    }

    private void ChangeHealthValue(int valueToAdd)
    {
        _health += valueToAdd;
        HealthUISituation();
        Boat.ChangeBoatAccordingToHealth((float)_health / 100);
    }

    private void HealthUISituation()
    {
        if (HealthUI == null) return;

        float percentage = (float)_health / 100;

        HealthUI.fillAmount = percentage;
        HealthUI.color = HealthGradient.Evaluate(percentage);
    }

    private void Damage(Vector3 hitPosition)
    {
        bool justWood = Random.Range(0, 3) == 1 && _health > 60;

        if (justWood)
        {
            SpreadWood(hitPosition);
        }
        else
        {
            var pieceRemoved = Boat.RemoveRandomPieceWhenDamaged();
            if (pieceRemoved == false) SpreadWood(hitPosition);
        }
    }

    private void SpreadWood(Vector3 hitPosition)
    {
        int quantityOfWoods = Random.Range(2, 6);

        for (int i = 0; i < quantityOfWoods; i++)
        {
            var wood = Instantiate(Boat.WoodPrefab, hitPosition, Quaternion.identity);
            wood.Impulse();
        }
    }

    public virtual void Die()
    {
        HealthUI.transform.parent.gameObject.SetActive(false);
    }

    public void DestroyObject() => Destroy(gameObject);

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var damageFactor = (1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;

            GotHit(damageFactor, collision.contacts[0].point);
        }
    }
}
