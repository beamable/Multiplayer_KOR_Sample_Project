using UnityEngine;

namespace Beamable.Samples.Core.Components
{
	/// <summary>
	/// Base class for any <see cref="MonoBehaviour"/> which follows the popular 
	/// Singleton design patter.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class SingletonMonobehavior<T> : MonoBehaviour where T : MonoBehaviour
	{
		//  Properties ------------------------------------------
		public static bool IsInstantiated
		{
			get
			{
				return _Instance != null;
			}
		}

		/// <summary>
		/// Reference to the runtime instance of the singleton class of type <see cref="T"/>.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (!IsInstantiated)
				{
					Instantiate();
				}
				return _Instance;
			}
			set
			{
				_Instance = value;
			}
		}

		//  Fields -------------------------------------------------
		private static T _Instance; 
		public delegate void OnInstantiateCompletedDelegate(T instance);
		public static OnInstantiateCompletedDelegate OnInstantiateCompleted;

		//  Instantiation ------------------------------------------

		/// <summary>
		/// Create and/or return the instance.
		/// </summary>
		/// <returns></returns>

		public static T Instantiate()
		{
			if (!IsInstantiated)
			{
				_Instance = GameObject.FindObjectOfType<T>();
				
				if (_Instance == null)
            {
					GameObject go = new GameObject();
					_Instance = go.AddComponent<T>();
					go.name = _Instance.GetType().FullName;
				}
				if (OnInstantiateCompleted != null)
				{
					OnInstantiateCompleted(_Instance);
				}
			}

			// DontDestroyOnLoad only works for root GameObjects or components on root GameObjects.
			_Instance.transform.SetParent(null);

			// Keep this alive between scenes
			DontDestroyOnLoad(_Instance.gameObject);

			return _Instance;
		}


		//  Unity Methods ------------------------------------------
		protected virtual void Awake()
		{
			Instantiate();
		}

		/// <summary>
		/// Destroy the <see cref="SingletonMonobehavior{T}"/>
		/// </summary>
		public static void Destroy()
		{
			if (IsInstantiated)
			{
				Destroy(_Instance.gameObject);
				_Instance = null;
			}
		}
		private static void Destroy(GameObject go)
		{
			if (Application.isPlaying)
			{
				Debug.Log("Destroy() 1: " + _Instance.GetInstanceID());
				Destroy(go);
			}
			else
			{
				Debug.Log("Destroy() 2: " + _Instance.GetInstanceID());
				DestroyImmediate(go);
			}
		}
	}
}