using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script réalisé à l'aide de ce tuto : https://www.youtube.com/watch?v=SkkxwsXwEhc&list=PLzDRvYVwl53tqzN5R-j33Sd7kf_f6b6z4&index=6
//Sert à gérer les particules de sang permanentes avec le script MeshParticles

//Exemple pour call la fonction : Bloodstains._instance.SpawnBlood(transform.position, -transform.up);
public class Bloodstains : MonoBehaviour
{
    public static Bloodstains _instance { get; private set; }
    private MeshParticles meshParticle;
    private List<Single> singleList;

    private void Awake()
    {
        _instance = this;
        meshParticle = GetComponent<MeshParticles>();
        singleList = new List<Single>();
    }
    private void Update()
    {
        for (int i = 0; i < singleList.Count; i++)
        {
            Single single = singleList[i];
            single.Update();
            if (single.IsParticleComplete())
            {
                singleList.RemoveAt(i);
                i--;
            }
        }
    }

    //Fonction pour faire apparaitre des taches de sang au sol
    public void SpawnBlood(Vector3 position, Vector3 direction)
    {
        float bloodParticleCount = Random.Range(1, 5);
        for (int i = 0; i < bloodParticleCount; i++)
        {
            Vector3 directionAngle = Quaternion.Euler(0, 0, Random.Range(-15f, 15f)) * direction;
            singleList.Add(new Single(position, directionAngle, meshParticle));
        }
    }

    //Représente une particule de sang
    private class Single
    {

        private MeshParticles meshParticle;
        private Vector3 position;
        private Vector3 direction;
        private int quadIndex;
        private Vector3 quadSize;
        private float moveSpeed;
        private float rotation;
        private int uvIndex;

        public Single(Vector3 position, Vector3 direction, MeshParticles meshParticle)
        {
            this.position = position;
            this.direction = direction;
            this.meshParticle = meshParticle;

            //propriétées aléatoires
            float randomSize = Random.Range(0.1f, 0.5f);
            quadSize = new Vector3(randomSize, randomSize);
            rotation = Random.Range(0, 360f);
            moveSpeed = Random.Range(3f, 8f);
            uvIndex = Random.Range(0, 8);

            quadIndex = meshParticle.AddQuad(position, rotation, quadSize, false, uvIndex);
        }

        //la tache de sange tourne et bouge un peu avant de se stopper
        public void Update()
        {
            position += direction * moveSpeed * Time.deltaTime;
            rotation += 360f * (moveSpeed / 10f) * Time.deltaTime;

            meshParticle.UpdateQuad(quadIndex, position, rotation, quadSize, false, uvIndex);

            float slowDownFactor = 7f;
            moveSpeed -= moveSpeed * slowDownFactor * Time.deltaTime;
        }

        public bool IsParticleComplete()
        {
            return moveSpeed < .1f;
        }

    }
}
