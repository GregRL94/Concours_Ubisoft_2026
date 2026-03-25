using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticles : MonoBehaviour
{
    //Ce script permet de créer des quads permanents dans le jeu, ces quads ont la possibilité d'avoir un mouvement animé
    //Utilisé par le script Bloodstains pour faire les taches de sange au sol
    private const int MAX_QUADS = 15000;

    // Set dans l'editeur avec des Pixel Values
    [System.Serializable]
    public struct ParticleUVPixels
    {
        public Vector2Int uv00Pixels;
        public Vector2Int uv11Pixels;
    }

    // Textures normalisées des coords UV
    private struct UVCoords
    {
        public Vector2 uv00;
        public Vector2 uv11;
    }
    [SerializeField] private ParticleUVPixels[] ParticleUVPixelsArray;
    private UVCoords[] uvCoordsArray;
    private Mesh mesh;
    private Vector3[] vertices;
    private Vector2[] uv;
    private int[] triangles;

    private int quadIndex;

    private bool updateVertices;
    private bool updateUV;
    private bool updateTriangles;


    private void Awake()
    {
        mesh = new Mesh();

        vertices = new Vector3[4 * MAX_QUADS];
        uv = new Vector2[4 * MAX_QUADS];
        triangles = new int[6 * MAX_QUADS];

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 10000f);

        GetComponent<MeshFilter>().mesh = mesh;

        //Set up UV normalisées
        Material material = GetComponent<MeshRenderer>().material;
        Texture texture = material.mainTexture;
        int textureWidth = texture.width;
        int textureHeight = texture.height;

        List<UVCoords> uvCoordsList = new List<UVCoords>();
        foreach (ParticleUVPixels particleUVPixels in ParticleUVPixelsArray)
        {
            UVCoords uVCoords = new UVCoords()
            {
                uv00 = new Vector2((float)particleUVPixels.uv00Pixels.x / textureWidth, (float)particleUVPixels.uv00Pixels.y / textureHeight),
                uv11 = new Vector2((float)particleUVPixels.uv11Pixels.x / textureWidth, (float)particleUVPixels.uv11Pixels.y / textureHeight)
            };
            uvCoordsList.Add(uVCoords);
        }
        uvCoordsArray = uvCoordsList.ToArray();
    }

    public int AddQuad(Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
    {
        if (quadIndex >= MAX_QUADS)
        {
            Debug.LogWarning("Maximum de quads atteints"); //Mesh rempli
            return 0;
        }
        UpdateQuad(quadIndex, position, rotation, quadSize, skewed, uvIndex);

        int spawnIndex = quadIndex;
        quadIndex++;

        return spawnIndex;
    }

    public void UpdateQuad(int quadIndex, Vector3 position, float rotation, Vector3 quadSize, bool skewed, int uvIndex)
    {
        //Relocaliser les vertices
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        if (skewed)
        {
            vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, -quadSize.y);
            vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(-quadSize.x, +quadSize.y);
            vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, +quadSize.y);
            vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation) * new Vector3(+quadSize.x, -quadSize.y);
        }
        else
        {
            vertices[vIndex0] = position + Quaternion.Euler(0, 0, rotation - 180) * quadSize;
            vertices[vIndex1] = position + Quaternion.Euler(0, 0, rotation - 270) * quadSize;
            vertices[vIndex2] = position + Quaternion.Euler(0, 0, rotation - 0) * quadSize;
            vertices[vIndex3] = position + Quaternion.Euler(0, 0, rotation - 90) * quadSize;
        }

        //Créer les UV
        UVCoords uVCoords = uvCoordsArray[uvIndex];
        uv[vIndex0] = uVCoords.uv00;
        uv[vIndex1] = new Vector2(uVCoords.uv00.x, uVCoords.uv11.y);
        uv[vIndex2] = uVCoords.uv11;
        uv[vIndex3] = new Vector2(uVCoords.uv11.x, uVCoords.uv00.y);

        //Créer les triangles
        int tIndex = quadIndex * 6;

        triangles[tIndex + 0] = vIndex0;
        triangles[tIndex + 1] = vIndex1;
        triangles[tIndex + 2] = vIndex2;

        triangles[tIndex + 3] = vIndex0;
        triangles[tIndex + 4] = vIndex2;
        triangles[tIndex + 5] = vIndex3;

        updateVertices = true;
        updateUV = true;
        updateTriangles = true;
    }

    public void DestroyQuad(int quadIndex)
    {
        // Destroy vertices
        int vIndex = quadIndex * 4;
        int vIndex0 = vIndex;
        int vIndex1 = vIndex + 1;
        int vIndex2 = vIndex + 2;
        int vIndex3 = vIndex + 3;

        vertices[vIndex0] = Vector3.zero;
        vertices[vIndex1] = Vector3.zero;
        vertices[vIndex2] = Vector3.zero;
        vertices[vIndex3] = Vector3.zero;

        updateVertices = true;
    }

    private void LateUpdate()
    {
        if (updateVertices)
        {
            mesh.vertices = vertices;
            updateVertices = false;
        }
        if (updateUV)
        {
            mesh.uv = uv;
            updateUV = false;
        }
        if (updateTriangles)
        {
            mesh.triangles = triangles;
            updateTriangles = false;
        }
    }

}
