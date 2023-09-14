using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class CoroutineDispatcher : MonoBehaviour
    {
        private static readonly Dictionary<int, IEnumerator> Coroutines = new Dictionary<int, IEnumerator>();

        private static MonoBehaviour _context;
        private static MonoBehaviour Context
        {
            get
            {
                if (_context == null)
                {
                    _context = new GameObject().AddComponent<CoroutineDispatcher>();
                    _context.name = "Coroutine Dispatcher";
                    DontDestroyOnLoad(_context.gameObject); 
                }
                return _context;
            }
        }

        public new static Coroutine StartCoroutine(IEnumerator coroutine)
        {
            return Context.StartCoroutine(coroutine);
        }
        
        public new static void StopCoroutine(IEnumerator coroutine)
        {
            Context.StopCoroutine(coroutine);
        }
    
        public new static void StopCoroutine(Coroutine coroutine)
        {
            Context.StopCoroutine(coroutine);
        }

        public static void StopCoroutine(int id)
        {
            if (!Coroutines.ContainsKey(id)) return;
            Context.StopCoroutine(Coroutines[id]);
        }
        
        public static void ShutDown()
        {
            Context.StopAllCoroutines();
        }

        public static Coroutine ExecuteWithDelay(float delay, Action callback)
        {
            return Context.StartCoroutine(ExecuteWithDelayCoroutine(delay, callback));
        }
        
        public static void ExecuteWithDelay(float delay, Action callback, int id)
        {
            var coroutine = ExecuteWithDelayCoroutine(delay, callback);
            Coroutines[id] = coroutine;
            StartCoroutine(coroutine);
        }
        
        public static void ExecuteNextFrame(Action callback)
        {
            StartCoroutine(ExecuteNextFrameCoroutine(callback));
        }
    
        private static IEnumerator ExecuteWithDelayCoroutine(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback?.Invoke();
        }
        
        private static IEnumerator ExecuteNextFrameCoroutine(Action callback)
        {
            yield return new WaitForEndOfFrame();
            // yield return null;
            callback?.Invoke();
        }
    }
}