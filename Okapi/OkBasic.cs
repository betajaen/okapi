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
      if (Parent != null)
      {
        Parent.Remove(this);
      }

      Id = -1;
      Exists = true;
      Active = true;
      Visible = true;
      CameraMask = Int32.MaxValue;
      NextSibling = null;
      PreviousSibling = null;
      Parent = null;
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
      Alive = false;
      Exists = false;
    }

    public virtual void Revive()
    {
      Alive = true;
      Exists = true;
    }


    public int Id { get; private set; }
    public bool Exists { get; private set; }
    public bool Active { get; private set; }
    public bool Alive { get; private set; }
    public bool Visible { get; private set; }

    public OkBasic NextSibling { get; internal set; }
    public OkBasic PreviousSibling { get; internal set; }
    public OkGroup Parent { get; internal set; }

    public int CameraMask { get; set; }

  }

}
