using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyEffect : MonoBehaviour
{
    //script utilisé par plusieurs objets pour les détruire à partir d'un événement dans l'animation
    public void Destroy()
    {
        Destroy(gameObject);
    }
}
