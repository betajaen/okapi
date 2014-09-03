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

  [Flags]
  public enum OKDirection
  {
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8,
    Horizontal = Left | Right,
    Vertical = Up | Down,
    Any = Horizontal | Vertical
  }

  public class OkObject : OkBasic
  {

    private OkPoint mPosition;
    private OkPoint mLastPosition;
    private bool mTransformOutOfDate;

    public OkObject(OkPoint positionValue, OkPoint sizeValue)
    {
      Reset();
      position = positionValue;
      size = sizeValue;
    }

    public OkObject(int x, int y, int width, int height)
    {
      Reset();
    }

    protected override void Reset()
    {
      base.Reset();
      maxVelocity = new Vector2(10000.0f, 10000.0f);
      health = 1;
      mass = 1.0f;
      elasticity = 0.0f;
      touching = OKDirection.None;
      wasTouching = OKDirection.None;
      collisionEdges = OKDirection.Any;
    }

    public int x
    {
      get { return mPosition.x; }
      set
      {
        mPosition.x = value;
        mTransformOutOfDate = true;
      }
    }

    public int y
    {
      get { return mPosition.y; }
      set
      {
        mPosition.y = value;
        mTransformOutOfDate = true;
      }
    }

    public OkPoint position
    {
      get { return mPosition; }
      set
      {
        mPosition = value;
        mTransformOutOfDate = true;
      }
    }

    public OkPoint lastPosition
    {
      get { return mLastPosition; }
    }

    public OkPoint size
    {
      get { return mSize; }
      set
      {
        mSize = value;
        mTransformOutOfDate = true;
      }
    }

    private OkPoint mSize;

    public int width
    {
      get { return size.x; }
      set
      {
        mSize.x = value;
        mTransformOutOfDate = true;
      }
    }

    public int height
    {
      get { return size.y; }
      set
      {
        mSize.y = value;
        mTransformOutOfDate = true;
      }
    }

    private Vector2 mVelocity;

    public Vector2 velocity
    {
      get { return mVelocity; }
      set { mVelocity = value; }
    }

    public float velocityX
    {
      get { return mVelocity.x; }
      set { mVelocity.x = value; }
    }

    public float velocityY
    {
      get { return mVelocity.y; }
      set { mVelocity.y = value; }
    }

    public Vector2 acceleration { get; set; }

    public Vector2 drag { get; set; }

    public Vector2 maxVelocity { get; set; }

    public Vector2 scrollFactor { get; set; }

    public float mass
    {
      get { return mMass; }
      set
      {
        mMass = value;
        mInverseMass = 1.0f / mMass;
      }
    }

    public float inverseMass
    {
      get { return mInverseMass; }
    }

    private float mMass;
    private float mInverseMass;

    public float elasticity { get; set; }

    public int health { get; set; }

    public OKDirection touching { get; private set; }

    public OKDirection wasTouching { get; private set; }

    private OKDirection mCollisionEdges;

    public OKDirection collisionEdges
    {
      get
      {
        return mCollisionEdges;
      }
      set
      {
        mCollisionEdges = value;
      }
    }

    public bool solid
    {
      get
      {
        return (mCollisionEdges & OKDirection.Any) > OKDirection.None;
      }
      set
      {
        if (value)
          mCollisionEdges = OKDirection.Any;
        else
          mCollisionEdges = OKDirection.None;
      }
    }

    public bool immovable { get; set; }

    public override void PreUpdate()
    {
      base.PreUpdate();
      mLastPosition = mPosition;
    }

    public override void PostUpdate()
    {
      base.PostUpdate();
      wasTouching = touching;
      touching = OKDirection.None;
    }


    public OkPoint GetScreenXY(OkCamera camera)
    {
      if (camera == null)
        camera = OkG.Camera;

      return new OkPoint(
        x - Mathf.RoundToInt(camera.scroll.x * scrollFactor.x),
        y - Mathf.RoundToInt(camera.scroll.y * scrollFactor.y)
        );
    }


    public void UpdateMotion()
    {
      float delta, velocityDelta;

      velocityDelta = (OkU.ComputeVelocity(mVelocity.x, acceleration.x, drag.x, maxVelocity.x) - velocity.x) * 0.5f;
      mVelocity.x += velocityDelta;
      delta = mVelocity.x * OkG.Elapsed;
      mVelocity.x += velocityDelta;
      x += Mathf.RoundToInt(delta);

      velocityDelta = (OkU.ComputeVelocity(mVelocity.y, acceleration.y, drag.y, maxVelocity.y) - velocity.y) * 0.5f;
      mVelocity.y += velocityDelta;
      delta = mVelocity.y * OkG.Elapsed;
      mVelocity.y += velocityDelta;
      y += Mathf.RoundToInt(delta);
    }

    public bool IsTouching(OKDirection direction)
    {
      return (touching & direction) > OKDirection.None;
    }

    public bool IsStartedTouching(OKDirection direction)
    {
      return ((touching & direction) > OKDirection.None) && ((wasTouching & direction) == OKDirection.None);
    }

    public bool IsStoppedTouching(OKDirection direction)
    {
      return ((wasTouching & direction) > OKDirection.None) && ((touching & direction) == OKDirection.None);
    }

    public void Hurt(int damage)
    {
      health = health - damage;
      if (health <= 0)
      {
        Kill();
      }
    }

    #region seperation

    public delegate bool OkObjectSeperationFunction(OkObject object1, OkObject object2);

    public static bool Seperate(OkObject object1, OkObject object2)
    {
      if (object1.immovable && object2.immovable)
        return false;
      return SeperateX(object1, object2) || SeperateY(object1, object2);
    }

    private const int kOverlapBias = 4;

    public static bool SeperateX(OkObject object1, OkObject object2)
    {
      // Can't seperate two immovable objects
      if (object1.immovable && object2.immovable)
        return false;

      //      if (object1 is OkTilemap)
      //      {
      //        return (object1 as OkTilemap).OverlapsWithCallback(object2, SeperateX);
      //      }
      //      else if (object2 is OkTilemap)
      //      {
      //        return (object2 as OkTilemap).OverlapsWithCallback(object1, SeperateX, true);
      //      }

      // First get the two object deltas
      int overlap = 0;
      int object1Delta = object1.x - object1.lastPosition.x;
      int object2Delta = object2.x - object2.lastPosition.x;

      if (object1Delta != object2Delta)
      {
        // Check if the X hulls actually overlap
        int object1DeltaAbs = OkU.Abs(object1Delta);
        int object2DeltaAbs = OkU.Abs(object2Delta);

        OkRect object1Rect = new OkRect(
          object1.x - ((object1Delta > 0) ? object1Delta : 0),
          object1.lastPosition.y,
          object1.width + object1DeltaAbs,
          object1.height
          );

        OkRect object2Rect = new OkRect(
          object2.x - ((object2Delta > 0) ? object2Delta : 0),
          object2.lastPosition.y,
          object2.width + object2DeltaAbs,
          object2.height
          );

        if ((object1Rect.x + object1Rect.width > object2Rect.x) && (object1Rect.x < object2Rect.x + object2Rect.width) &&
            (object1Rect.y + object1Rect.height > object2Rect.y) && (object1Rect.y < object2Rect.y + object2Rect.height))
        {
          int maxOverlap = object1DeltaAbs + object2DeltaAbs + kOverlapBias;

          // If they did overlap (and can), figure out how much and flip the corresponding flags.
          if (object1Delta > object2Delta)
          {
            overlap = object1.x + object1.width - object2.x;

            // TODO: Last two conditionals may be wrong.
            if (
              (overlap > maxOverlap)
              || ((object1.collisionEdges & OKDirection.Right) == OKDirection.None)
              || ((object2.collisionEdges & OKDirection.Left) == OKDirection.None))
            {
              overlap = 0;
            }
            else
            {
              object1.touching |= OKDirection.Right;
              object2.touching |= OKDirection.Left;
            }
          }
          else if (object1Delta < object2Delta)
          {
            overlap = object1.x - object2.width - object2.x;

            // TODO: Last two conditionals may be wrong.
            if (
                (-overlap > maxOverlap)
              || ((object1.collisionEdges & OKDirection.Left) == OKDirection.None)
              || ((object2.collisionEdges & OKDirection.Right) == OKDirection.None))
            {
              overlap = 0;
            }
            else
            {
              object1.touching |= OKDirection.Left;
              object2.touching |= OKDirection.Right;
            }
          }
        }
      }

      // Adjust their positions and velocities if there was any overlap.
      if (overlap != 0)
      {

        float object1Velocity = object1.velocityX;
        float object2Velocity = object2.velocityX;

        if (object1.immovable == false && object2.immovable == false)
        {
          overlap = overlap / 2;
          object1.x -= overlap;
          object2.x += overlap;

          float object1NewVelocity = Mathf.Sqrt((object2Velocity * object2Velocity * object2.mass) * object1.inverseMass) * (object2Velocity > 0.0f ? 1.0f : -1.0f);
          float object2NewVelocity = Mathf.Sqrt((object1Velocity * object1Velocity * object1.mass) * object2.inverseMass) * (object1Velocity > 0.0f ? 1.0f : -1.0f);
          float average = (object1NewVelocity + object2NewVelocity) * 0.5f;
          object1NewVelocity -= average;
          object2NewVelocity -= average;

          object1.velocityX = average + object1NewVelocity * object1.elasticity;
          object2.velocityX = average + object2NewVelocity * object2.elasticity;

        }
        else if (object1.immovable == true && object2.immovable == false)
        {
          object1.x -= overlap;
          object1.velocityX = object2Velocity - object1Velocity * object1.elasticity;
        }
        else if (object1.immovable == false && object2.immovable == true)
        {
          object2.x += overlap;
          object2.velocityX = object1Velocity - object2Velocity * object2.elasticity;
        }

        return true;

      }

      return false;
    }

    static bool SeperateY(OkObject object1, OkObject object2)
    {
      // Can't seperate two immovable objects
      if (object1.immovable && object2.immovable)
        return false;

      //      if (object1 is OkTilemap)
      //      {
      //        return (object1 as OkTilemap).OverlapsWithCallback(object2, SeperateY);
      //      }
      //      else if (object2 is OkTilemap)
      //      {
      //        return (object2 as OkTilemap).OverlapsWithCallback(object1, SeperateY, true);
      //      }

      // First get the two object deltas
      int overlap = 0;
      int object1Delta = object1.y - object1.lastPosition.y;
      int object2Delta = object2.y - object2.lastPosition.y;

      if (object1Delta != object2Delta)
      {
        // Check if the Y hulls actually overlap
        int object1DeltaAbs = OkU.Abs(object1Delta);
        int object2DeltaAbs = OkU.Abs(object2Delta);

        OkRect object1Rect = new OkRect(
          object1.x,
          object1.y - ((object1Delta > 0) ? object1Delta : 0),
          object1.width,
          object1.height + object1DeltaAbs
          );

        OkRect object2Rect = new OkRect(
          object2.x,
          object2.y - -((object2Delta > 0) ? object2Delta : 0),
          object2.width,
          object2.height + object2DeltaAbs
          );

        if ((object1Rect.x + object1Rect.width > object2Rect.x) &&
            (object1Rect.x < object2Rect.y + object2Rect.width) &&
            (object1Rect.y + object1Rect.height > object2Rect.y) &&
            (object1Rect.y < object2Rect.y + object2Rect.height))
        {
          int maxOverlap = object1DeltaAbs + object2DeltaAbs + kOverlapBias;

          // If they did overlap (and can), figure out how much and flip the corresponding flags.
          if (object1Delta > object2Delta)
          {
            overlap = object1.y + object1.width - object2.y;

            // TODO: Last two conditionals may be wrong.
            if (
              (overlap > maxOverlap)
              || ((object1.collisionEdges & OKDirection.Down) == OKDirection.None)
              || ((object2.collisionEdges & OKDirection.Up) == OKDirection.None))
            {
              overlap = 0;
            }
            else
            {
              object1.touching |= OKDirection.Down;
              object2.touching |= OKDirection.Up;
            }
          }
          else if (object1Delta < object2Delta)
          {
            overlap = object1.y - object2.width - object2.y;

            // TODO: Last two conditionals may be wrong.
            if (
                (-overlap > maxOverlap)
              || ((object1.collisionEdges & OKDirection.Up) == OKDirection.None)
              || ((object2.collisionEdges & OKDirection.Down) == OKDirection.None))
            {
              overlap = 0;
            }
            else
            {
              object1.touching |= OKDirection.Up;
              object2.touching |= OKDirection.Down;
            }
          }
        }
      }

      // Adjust their positions and velocities if there was any overlap.
      if (overlap != 0)
      {

        float object1Velocity = object1.velocityY;
        float object2Velocity = object2.velocityY;

        if (object1.immovable == false && object2.immovable == false)
        {
          overlap = overlap / 2;
          object1.y -= overlap;
          object2.y += overlap;

          float object1NewVelocity = Mathf.Sqrt((object2Velocity * object2Velocity * object2.mass) * object1.inverseMass) * (object2Velocity > 0.0f ? 1.0f : -1.0f);
          float object2NewVelocity = Mathf.Sqrt((object1Velocity * object1Velocity * object1.mass) * object2.inverseMass) * (object1Velocity > 0.0f ? 1.0f : -1.0f);
          float average = (object1NewVelocity + object2NewVelocity) * 0.5f;
          object1NewVelocity -= average;
          object2NewVelocity -= average;

          object1.velocityY = average + object1NewVelocity * object1.elasticity;
          object2.velocityY = average + object2NewVelocity * object2.elasticity;

        }
        else if (object1.immovable == true && object2.immovable == false)
        {
          object1.y -= overlap;
          object1.velocityY = object2Velocity - object1Velocity * object1.elasticity;
        }
        else if (object1.immovable == false && object2.immovable == true)
        {
          object2.y += overlap;
          object2.velocityY = object1Velocity - object2Velocity * object2.elasticity;
        }

        return true;

      }

      return false;
    }

    #endregion

    #region overlapping

    public virtual bool Overlaps(OkBasic objectOrGroup, bool inScreenSpace = false, OkCamera camera = null)
    {
      if (objectOrGroup is OkGroup)
      {
        bool results = false;
        OkGroup group = objectOrGroup as OkGroup;
        OkBasic child = group.firstChild;
        while (child != null)
        {
          OkBasic next = child.NextSibling;
          if (Overlaps(child, inScreenSpace, camera))
          {
            results = true;
          }
          child = next;
        }
        return results;
      }

      //      if (objectOrGroup is OkTilemap)
      //      {
      //        return (objectOrGroup as OkTilemap).Overlaps(this, inScreenSpace, camera);
      //      }

      if (objectOrGroup is OkObject == false)
        return false;

      OkObject @object = objectOrGroup as OkObject;

      if (!inScreenSpace)
      {
        return (@object.x + @object.width > x) &&
               (@object.x < x + width) &&
               (@object.y + @object.height > y) &&
               (@object.y < y + height);
      }

      if (camera == null)
      {
        camera = OkG.Camera;
      }

      OkPoint objectScreenPos = @object.GetScreenXY(camera);
      OkPoint screenPos = GetScreenXY(camera);

      return (objectScreenPos.x + @object.width > screenPos.x) &&
             (objectScreenPos.x < screenPos.x + width) &&
             (objectScreenPos.y + @object.height > screenPos.y) &&
             (objectScreenPos.y < screenPos.y + height);

    }

    public virtual bool OverlapsAt(int atX, int atY, OkBasic objectOrGroup, bool inScreenSpace = false, OkCamera camera = null)
    {
      if (objectOrGroup is OkGroup)
      {
        bool results = false;
        OkGroup group = objectOrGroup as OkGroup;
        OkBasic child = group.firstChild;
        while (child != null)
        {
          OkBasic next = child.NextSibling;
          if (OverlapsAt(atX, atY, child, inScreenSpace, camera))
          {
            results = true;
          }
          child = next;
        }
        return results;
      }

      //      if (objectOrGroup is OkTilemap)
      //      {
      //        OkTilemap tilemap = objectOrGroup as OkTilemap;
      //        return tilemap.OverlapsAt(tilemap.x - (atX - x), tilemap.y - (atY - y), this, inScreenSpace, camera);
      //      }

      if (objectOrGroup is OkObject == false)
        return false;

      OkObject @object = objectOrGroup as OkObject;

      if (!inScreenSpace)
      {
        return (@object.x + @object.width > atX) &&
               (@object.x < atX + width) &&
               (@object.y + @object.height > atY) &&
               (@object.y < atY + height);
      }

      if (camera == null)
      {
        camera = OkG.Camera;
      }

      OkPoint objectScreenPos = @object.GetScreenXY(camera);
      OkPoint screenPos = new OkPoint(atX, atY) - GetScreenXY(camera);

      return (objectScreenPos.x + @object.width > screenPos.x) &&
             (objectScreenPos.x < screenPos.x + width) &&
             (objectScreenPos.y + @object.height > screenPos.y) &&
             (objectScreenPos.y < screenPos.y + height);

    }

    public virtual bool OverlapsPoint(OkPoint point, bool inScreenSpace = false, OkCamera camera = null)
    {
      if (inScreenSpace == false)
      {
        return (point.x > x) && (point.x < x + width) && (point.y > y) && (point.y < y + height);
      }

      if (camera == null)
      {
        camera = OkG.Camera;
      }

      int cx = Mathf.RoundToInt(point.x - camera.scroll.x);
      int cy = Mathf.RoundToInt(point.y - camera.scroll.y);

      OkPoint cp = GetScreenXY(camera);

      return (cx > cp.x) && (cx < cp.x + width) && (cy > cp.y) && (cy < cp.y + height);
    }

    public bool OnScreen(OkCamera camera = null)
    {
      if (camera == null)
        camera = OkG.Camera;
      OkPoint point = GetScreenXY(camera);
      return (point.x + width > 0) && (point.x < camera.width) && (point.y + height > 0) && (point.y < camera.height);
    }


    #endregion

  }

}
