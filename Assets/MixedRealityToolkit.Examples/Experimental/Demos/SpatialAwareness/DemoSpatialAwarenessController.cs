﻿using UnityEngine;

using Microsoft.MixedReality.Toolkit.SpatialAwareness;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;

namespace Microsoft.MixedReality.Toolkit.Experimental.Examples
{
    public class DemoSpatialAwarenessController : MonoBehaviour, IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>
    {
        public GameObject SceneObjectPrefab;

        public Transform ParentGameObject;

        public IMixedRealitySpatialAwarenessSceneUnderstandingObserver observer;

        Animator animator;
        public bool triggered = false;

        void OnEnable()
        {
            CoreServices.SpatialAwarenessSystem.RegisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);

            var access = CoreServices.SpatialAwarenessSystem as IMixedRealityDataProviderAccess;

            if (access == null)
            {
                Debug.LogError("Couldn't get access to CoreServices.SpatialAwarenessSystem");
                return;
            }

            observer = access.GetDataProvider<IMixedRealitySpatialAwarenessSceneUnderstandingObserver>();

            if (observer == null)
            {
                Debug.LogError("Couldn't access Scene Understanding Observer!");
                return;
            }

            Debug.Log($"observer.UpdateInterval = {observer.UpdateInterval}");

            animator = GetComponent<Animator>();
            animator.SetBool("Persistent", observer.UsePersistentObjects);
        }

        void OnDisable()
        {
            CoreServices.SpatialAwarenessSystem?.UnregisterHandler<IMixedRealitySpatialAwarenessObservationHandler<SpatialAwarenessSceneObject>>(this);
        }

        public void OnObservationAdded(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            var sceneObject = eventData.SpatialObject;
            var go = Instantiate(SceneObjectPrefab);
            go.transform.SetPositionAndRotation(sceneObject.Position, sceneObject.Rotation);
            go.transform.SetParent(ParentGameObject);
            foreach (var x in go.GetComponents<ISpatialAwarenessSceneObjectConsumer>())
            {
                x.OnSpatialAwarenessSceneObjectCreated(sceneObject);
            }
        }

        public void OnObservationRemoved(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            Debug.Log($"Removed {eventData.GuidId}");
        }

        public void OnObservationUpdated(MixedRealitySpatialAwarenessEventData<SpatialAwarenessSceneObject> eventData)
        {
            Debug.Log($"Updated {eventData.GuidId}");
        }

        public void HandleIt()
        {
            Debug.Log("Got it");
        }
    }
}
