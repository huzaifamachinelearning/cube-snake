using UnityEngine;
using TMPro;
using System;

public class Cube : MonoBehaviour
{
    public int number = 2;

    [Header("Text")]
    public TextMeshPro text;

    [Header("Cube Renderer")]
    public Renderer cubeRenderer; // Takes Cube(mesh Render) that is the default Cube in Unity. Mesh is the collection of vertices and edges of a shape

    private Material uniqueMaterial;

    [Header("Colors")]
    public Color color2 = Color.red;
    public Color color4 = new Color(0f, 0f, 0.5f); // Dark blue
    public Color color8 = Color.green;
    public Color color16 = new Color(0.76f, 0.6f, 0.42f); // Camel (brownish tan)
    public Color color32 = new Color(1f, 0.5f, 0f); // Orange
    public Color color64 = new Color(1f, 1f, 0.8f); // Light yellow
    public Color color128 = new Color(0.8f, 0.8f, 0f); // Dark yellow
    public Color color256 = new Color(0.5f, 0.7f, 1f); // Light blue
    public Color color512 = new Color(0.5f, 0f, 0.5f); // Purple
    public Color color1024 = new Color(1f, 0f, 1f); // Magenta
    public Color color2048 = new Color(0f, 1f, 1f); // Cyan
    public Color color4096 = new Color(1f, 0.65f, 0f); // Gold
    public Color color8192 = new Color(0.13f, 0.55f, 0.13f); // Forest green
    public Color color16384 = new Color(0.5f, 0f, 0f); // Maroon
    public Color color32768 = new Color(0.6f, 0.4f, 0.2f); // Chocolate
    public Color color65536 = new Color(0.75f, 0f, 0.75f); // Medium purple

    void Start() // Unity uses reflection to call specific methods by name these include "Start()", "Update()" , Awake() 
    // the Start() function is called only once if the Script is enabled(by enabled we mean the check on the left side when we add a script to material) on the object
    {
        // Create unique material instance
        if (cubeRenderer != null && cubeRenderer.sharedMaterial != null)
        {
            uniqueMaterial = new Material(cubeRenderer.sharedMaterial);
            cubeRenderer.material = uniqueMaterial;
        }
        UpdateVisual();
    }

    public void SetNumber(int value)
    {
        number = value;
        UpdateVisual();
    }

    public void AddNumber(int value)
    {
        number += value;
        UpdateVisual();
    }

    public void UpdateVisual()
    {
        // Update text
        if (text != null)
        {
            text.text = number.ToString();

            // Prevent text wrapping for 2+ digit numbers
            text.enableAutoSizing = false;
            text.alignment = TextAlignmentOptions.Center;

            // Ensure rect transform is large enough
            RectTransform rectTransform = text.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = new Vector2(5, 5);
            }
        }

        // Update cube color
        if (uniqueMaterial != null)
        {
            uniqueMaterial.color = GetColor();
        }
    }

    Color GetColor()
    {
        switch (number)
        {
            case 2:
                return color2;
            case 4:
                return color4;
            case 8:
                return color8;
            case 16:
                return color16;
            case 32:
                return color32;
            case 64:
                return color64;
            case 128:
                return color128;
            case 256:
                return color256;
            case 512:
                return color512;
            case 1024:
                return color1024;
            case 2048:
                return color2048;
            case 4096:
                return color4096;
            case 8192:
                return color8192;
            case 16384:
                return color16384;
            case 32768:
                return color32768;
            case 65536:
                return color65536;
            default:
                return color2; // Default to red (color2)
        }
    }
}