using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LatexGen : MonoBehaviour
{
    public struct MatrixItem
    {
        public string Main;
        public string Top;
        public string Bottom;
        public string Denominator;
        public bool Negative;
    }

    public Slider sliderX;
    public Slider sliderY;
    public Button btnGen;
    public Button btnClear;
    public Transform item;
    public Transform nodeMatrix;
    private int _x;
    private int _y;

    private void Awake()
    {
        _x = Convert.ToInt32(sliderX.value);
        _y = Convert.ToInt32(sliderY.value);
        sliderX.onValueChanged.AddListener(SliderXValueChanged);
        sliderY.onValueChanged.AddListener(SliderYValueChanged);
        btnGen.onClick.AddListener(GenMatrix);
        btnClear.onClick.AddListener(ClearMatrix);
        UpdateUI();
    }

    private void SliderXValueChanged(float value)
    {
        _x = Convert.ToInt32(value);
        UpdateUI();
    }
    
    private void SliderYValueChanged(float value)
    {
        _y = Convert.ToInt32(value);
        UpdateUI();
    }

    private void UpdateUI()
    {
        nodeMatrix.GetComponent<RectTransform>().sizeDelta = new Vector2(_x * 100 + (_x - 1) * 10, _y * 100 + (_y - 1) * 10);
        var trCount = nodeMatrix.childCount;
        if (trCount < _x * _y)
        {
            for (var i = 0; i < _x * _y - trCount; i++)
            {
                Instantiate(item, nodeMatrix);
            }
        }
        else
        {
            for (var i = 0; i < trCount - _x * _y; i++)
            {
                DestroyImmediate(nodeMatrix.GetChild(0).gameObject);
            }
        }
    }
    
    private void ClearMatrix()
    {
        for (var i = 0; i < _x * _y; i++)
        {
            CleatTr(nodeMatrix.GetChild(i));
        }
    }

    private void GenMatrix()
    {
        var str = "\\begin{bmatrix}";
        for (var i = 0; i < _x * _y; i++)
        {
            str += GenSingleItem(GetByTr(nodeMatrix.GetChild(i)));
            if (i != 0 && (i + 1) % _x == 0)
            {
                str += "\\\\";
            }
            else if (i != _x * _y - 1)
            {
                str += "&";
            }
        }

        str += "\\end{bmatrix}";
        GUIUtility.systemCopyBuffer = str;
    }

    private MatrixItem GetByTr(Transform tr)
    {
        var matrixItem = new MatrixItem
        {
            Main = tr.Find("Main").GetComponent<TMP_InputField>().text,
            Top = tr.Find("Top").GetComponent<TMP_InputField>().text,
            Bottom = tr.Find("Bottom").GetComponent<TMP_InputField>().text, 
            Denominator = tr.Find("Deno").GetComponent<TMP_InputField>().text,    
            Negative = tr.Find("Negative").GetComponent<Toggle>().isOn
        };
        return matrixItem;
    }

    private void CleatTr(Transform tr)
    {
        tr.Find("Main").GetComponent<TMP_InputField>().text = "";
        tr.Find("Top").GetComponent<TMP_InputField>().text = "";
        tr.Find("Bottom").GetComponent<TMP_InputField>().text = "";
        tr.Find("Deno").GetComponent<TMP_InputField>().text = "";
        tr.Find("Negative").GetComponent<Toggle>().isOn = false;
    }
    
    private string GenSingleItem(MatrixItem item)
    {
        var str = "";
        if (item.Negative)
        {
            str += "-";
        }

        if (!string.IsNullOrEmpty(item.Denominator))
        {
            str += "\\frac{" + GetMain(item.Main) + "}{" + item.Denominator + "}";
        }
        else
        {
            str += GetMain(item.Main);
        }
        if (!string.IsNullOrEmpty(item.Bottom))
        {
            str += "_{"+item.Bottom+"}";
        }
        if (!string.IsNullOrEmpty(item.Top))
        {
            str += "^{"+item.Top+"}";
        }
        
        return str;
    }

    private string GetMain(string str)
    {
        return string.IsNullOrEmpty(str) ? "0" : str;
    }
}
