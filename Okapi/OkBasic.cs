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

namespace Okapi
{

  public abstract class OkBasic
  {
    public static int VisibleCount;

    protected OkBasic()
    {
      Reset();
    }

    protected virtual void Reset()
    {
      if (parent != null)
      {
        parent.Remove(this);
      }

      id = -1;
      exists = true;
      active = true;
      visible = true;
      cameraMask = Int32.MaxValue;
      nextSibling = null;
      previousSibling = null;
      parent = null;
    }

    public virtual void Destroy()
    {
    }

    public virtual void PreUpdate()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void PostUpdate()
    {
    }

    public virtual void PreDraw()
    {
    }

    public virtual void Draw()
    {
    }

    public virtual void Kill()
    {
      alive = false;
      exists = false;
    }

    public virtual void Revive()
    {
      alive = true;
      exists = true;
    }


    public int id { get; private set; }
    public bool exists { get; private set; }
    public bool active { get; private set; }
    public bool alive { get; private set; }
    public bool visible { get; private set; }

    public OkBasic nextSibling { get; internal set; }
    public OkBasic previousSibling { get; internal set; }
    public OkGroup parent { get; internal set; }

    public int cameraMask { get; set; }

  }

}
