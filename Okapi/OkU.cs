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

    public enum DisplayForcedOrientation
    {
      SameAsDisplay,
      Rotated
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

    // gameSize is in pixels, it is scaled up or down by gamePlacement and/or gameScale.
    // It has an orientation which is from gameOrientation, this is relative to the user's eye level and not the device.
    // 
    // displaySize should always be given in it's landscape orientation (i.e. the x is always the largest)
    // displayOrientation can only be Portrait or Landscape
    public static bool ComputeGamePlacementOnDisplay(OkPoint gameSize, float gameScale, GamePlacement gamePlacement, DisplayOrientation gameOrientation, OkPoint displaySize, DisplayOrientation displayOrientation, out OkRect computedGameRect, out DisplayForcedOrientation computedGameOrientation, out float computedScale)
    {
      computedGameRect = new OkRect(0, 0, (int)(gameSize.x * gameScale), (int)(gameSize.y * gameScale));
      computedScale = gameScale;
      computedGameOrientation = DisplayForcedOrientation.SameAsDisplay;

      int gameWidth = 0;
      int gameHeight = 0;
      int displayWidth = 0;
      int displayHeight = 0;

      OkPoint landscapeGameSize = gameSize.SortElements();
      OkPoint landscapeDisplaySize = displaySize.SortElements();

      switch (displayOrientation)
      {
        case DisplayOrientation.Landscape:
        {
          switch (gameOrientation)
          {
            case DisplayOrientation.Any:
            case DisplayOrientation.Landscape:
            {
              // Display is Landscape
              // Game wants to be in Landscape or anything
              // Get's landscape
              computedGameOrientation = DisplayForcedOrientation.SameAsDisplay;
              gameWidth = landscapeGameSize.x;
              gameHeight = landscapeGameSize.y;
              displayWidth = landscapeDisplaySize.x;
              displayHeight = landscapeDisplaySize.y;
            }
            break;
            case DisplayOrientation.Portrait:
            {
              // Display is Portrait
              // Game wants to be in Landscape or anything
              // Get's portrait
              computedGameOrientation = DisplayForcedOrientation.Rotated;
              gameWidth = landscapeDisplaySize.y;
              gameHeight = landscapeDisplaySize.x;
              displayWidth = landscapeDisplaySize.y;
              displayHeight = landscapeDisplaySize.x;
            }
            break;
          }
        }
        break;
        case DisplayOrientation.Portrait:
        {
          switch (gameOrientation)
          {
            case DisplayOrientation.Landscape:
            {
              // Display is in Portrait
              // Game wants to be in Landscape
              // Get's landscape
              computedGameOrientation = DisplayForcedOrientation.Rotated;
              gameWidth = landscapeGameSize.x;
              gameHeight = landscapeGameSize.y;
              displayWidth = landscapeDisplaySize.y;
              displayHeight = landscapeDisplaySize.x;
            }
            break;
            case DisplayOrientation.Any:
            case DisplayOrientation.Portrait:
            {
              // Display is in Portrait
              // Game wants to be in Portrait
              // Get's Portrait
              computedGameOrientation = DisplayForcedOrientation.SameAsDisplay;
              gameWidth = landscapeGameSize.y;
              gameHeight = landscapeGameSize.x;
              displayWidth = landscapeDisplaySize.y;
              displayHeight = landscapeDisplaySize.x;
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
          int width = (int)(gameWidth * gameScale);
          int height = (int)(gameHeight * gameScale);
          int left = 0;
          int top = 0;

          computedGameRect = new OkRect(left, top, width, height);
          computedScale = gameScale;
        }
        break;
        case GamePlacement.FixedScaleCentred:
        {
          int width = (int)(gameWidth * gameScale);
          int height = (int)(gameHeight * gameScale);
          int left = displayWidth / 2 - width / 2;
          int top = displayHeight / 2 - height / 2;

          computedGameRect = new OkRect(left, top, width, height);
          computedScale = gameScale;
        }
        break;
        case GamePlacement.FitToScreenCentred:
        {


          gameScale = 1.0f / Mathf.Max((float)gameWidth / (float)displayWidth, (float)gameHeight / (float)displayHeight);

          int width = (int)(gameWidth * gameScale);
          int height = (int)(gameHeight * gameScale);
          int left = displayWidth / 2 - width / 2;
          int top = displayHeight / 2 - height / 2;

          computedGameRect = new OkRect(left, top, width, height);
          computedScale = gameScale;
        }
        break;
      }

      return (displayWidth * displayHeight - computedGameRect.Area() >= 0);
    }

  }

}
