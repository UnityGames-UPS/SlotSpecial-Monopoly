using System.Runtime.InteropServices;
using UnityEngine;

public class JSFunctCalls : MonoBehaviour
{
  [DllImport("__Internal")] private static extern void SendPostMessage(string message);

  [DllImport("__Internal")] private static extern void RegisterVisibilityChangeListener(string gameObjectName);

  [DllImport("__Internal")] private static extern void RequestFullscreen();

  [DllImport("__Internal")] private static extern void ExitFullscreen();

  [DllImport("__Internal")] private static extern void RegisterFullscreenChangeListener(string gameObjectName);

  [DllImport("__Internal")] private static extern void RegisterResizeListener(string gameObjectName, string methodName);

  [DllImport("__Internal")] private static extern void RegisterTokenListener(string gameObjectName, string methodName);

  // Start, not Awake: the receiver's Awake must run before the initial dimensions callback.
  void Start()
  {
    RegisterDimensionsListener();
  }

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

  // Self-contained resize bridge: the page drives OC.SwitchDisplay("width,height") on its own resize.
  internal void RegisterDimensionsListener(string gameObjectName = "OC", string methodName = "SwitchDisplay")
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterResizeListener(gameObjectName, methodName);
#else
    Debug.Log($"[JS] Resize listener not registered ('{gameObjectName}.{methodName}', editor mode)");
#endif
  }

  // Inbound auth: routes the host's "TokenReceived" message to gameObjectName.methodName(json).
  internal void RegisterAuthTokenListener(string gameObjectName, string methodName = "ReceiveAuthToken")
  {
#if UNITY_WEBGL && !UNITY_EDITOR
    RegisterTokenListener(gameObjectName, methodName);
#else
    Debug.Log($"[JS] Token listener not registered ('{gameObjectName}.{methodName}', editor mode)");
#endif
  }
}
