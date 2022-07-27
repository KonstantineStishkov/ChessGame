using System;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
internal class SettingsCollection
{
    public int ResolutionIndex;
    public int RenderIndex;
    public bool FullScreen;

    public SettingsCollection(int resolutionIndex, int renderIndex, bool fullScreen)
    {
        ResolutionIndex = resolutionIndex;
        RenderIndex = renderIndex;
        FullScreen = fullScreen;
    }
    public SettingsCollection()
    {
    }
}