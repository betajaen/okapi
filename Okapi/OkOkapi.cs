/*
    Okapi
    -------
    
    Copyright (c) 2014 Robin Southern
    
                                                                                  
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
                                                                                  
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
                                                                                  
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE. 
    
*/

using System;
using UnityEngine;

namespace Okapi
{

  [RequireComponent(typeof(Camera))]
  public sealed class OkOkapi : MonoBehaviour
  {

    [SerializeField]
    private GiraffeAtlas[] mAtlases;

    [SerializeField]
    private String mGameName;

    [NonSerialized]
    private OkGame mGame;

    [SerializeField]
    private int mGameWidth;

    [SerializeField]
    private int mGameHeight;

    [SerializeField]
    private OkU.GamePlacement mGamePlacement;

    [SerializeField]
    private OkU.DisplayOrientation mGameOrientation;

    [SerializeField]
    private float mCustomScale;

    [NonSerialized]
    private bool mStarted;

    public void AddAtlas(GiraffeAtlas atlas)
    {
      Array.Resize(ref mAtlases, mAtlases == null ? 1 : mAtlases.Length + 1);
      mAtlases[mAtlases.Length - 1] = atlas;
    }

    public String gameName
    {
      get { return mGameName; }
      set { mGameName = value; }
    }

    public OkPoint resolution
    {
      get
      {
        return new OkPoint(mGameWidth, mGameHeight);
      }
      set
      {
        mGameWidth = value.x;
        mGameHeight = value.y;
        if (Application.isPlaying && mStarted)
        {
          OnResolutionChange();
        }
      }
    }

    public OkU.GamePlacement placement
    {
      get
      {
        return mGamePlacement;
      }
      set
      {
        mGamePlacement = value;
        if (Application.isPlaying && mStarted)
        {
          OnResolutionChange();
        }
      }
    }

    public OkU.DisplayOrientation orientation
    {
      get
      {
        return mGameOrientation;
      }
      set
      {
        mGameOrientation = value;
        if (Application.isPlaying && mStarted)
        {
          OnResolutionChange();
        }
      }
    }

    public float customScale
    {
      get
      {
        return mCustomScale;
      }
      set
      {
        mCustomScale = value;
        if (Application.isPlaying && mStarted)
        {
          OnResolutionChange();
        }
      }
    }

    public void Awake()
    {
      msInstance = this;
      mGame = ScriptableObject.CreateInstance(gameName) as OkGame;
      mStarted = true;
    }

    public static OkOkapi Instance
    {
      get { return msInstance; }
    }

    private static OkOkapi msInstance;

    void FixedUpdate()
    {
      mGame.ProcessUpdate();
    }

    void OnDisplayChange()
    {
    }

    void OnResolutionChange()
    {
    }

  }

}
