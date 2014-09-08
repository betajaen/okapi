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

    public OkSurface()
    {
      mMesh = new Mesh();
      mMesh.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Destroy()
    {
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

  }

  public class OkDrawing
  {
  }

}
