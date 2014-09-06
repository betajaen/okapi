﻿using System;
using UnityEditor;
using UnityEngine;
using System.Collections;

namespace OkapiEditor
{

  public enum OkDeviceType
  {
    None,
    Screen,
    Mobile
  }

  public class OkResolution
  {
    public static GUIContent[] DisplayNames
    {
      get
      {
        if (msNames == null)
        {
          msNames = new GUIContent[msResolutions.Length];
          for (int i = 0; i < msResolutions.Length; i++)
          {
            msNames[i] = new GUIContent(msResolutions[i].displayName);
          }
        }
        return msNames;
      }
    }

    public static OkResolution[] Resolutions
    {
      get { return msResolutions; }
    }

    private static readonly OkResolution[] msResolutions =
    {
      new OkResolution(String.Empty, "480p", OkDeviceType.Screen, 640,480),
      new OkResolution(String.Empty, "720p", OkDeviceType.Screen, 1280,720),
      new OkResolution(String.Empty, "1080p", OkDeviceType.Screen, 1920,1080),
      new OkResolution(String.Empty, "Flash", OkDeviceType.Screen, 640, 480),
      new OkResolution(String.Empty, "Common Android and Apple", OkDeviceType.Mobile, 960, 640),
      new OkResolution("PC", "SVGA", OkDeviceType.Screen, 800, 600),
      new OkResolution("PC", "XGA", OkDeviceType.Screen, 1024, 768),
      new OkResolution("PC", "WXGA", OkDeviceType.Screen, 1280, 800),
      new OkResolution("Retro", "Amiga Hires Laced", OkDeviceType.Screen, 640, 512),
      new OkResolution("Retro", "Atari 400, 800", OkDeviceType.Screen, 320, 192),
      new OkResolution("Retro", "Nintendo Entertainment System", OkDeviceType.Screen, 256, 240),
      new OkResolution("Retro", "Sega Genesis", OkDeviceType.Screen, 320, 224),
      new OkResolution("Apple", "iPod Touch 1-3 Gen", OkDeviceType.Mobile, 480, 320),
      new OkResolution("Apple", "iPod Touch 4 Gen", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Apple", "iPhone 3G, 3Gs", OkDeviceType.Mobile, 480, 320),
      new OkResolution("Apple", "iPhone 4, 4S", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Apple", "iPhone 5, 5c, 5S", OkDeviceType.Mobile, 1136, 640),
      new OkResolution("Apple", "iPad 1-2 Gen, Mini", OkDeviceType.Mobile, 1024, 768),
      new OkResolution("Apple", "iPad 3-4 Gen, Air, Mini 2 Gen", OkDeviceType.Mobile, 2048, 1536),
      new OkResolution("Android", "3:2", OkDeviceType.Mobile, 960, 640),
      new OkResolution("Android", "4:3", OkDeviceType.Mobile, 1024, 768),
      new OkResolution("Android", "5:3", OkDeviceType.Mobile, 1280, 768),
      new OkResolution("Android", "16:9", OkDeviceType.Mobile, 1280, 720),
      new OkResolution("Android", "16:10", OkDeviceType.Mobile, 1650, 1050),
    };
    private static GUIContent[] msNames;

    protected OkResolution(String group_, String name_, OkDeviceType type_, int width_, int height_)
    {
      width = width_;
      height = height_;
      type = type_;
      name = name_;
      if (String.IsNullOrEmpty(group_))
      {
        if (String.IsNullOrEmpty(name) == false)
          displayName = String.Format("{0} x {1} - {2}", width, height, name);
        else
          displayName = String.Format("{0} x {1}", width, height);
      }
      else
      {
        if (String.IsNullOrEmpty(name) == false)
          displayName = String.Format("{0}/{1} x {2} - {3}", group_, width, height, name);
        else
          displayName = String.Format("{0}/{1} x {2}", group_, width, height);
      }
    }

    public readonly String displayName, name;
    public readonly int width, height;
    public OkDeviceType type;

  }

  public class OkSetupPreview
  {

    enum OkDeviceOrientation
    {
      Landscape,
      Portrait
    }

    private GUIStyle mBackgroundStyle, mToolbarStyle, mToolbarLabelStyle, mToolbarDropdownStyle;
    private OkSetupWizard mWizard;
    public int mDisplayWidth, mDisplayHeight;
    public OkDeviceType mDeviceType;
    private OkDeviceOrientation mDeviceOrientation;

    public OkSetupPreview(OkSetupWizard wizard)
    {
      mWizard = wizard;
      mDisplayWidth = OkSetup.gameResolutionWidth;
      mDisplayHeight = OkSetup.gameResolutionHeight;
      mDeviceType = OkDeviceType.None;
      Debug.Log("hello");
    }

    public void Inspect()
    {

      if (mBackgroundStyle == null)
      {
        mBackgroundStyle = new GUIStyle(EditorStyles.objectFieldThumb);
        mBackgroundStyle.padding = new RectOffset(1, 2, 1, 1);
        mToolbarStyle = new GUIStyle(EditorStyles.toolbar);
        mToolbarStyle.fixedHeight = 25;
        mToolbarStyle.padding = new RectOffset(0, 0, 0, -1);
        mToolbarStyle.margin = new RectOffset(0, 0, 0, -1);

        mToolbarDropdownStyle = new GUIStyle(EditorStyles.toolbarDropDown);
        mToolbarDropdownStyle.fixedHeight = 25;
        mToolbarLabelStyle = new GUIStyle(EditorStyles.toolbarDropDown);
        mToolbarLabelStyle.normal.background = null;
        mToolbarLabelStyle.fixedHeight = 25;

      }

      GUILayout.BeginVertical(mBackgroundStyle);

      GUILayout.BeginHorizontal(mToolbarStyle, GUILayout.Height(25));
      GUILayout.Label("Resolution", mToolbarLabelStyle, GUILayout.Height(25));

      if (mWizard.ResolutionField(ref mDisplayWidth, ref mDisplayHeight, 1))
      {

      }

      mDeviceOrientation = (OkDeviceOrientation)EditorGUILayout.EnumPopup(mDeviceOrientation, mToolbarDropdownStyle, GUILayout.Height(25));

      GUILayout.EndHorizontal();

      GUILayout.EndVertical();

      GUILayout.BeginVertical(mBackgroundStyle, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
      {
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        {
          GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
      }
      GUILayout.EndVertical();



    }

    public void Refresh()
    {

      mWizard.Repaint();
    }
  }

}
