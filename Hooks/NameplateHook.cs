using UnityEngine;
using Ex;
using ExClient.Utils;
using TMPro;

namespace ExClient {

	/// <summary> Hook Behaviour to represent the <see cref="Ex.Nameplate"/> server Component </summary>
	public class NameplateHook : MonoBehaviour {
		/// <summary> Name of the entity to display </summary>
		public string nname;
		/// <summary> 3D text object used to display the name with </summary>
		public TextMeshPro name3d;

		[ExEntityLink.AutoRegisterChange]
		public static void OnNameplateChanged(Nameplate nameplate, ExEntityLink link) {
			NameplateHook hook = link.Require<NameplateHook>();

			if (hook.name3d == null) {
				var prefab = Resources.Load<TextMeshPro>("Nameplate");
				hook.name3d = Instantiate(prefab, hook.transform.position, hook.transform.rotation);
				hook.name3d.transform.SetParent(hook.transform);
				hook.name3d.transform.localPosition = new Vector3(0, 2, 0);

			}

			if (hook.nname != nameplate.name) {
				hook.nname = nameplate.name;
				hook.name3d.text = hook.nname;

			}

		}

		[ExEntityLink.AutoRegisterRemove]
		public static void OnNameplateRemoved(Nameplate nameplate, ExEntityLink link) {
			NameplateHook hook = link.GetComponent<NameplateHook>();
			if (hook != null) {
				NameplateHook.Destroy(hook);
			}
		}

	}


}
