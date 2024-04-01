using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

namespace LiteRP
{
    [RequireComponent(typeof(Camera))]
    public class StandardRenderRequest : MonoBehaviour
    {
        [SerializeField]
        RenderTexture texture2D;

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            if (GUILayout.Button("Render Request"))
            {
                SendRenderRequest();
            }
            GUILayout.EndVertical();
        }

        private void SendRenderRequest()
        {
            var cam = GetComponent<Camera>();
            var req = new RenderPipeline.StandardRequest();

            if (!RenderPipeline.SupportsRenderRequest(cam, req))
                return;

            req.destination = texture2D;
            RenderPipeline.SubmitRenderRequest(cam, req);
        }
    }
}
