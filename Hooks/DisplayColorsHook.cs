using UnityEngine;
using ExClient.Utils;

namespace ExClient {

	/// <summary> Hook Behaviour to represent the <see cref="Ex.DisplayColors"/> server Component </summary>
	public class DisplayColorsHook : MonoBehaviour {

		[ExEntityLink.AutoRegisterChange]
		public static void OnDisplayColorsChanged(Ex.DisplayColors comp, ExEntityLink link) {
			var hook = link.Require<DisplayColorsHook>();
			
			
		}
	
		[ExEntityLink.AutoRegisterRemove]
		public static void OnDisplayColorsRemoved(Ex.DisplayColors comp, ExEntityLink link) {
			var hook = link.GetComponent<DisplayColorsHook>();
		
			if (hook != null) {
				Component.Destroy(hook);
			}
		
		}
	
	}

}
