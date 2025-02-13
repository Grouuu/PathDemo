
using System;

public struct Int4 {

	public int x;
	public int y;
	public int z;
	public int w;

	public Int4 (int x, int y, int z, int w) {
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
	}

	public override string ToString() {
		return $"{x}/{y} {z}/{w}";
	}

}

[Serializable]
public struct SizeInt {

	public int width;
	public int height;

	public SizeInt (int width, int height) {
		this.width = width;
		this.height = height;
	}

	public override string ToString () {
		return $"{width} {height}";
	}

}