namespace XPathExtentions;

using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml;
using UnityEngine.Scripting;
using System.Linq;
using System;

[Preserve]
[XmlPatchMethodsClass]
public static class XPathExtensions
{
    [XmlPatchMethod("multiply")]
    public static int MultiplyByXPath(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod = null)
    {
        List<XObject> list;
        if (!_targetFile.GetXpathResults(_xpath, out list))
        {
            return 0;
        }

        foreach (XObject xobject in list)
        {
            IXmlLineInfo xmlLineInfo = xobject;
            XElement xelement = xobject as XElement;
            if (xelement == null)
            {
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
                string[] multiplierStrings = xtext.Value.Split(',');
                float[] floats = new float[stringValues.Length];

                for ( int i = 0; i < stringValues.Length; i++ )
                {
                    float originalValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, stringValues[i]);
                    float floatValue;

                    if (multiplierStrings.Length > i)
                    {
                        string multiplierString = multiplierStrings[i];
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }
                    else
                    {
                        string multiplierString = multiplierStrings[multiplierStrings.Length - 1];
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }

                    floats[i] = originalValue * floatValue;
                }

                string newValue = floats.Length == 1 ? floats[0].ToCultureInvariantString() : string.Join(',', floats);
                xattribute.Value = newValue;

                if (_patchingMod != null)
                {
                    xattribute.Parent?.AddFirst(new XComment(string.Format("Attribute \"{0}\" replaced by: \"{1}\", with value of {2}", xattribute.Name, _patchingMod.Name, xattribute.Value)));
                }
            }
            else
            {

                throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Multiplying attribute ({0}, line {1} at pos {2}) x {3} not supported yet?",
                    xelement.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));
                //xelement.ReplaceNodes(_patchSourceElement.Nodes());
                //if (_patchingMod != null)
                //{
                //    XComment xcomment2 = new XComment("Element contents replaced by: \"" + _patchingMod.Name + "\"");
                //    xelement.AddFirst(xcomment2);
                //}
            }
        }
        return _targetFile.ClearXpathResults();
    }

    [XmlPatchMethod("divide")]
    public static int DivideByXPath(XmlFile _targetFile, string _xpath, XElement _patchSourceElement, XmlFile _patchFile, Mod _patchingMod = null)
    {
        List<XObject> list;
        if (!_targetFile.GetXpathResults(_xpath, out list))
        {
            return 0;
        }

        foreach (XObject xobject in list)
        {
            IXmlLineInfo xmlLineInfo = xobject;
            XElement xelement = xobject as XElement;
            if (xelement == null)
            {
                XAttribute xattribute = xobject as XAttribute ?? throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Matched node type ({0}, line {1} at pos {2}) can not be set", xobject.NodeType.ToStringCached<XmlNodeType>(), xmlLineInfo.LineNumber, xmlLineInfo.LinePosition));


                if (!_patchSourceElement.Nodes().Any<XNode>())
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
                string[] multiplierStrings = xtext.Value.Split(',');
                float[] floats = new float[stringValues.Length];

                for (int i = 0; i < stringValues.Length; i++)
                {
                    float originalValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, stringValues[i]);
                    float floatValue;

                    if (multiplierStrings.Length > i)
                    {
                        string multiplierString = multiplierStrings[i];
                        floatValue = ThrowIfFloatParseFails(_patchSourceElement, xmlLineInfo, xattribute, multiplierString);
                    }
                    else
                    {
                        string multiplierString = multiplierStrings[multiplierStrings.Length - 1];
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

                    floats[i] = originalValue / floatValue;
                }

                string newValue = floats.Length == 1 ? floats[0].ToCultureInvariantString() : string.Join(',', floats);
                xattribute.Value = newValue;

                if (_patchingMod != null)
                {
                    xattribute.Parent?.AddFirst(new XComment(string.Format("Attribute \"{0}\" replaced by: \"{1}\", with value of {2}", xattribute.Name, _patchingMod.Name, xattribute.Value)));
                }
            }
            else
            {
                throw new XmlPatchException(_patchSourceElement, "DivideByXPath", string.Format("Dividing attribute ({0}, line {1} at pos {2}) x {3} not supported yet?",
                    xelement.GetXPath(),
                    xmlLineInfo.LineNumber,
                    xmlLineInfo.LinePosition,
                    _patchSourceElement.FirstNode.NodeType.ToStringCached<XmlNodeType>()
                ));
                //xelement.ReplaceNodes(_patchSourceElement.Nodes());
                //if (_patchingMod != null)
                //{
                //    XComment xcomment2 = new XComment("Element contents replaced by: \"" + _patchingMod.Name + "\"");
                //    xelement.AddFirst(xcomment2);
                //}
            }
        }
        return _targetFile.ClearXpathResults();
    }

    private static float ThrowIfFloatParseFails(XElement _patchSourceElement, IXmlLineInfo xmlLineInfo, XAttribute xattribute, string stringValue)
    {
        if (!float.TryParse(stringValue.Trim(), out float floatValue))
        {
            throw new XmlPatchException(_patchSourceElement, "MultiplyByXPath", string.Format("Multiplying attribute ({0}, line {1} at pos {2}) failed to parse {3} as float!",
                xattribute.GetXPath(),
                xmlLineInfo.LineNumber,
                xmlLineInfo.LinePosition,
                stringValue
            ));
        }

        return floatValue;
    }
}
