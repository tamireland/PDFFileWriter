﻿/////////////////////////////////////////////////////////////////////
//
//	PdfFileWriter
//	PDF File Write C# Class Library.
//
//	Barcode
//	Single diminsion barcode class.
//
//	Uzi Granot
//	Version: 1.0
//	Date: April 1, 2013
//	Copyright (C) 2013-2019 Uzi Granot. All Rights Reserved
//
//	PdfFileWriter C# class library and TestPdfFileWriter test/demo
//  application are free software.
//	They is distributed under the Code Project Open License (CPOL).
//	The document PdfFileWriterReadmeAndLicense.pdf contained within
//	the distribution specify the license agreement and other
//	conditions and notes. You must read this document and agree
//	with the conditions specified in order to use this software.
//
//	For version history please refer to PdfDocument.cs
//
/////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;

namespace PdfFileWriter
{
internal enum ValueType
	{
	Other,
	String,
	Dictionary
	}

/// <summary>
/// PDF dictionary class
/// </summary>
/// <remarks>
/// <para>
/// Dictionary key value pair class. Holds one key value pair.
/// </para>
/// </remarks>
public class PdfDictionary
	{
	internal List<PdfKeyValue> KeyValue;
	internal PdfObject Parent;
	internal PdfDocument Document;

	internal PdfDictionary
			(
			PdfObject	Parent
			)
		{
		KeyValue = new List<PdfKeyValue>();
		this.Parent = Parent;
		this.Document = Parent.Document;
		return;
		}

