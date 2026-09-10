using System.Runtime.InteropServices;
using UnityEngine;

public class JSFunctCalls : MonoBehaviour
{
  [DllImport("__Internal")] private static extern void SendPostMessage(string message);

  [DllImport("__Internal")] private static extern void RegisterVisibilityChangeListener(string gameObjectName);

  [DllImport("__Internal")] private static extern void RequestFullscreen();

  [DllImport("__Internal")] private static extern void ExitFullscreen();

  [DllImport("__Internal")] private static extern void RegisterFullscreenChangeListener(string gameObjectName);

  internal void SendCustomMessage(string message)
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    SendPostMessage(message);
#endif
  }

  internal void RegisterVisibilityListener(string gameObjectName)
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log($"[JS] Registering visibility change listener on '{gameObjectName}'");
    RegisterVisibilityChangeListener(gameObjectName);
#else
    Debug.Log("[JS] Visibility listener not registered (editor mode)");
#endif
  }

  internal void RequestExpandGame()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log("[JS] Requesting fullscreen expand");
    RequestFullscreen();
#else
    Debug.Log("[JS] Would request fullscreen (editor mode)");
#endif
  }

  internal void RequestShrinkGame()
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log("[JS] Requesting exit fullscreen (shrink)");
    ExitFullscreen();
#else
    Debug.Log("[JS] Would exit fullscreen (editor mode)");
#endif
  }

  internal void RegisterFullscreenListener(string gameObjectName)
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    Debug.Log($"[JS] Registering fullscreen change listener on '{gameObjectName}'");
    RegisterFullscreenChangeListener(gameObjectName);
#else
    Debug.Log("[JS] Fullscreen listener not registered (editor mode)");
#endif
  }
}
