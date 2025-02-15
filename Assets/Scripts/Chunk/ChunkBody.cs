using UnityEngine;

public class ChunkBody : MonoBehaviour {

	#if UNITY_EDITOR
	private void OnDrawGizmos () {
		Debug.DrawCenteredRectangle(transform.position, new Vector2(ChunkSampler.chunkSizeInUnit, ChunkSampler.chunkSizeInUnit), Color.red, 0);
	}
	#endif
}
