using UnityEngine;

public class Sampler : MonoBehaviour {

	[SerializeField] private Texture2D _map;

	private Vector2 _samplingSize;

	public void SetSamplingSize (Vector2 size) {
		_samplingSize = size;
	}

	/** Return a value between 0.5 and 1.5, but return 0 if no obstacle is allowed at this position */
	public float GetFactorAt (Vector2 position) {

		float noiseScale = 1f;
		float noiseValue = Mathf.Clamp01(Mathf.PerlinNoise(position.x * noiseScale, position.y * noiseScale)); // [0, 1]

		int textureX = Mathf.RoundToInt(position.x / _samplingSize.x);
		int textureY = Mathf.RoundToInt(position.y / _samplingSize.x + _map.height / 2);
		Color textureColor = _map.GetPixelData<Color32>(0)[textureX + textureY * _map.width];
		float textureValue = textureColor.grayscale; // [0, 1]

		if (textureValue == 0) {
			// black = no obstacle allowed
			return 0;
		}

		return 0.5f + (textureValue * 0.75f) + noiseValue * 0.25f;
	}

}
