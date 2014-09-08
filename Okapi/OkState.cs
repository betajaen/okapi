using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Okapi
{
  public class OkState : OkGroupOf<OkLayer>
  {
    private OkLayer mMainLayer;

    private bool mDoesUpdate;

    private bool mDoesDraw;

    public OkState()
    {
      mDoesUpdate = true;
      mDoesUpdate = true;
      mMainLayer = new OkLayer();
      Add(mMainLayer);
    }

    public OkLayer mainLayer
    {
      get { return mMainLayer; }
      set
      {
        if (value.parent == this)
        {
          mMainLayer = value;
        }
        else
        {
          throw new Exception("Cannot set layer as mainLayer when it does not belong to that state!");
        }
      }
    }

    public bool doesUpdate
    {
      get { return mDoesUpdate; }
      set { mDoesUpdate = value; }
    }

    public bool doesDraw
    {
      get { return mDoesDraw; }
      set { mDoesDraw = value; }
    }

  }
}
