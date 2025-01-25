using System;
namespace ImGui;


public extension ImGui
{
	typealias char = char8;
	typealias uchar = uint8;
	typealias ushort = uint16;
	typealias short = int16;
	typealias size = uint64;
	typealias charPtr = char8*;

	public extension Vec2
	{
		public readonly static Vec2 Zero = .(0, 0);
		public readonly static Vec2 Ones = .(1, 1);
		public readonly static Vec2 OneZero = .(1, 0);
		public readonly static Vec2 ZeroOne = .(0, 1);
		public readonly static Vec2 NOneZero = .(-1, 0);
	}

	public extension Vec4
	{
		public readonly static Vec4 Zero = .(0, 0, 0, 0);
		public readonly static Vec4 Ones = .(1, 1, 1, 1);
	}

	[CRepr]
	public struct STB_TexteditState
	{
		int32 cursor;
		// position of the text cursor within the string

		int32 select_start;          // selection start point
		int32 select_end;
		// selection start and end point in characters; if equal, no selection.
		// note that start may be less than or greater than end (e.g. when
		// dragging the mouse, start is where the initial click was, and you
		// can drag in either direction)

		uchar insert_mode;
		// each textfield keeps its own insert mode state. to keep an app-wide
		// insert mode, copy this value in/out of the app state

		int32 row_count_per_page;
		// page size in number of row.
		// this value MUST be set to >0 for pageup or pagedown in multilines documents.

		uchar cursor_at_end_of_line; // not implemented yet
		uchar initialized;
		uchar has_preferred_x;
		uchar single_line;
		uchar padding1, padding2, padding3;
		float preferred_x; // this determines where the cursor up/down tries to seek to along x
		StbUndoState undostate;
	}

	public struct StbUndoRecord
	{
	   // private data
	   int32  @where;
	   int32 insert_length;
	   int32 delete_length;
	   int32 char_storage;
	}

	public struct StbUndoState
	{
		const int32 IMSTB_TEXTEDIT_UNDOSTATECOUNT = 99;
		const int32 IMSTB_TEXTEDIT_UNDOCHARCOUNT = 999;
		// private data
		StbUndoRecord[IMSTB_TEXTEDIT_UNDOSTATECOUNT] undo_rec;
		int32[IMSTB_TEXTEDIT_UNDOCHARCOUNT]          undo_char;
		short undo_point, redo_point;
		int undo_char_point, redo_char_point;
	}
}