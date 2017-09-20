using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject spawnPrefab;
    public int gridWidth;
    public int gridHeight;

    int max = 0;

    private string[] states =
    {
        "Attack01",
        "Attack02",
        "Death",
        "GetHit",
        "Idle",
        "Run",
        "Victory",
        "Walk"
    };


    Texture[] textures;
    Material[] mats;

    void Start()
    {
        max = states.Length;
        textures = new Texture2D[max];
        mats = new Material[max];
        Material m = spawnPrefab.GetComponent<Renderer>().sharedMaterial;
        for(int i =0;i<max;i++)
        {
            textures[i] = Resources.Load<Texture>("AnimMap/Footman_Blue_" + states[i]);
            mats[i] = Instantiate(m) as Material;
            mats[i].SetTexture("_AnimMap", textures[i % max]);
            mats[i].name = states[i].ToString();
        }

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GameObject go = Instantiate<GameObject>(spawnPrefab, new Vector3(i * 2, 0, j * 2), Quaternion.identity);
                go.GetComponent<Renderer>().material = mats[i % 8];
                //mat.SetTexture("_AnimMap", textures[i % max]);
            }
        }
    }

}
