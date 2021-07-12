using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Voxel : MonoBehaviour
{
    #region --- helper ---
    private enum enumCorner
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
    }
    public enum enumSide
    {
        EFAB_back,
        FHBD_right,
        HGDC_forward,
        GECA_left,
        EFGH_up,
        ABCD_down,
    }
    private enum enumTexture
    {
        dirt,
        grassdirt,
        grass,
        water,

        leaf,
        stone,
        coal,
        bark,

        treecut,
        wood,
        brick,
        lava,

        sand,
        mud,
        bark2,
        wood2,
    }
    #endregion

    [Range(0.25f, 0.5f)]
    public float voxelSize = 0.5f;

    private Vector3[] vertices = new Vector3[24];
    private Vector2[] uv = new Vector2[24];
    private int[] triangles = new int[36];
    private int A, B, C, D, E, F, G, H;
    private Mesh mesh = null;

    private void Start()
    {
        //get pointer to the mesh
        mesh = new Mesh();
        this.GetComponent<MeshFilter>().mesh = mesh;
        this.GetComponent<MeshCollider>().sharedMesh = mesh;

        //create the data for voxel
        setVerticesSide(enumSide.EFAB_back);
        setVerticesSide(enumSide.FHBD_right);
        setVerticesSide(enumSide.HGDC_forward);
        setVerticesSide(enumSide.GECA_left);
        setVerticesSide(enumSide.EFGH_up);
        setVerticesSide(enumSide.ABCD_down);

        setUVSide(enumSide.EFAB_back, enumTexture.grassdirt);
        setUVSide(enumSide.FHBD_right, enumTexture.grassdirt);
        setUVSide(enumSide.HGDC_forward, enumTexture.grassdirt);
        setUVSide(enumSide.GECA_left, enumTexture.grassdirt);
        setUVSide(enumSide.EFGH_up, enumTexture.grass);
        setUVSide(enumSide.ABCD_down, enumTexture.dirt);

        setTriangleSide(enumSide.EFAB_back, 1);
        setTriangleSide(enumSide.FHBD_right, 1);
        setTriangleSide(enumSide.HGDC_forward, 1);
        setTriangleSide(enumSide.GECA_left, 1);
        setTriangleSide(enumSide.EFGH_up, 1);
        setTriangleSide(enumSide.ABCD_down, 1);

        //give data to the gameobject mesh (to be visible and rendered)
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    private Vector3 Corner(enumCorner code)
    {
        /*        
                   G           H 
               
               E           F 
                                            
                                           
                   C           D            
                                            
               A           B                

       */

        float LO = -voxelSize;
        float HI = voxelSize;

        switch (code)
        {
            case enumCorner.A:
                return new Vector3(LO, LO, LO);     //  A   -  -  -
            case enumCorner.B:
                return new Vector3(HI, LO, LO);     //  B   +  -  -
            case enumCorner.C:
                return new Vector3(LO, LO, HI);     //  C   -  -  +
            case enumCorner.D:
                return new Vector3(HI, LO, HI);     //  D   +  -  +
            case enumCorner.E:
                return new Vector3(LO, HI, LO);     //  E   -  +  -
            case enumCorner.F:
                return new Vector3(HI, HI, LO);     //  F   +  +  -
            case enumCorner.G:
                return new Vector3(LO, HI, HI);     //  G   -  +  +
            case enumCorner.H:
                return new Vector3(HI, HI, HI);     //  H   +  +  +
            default:
                Debug.Log("Corner: Error [enumCorner]=" + code.ToString());
                return Vector3.zero;
        }
    }
    private void setVerticesSide(enumSide side)
    {
        int i;

        try
        {
            i = (int)side * 4;

            switch (side)
            {
                case enumSide.EFAB_back:
                    vertices[i++] = Corner(enumCorner.E);
                    vertices[i++] = Corner(enumCorner.F);
                    vertices[i++] = Corner(enumCorner.A);
                    vertices[i++] = Corner(enumCorner.B);
                    break;
                case enumSide.FHBD_right:
                    vertices[i++] = Corner(enumCorner.F);
                    vertices[i++] = Corner(enumCorner.H);
                    vertices[i++] = Corner(enumCorner.B);
                    vertices[i++] = Corner(enumCorner.D);
                    break;
                case enumSide.HGDC_forward:
                    vertices[i++] = Corner(enumCorner.H);
                    vertices[i++] = Corner(enumCorner.G);
                    vertices[i++] = Corner(enumCorner.D);
                    vertices[i++] = Corner(enumCorner.C);
                    break;
                case enumSide.GECA_left:
                    vertices[i++] = Corner(enumCorner.G);
                    vertices[i++] = Corner(enumCorner.E);
                    vertices[i++] = Corner(enumCorner.C);
                    vertices[i++] = Corner(enumCorner.A);
                    break;
                case enumSide.EFGH_up:
                    vertices[i++] = Corner(enumCorner.E);
                    vertices[i++] = Corner(enumCorner.F);
                    vertices[i++] = Corner(enumCorner.G);
                    vertices[i++] = Corner(enumCorner.H);
                    break;
                case enumSide.ABCD_down:
                    vertices[i++] = Corner(enumCorner.A);
                    vertices[i++] = Corner(enumCorner.B);
                    vertices[i++] = Corner(enumCorner.C);
                    vertices[i++] = Corner(enumCorner.D);
                    break;
                default:
                    Debug.Log("setVerticesSide: Error [enumSide]=" + side.ToString());
                    break;
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    private void setUVSide(enumSide side, enumTexture texture)
    {
        Vector2 LO = new Vector2(0f, 0f);
        Vector2 HI = new Vector2(1f, 1f);
        Vector3 pixels = new Vector2(128f, 128f);
        float width = 512f;
        float height = 512f;

        // 1. get the LO, HI pixels of the texture within the sprite
        switch (texture)
        {
            // row 3
            case enumTexture.dirt:
                LO.x = pixels.x * 0;
                LO.y = pixels.y * 3;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.grassdirt:
                LO.x = pixels.x * 1;
                LO.y = pixels.y * 3;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.grass:
                LO.x = pixels.x * 2;
                LO.y = pixels.y * 3;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.water:
                LO.x = pixels.x * 3;
                LO.y = pixels.y * 3;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;

            // row 2
            case enumTexture.leaf:
                LO.x = pixels.x * 0;
                LO.y = pixels.y * 2;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.stone:
                LO.x = pixels.x * 1;
                LO.y = pixels.y * 2;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.coal:
                LO.x = pixels.x * 2;
                LO.y = pixels.y * 2;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.bark:
                LO.x = pixels.x * 3;
                LO.y = pixels.y * 2;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;

            // row 1
            case enumTexture.treecut:
                LO.x = pixels.x * 0;
                LO.y = pixels.y * 1;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.wood:
                LO.x = pixels.x * 1;
                LO.y = pixels.y * 1;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.brick:
                LO.x = pixels.x * 2;
                LO.y = pixels.y * 1;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.lava:
                LO.x = pixels.x * 3;
                LO.y = pixels.y * 1;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;

            // row 0
            case enumTexture.sand:
                LO.x = pixels.x * 0;
                LO.y = pixels.y * 0;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.mud:
                LO.x = pixels.x * 1;
                LO.y = pixels.y * 0;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.bark2:
                LO.x = pixels.x * 2;
                LO.y = pixels.y * 0;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
            case enumTexture.wood2:
                LO.x = pixels.x * 3;
                LO.y = pixels.y * 0;
                HI.x = LO.x + pixels.x;
                HI.y = LO.y + pixels.y;
                break;
        }

        // 2. convert the pixels to percent (0.0 to 1.0) for the uv
        LO = new Vector2(LO.x / width, LO.y / height);
        HI = new Vector2(HI.x / width, HI.y / height);
        
        // 3. plot the UV
        int i = (int)side * 4;
        switch (side)
        {
            case enumSide.EFAB_back:
            case enumSide.FHBD_right:
            case enumSide.HGDC_forward:
            case enumSide.GECA_left:
                uv[i++] = new Vector2(LO.x, HI.y);  //  -   +
                uv[i++] = new Vector2(HI.x, HI.y);  //  +   +
                uv[i++] = new Vector2(LO.x, LO.y);  //  -   -
                uv[i++] = new Vector2(HI.x, LO.y);  //  +   -
                break;
            case enumSide.EFGH_up:
                uv[i++] = new Vector2(LO.x, LO.y);  //  -   -
                uv[i++] = new Vector2(HI.x, LO.y);  //  +   -
                uv[i++] = new Vector2(LO.x, HI.y);  //  -   +
                uv[i++] = new Vector2(HI.x, HI.y);  //  +   +
                break;
            case enumSide.ABCD_down:
                uv[i++] = new Vector2(LO.x, HI.y);  //  -   +
                uv[i++] = new Vector2(HI.x, HI.y);  //  +   +
                uv[i++] = new Vector2(LO.x, LO.y);  //  -   -
                uv[i++] = new Vector2(HI.x, LO.y);  //  +   -
                break;
            default:
                Debug.Log("setUVSide: Error [enumSide]=" + side.ToString() + ", [enumTexture]=" + texture.ToString());
                break;
        }
    }
    public void setTriangleSide(enumSide side, int on = 1)
    {
        int i = (int)side * 6;

        switch (side)
        {
            case enumSide.EFAB_back:
                E = (on == 1) ? (0) : 0; //we will use the vertex index if on, else 0
                F = (on == 1) ? (1) : 0;
                A = (on == 1) ? (2) : 0;
                B = (on == 1) ? (3) : 0;
                triangles[i++] = A;      //clockwise connect determines will render
                triangles[i++] = E;
                triangles[i++] = F;
                triangles[i++] = B;
                triangles[i++] = A;
                triangles[i++] = F;
                break;
            case enumSide.FHBD_right:
                F = (on == 1) ? (4) : 0;
                H = (on == 1) ? (5) : 0;
                B = (on == 1) ? (6) : 0;
                D = (on == 1) ? (7) : 0;
                triangles[i++] = B;
                triangles[i++] = F;
                triangles[i++] = H;
                triangles[i++] = D;
                triangles[i++] = B;
                triangles[i++] = H;
                break;
            case enumSide.HGDC_forward:
                H = (on == 1) ? (8) : 0;
                G = (on == 1) ? (9) : 0;
                D = (on == 1) ? (10) : 0;
                C = (on == 1) ? (11) : 0;
                triangles[i++] = D;
                triangles[i++] = H;
                triangles[i++] = G;
                triangles[i++] = C;
                triangles[i++] = D;
                triangles[i++] = G;
                break;
            case enumSide.GECA_left:
                G = (on == 1) ? (12) : 0;
                E = (on == 1) ? (13) : 0;
                C = (on == 1) ? (14) : 0;
                A = (on == 1) ? (15) : 0;
                triangles[i++] = C;
                triangles[i++] = G;
                triangles[i++] = E;
                triangles[i++] = A;
                triangles[i++] = C;
                triangles[i++] = E;
                break;
            case enumSide.EFGH_up:
                E = (on == 1) ? (16) : 0;
                F = (on == 1) ? (17) : 0;
                G = (on == 1) ? (18) : 0;
                H = (on == 1) ? (19) : 0;
                triangles[i++] = E;
                triangles[i++] = G;
                triangles[i++] = H;
                triangles[i++] = F;
                triangles[i++] = E;
                triangles[i++] = H;
                break;
            case enumSide.ABCD_down:
                A = (on == 1) ? (20) : 0;
                B = (on == 1) ? (21) : 0;
                C = (on == 1) ? (22) : 0;
                D = (on == 1) ? (23) : 0;
                triangles[i++] = C;
                triangles[i++] = A;
                triangles[i++] = B;
                triangles[i++] = D;
                triangles[i++] = C;
                triangles[i++] = B;
                break;
            default:
                Debug.Log("setTrianglesSide: Error [enumSide]=" + side.ToString());
                break;
        }
    }
}
