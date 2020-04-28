using UnityEngine;
using System.Collections;
using Den.Tools;

public class EncodeDecode
{
	public static int GetHash (int bx, int bz) //x and z could be negative
	{
		int aax = bx>=0? bx:-bx; int aaz = bz>=0? bz :-bz;
		return (bx>=0? 0x40000000:0)  |  (bz>=0? 0x20000000:0)  |  ((aax & 0x3FFF) << 14)  |  (aaz & 0x3FFF);
	}

	public static int EncodeInt (byte x, byte y, byte z, byte w)
	{
		//return ((y & 0xFF) << 32)  |  ((x & 0xFF) << 24)  |  ((z & 0xFF) << 16) | (w & 0xFF); //in case of ints that could be <255
		return (x << 24)  |  (y << 16)  |  (z << 8) | w;
	}

	public static byte[] DecodeInt (int i)
	{
		byte[] result = new byte[4];
		result[0] = (byte)((i >> 24) & 0xFF);
		result[1] = (byte)((i >> 16) & 0xFF);
		result[2] = (byte)((i >> 8) & 0xFF);
		result[3] = (byte)(i & 0xFF);
		return result;
	}

	public static float EncodeFloat (byte x, byte y, byte z, byte w)
	{
		int sign = x >> 3;
		int rem = x & 0x7;

		int exponent = y;
		int fraction = (z << 15) | (y << 7) | rem;
		
		//Debug.Log((1f * sign * Mathf.Pow(2,exponent-127) * fraction) + " exp:" + exponent.LogBinary() + " frac:" + fraction.LogBinary());

		Debug.Log(y + " " +  Mathf.Pow(2,exponent-127));

		return 1f * sign * Mathf.Pow(2,exponent) * fraction;
	}

	public static byte[] DecodeFloat (float f)
	{
		//int x = (sign << 3) | rem;

		byte[] result = new byte[4];
		return result;
	}
}
