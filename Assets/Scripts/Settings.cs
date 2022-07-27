using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
public class Settings : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private RenderPipelineAsset[] renderAssets;
    [SerializeField] private GameObject[] LightSets;
    [SerializeField] private ChessBoard chessBoard;

    public ScreenResolution[] resolutions;
    public int RenderIndex;
    public int ResolutionIndex;
    public static Settings Instance;
    private string path;

    private RenderPipelineAsset Render;
    private ScreenResolution Resolution;
    private bool FullScreen;

    const string file = "/settings.json";

    public void Awake()
    {
        Instance = this;
        path = Application.persistentDataPath + file;
        SetDefaultResolutions();

        if (File.Exists(Application.persistentDataPath + file))
            LoadSettings();
        else
            SetDefaults();
    }
    public void SetRender(int index)
    {
        RenderIndex = index;
        Render = renderAssets[index];
    }
    public void SetResolution(int index)
    {
        ResolutionIndex = index;
        Resolution = resolutions[index];
    }
    public void SetFullScreen(bool isFull)
    {
        FullScreen = isFull;
    }
    public void ApplySettings()
    {
        Screen.SetResolution(Resolution.Width, Resolution.Height, FullScreen);

        if (GraphicsSettings.renderPipelineAsset != Render)
        {
            GraphicsSettings.renderPipelineAsset = Render;
            chessBoard.SwitchRender();
        }

        for (int i = 0; i < LightSets.Length; i++)
        {
            LightSets[i].SetActive(i == RenderIndex);
        }
        SaveSettings();
    }
    private void SaveSettings()
    {
        string jsonString = JsonUtility.ToJson(new SettingsCollection(ResolutionIndex, RenderIndex, FullScreen));
        File.WriteAllText(path, jsonString);
    }
    private void LoadSettings()
    {
        string jsonString = File.ReadAllText(path);
        SettingsCollection settings = JsonUtility.FromJson<SettingsCollection>(jsonString);
        SetRender(settings.RenderIndex);
        SetResolution(settings.ResolutionIndex);
        FullScreen = settings.FullScreen;
        ApplySettings();
    }
    private void SetDefaults()
    {
        SetRender(0);
        SetResolution(0);
    }
    private void SetDefaultResolutions()
    {
        resolutions = new ScreenResolution[] {new ScreenResolution(640, 480),
                                              new ScreenResolution(1024, 768),
                                              new ScreenResolution(1280, 1024),
                                              new ScreenResolution(1366, 768),
                                              new ScreenResolution(1440, 900),
                                              new ScreenResolution(1600, 900),
                                              new ScreenResolution(1680, 1050),
                                              new ScreenResolution(1920, 1080)};
    }
}
