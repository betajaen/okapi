using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Okapi
{

  public class OkLayer : OkGroupOf<OkBasic>
  {

    private int mDepth;

    public OkLayer()
    {
    }

    public int depth
    {
      get { return mDepth; }
      set { mDepth = value; }
    }

  }

}
