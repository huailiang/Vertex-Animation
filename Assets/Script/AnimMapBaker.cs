/*
 * 用来烘焙动作贴图。烘焙对象使用animation组件，并且在导入时设置Rig为Legacy
 */
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//动画的相关数据
public struct AnimData
{
    public int vertexCount;
    public int mapWidth;
    public List<AnimationState> animClips;
    public string name;
    private Animation animation;
    private SkinnedMeshRenderer skin;


    public AnimData(Animation anim, SkinnedMeshRenderer smr, string goName)
    {
        vertexCount = smr.sharedMesh.vertexCount;
        mapWidth = Mathf.NextPowerOfTwo(vertexCount);
        animClips = new List<AnimationState>(anim.Cast<AnimationState>());
        animation = anim;
        skin = smr;
        name = goName;
    }


    public void AnimationPlay(string animName)
    {
        animation.Play(animName);
    }

    public void SampleAnimAndBakeMesh(ref Mesh m)
    {
        SampleAnim();
        BakeMesh(ref m);
    }

    private void SampleAnim()
    {
        if (animation == null)
        {
            Debug.LogError("animation is null!!");
            return;
        }
        animation.Sample();
    }

    private void BakeMesh(ref Mesh m)
    {
        if (skin == null)
        {
            Debug.LogError("skin is null!!");
            return;
        }
        skin.BakeMesh(m);
    }
}


// 烘焙后的数据
public struct BakedData
{
    public string name;
    public float animLen;
    public byte[] rawAnimMap;
    public int animMapWidth;
    public int animMapHeight;

    public BakedData(string name, float animLen, Texture2D animMap)
    {
        this.name = name;
        this.animLen = animLen;
        this.animMapHeight = animMap.height;
        this.animMapWidth = animMap.width;
        this.rawAnimMap = animMap.GetRawTextureData();
    }
}

// 烘焙器
public class AnimMapBaker
{
    private AnimData? animData = null;
    private List<Vector3> vertices = new List<Vector3>();
    private Mesh bakedMesh;
    private List<BakedData> bakedDataList = new List<BakedData>();

    public void SetAnimData(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("go is null!!");
            return;
        }
        Animation anim = go.GetComponent<Animation>();
        SkinnedMeshRenderer smr = go.GetComponentInChildren<SkinnedMeshRenderer>();
        if (anim == null || smr == null)
        {
            Debug.LogError("anim or smr is null!!");
            return;
        }
        bakedMesh = new Mesh();
        animData = new AnimData(anim, smr, go.name);
    }

    public List<BakedData> Bake()
    {
        if (animData == null)
        {
            Debug.LogError("bake data is null!!");
            return bakedDataList;
        }
        //每一个动作都生成一个动作图
        for (int i = 0; i < animData.Value.animClips.Count; i++)
        {
            if (!animData.Value.animClips[i].clip.legacy)
            {
                Debug.LogError(string.Format("{0} is not legacy!!", animData.Value.animClips[i].clip.name));
                continue;
            }
            BakePerAnimClip(animData.Value.animClips[i]);
        }
        return bakedDataList;
    }

    private void BakePerAnimClip(AnimationState curAnim)
    {
        int curClipFrame = 0;
        float sampleTime = 0;
        float perFrameTime = 0;

        curClipFrame = Mathf.ClosestPowerOfTwo((int)(curAnim.clip.frameRate * curAnim.length));
        perFrameTime = curAnim.length / curClipFrame; ;
        Texture2D animMap = new Texture2D(animData.Value.mapWidth, curClipFrame, TextureFormat.RGBAHalf, false);
        animMap.name = string.Format("{0}_{1}", animData.Value.name, curAnim.name);
        animData.Value.AnimationPlay(curAnim.name);
        for (int i = 0; i < curClipFrame; i++)
        {
            curAnim.time = sampleTime;
            animData.Value.SampleAnimAndBakeMesh(ref bakedMesh);
            for (int j = 0; j < bakedMesh.vertexCount; j++)
            {
                Vector3 vertex = bakedMesh.vertices[j];
                animMap.SetPixel(j, i, new Color(vertex.x, vertex.y, vertex.z));
            }
            sampleTime += perFrameTime;
        }
        animMap.Apply();
        bakedDataList.Add(new BakedData(animMap.name, curAnim.clip.length, animMap));
    }
    
}
