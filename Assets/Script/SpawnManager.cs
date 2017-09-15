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

    void Start()
    {
        max = states.Length;
        textures = new Texture2D[max];
        for(int i =0;i<max;i++)
        {
            textures[i] = Resources.Load<Texture>("AnimMap/Footman_Blue_" + states[i]);
        }

        for (int i = 0; i < gridWidth; i++)
        {
            for (int j = 0; j < gridHeight; j++)
            {
                GameObject go = Instantiate<GameObject>(spawnPrefab, new Vector3(i * 2, 0, j * 2), Quaternion.identity);
                //Material mat = go.GetComponent<Renderer>().material;
                //mat.SetTexture("_AnimMap", textures[2]);
            }
        }
    }

}
