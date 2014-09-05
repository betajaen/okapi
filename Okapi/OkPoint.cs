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

  public struct OkPoint
  {

    public OkPoint(int xValue, int yValue)
    {
      x = xValue;
      y = yValue;
    }

    public static OkPoint operator +(OkPoint lhs, OkPoint rhs)
    {
      return new OkPoint(lhs.x + rhs.x, lhs.y + rhs.y);
    }

    public static OkPoint operator -(OkPoint lhs, OkPoint rhs)
    {
      return new OkPoint(lhs.x - rhs.x, lhs.y - rhs.y);
    }

    public static OkPoint operator *(OkPoint lhs, OkPoint rhs)
    {
      return new OkPoint(lhs.x * rhs.x, lhs.y * rhs.y);
    }

    public static OkPoint operator /(OkPoint lhs, OkPoint rhs)
    {
      return new OkPoint(lhs.x / rhs.x, lhs.y / rhs.y);
    }

    public static OkPoint operator *(OkPoint lhs, int rhs)
    {
      return new OkPoint(lhs.x * rhs, lhs.y * rhs);
    }

    public static OkPoint operator /(OkPoint lhs, int rhs)
    {
      return new OkPoint(lhs.x / rhs, lhs.y / rhs);
    }

    public static OkPoint operator *(int lhs, OkPoint rhs)
    {
      return new OkPoint(lhs * rhs.x, lhs * rhs.y);
    }

    public static OkPoint operator /(int lhs, OkPoint rhs)
    {
      return new OkPoint(lhs / rhs.y, lhs / rhs.y);
    }

    public static OkPoint operator *(OkPoint lhs, float rhs)
    {
      return new OkPoint((int)(lhs.x * rhs), (int)(lhs.y * rhs));
    }

    public static OkPoint operator *(float lhs, OkPoint rhs)
    {
      return new OkPoint((int)(lhs * rhs.x), (int)(lhs * rhs.y));
    }

    public void SwapElements()
    {
      int t = y;
      y = x;
      x = t;
    }

    public int Squared()
    {
      return x * x + y * y;
    }

    public int MaxElement()
    {
      return Mathf.Max(x, y);
    }

    public int MinElements()
    {
      return Mathf.Min(x, y);
    }

    public override string ToString()
    {
      return String.Format("[{0},{1}]", x, y);
    }

    public int x, y;
  }

}
