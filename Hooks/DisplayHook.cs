using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using ExClient.Utils;

using static ExClient.Utils.Res;

namespace ExClient {
	/// <summary> Hook Behaviour to represent the <see cref="Ex.Display"/> server Component </summary>
	public class DisplayHook : MonoBehaviour {
		/// <summary> assigned prefab name to display </summary>
		public string prefab;
		/// <summary> Created instance of the prefab </summary>
		public Transform child;

		[ExEntityLink.AutoRegisterChange]
		public static void OnDisplayChanged(Ex.Display display, ExEntityLink link) {
			DisplayHook hook = link.Require<DisplayHook>();

			if (hook.child == null || hook.prefab != display.prefab) {
				if (hook.child != null) {
					GameObject.Destroy(hook.child.gameObject);
				}

				hook.prefab = display.prefab;
				Transform prefab = SafeLoad<Transform>(display.prefab, "Models/Error");
				Transform copy = GameObject.Instantiate(prefab, link.transform);
				hook.child = copy;
				hook.child.SendMessage("SetColors", display, SendMessageOptions.DontRequireReceiver);
			}

			hook.child.localPosition = display.position;
			hook.child.localRotation = Quaternion.Euler(display.rotation);

			hook.child.SendMessage("SetColors", display, SendMessageOptions.DontRequireReceiver);
		}

		[ExEntityLink.AutoRegisterRemove]
		public static void OnDisplayRemoved(Ex.Display display, ExEntityLink link) {
			DisplayHook hook = link.GetComponent<DisplayHook>();
			if (hook != null) {
				if (hook.child != null) {
					GameObject.Destroy(hook.child.gameObject);
				}
				DisplayHook.Destroy(hook);
			}

		}


	}
}
