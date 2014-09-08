using UnityEngine;

namespace Okapi
{

  internal class OkSurface
  {

    private Mesh mMesh;
    private Material mMaterial;
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

  }

  public class OkDrawing
  {
  }

}
