﻿/////////////////////////////////////////////////////////////////////
//
//	PdfFileWriter
//	PDF File Write C# Class Library.
//
//	TextBox
//  Support class for PdfContents class. Format text to fit column.
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
using System.Drawing;

namespace PdfFileWriter
{
/// <summary>
/// TextBoxLine class
/// </summary>
public class TextBoxLine
	{
	/// <summary>
	/// Gets line ascent.
	/// </summary>
	public double		Ascent {get; internal set;}

	/// <summary>
	/// Gets line descent.
	/// </summary>
	public double		Descent {get; internal set;}

	/// <summary>
	/// Line is end of paragraph.
	/// </summary>
	public bool		EndOfParagraph {get; internal set;}

	/// <summary>
	/// Gets array of line segments.
	/// </summary>
	public TextBoxSeg[]	SegArray {get; internal set;}

	/// <summary>
	/// Gets line height.
	/// </summary>
	public double LineHeight
		{
		get
			{
			return Ascent + Descent;
			}
		}

	/// <summary>
	/// TextBoxLine constructor.
	/// </summary>
	/// <param name="Ascent">Line ascent.</param>
	/// <param name="Descent">Line descent.</param>
	/// <param name="EndOfParagraph">Line is end of paragraph.</param>
	/// <param name="SegArray">Segments' array.</param>
	public TextBoxLine
			(
			double Ascent,
			double Descent,
			bool EndOfParagraph,
			TextBoxSeg[] SegArray
			)
		{
		this.Ascent = Ascent;
		this.Descent = Descent;
		this.EndOfParagraph = EndOfParagraph;
		this.SegArray = SegArray;
		return;
		}
	}

/// <summary>
/// TextBox line segment class
/// </summary>
public class TextBoxSeg
	{
	/// <summary>
	/// Gets segment font.
	/// </summary>
	public PdfFont		Font {get; internal set;}

	/// <summary>
	/// Gets segment font size.
	/// </summary>
	public double		FontSize {get; internal set;}

	/// <summary>
	/// Gets segment drawing style.
	/// </summary>
	public DrawStyle	DrawStyle {get; internal set;}

	/// <summary>
	/// Gets segment color.
	/// </summary>
	public Color		FontColor {get; internal set;}

	/// <summary>
	/// Gets segment width.
	/// </summary>
	public double		SegWidth {get; internal set;}

	/// <summary>
	/// Gets segment space character count.
	/// </summary>
	public int		SpaceCount {get; internal set;}

	/// <summary>
	/// Gets segment text.
	/// </summary>
	public string		Text {get; internal set;}

	/// <summary>
	/// Gets annotation action
	/// </summary>
	public AnnotAction	AnnotAction {get; internal set;}

	/// <summary>
	/// TextBox segment constructor.
	/// </summary>
	/// <param name="Font">Segment font.</param>
	/// <param name="FontSize">Segment font size.</param>
	/// <param name="DrawStyle">Segment drawing style.</param>
	/// <param name="FontColor">Segment color.</param>
	/// <param name="AnnotAction">Segment annotation action.</param>
	public TextBoxSeg
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			Color		FontColor,
			AnnotAction	AnnotAction
			)
		{
		this.Font = Font;
		this.FontSize = FontSize;
		this.DrawStyle = DrawStyle;
		this.FontColor = FontColor;
		Text = string.Empty;
		this.AnnotAction = AnnotAction;
		return;
		}

	/// <summary>
	/// TextBox segment copy constructor.
	/// </summary>
	/// <param name="CopySeg">Source TextBox segment.</param>
	internal TextBoxSeg
			(
			TextBoxSeg		CopySeg
			)
		{
		this.Font = CopySeg.Font;
		this.FontSize = CopySeg.FontSize;
		this.DrawStyle = CopySeg.DrawStyle;
		this.FontColor = CopySeg.FontColor;
		Text = string.Empty;
		this.AnnotAction = CopySeg.AnnotAction;
		return;
		}

	/// <summary>
	/// Compare two TextBox segments.
	/// </summary>
	/// <param name="Font">Segment font.</param>
	/// <param name="FontSize">Segment font size.</param>
	/// <param name="DrawStyle">Segment drawing style.</param>
	/// <param name="FontColor">Segment color.</param>
	/// <param name="AnnotAction">Segment annotation action.</param>
	/// <returns>Result</returns>
	internal bool IsEqual
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			Color		FontColor,
			AnnotAction AnnotAction
			)
		{
		// test all but annotation action
		return this.Font == Font && this.FontSize == FontSize && this.DrawStyle == DrawStyle &&
			this.FontColor == FontColor && AnnotAction.IsEqual(this.AnnotAction, AnnotAction);
		}
	}

