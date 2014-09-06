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

    [HideInInspector]
    [SerializeField]
    private OkU.GamePlacement mPlacement;

    [HideInInspector]
    [SerializeField]
    private OkU.DisplayOrientation mPreferredOrientation;

    [HideInInspector]
    [SerializeField]
    private float mCustomScale;

    [NonSerialized]
    private bool mStarted;

    public OkU.GamePlacement placement
    {
      get
      {
        return mPlacement;
      }
      set
      {
        mPlacement = value;
        if (Application.isPlaying && mStarted)
        {
          ApplyPresentation();
        }
      }
    }

    public OkU.DisplayOrientation preferredOrientation
    {
      get
      {
        return mPreferredOrientation;
      }
      set
      {
        mPreferredOrientation = value;
        if (Application.isPlaying && mStarted)
        {
          ApplyPresentation();
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
          ApplyPresentation();
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


    void ApplyPresentation()
    {

    }

  }

}
