using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour
{
	protected static readonly Dictionary<Type, Manager> _managers = new Dictionary<Type, Manager>();
	public static T Get<T>() where T : Manager {
		if (_managers.ContainsKey(typeof(T))) {
			return _managers[typeof(T)] as T;
		} else {
			return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
		}
	}
    protected virtual void Awake() {
		_managers.Add(GetType(), this);
		DontDestroyOnLoad(this);
	}
}
