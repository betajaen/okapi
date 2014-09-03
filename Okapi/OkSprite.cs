using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Okapi
{
  public class OkSprite : OkObject
  {
    public OkSprite(OkPoint positionValue, OkPoint sizeValue)
      : base(positionValue, sizeValue)
    {
    }

    public OkSprite(int x, int y, int width, int height)
      : base(x, y, width, height)
    {
    }
  }
}
