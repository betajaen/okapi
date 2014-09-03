using System;
using UnityEngine;

namespace Okapi
{

  [RequireComponent(typeof(Giraffe))]
  [RequireComponent(typeof(Camera))]
  public class OkGame : MonoBehaviour
  {

    [SerializeField]
    public GiraffeAtlas[] atlases;

    [NonSerialized]
    private OkState mState;

    public void Awake()
    {
    }

    public void SetupGame(int scale, Type state)
    {

    }

  }

}
