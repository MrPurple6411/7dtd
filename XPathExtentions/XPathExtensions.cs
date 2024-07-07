namespace XPathExtentions;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using UnityEngine.Scripting;
using System.Linq;
using System;
using System.Text;
using NCalc;

[Preserve]
[XmlPatchMethodsClass]
public static class XPathExtensions
{
    [XmlPatchMethod("multiply")]
    public static int MultiplyByXPath(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod = null)
    {
        if (!_targetFile.GetXpathResults(_xpath, out List<XObject> list))
        {
            return 0;
        }

        foreach (XObject xobject in list)
        {
            IXmlLineInfo xmlLineInfo = xobject;
            if (xobject is XElement xelement)
            {
                throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Multiplying ({0}, line {1} at pos {2}) x {3} not supported.",
                    xelement.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));
            }

            XAttribute xattribute = xobject as XAttribute ?? throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Matched node type ({0}, line {1} at pos {2}) can not be set", xobject.NodeType.ToStringCached<XmlNodeType>(), xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));

            if (!_patchSourceElement.Nodes().Any<XNode>())
            {
                throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Multiplying value ({0}, line {1} at pos {2}) without any replacement text given as child element",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition
                    ));
            }

            XText xtext = _patchSourceElement.FirstNode as XText ?? throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Multiplying attribute ({0}, line {1} at pos {2}) from a non-text source: {3}",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));

            string originalString = xattribute.Value;
            string[] stringValues = originalString.Split(',');
            string[] modifierStrings = xtext.Value.Split(',');
            string[] finalStrings = new string[stringValues.Length];

            for (int i = 0; i < stringValues.Length; i++)
            {
                string[] strings = stringValues[i].Split('-');
                StringBuilder stringBuilder = new();

                for (int j = 0; j < strings.Length; j++)
                {
                    string currentString = strings[j];
                    float originalValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, currentString);
                    float floatValue;

                    if (modifierStrings.Length > i)
                    {
                        string multiplierString = modifierStrings[i];
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }
                    else
                    {
                        string multiplierString = modifierStrings.Last();
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }

                    stringBuilder.Append($"{originalValue * floatValue}");
                    if (i < strings.Length - 1)
                        stringBuilder.Append("-");
                }

                finalStrings[i] = stringBuilder.ToString();
            }

            string newValue = finalStrings.Length == 1 ? finalStrings[0] : string.Join(",", finalStrings);
            xattribute.Value = newValue;

            if (_patchingMod != null)
            {
                xattribute.Parent?.AddFirst(new XComment(string.Format("Attribute \"{0}\" replaced by: \"{1}\", with value of {2}", xattribute.Name, _patchingMod.Name, xattribute.Value)));
            }
        }
        return _targetFile.ClearXpathResults();
    }

    [XmlPatchMethod("divide")]
    public static int DivideByXPath(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod = null)
    {
        if (!_targetFile.GetXpathResults(_xpath, out List<XObject> list))
        {
            return 0;
        }

        foreach (XObject xobject in list)
        {
            IXmlLineInfo xmlLineInfo = xobject;
            if (xobject is XElement xelement)
            {
                throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Dividing ({0}, line {1} at pos {2}) x {3} not supported.",
                    xelement.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));
            }

            XAttribute xattribute = xobject as XAttribute ?? throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Matched node type ({0}, line {1} at pos {2}) can not be set", xobject.NodeType.ToStringCached<XmlNodeType>(), xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));

            if (!_patchSourceElement.Nodes().Any())
            {
                throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Dividing value ({0}, line {1} at pos {2}) without any replacement text given as child element",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition
                    ));
            }

            XText xtext = _patchSourceElement.FirstNode as XText ?? throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Dividing attribute ({0}, line {1} at pos {2}) from a non-text source: {3}",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));

            string originalString = xattribute.Value;
            string[] stringValues = originalString.Split(',');
            string[] modifierStrings = xtext.Value.Split(',');
            string[] finalStrings = new string[stringValues.Length];

            for (int i = 0; i < stringValues.Length; i++)
            {
                string[] strings = stringValues[i].Split('-');
                StringBuilder stringBuilder = new();

                for (int j = 0; j < strings.Length; j++)
                {
                    string currentString = strings[j];
                    float originalValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, currentString);
                    float floatValue;

                    if (modifierStrings.Length > i)
                    {
                        string multiplierString = modifierStrings[i];
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }
                    else
                    {
                        string multiplierString = modifierStrings.Last();
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }

                    if (Math.Abs(floatValue) < 0.0000001)
                    {
                        throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Dividing by 0 at ({0}, line {1} at pos {2})",
                            xattribute.GetXPath(),
                            xmlLineInfo.LineNumber,
                            xmlLineInfo.LinePosition
                            ));
                    }

                    stringBuilder.Append($"{originalValue * floatValue}");
                    if (i < strings.Length - 1)
                        stringBuilder.Append("-");
                }

                finalStrings[i] = stringBuilder.ToString();
            }

            string newValue = finalStrings.Length == 1 ? finalStrings[0] : string.Join(",", finalStrings);
            xattribute.Value = newValue;

            if (_patchingMod != null)
            {
                xattribute.Parent?.AddFirst(new XComment(string.Format("Attribute \"{0}\" replaced by: \"{1}\", with value of {2}", xattribute.Name, _patchingMod.Name, xattribute.Value)));
            }
        }
        return _targetFile.ClearXpathResults();
    }
    
    [XmlPatchMethod("ncalc")]
    public static int NCalcByXPath(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod = null)
    {
        if (!_targetFile.GetXpathResults(_xpath, out List<XObject> list))
        {
            return 0;
        }

        foreach (XObject xobject in list)
        {
            IXmlLineInfo xmlLineInfo = xobject;
            if (xobject is XElement xelement)
            {
                throw new XmlPatchException(_patchSourceElement, "NCalcByXPath", string.Format("NCalcByXPath attribute ({0}, line {1} at pos {2}) x {3} not supported.",
                    xelement.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));
            }

            XAttribute xattribute = xobject as XAttribute ?? throw new XmlPatchException(_patchSourceElement, "NCalcByXPath", string.Format("Matched node type ({0}, line {1} at pos {2}) can not be set", xobject.NodeType.ToStringCached<XmlNodeType>(), xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));

            if (!_patchSourceElement.Nodes().Any())
            {
                throw new XmlPatchException(_patchSourceElement, "NCalcByXPath", string.Format("NCalcByXPath value ({0}, line {1} at pos {2}) without any replacement text given as child element",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition
                    ));
            }

            Log.Out($"child nodes = {_patchSourceElement.Nodes().Count()}");

            XText xtext = _patchSourceElement.FirstNode as XText ?? throw new XmlPatchException(_patchSourceElement, "NCalcByXPath", string.Format("NCalcByXPath attribute ({0}, line {1} at pos {2}) from a non-text source: {3}",
                    xattribute.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));

            string originalString = xattribute.Value;
            string newValue = xtext.Value;

            if (newValue.Contains("{0}"))
            {
                string[] stringValues = originalString.Split(',');
                string[] finalStrings = new string[stringValues.Length];

                for (int i = 0; i < stringValues.Length; i++)
                {
                    string[] strings = stringValues[i].Split('-');
                    StringBuilder stringBuilder = new();

                    for (int j = 0; j < strings.Length; j++)
                    {
                        string expression = string.Format(newValue, strings[j]);
                        string newString = new Expression(expression).Evaluate().ToString();

                        Log.Error($"{expression}= {newString}");

                        stringBuilder.Append(newString);
                        if (i < strings.Length - 1)
                            stringBuilder.Append("-");
                    }

                    finalStrings[i] = stringBuilder.ToString();
                }

                newValue = finalStrings.Length == 1 ? finalStrings[0] : string.Join(",", finalStrings);
                Log.Error($"newValue= {newValue}");
            }
            else
            {
                newValue = new Expression(newValue).Evaluate().ToString();
            }

            xattribute.Value = newValue;

            if (_patchingMod != null)
            {
                xattribute.Parent?.AddFirst(new XComment(string.Format("Attribute \"{0}\" replaced by: \"{1}\", with value of {2}", xattribute.Name, _patchingMod.Name, xattribute.Value)));
            }
        }
        return _targetFile.ClearXpathResults();
    }

    private static float ThrowIfFloatParseFails(XElement _patchSourceElement, IXmlLineInfo xmlLineInfo, XAttribute xattribute, string stringValue)
    {
        return float.TryParse(stringValue.Trim(), out float floatValue)
            ? floatValue
            : throw new XmlPatchException(_patchSourceElement, "ThrowIfFloatParseFails", string.Format("({0}, line {1} at pos {2}) failed to parse {3} as float!",
                xattribute.GetXPath(),
                xmlLineInfo.LineNumber,
                xmlLineInfo.LinePosition,
                stringValue
            ));
    }
}
