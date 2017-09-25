using UnityEngine;
#if UNITY_EDITOR
using xCBM;
#endif


[ExecuteInEditMode]
public class xCBMShader : MonoBehaviour{
	#if UNITY_EDITOR

	private Shader shader;
	private Material m_Material;

	private Material material {
		get {
			if (m_Material == null){
				m_Material = new Material(shader);
				m_Material.hideFlags = HideFlags.HideAndDontSave;
			}
			return m_Material;
		}
	}

	private void OnEnable(){
		shader = Shader.Find("Hidden/xCBMShader");
	}

	private void OnDisable(){
		if(m_Material) DestroyImmediate(m_Material);
	}

	// Called by camera to apply image effect
	void OnRenderImage (RenderTexture source, RenderTexture destination){
		Graphics.Blit (source, destination, material);
	}
	#endif
}
