using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 3f;
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
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            //Applique les degats au joueur ici
            Destroy(gameObject);
        }
    }
}
