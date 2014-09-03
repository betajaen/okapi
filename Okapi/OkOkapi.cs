using System;
using UnityEngine;

namespace Okapi
{

  [RequireComponent(typeof(Giraffe))]
  [RequireComponent(typeof(Camera))]
  public sealed class OkOkapi : MonoBehaviour
  {

    [SerializeField]
    public GiraffeAtlas[] atlases;

    [SerializeField]
    public String gameName;

    [NonSerialized]
    private OkGame mGame;

    [NonSerialized]
    private OkState mState;

    private static OkOkapi msInstance;

    public void Awake()
    {
      msInstance = this;
    }

  }

}
