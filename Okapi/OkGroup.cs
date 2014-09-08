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
  public abstract class OkGroup : OkBasic
  {
    public abstract void Add(OkBasic basic);
    public abstract void Remove(OkBasic basic);
    public abstract OkBasic firstChild { get; }
    public abstract OkBasic lastChild { get; }
    public abstract int childCount { get; }
    public abstract int maxChildCount { get; set; }
  }

  public class OkGroupOf<T> : OkGroup where T : OkBasic
  {

    private T mFirstChild;
    private T mLastChild;
    private int mChildCount;
    private int mMaxChildCount;

    public OkGroupOf()
    {
      mFirstChild = null;
      mLastChild = null;
      mChildCount = 0;
      mMaxChildCount = Int32.MaxValue;
    }

    public override void Destroy()
    {
      base.Destroy();

      OkBasic basic = mFirstChild;

      while (basic != null)
      {
        OkBasic next = basic.nextSibling;
        basic.Destroy();
        basic.nextSibling = null;
        basic.previousSibling = null;
        basic.parent = null;
        basic = next;
      }

      mFirstChild = null;
      mLastChild = null;
      mChildCount = 0;
      mMaxChildCount = Int32.MaxValue;
    }

    public virtual void Add(T value)
    {

      if (value.parent != null)
      {
        value.parent.Remove(value);
      }

      if (mLastChild != null)
      {
        mLastChild.nextSibling = value;
        value.previousSibling = mLastChild;
        mLastChild = value;
      }
      else
      {
        mFirstChild = mLastChild = value;
      }

      value.parent = this;
    }

    public virtual void Remove(T value)
    {
      if (value.parent == null || value.parent != this)
      {
        throw new InvalidOperationException(String.Format("Object {0} does not belong to this Group {1} or any Group", value, this));
      }

      OkBasic next = value.nextSibling;
      OkBasic previous = value.previousSibling;

      if (previous != null)
      {
        previous.nextSibling = next;
      }

      if (next != null)
      {
        next.previousSibling = previous;
      }

      if (mFirstChild == value)
      {
        mFirstChild = next as T;
      }

      if (mLastChild == value)
      {
        mLastChild = previous as T;
      }

      value.nextSibling = null;
      value.previousSibling = null;
      value.parent = null;

    }

    public override void Update()
    {
      OkBasic basic = mFirstChild;

      while (basic != null)
      {
        OkBasic next = basic.nextSibling;
        if (basic.exists && basic.active)
        {
          basic.PreUpdate();
          basic.Update();
          basic.PostUpdate();
        }
        basic = next;
      }
    }

    public override void PreDraw()
    {
      OkBasic basic = mFirstChild;

      while (basic != null)
      {
        OkBasic next = basic.nextSibling;
        if (basic.exists && basic.visible)
        {
          basic.PreDraw();
        }
        basic = next;
      }
    }

    public override void Draw()
    {
      OkBasic basic = mFirstChild;

      while (basic != null)
      {
        OkBasic next = basic.nextSibling;
        if (basic.exists && basic.visible)
        {
          basic.Draw();
        }
        basic = next;
      }
    }

    public override void Add(OkBasic basic)
    {
      if (basic is T)
      {
        Add(basic as T);
      }
      else
      {
        throw new ArgumentOutOfRangeException(String.Format("An Object {0} cannot be inserted into a Group of {1}", basic.GetType(), typeof(T)));
      }
    }

    public override void Remove(OkBasic basic)
    {
      if (basic is T)
      {
        Remove(basic as T);
      }
      else
      {
        throw new ArgumentOutOfRangeException(String.Format("An Object {0} cannot be removed from a Group of {1}", basic.GetType(), typeof(T)));
      }
    }

    public override OkBasic firstChild
    {
      get { return mFirstChild; }
    }

    public override OkBasic lastChild
    {
      get { return mLastChild; }
    }

    public override int childCount
    {
      get { return mChildCount; }
    }

    public override int maxChildCount
    {
      get { return mMaxChildCount; }
      set { mMaxChildCount = value; }
    }

  }

}
