using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float _speed = 10f;
    public float _damage;
    public float lifetime = 3f;

    public void Initialize(float dmg, float speed)
    {
        _damage = dmg;
        _speed = speed;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // On detruit la balle apres X secondes pour nettoyer la hierarchie
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        //La balle avance toujours vers son "haut" (l'axe Y local du triangle)
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //on cherche l'interface IHit sur la joueur pour lui mettre les degats
            if (collision.TryGetComponent(out IHit playerhit))
            {
                Vector2 direction = (collision.transform.position - transform.position).normalized;
                playerhit.OnHit(_damage,5f,direction);
            }
            Destroy(gameObject);
        }
    }
}
