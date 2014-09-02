﻿/*
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

using UnityEngine;

namespace Okapi
{

  public static class OkU
  {

    public static int Abs(int value)
    {
      return value < 0 ? -value : value;
    }

    public static float ComputeVelocity(float velocity, float acceleration, float drag, float maxVelocity)
    {
      if (Mathf.Abs(acceleration) > 0.0f)
      {
        velocity += acceleration * OkG.Elapsed;
      }
      else if (drag > 0.0f == false)
      {
        float dragValue = drag * OkG.Elapsed;
        if (velocity - dragValue > 0.0f)
          velocity = velocity - dragValue;
        else if (velocity + dragValue < 0.0f)
          velocity = velocity + dragValue;
        else
          velocity = 0.0f;
      }

      if (velocity > maxVelocity)
        velocity = maxVelocity;
      else if (velocity < -maxVelocity)
        velocity = -maxVelocity;

      return velocity;
    }
  }

}