	internal PdfDictionary
			(
			PdfDocument Document
			)
		{
		KeyValue = new List<PdfKeyValue>();
		this.Document = Document;
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Find key value pair in dictionary.
	// return index number or -1 if not found.
	////////////////////////////////////////////////////////////////////
	
	internal int Find
			(
			string Key		// key (first character must be forward slash /)
			)
		{
		// look through the dictionary
		for(int Index = 0; Index < KeyValue.Count; Index++) if(KeyValue[Index].Key == Key) return Index;

		// not found
		return -1;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void Add
			(
			string Key,		// key (first character must be forward slash /)
			string Str
			)
		{
		Add(Key, Str, ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddName
			(
			string Key,		// key (first character must be forward slash /)
			string Str
			)
		{
		Add(Key, "/" + Str, ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddInteger
			(
			string			Key,		// key (first character must be forward slash /)
			int			Integer
			)
		{
		Add(Key, Integer.ToString(), ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddReal
			(
			string			Key,		// key (first character must be forward slash /)
			double			Real
			)
		{
		if(Math.Abs(Real) < 0.0001) Real = 0;
		Add(Key, string.Format(NFI.PeriodDecSep, "{0}", (float) Real), ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddReal
			(
			string			Key,		// key (first character must be forward slash /)
			float			Real
			)
		{
		if(Math.Abs(Real) < 0.0001) Real = 0;
		Add(Key, string.Format(NFI.PeriodDecSep, "{0}", Real), ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddRectangle
			(
			string			Key,		// key (first character must be forward slash /)
			PdfRectangle	Rect
			)
		{
		AddRectangle(Key, Rect.Left, Rect.Bottom, Rect.Right, Rect.Top);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddRectangle
			(
			string			Key,		// key (first character must be forward slash /)
			double			Left,
			double			Bottom,
			double			Right,
			double			Top
			)
		{
		Add(Key, string.Format(NFI.PeriodDecSep, "[{0} {1} {2} {3}]",
			Parent.ToPt(Left), Parent.ToPt(Bottom), Parent.ToPt(Right), Parent.ToPt(Top)));
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddBoolean
			(
			string			Key,		// key (first character must be forward slash /)
			bool			Bool
			)
		{
		Add(Key, Bool ? "true" : "false", ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddPdfString
			(
			string			Key,		// key (first character must be forward slash /)
			string			Str
			)
		{
		Add(Key, Str, ValueType.String);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is string format
	////////////////////////////////////////////////////////////////////
	
	internal void AddFormat
			(
			string			Key,		// key (first character must be forward slash /)
			string			FormatStr,
			params object[] FormatList
			)
		{
		Add(Key, string.Format(NFI.PeriodDecSep, FormatStr, FormatList), ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// The value is a reference to indirect object number.
	////////////////////////////////////////////////////////////////////
	
	internal void AddIndirectReference
			(
			string		Key,	// key (first character must be forward slash /)
			PdfObject	Obj		// PdfObject. The method creates an indirect reference "n 0 R" to the object.
			)
		{
		Add(Key, string.Format("{0} 0 R", Obj.ObjectNumber), ValueType.Other);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// If dictionary does not exist, create it.
	// If key is not found, add the pair as new entry.
	// If key is found, replace old pair with new one.
	////////////////////////////////////////////////////////////////////
	
	internal void AddDictionary
			(
			string			Key,		// key (first character must be forward slash /)
			PdfDictionary	Value		// value
			)
		{
		Add(Key, Value, ValueType.Dictionary);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Add key value pair to dictionary.
	// If dictionary does not exist, create it.
	// If key is not found, add the pair as new entry.
	// If key is found, replace old pair with new one.
	////////////////////////////////////////////////////////////////////
	
	private void Add
			(
			string		Key,		// key (first character must be forward slash /)
			object		Value,		// value
			ValueType	Type		// value type
			)
		{
		// search for existing key
		int Index = Find(Key);

		// not found - add new pair
		if(Index < 0) KeyValue.Add(new PdfKeyValue(Key, Value, Type));

		// found replace value
		else
			{
			KeyValue[Index].Value = Value;
			KeyValue[Index].Type = Type;
			}

		// exit
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Get dictionary value
	// Return string if key is found, null if not
	////////////////////////////////////////////////////////////////////
	
	internal PdfKeyValue GetValue
			(
			string		Key		// key (first character must be forward slash /)
			)
		{
		int Index = Find(Key);
		return Index >= 0 ? KeyValue[Index] : null;
		}

	////////////////////////////////////////////////////////////////////
	// Remove key value pair from dictionary
	////////////////////////////////////////////////////////////////////
	
	internal void Remove
			(
			string		Key		// key (first character must be forward slash /)
			)
		{
		int Index = Find(Key);
		if(Index >= 0) KeyValue.RemoveAt(Index);
		return;
		}

	////////////////////////////////////////////////////////////////////
	// Write dictionary to PDF file
	// Called from WriteObjectToPdfFile to output a dictionary
	////////////////////////////////////////////////////////////////////

	internal void WriteToPdfFile()
		{
		int EolMarker = 100;
		StringBuilder Str = new StringBuilder();

		WriteToPdfFile(Str, ref EolMarker);

		// write to pdf file
		Document.PdfFile.WriteString(Str);
		return;
		}
	
	private void WriteToPdfFile(StringBuilder Str, ref int EolMarker)
		{
		Str.Append("<<");

		// output dictionary
		foreach(PdfKeyValue KeyValueItem in KeyValue)
			{
			// add new line to cut down very long lines (just appearance)
			if(Str.Length > EolMarker)
				{
				Str.Append("\n");
				EolMarker = Str.Length + 100;
				}

			// append the key
			Str.Append(KeyValueItem.Key);

			// dictionary type
			switch(KeyValueItem.Type)
				{
				// dictionary
				case ValueType.Dictionary:
					((PdfDictionary) KeyValueItem.Value).WriteToPdfFile(Str, ref EolMarker);
					break;

				// PDF string special case
				case ValueType.String:
					Str.Append(Document.TextToPdfString((string) KeyValueItem.Value, Parent));
					break;

				// all other key value pairs
				default:
					// add one space between key and value unless value starts with a clear separator
					char FirstChar = ((string) KeyValueItem.Value)[0];
					if(FirstChar != '/' && FirstChar != '[' && FirstChar != '<' && FirstChar != '(') Str.Append(' ');

					// add value
					Str.Append(KeyValueItem.Value);
					break;
				}
			}

		// terminate dictionary
		Str.Append(">>\n");
		return;
		}
	}

internal class PdfKeyValue
	{
	internal string		Key;		// key first character must be forward slash ?
	internal object		Value;		// value associated with key
	internal ValueType	Type;		// value is a PDF string

	////////////////////////////////////////////////////////////////////
	// Constructor
	////////////////////////////////////////////////////////////////////

	internal PdfKeyValue
			(
			string		Key,		// key first character must be forward slash ?
			object		Value,		// value associated with key
			ValueType	Type		// value type
			)
		{
		if(Key[0] != '/') throw new ApplicationException("Dictionary key must start with /");
		this.Key = Key;
		this.Value = Value;
		this.Type = Type;
		return;
		}
	}
}
