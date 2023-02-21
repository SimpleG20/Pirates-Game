using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ChaserEnemy : Enemy
{

    // Start is called before the first frame update
    void Start()
    {
        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override async void Die()
    {
        Boat.ExplodeAnimation();
        await Task.Delay(1000);

        if (TokenSource.IsCancellationRequested) return;
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var damageFactor = (1 - Vector3.Distance(transform.position, collision.transform.position)) / 0.7f;

            Events.OnHitPlayerBoat(damageFactor * 3.5f, collision.contacts[0].point);
            GotHit(100, transform.position);
        }
        
    }
}
