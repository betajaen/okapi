using System;
using System.Collections.Generic;
using UnityEngine;

namespace Okapi
{

  public class OkTexture
  {
    [SerializeField]
    public int x;

    [SerializeField]
    public int y;

    [SerializeField]
    public int w;

    [SerializeField]
    public int h;

    [NonSerialized]
    public float u0;

    [NonSerialized]
    public float v0;

    [NonSerialized]
    public float u1;

    [NonSerialized]
    public float v1;

    [NonSerialized]
    public OkTextureAtlas atlas;

  }

  public sealed class OkTextureAtlas : ScriptableObject, UnityEngine.ISerializationCallbackReceiver
  {

    [SerializeField]
    private String[] __texturesByKey;

    [SerializeField]
    private OkTexture[] __texturesByValue;

    [SerializeField]
    private Texture2D mTexture;

    [NonSerialized]
    private Dictionary<String, OkTexture> mTextures;

    void OnEnable()
    {
    }

    void OnDisable()
    {
    }

    public void OnBeforeSerialize()
    {
      __texturesByKey = new String[mTextures.Count];
      __texturesByValue = new OkTexture[mTextures.Count];

      int index = 0;

      foreach (var kvp in mTextures)
      {
        __texturesByKey[index] = kvp.Key;
        __texturesByValue[index++] = kvp.Value;
      }
    }

    public void OnAfterDeserialize()
    {
      //      _myDictionary = new Dictionary<int, string>();
      //      for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
      //        _myDictionary.Add(_keys[i], _values[i]);
    }
  }

}
