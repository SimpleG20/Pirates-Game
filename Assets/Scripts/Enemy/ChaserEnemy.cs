using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChaserEnemy : Enemy
{
    [SerializeField] private float _attackSpeed;
    [SerializeField] private Boat _boat;

    private float _defatulSpeed;

    // Start is called before the first frame update
    void Start()
    {
        Initialization();

        _defatulSpeed = MovementSpeed;
        DamageWhenHit = 45;
    }

    // Update is called once per frame
    void Update()
    {
        if (_end) return;

        Move();
        RotateDirectlyToPlayer();
    }

    public override void Move()
    {
        if (Vector3.Distance(transform.position, Player.transform.position) < FieldVision) Agent.speed = _attackSpeed;
        else Agent.speed = _defatulSpeed;

        base.Move();
    }

    public override void Damage(Vector3 hitPosition)
    {
        SpreadWoodWhenDamaged(hitPosition, _boat);
    }

    protected override void HadleHealth(int valueToAdd)
    {
        base.HadleHealth(valueToAdd);

        _boat.ChangeBoatAccordingToHealth((float)_health / 100);
    }

    public override void Die(bool playerKilled)
    {
        base.Die(playerKilled);
        _boat.ExplodeAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var damageFactor = Mathf.Abs(1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;

            Events.OnHitPlayerBoat(damageFactor * 3.5f, collision.contacts[0].point);
            GotHit(100, transform.position, false);
        }
        
    }

}
