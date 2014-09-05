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

  public static class OkU
  {

    public enum GamePlacement
    {
      FixedScale,
      FixedScaleCentred,
      FitToScreenCentred,
    }

    [Flags]
    public enum DisplayOrientation
    {
      Landscape = 1,
      Portrait = 2,
      Any = Landscape | Portrait
    }

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

    public static DisplayOrientation SwapDisplayOrientation(DisplayOrientation orientation)
    {
      return orientation == DisplayOrientation.Portrait ? DisplayOrientation.Landscape : DisplayOrientation.Portrait;
    }

    public static bool ComputeScreenBoundary(OkPoint gameSize, float gameScale, GamePlacement gamePlacement, DisplayOrientation gameOrientation, OkPoint displaySize, DisplayOrientation displayOrientation, out OkRect computedGameRect, out DisplayOrientation computedGameOrientation, out float computedScale)
    {
      computedGameRect = new OkRect(0, 0, (int)(gameSize.x * gameScale), (int)(gameSize.y * gameScale));
      computedScale = gameScale;
      computedGameOrientation = displayOrientation;

      switch (displayOrientation)
      {
        case DisplayOrientation.Landscape:
        {
          switch (gameOrientation)
          {
            case DisplayOrientation.Any:
            case DisplayOrientation.Portrait:
            {
              gameSize.SwapElements();
            }
            break;
          }
        }
        break;
        case DisplayOrientation.Portrait:
        {
          switch (gameOrientation)
          {
            case DisplayOrientation.Any:
            case DisplayOrientation.Landscape:
            {
              gameSize.SwapElements();
            }
            break;
          }
        }
        break;
      }

      switch (gamePlacement)
      {
        case GamePlacement.FixedScale:
        {
          int width = (int)(gameSize.x * gameScale);
          int height = (int)(gameSize.y * gameScale);
          int left = 0;
          int top = 0;

          computedGameRect = new OkRect(left, top, width, height);
          computedGameOrientation = displayOrientation;
          computedScale = gameScale;
        }
        break;
        case GamePlacement.FixedScaleCentred:
        {
          int width = (int)(gameSize.x * gameScale);
          int height = (int)(gameSize.y * gameScale);
          int left = displaySize.x / 2 - width / 2;
          int top = displaySize.y / 2 - height / 2;

          computedGameRect = new OkRect(left, top, width, height);
          computedGameOrientation = displayOrientation;
          computedScale = gameScale;
        }
        break;
        case GamePlacement.FitToScreenCentred:
        {


          gameScale = 1.0f / Mathf.Max((float)gameSize.x / (float)displaySize.x, (float)gameSize.y / (float)displaySize.y);

          int width = (int)(gameSize.x * gameScale);
          int height = (int)(gameSize.y * gameScale);
          int left = displaySize.x / 2 - width / 2;
          int top = displaySize.y / 2 - height / 2;

          computedGameRect = new OkRect(left, top, width, height);
          computedGameOrientation = displayOrientation;
          computedScale = gameScale;
        }
        break;
      }

      return (displaySize.x * displaySize.y - computedGameRect.Area() >= 0);
    }

  }

}
