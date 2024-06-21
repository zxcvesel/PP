using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace TMPro
{
	public class TMP_SubMeshUI : MaskableGraphic
	{
		[SerializeField]
		private TMP_FontAssetUtilities m_fontAsset;
		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;
		[SerializeField]
		private Material m_material;
		[SerializeField]
		private Material m_sharedMaterial;
		[SerializeField]
		private bool m_isDefaultMaterial;
		[SerializeField]
		private float m_padding;
		[SerializeField]
		private CanvasRenderer m_canvasRenderer;
		[SerializeField]
		private TextMeshProUGUI m_TextComponent;
		[SerializeField]
		private int m_materialReferenceIndex;
	}
}
