using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MaterialPropertyBlockSetter : MonoBehaviour
{
   [SerializeField] private Renderer objectRenderer;
   [SerializeField] private Color color;
   
   private MaterialPropertyBlock materialPropertyBlock;
   
   private void Awake()
   {
      SetMaterialColorProperty();
   }

   private void SetMaterialColorProperty()
   {
      objectRenderer ??= GetComponent<Renderer>();

      if (objectRenderer == null) return;
      
      materialPropertyBlock = new MaterialPropertyBlock();
      materialPropertyBlock.SetColor("_Color", color);
      objectRenderer.SetPropertyBlock(materialPropertyBlock);
   }
}