/// <summary>
/// TextBox class
/// </summary>
/// <remarks>
/// <para>
/// <a href="http://www.codeproject.com/Articles/570682/PDF-File-Writer-Csharp-Class-Library-Version#DrawTextBox">For example of drawing TextBox see 3.12. Draw Text Box</a>
/// </para>
/// </remarks>
public class TextBox
	{
	/// <summary>
	/// Gets box width.
	/// </summary>
	public double BoxWidth {get; internal set;}

	/// <summary>
	/// Gets box height.
	/// </summary>
	public double BoxHeight {get; internal set;}

	/// <summary>
	/// Gets line count.
	/// </summary>
	public int LineCount {get { return LineArray.Count; }}

	/// <summary>
	/// Gets paragraph count.
	/// </summary>
	public int ParagraphCount {get; internal set;}

	/// <summary>
	/// Gets first line is indented.
	/// </summary>
	public double FirstLineIndent {get; internal set;}	

	private double LineBreakFactor;	 // should be >= 0.1 and <= 0.9
	private char PrevChar;
	private double LineWidth;
	private double LineBreakWidth;
	private int BreakSegIndex;
	private int BreakPtr;
	private double BreakWidth;
	private List<TextBoxSeg> SegArray;
	private List<TextBoxLine> LineArray;

	/// <summary>
	/// TextBox constructor
	/// </summary>
	/// <param name="BoxWidth">Box width.</param>
	/// <param name="FirstLineIndent">First line is indented.</param>
	/// <param name="LineBreakFactor">Line break factor.</param>
	public TextBox
			(
			double		BoxWidth,
			double		FirstLineIndent = 0.0,
			double		LineBreakFactor = 0.5
			)
		{
		if(BoxWidth <= 0.0) throw new ApplicationException("Box width must be greater than zero");
		this.BoxWidth = BoxWidth;
		this.FirstLineIndent = FirstLineIndent;
		if(LineBreakFactor < 0.1 || LineBreakFactor > 0.9) throw new ApplicationException("LineBreakFactor must be between 0.1 and 0.9");
		this.LineBreakFactor = LineBreakFactor;
		SegArray = new List<TextBoxSeg>();
		LineArray = new List<TextBoxLine>();
		Clear();
		return;
		}

	/// <summary>
	/// Clear TextBox
	/// </summary>
	public void Clear()
		{
		BoxHeight = 0.0;
		ParagraphCount = 0;
		PrevChar = ' ';
		LineWidth = 0.0;
		LineBreakWidth = 0.0;
		BreakSegIndex = 0;
		BreakPtr = 0;
		BreakWidth = 0;
		SegArray.Clear();
		LineArray.Clear();
		return;
		}

	/// <summary>
	/// Access TextBoxLine array.
	/// </summary>
	/// <param name="Index">Index</param>
	/// <returns>TextBoxLine</returns>
	public TextBoxLine this[int Index]
		{
		get
			{
			return LineArray[Index];
			}
		}

	/// <summary>
	/// TextBox height including extra line and paragraph space.
	/// </summary>
	/// <param name="LineExtraSpace">Extra line space.</param>
	/// <param name="ParagraphExtraSpace">Extra paragraph space.</param>
	/// <returns>Height</returns>
	public double BoxHeightExtra
			(
			double LineExtraSpace,
			double ParagraphExtraSpace
			)
		{
		double Height = BoxHeight;
		if(LineArray.Count > 1 && LineExtraSpace != 0.0) Height += LineExtraSpace * (LineArray.Count - 1);
		if(ParagraphCount > 1 && ParagraphExtraSpace != 0.0) Height += ParagraphExtraSpace * (ParagraphCount - 1);
		return Height;
		}

	/// <summary>
	/// Thwe height of the first LineCount lines including extra line and paragraph space.
	/// </summary>
	/// <param name="LineCount">The requested number of lines.</param>
	/// <param name="LineExtraSpace">Extra line space.</param>
	/// <param name="ParagraphExtraSpace">Extra paragraph space.</param>
	/// <returns>Height</returns>
	public double BoxHeightExtra
			(
			int LineCount,
			double LineExtraSpace,
			double ParagraphExtraSpace
			)
		{
		// textbox is empty
		if(LineArray.Count == 0) return 0.0;

		// line count is greater than available lines
		if(LineCount >= LineArray.Count)
			{
			return BoxHeightExtra(LineExtraSpace, ParagraphExtraSpace);
			}

		// calculate height for requested line count
		double Height = 0;
		for(int Index = 0;; Index++)
			{
			TextBoxLine Line = LineArray[Index];
			Height += Line.LineHeight;
			if(Index + 1 == LineCount) break;
			Height += LineExtraSpace;
			if(Line.EndOfParagraph) Height += ParagraphExtraSpace;
			}
		return Height;
		}

	/// <summary>
	/// The height of a block of lines within TextBox not excedding request height.
	/// </summary>
	/// <param name="LineStart">Start line</param>
	/// <param name="LineEnd">End line</param>
	/// <param name="RequestHeight">Requested height</param>
	/// <param name="LineExtraSpace">Extra line space.</param>
	/// <param name="ParagraphExtraSpace">Extra paragraph space.</param>
	/// <returns>Height</returns>
	/// <remarks>
	/// LineStart will be adjusted forward to skip blank lines. LineEnd 
	/// will be one after a non blank line. 
	/// </remarks>
	public double BoxHeightExtra
			(
			ref int	LineStart,
			out int	LineEnd,
			double		RequestHeight,
			double		LineExtraSpace,
			double		ParagraphExtraSpace
			)
		{
		// skip blank lines
		for(; LineStart < LineArray.Count; LineStart++)
			{
			TextBoxLine Line = LineArray[LineStart];
			if(!Line.EndOfParagraph || Line.SegArray.Length > 1 || Line.SegArray[0].SegWidth != 0) break;
			}

		// end of textbox
		if(LineStart >= LineArray.Count)
			{
			LineStart = LineEnd = LineArray.Count;
			return 0.0;
			}

		// calculate height for requested line count
		double Total = 0.0;
		double Height = 0.0;
		int End = LineEnd = LineStart;
		for(;;)
			{
			TextBoxLine Line = LineArray[End];
			if(Total + Line.LineHeight > RequestHeight) break;
			Total += Line.LineHeight;
			End++;
			if(!Line.EndOfParagraph || Line.SegArray.Length > 1 || Line.SegArray[0].SegWidth != 0)
				{
				LineEnd = End;
				Height = Total;
				}

			if(End == LineCount) break;

			Total += LineExtraSpace;
			if(Line.EndOfParagraph) Total += ParagraphExtraSpace;
			}

		return Height;
		}

	/// <summary>
	/// Longest line width
	/// </summary>
	public double LongestLineWidth
		{
		get
			{
			double MaxWidth = 0;
			foreach(TextBoxLine Line in LineArray)
				{
				double LineWidth = 0;
				foreach(TextBoxSeg Seg in Line.SegArray) LineWidth += Seg.SegWidth;
				if(LineWidth > MaxWidth) MaxWidth = LineWidth;
				}
			return MaxWidth;
			}
		}

	/// <summary>
	/// Terminate TextBox
	/// </summary>
	public void Terminate()
		{
		// terminate last line
		if(SegArray.Count != 0) AddLine(true);

		// remove trailing empty paragraphs
		for(int Index = LineArray.Count - 1; Index >= 0; Index--)
			{
			TextBoxLine Line = LineArray[Index];
			if(!Line.EndOfParagraph || Line.SegArray.Length > 1 || Line.SegArray[0].SegWidth != 0) break;
			BoxHeight -= Line.Ascent + Line.Descent;
			ParagraphCount--;
			LineArray.RemoveAt(Index);
			}

		// exit
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="Text">Text</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			string		Text
			)
		{
		AddText(Font, FontSize, DrawStyle.Normal, Color.Black, Text, (AnnotAction) null);
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="Text">Text</param>
	/// <param name="AnnotAction">Annotation action</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			string		Text,
			AnnotAction AnnotAction
			)
		{
		AddText(Font, FontSize, DrawStyle.Underline, Color.Blue, Text, AnnotAction);
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="Text">Text</param>
	/// <param name="WebLinkStr">Web link</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			string		Text,
			string		WebLinkStr
			)
		{
		AddText(Font, FontSize, DrawStyle.Underline, Color.Blue, Text, new AnnotWebLink(WebLinkStr));
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="DrawStyle">Drawing style</param>
	/// <param name="Text">Text</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			string		Text
			)
		{
		AddText(Font, FontSize, DrawStyle, Color.Empty, Text, (AnnotAction) null);
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="FontColor">Text color</param>
	/// <param name="Text">Text</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			Color		FontColor,
			string		Text
			)
		{
		AddText(Font, FontSize, DrawStyle.Normal, FontColor, Text, (AnnotAction) null);
		return;
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="DrawStyle">Drawing style</param>
	/// <param name="FontColor">Text color</param>
	/// <param name="Text">Web link (URL)</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			Color		FontColor,
			string		Text
			)
		{
		AddText(Font, FontSize, DrawStyle, FontColor, Text, (AnnotAction) null);
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="DrawStyle">Drawing style</param>
	/// <param name="FontColor">Text color</param>
	/// <param name="Text">Text</param>
	/// <param name="WebLinkStr">Web link (URL)</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			Color		FontColor,
			string		Text,
			string		WebLinkStr
			)
		{
		AddText(Font, FontSize, DrawStyle, FontColor, Text, new AnnotWebLink(WebLinkStr));
		}

	/// <summary>
	/// Add text to text box.
	/// </summary>
	/// <param name="Font">Font</param>
	/// <param name="FontSize">Font size</param>
	/// <param name="DrawStyle">Drawing style</param>
	/// <param name="FontColor">Text color</param>
	/// <param name="Text">Text</param>
	/// <param name="AnnotAction">Web link</param>
	public void AddText
			(
			PdfFont		Font,
			double		FontSize,
			DrawStyle	DrawStyle,
			Color		FontColor,
			string		Text,
			AnnotAction AnnotAction
			)
		{
		// text is null or empty
		if(string.IsNullOrEmpty(Text)) return;

		// create new text segment
		TextBoxSeg Seg;

		// segment array is empty or new segment is different than last one
		if(SegArray.Count == 0 || !SegArray[SegArray.Count - 1].IsEqual(Font, FontSize, DrawStyle, FontColor, AnnotAction))
			{
			Seg = new TextBoxSeg(Font, FontSize, DrawStyle, FontColor, AnnotAction);
			SegArray.Add(Seg);
			}

		// add new text to most recent text segment
		else
			{
			Seg = SegArray[SegArray.Count - 1];
			}

		// save text start pointer
		int TextStart = 0;

		// loop for characters
		for(int TextPtr = 0; TextPtr < Text.Length; TextPtr++)
			{
			// shortcut to current character
			char CurChar = Text[TextPtr];

			// end of paragraph
			if(CurChar == '\n' || CurChar == '\r')
				{
				// append text to current segemnt
				Seg.Text += Text.Substring(TextStart, TextPtr - TextStart);

				// test for new line after carriage return
				if(CurChar == '\r' && TextPtr + 1 < Text.Length && Text[TextPtr + 1] == '\n') TextPtr++;

				// move pointer to one after the eol
				TextStart = TextPtr + 1;

				// add line
				AddLine(true);

				// update last character
				PrevChar = ' ';

				// end of text
				if(TextPtr + 1 == Text.Length) return;

				// add new empty segment
				Seg = new TextBoxSeg(Font, FontSize, DrawStyle, FontColor, AnnotAction);
				SegArray.Add(Seg);
				continue;
				}

			// character width
			double CharWidth = Font.CharWidth(FontSize, Seg.DrawStyle, CurChar);

			// space
			if(CurChar == ' ')
				{
				// test for transition from non space to space
				// this is a potential line break point
				if(PrevChar != ' ')
					{
					// save potential line break information
					LineBreakWidth = LineWidth;
					BreakSegIndex = SegArray.Count - 1;
					BreakPtr = Seg.Text.Length + TextPtr - TextStart;
					BreakWidth = Seg.SegWidth;
					}

				// add to line width
				LineWidth += CharWidth;
				Seg.SegWidth += CharWidth;

				// update last character
				PrevChar = CurChar;
				continue;
				}

			// add current segment width and to overall line width
			Seg.SegWidth += CharWidth;
			LineWidth += CharWidth;

			// for next loop set last character
			PrevChar = CurChar;

			// box width
			double Width = BoxWidth;
			if(FirstLineIndent != 0 && (LineArray.Count == 0 || LineArray[LineArray.Count - 1].EndOfParagraph)) Width -= FirstLineIndent;

			// current line width is less than or equal box width
			if(LineWidth <= Width) continue;

			// append text to current segemnt
			Seg.Text += Text.Substring(TextStart, TextPtr - TextStart + 1);
			TextStart = TextPtr + 1;

			// there are no breaks in this line or last segment is too long
			if(LineBreakWidth < LineBreakFactor * Width)
				{
				BreakSegIndex = SegArray.Count - 1;
				BreakPtr = Seg.Text.Length - 1;
				BreakWidth = Seg.SegWidth - CharWidth;
				}

			// break line
			BreakLine();

			// add line up to break point
			AddLine(false);
			}

		// save text
		Seg.Text += Text.Substring(TextStart);

		// exit
		return;
		}

	private void BreakLine()
		{
		// break segment at line break seg index into two segments
		TextBoxSeg BreakSeg = SegArray[BreakSegIndex];

		// add extra segment to segment array
		if(BreakPtr != 0)
			{
			TextBoxSeg ExtraSeg = new TextBoxSeg(BreakSeg);
			ExtraSeg.SegWidth = BreakWidth;
			ExtraSeg.Text = BreakSeg.Text.Substring(0, BreakPtr);
			SegArray.Insert(BreakSegIndex, ExtraSeg);
			BreakSegIndex++;
			}

		// remove blanks from the area between the two sides of the segment
		for(; BreakPtr < BreakSeg.Text.Length && BreakSeg.Text[BreakPtr] == ' '; BreakPtr++);

		// save the area after the first line
		if(BreakPtr < BreakSeg.Text.Length)
			{
			BreakSeg.Text = BreakSeg.Text.Substring(BreakPtr);
			BreakSeg.SegWidth = BreakSeg.Font.TextWidth(BreakSeg.FontSize, BreakSeg.Text);
			}
		else
			{
			BreakSeg.Text = string.Empty;
			BreakSeg.SegWidth = 0.0;
			}
		BreakPtr = 0;
		BreakWidth = 0.0;
		return;
		}

	private void AddLine
			(
			bool EndOfParagraph
			)
		{
		// end of paragraph
		if(EndOfParagraph) BreakSegIndex = SegArray.Count;

		// test for box too narrow
		if(BreakSegIndex < 1) throw new ApplicationException("TextBox is too narrow.");

		// test for possible trailing blanks
		if(SegArray[BreakSegIndex - 1].Text.EndsWith(" "))
			{
			// remove trailing blanks
			while(BreakSegIndex > 0)
				{
				TextBoxSeg TempSeg = SegArray[BreakSegIndex - 1];
				TempSeg.Text = TempSeg.Text.TrimEnd(new char[] {' '});
				TempSeg.SegWidth = TempSeg.Font.TextWidth(TempSeg.FontSize, TempSeg.Text);
				if(TempSeg.Text.Length != 0 || BreakSegIndex == 1 && EndOfParagraph) break;
				BreakSegIndex--;
				SegArray.RemoveAt(BreakSegIndex);
				}
			}

		// test for abnormal case of a blank line and not end of paragraph
		if(BreakSegIndex > 0)
			{
			// allocate segment array
			TextBoxSeg[] LineSegArray = new TextBoxSeg[BreakSegIndex];

			// copy segments
			SegArray.CopyTo(0, LineSegArray, 0, BreakSegIndex);

			// line ascent and descent
			double LineAscent = 0;
			double LineDescent = 0;

			// loop for segments until line break segment index
			foreach(TextBoxSeg Seg in LineSegArray)
				{
				double Ascent = Seg.Font.AscentPlusLeading(Seg.FontSize);
				if(Ascent > LineAscent) LineAscent = Ascent;
				double Descent = Seg.Font.DescentPlusLeading(Seg.FontSize);
				if(Descent > LineDescent) LineDescent = Descent;

				int SpaceCount = 0;
				foreach(char Chr in Seg.Text) if(Chr == ' ') SpaceCount++;
				Seg.SpaceCount = SpaceCount;
				}

			// add line
			LineArray.Add(new TextBoxLine(LineAscent, LineDescent, EndOfParagraph, LineSegArray));

			// update column height
			BoxHeight += LineAscent + LineDescent;

			// update paragraph count
			if(EndOfParagraph) ParagraphCount++;

			// remove segments
			SegArray.RemoveRange(0, BreakSegIndex);
			}

		// switch to next line
		LineBreakWidth = 0.0;
		BreakSegIndex = 0;

		// new line width
		LineWidth = 0.0;
		foreach(TextBoxSeg Seg in SegArray) LineWidth += Seg.SegWidth;
		return;
		}
	}
}
