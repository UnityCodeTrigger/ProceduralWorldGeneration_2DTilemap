using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New NoiseMap", menuName = "ScriptableObjects/NoiseMap", order = 1)]
public class SO_Noisemap : ScriptableObject
{
    public int width, height;
    public Wave[] waves;

    [HideInInspector] public float[,] noise;
    [HideInInspector] public Texture2D noiseMapTexture;

    public void GenerateTexture()
    {
        noiseMapTexture = new Texture2D(width, height);
        noise = Noisemap.GetNoiseMap(width, height, 1, waves);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMapTexture.SetPixel(x,y,new Color(noise[x,y], noise[x, y], noise[x, y]));
            }
        }
        noiseMapTexture.Apply();
    }

}

#if UNITY_EDITOR
[CustomEditor(typeof(SO_Noisemap))]
public class Editor_NoiseMap : Editor
{
    public override void OnInspectorGUI()
    {
        SO_Noisemap noiseMap = (SO_Noisemap)target;
        if(GUILayout.Button("Generate Texture!"))
        {
            noiseMap.GenerateTexture();
        }
        GUILayout.Label(noiseMap.noiseMapTexture);
        DrawDefaultInspector();
    }
}
#endif