using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Okapi
{
  public class OkTilemap : OkObject
  {

    private int mTileWidth, mTileHeight;

    public OkTilemap(OkPoint positionValue, OkPoint sizeValue)
      : base(positionValue, sizeValue)
    {
    }

    public OkTilemap(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
    }

    public void OverlapsWithCallback(OkObject @object, OkPoint position, OkObjectSeperationFunction callback = null,
      bool flipCallbackparams = false, int positionX = Int32.MinValue, int positionY = Int32.MinValue)
    {
      bool results = false;

      int px = 0;
      int py = 0;

      if (positionX != Int32.MinValue && positionY != Int32.MinValue)
      {
        px = positionX;
        py = positionY;
      }

      int selectionX = ((@object.x - px) / mTileWidth);

    }

  }
}
