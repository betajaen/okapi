using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Okapi
{
  public class OkCamera : OkBasic
  {
    public const int kMaxCameras = 32;

    private static OkCamera[] msCameras;

    static OkCamera()
    {
      msCameras = new OkCamera[kMaxCameras];
    }

    private int mIndex;
    private bool mTransformOutOfDate;

    public OkCamera(int x, int y, int width, int height)
    {

      mPosition = new OkPoint(x, y);
      mSize = new OkPoint(width, height);
      mIndex = -1;

      for (int i = 0; i < kMaxCameras; i++)
      {
        if (msCameras[i] == null)
        {
          mIndex = i;
          msCameras[i] = this;
          break;
        }
      }

      if (mIndex == -1)
      {
        throw new Exception("OkCamera has reached the maximum limit of 32 cameras.");
      }

    }

    public override void Destroy()
    {
      base.Destroy();
      msCameras[mIndex] = null;
      mIndex = -1;
    }

    private Vector2 mScroll;

    public Vector2 scroll
    {
      get { return mScroll; }
      set
      {
        mScroll = value;
        mTransformOutOfDate = true;
      }
    }

    private OkPoint mPosition;

    public OkPoint position
    {
      get { return mPosition; }
      set
      {
        mPosition = value;
        mTransformOutOfDate = true;
      }
    }

    public int x
    {
      get { return mPosition.x; }
      set
      {
        mPosition.x = value;
        mTransformOutOfDate = true;
      }
    }

    public int y
    {
      get { return mPosition.y; }
      set
      {
        mPosition.y = value;
        mTransformOutOfDate = true;
      }
    }

    private OkPoint mSize;

    public OkPoint size
    {
      get { return mSize; }
      set
      {
        mSize = value;
        mTransformOutOfDate = true;
      }
    }

    public int width
    {
      get { return mSize.x; }
      set
      {
        mSize.x = value;
        mTransformOutOfDate = true;
      }
    }

    public int height
    {
      get { return mSize.y; }
      set
      {
        mSize.y = value;
        mTransformOutOfDate = true;
      }
    }

  }
}
