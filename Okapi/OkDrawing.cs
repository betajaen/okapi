using System.Runtime.Remoting.Messaging;
using UnityEngine;

namespace Okapi
{

  internal class OkSurface
  {

    private Mesh mMesh;
    private Material mMaterial;
    private Vector2 mOffsetPosition;
    private float mOffsetScale;
    private float mOffsetRotation;

    private Vector3 mPosition, mScale;
    private Quaternion mRotation;
    private Matrix4x4 mTransform;

    private Vector3[] mDataPositions;
    private Color32[] mDataColours;
    private Vector2[] mDataTextureCoords;
    private int[] mIndexes;

    private int mVertexCount;
    private int mIndexCount;

    private int mIteratorVertices;
    private int mIteratorIndexes;
    private int mVertexIndex;
    private Color32 mColour;
    private bool mClearMesh;

    private readonly Vector3[] mQuadPosition = new Vector3[4];
    private readonly Vector3[] mQuadTextureCoord = new Vector3[4];

    public OkSurface()
    {
      mMesh = new Mesh();
      mMesh.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Destroy()
    {
      mDataPositions = null;
      mDataTextureCoords = null;
      mIndexes = null;
      Object.Destroy(mMesh);
    }

    public Vector2 position
    {
      get { return mOffsetPosition; }
      set
      {
        mOffsetPosition = value;
        RefreshTransform();
      }
    }

    public float scale
    {
      get { return mOffsetScale; }
      set
      {
        mOffsetScale = Mathf.Abs(value);
        RefreshTransform();
      }
    }

    public float rotation
    {
      get { return mOffsetRotation; }
      set
      {
        mOffsetRotation = value;
        RefreshTransform();
      }
    }

    void RefreshTransform()
    {
      mScale.x = mOffsetScale;
      mScale.y = -mOffsetScale;
      mScale.z = 0.0f;

      mPosition.x = mOffsetPosition.x;
      mPosition.y = mOffsetPosition.y;

      mRotation = Quaternion.AngleAxis(mOffsetRotation, new Vector3(0, 0, 1.0f));

      mTransform = Matrix4x4.TRS(mPosition, mRotation, mScale);
    }

    public void Draw()
    {
      mMaterial.SetPass(0);
      Graphics.DrawMeshNow(mMesh, mTransform);
    }

    public void Begin(int nbQuads)
    {
      int nbVertices = nbQuads * 4;
      int nbIndexes = nbQuads * 6;

      Reserve(nbVertices, nbIndexes);

      mIteratorVertices = 0;
      mIteratorIndexes = 0;
      mVertexIndex = 0;
      mColour = new Color32(255, 255, 255, 255);

    }

    void Reserve(int nbVertices, int nbIndexes)
    {
      mClearMesh = nbVertices != mVertexCount;

      if (nbVertices > mVertexCount || nbIndexes > mIndexCount)
      {

        mVertexCount = nbVertices;
        mDataPositions = new Vector3[mVertexCount];
        mDataTextureCoords = new Vector2[mVertexCount];
        mDataColours = new Color32[mVertexCount];

        mIndexCount = nbIndexes;
        mIndexes = new int[mIndexCount];
      }
      else
      {
        for (int i = nbIndexes; i < mIndexCount; i++)
          mIndexes[i] = 0;
      }
    }

    void End()
    {

      mMesh.MarkDynamic();

      if (mClearMesh)
      {
        mMesh.Clear();
      }

      mMesh.vertices = mDataPositions;
      mMesh.uv = mDataTextureCoords;
      mMesh.colors32 = mDataColours;

      mMesh.SetIndices(mIndexes, MeshTopology.Triangles, 0);
    }

    public void SetColour(Color32 colour)
    {
      mColour = colour;
    }

    public void Add(float r0, float s0, float r1, float s1, float u0, float v0, float u1, float v1)
    {

      // 0---1
      // |\  |
      // | \ |
      // 3--\2

      mQuadPosition[0].x = r0;
      mQuadPosition[0].y = s0;
      mQuadTextureCoord[0].x = u0;
      mQuadTextureCoord[0].y = v0;

      mQuadPosition[1].x = r1;
      mQuadPosition[1].y = s0;
      mQuadTextureCoord[1].x = u1;
      mQuadTextureCoord[1].y = u0;

      mQuadPosition[2].x = r1;
      mQuadPosition[2].y = s1;
      mQuadTextureCoord[2].x = u1;
      mQuadTextureCoord[2].y = v1;

      mQuadPosition[3].x = r0;
      mQuadPosition[3].y = s1;
      mQuadTextureCoord[3].x = u0;
      mQuadTextureCoord[3].y = v1;

      mDataPositions[mIteratorVertices] = mQuadPosition[0];
      mDataTextureCoords[mIteratorVertices] = mQuadTextureCoord[0];
      mDataColours[mIteratorVertices++] = mColour;

      mDataPositions[mIteratorVertices] = mQuadPosition[1];
      mDataTextureCoords[mIteratorVertices] = mQuadTextureCoord[1];
      mDataColours[mIteratorVertices++] = mColour;

      mDataPositions[mIteratorVertices] = mQuadPosition[2];
      mDataTextureCoords[mIteratorVertices] = mQuadTextureCoord[2];
      mDataColours[mIteratorVertices++] = mColour;

      mDataPositions[mIteratorVertices] = mQuadPosition[3];
      mDataTextureCoords[mIteratorVertices] = mQuadTextureCoord[3];
      mDataColours[mIteratorVertices++] = mColour;

      mIndexes[mIteratorIndexes++] = mVertexIndex;
      mIndexes[mIteratorIndexes++] = mVertexIndex + 1;
      mIndexes[mIteratorIndexes++] = mVertexIndex + 2;

      mIndexes[mIteratorIndexes++] = mVertexIndex;
      mIndexes[mIteratorIndexes++] = mVertexIndex + 2;
      mIndexes[mIteratorIndexes++] = mVertexIndex + 3;
      mVertexIndex += 4;

    }

  }

  public static class OkDrawing
  {

    private static OkSurface msSurface;

    internal static void __SetSurface(OkSurface surface)
    {
      msSurface = surface;
      msSurface.SetColour(msColour);
    }

    private static Color32 msColour = new Color32(255, 255, 255, 255);

    public static Color32 colour
    {
      get { return msColour; }
      set
      {
        msColour = colour;
        if (msSurface != null)
        {
          msSurface.SetColour(msColour);
        }
      }
    }

    public static void Draw(int x, int y, OkTexture texture)
    {
      msSurface.Add(x, y, x + texture.w, y + texture.h, texture.u0, texture.v0, texture.u1, texture.v1);
    }

    public static void Draw(int x, int y, int w, int h, OkTexture texture)
    {
      msSurface.Add(x, y, x + w, y + h, texture.u0, texture.v0, texture.u1, texture.v1);
    }

  }

}
