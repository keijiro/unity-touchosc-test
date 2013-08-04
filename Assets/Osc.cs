// Osc.cs - Extremely minimal OSC implementation
// Copyright (C) 2011 by Artem Baguinski.
// For details, see https://github.com/artm/Osc.cs

using System;
using System.Text;
using System.Collections;
using System.Net;

public class Osc
{
	public static object[] ToArray(byte[] data)
	{
		int offset = 0;
		string path = Osc.ReadString(data,ref offset);
		string types = Osc.ReadString(data,ref offset);
		object[] res = new object[types.Length];
		res[0] = path;
		int i = 1;
		foreach(char t in types.Substring(1)) {
			switch(t) {
			case 'i':
				res[i++] = ReadInt32(data, ref offset);
				break;
			case 'f':
				res[i++] = ReadFloat32(data, ref offset);
				break;
			case 's':
				res[i++] = ReadString(data, ref offset);
				break;
			default:
				throw new NotImplementedException("OSC type '" + t + "' not implemented");
			}
		}
		return res;
	}

	public static byte[] FromArray(params object[] args)
	{
		StringBuilder types = new StringBuilder(args.Length);
		int size = 0;

		foreach(object o in args) {
			types.Append(OscTypeOf(o));
			size += OscSizeOf(o);
		}
		types[0] = ',';
		size += OscSizeOf(types.Length);

		byte[] data = new byte[size];
		int offset = 0;
		Put(args[0] as String, data, ref offset);
		Put(types.ToString(), data, ref offset);
		for(int i = 1; i<args.Length; i++) {
			Put(args[i], data, ref offset);
		}
		return data;
	}

	#region details
	static String ReadString(byte[] data, ref int offset)
	{
		int count = 0;
		for(; (offset+count)<data.Length && data[offset+count]!=0;count++);
		String result = Encoding.UTF8.GetString(data, offset, count);
		offset += PaddedLength(count);
		return result;
	}

	static int ReadInt32(byte[] data, ref int offset)
	{
		int res = IPAddress.NetworkToHostOrder( BitConverter.ToInt32(data, offset) );
		offset += 4;
		return res;
	}

	static float ReadFloat32(byte[] data, ref int offset)
	{
		float res;
		if (!BitConverter.IsLittleEndian)
			res = BitConverter.ToSingle(data, offset);
		else {
			byte[] sub = new byte[4];
			Array.Copy(data, offset, sub, 0, 4);
			Array.Reverse(sub);
			res = BitConverter.ToSingle(sub, 0);
		}
		offset += 4;
		return res;
	}

	static int PaddedLength(int l)
	{
		return l + 4 - l % 4;
	}

	static int PaddedLength(String s)
	{
		return PaddedLength(s.Length);
	}

	static int OscSizeOf(object o) {
		switch (o.GetType().Name) {
		case "Int32":
		case "Float32":
			return 4;
		case "String":
			return PaddedLength(o as String);
		default:
			throw new NotImplementedException(
			             String.Format("Conversion from {0} to OSC types not implemented", o.GetType().Name));
		}
	}

	static char OscTypeOf(object o) {
		switch (o.GetType().Name) {
		case "Int32":
			return 'i';
		case "Float32":
			return 'f';
		case "String":
			return 's';
		default:
			throw new NotImplementedException(
			             String.Format("Conversion from {0} to OSC types not implemented", o.GetType().Name));
		}
	}

	static void Put(object o, byte[] data, ref int offset)
	{
		switch (o.GetType().Name) {
		case "Int32":
			Array.Copy(BitConverter.GetBytes( IPAddress.HostToNetworkOrder((int)o)), 0, data, offset, 4);
			offset += 4;
			break;
		case "Float32":
			byte[] tmp = BitConverter.GetBytes((float)o);
			if (BitConverter.IsLittleEndian)
				Array.Reverse(tmp);
			Array.Copy(tmp, 0, data, offset, 4);
			offset += 4;
			break;
		case "String":
			Put(o as String, data, ref offset);
			break;
		default:
			throw new NotImplementedException(
			             String.Format("Conversion from {0} to OSC types not implemented", o.GetType().Name));
		}
	}
	static void Put(String s, byte[] data, ref int offset)
	{
		Encoding.ASCII.GetBytes(s,0,s.Length,data,offset);
		offset += PaddedLength(s);
	}
	#endregion
}

