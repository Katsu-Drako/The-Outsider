using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersistentManager : Manager
{
	[HideInInspector]
	public string GUID;
	[ContextMenu("Delete data")]
    public void Delete() {
		PlayerPrefs.DeleteKey(GetType().ToString());
        Debug.Log("Deleted data.");
    }
	protected virtual void OnValidate() {
		if (!Application.isPlaying) {
			if (!string.IsNullOrEmpty(gameObject.scene.name) && (gameObject.scene.name != transform.root.name)) {
				if (string.IsNullOrEmpty(GUID))
					GUID = Guid.NewGuid().ToString(); 
			} else
				GUID = string.Empty;
		}
    }
	protected override void Awake() {
		base.Awake();
		Load();
	}
	void Load() {
		if (PlayerPrefs.HasKey(GetType().ToString())) {
			JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(GetType().ToString()), this);
		}
	}
	protected void Save() {
		PlayerPrefs.SetString(GetType().ToString(), JsonUtility.ToJson(this));
        PlayerPrefs.Save();
    }
    protected virtual void OnApplicationQuit() {
		Save();
    }
}