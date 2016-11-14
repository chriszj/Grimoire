using UnityEngine;
using System;
using System.Collections.Generic;


using GLIB.Core;

namespace GLIB.VFX {

    class ParticlesManager : BackModule<ParticlesManager> {

        struct ParticleMeta {
            public int id;
            public bool autoDestroy;
        }

        int _serialID = 0;

        Dictionary<ParticleMeta, GameObject> _particles;

        protected override void ProcessInitialization()
        {
            _particles = new Dictionary<ParticleMeta, GameObject>();
        }

        protected override void ProcessUpdate()
        {

            List<ParticleMeta> entriesToClean = new List<ParticleMeta>();

            // Look for particles to autodestroy.
            foreach (KeyValuePair<ParticleMeta, GameObject> entry in _particles) {
                if (entry.Key.autoDestroy)
                {
                    if (entry.Value != null && entry.Value.activeSelf)
                    {
                        ParticleSystem particleSystem = entry.Value.GetComponent<ParticleSystem>();

                        if (particleSystem == null || !particleSystem.isPlaying)  {
                            GameObject.Destroy(entry.Value.gameObject);
                            entriesToClean.Add(entry.Key);
                        }
                    }
                    else
                        entriesToClean.Add(entry.Key);
                }
            }

            // Destroy & Clean
            foreach (ParticleMeta key in entriesToClean) {
                _particles.Remove(key);
            }

            //Debug.Log("_particles Lenght" + _particles.Count);

        }

        protected override void ProcessTermination()
        {
            
        }

        /// <summary>
        /// Instantiate particles from a prefab of your choice, particles will be automatically destroyed when finished playing.
        /// You may leave position default null value in order to instantiate the prefab with its original position. The same goes for the rotation parameter.
        /// Please have in mind that scale is not implemented yet, so be cautious. 
        /// </summary>
        /// <param name="particlesPrefab"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="parent"></param>
        public GameObject InstantiateParticles(GameObject particlesPrefab, Vector3? position = null, Vector3? rotation = null, float scale = 1, GameObject parent = null, bool autoDestroy = true)
        {

            try
            {

                if (particlesPrefab == null)
                    throw new NullReferenceException("Error: Particle prefab must not be null");

                GameObject particlesInstance = Instantiate(particlesPrefab);

                Vector3 originalPosition = particlesInstance.transform.localPosition;

                Vector3 originalRotation = particlesInstance.transform.localRotation.eulerAngles;

                if (parent != null)
                    particlesInstance.transform.SetParent(parent.transform);

                // Particle System is not that much affected by its transform.localscale, so we normalize it to prevent position errors.
                particlesInstance.transform.localScale = new Vector3(1, 1, 1);


                if (rotation != null)
                    particlesInstance.transform.rotation = Quaternion.Euler((Vector3)rotation);
                else
                    particlesInstance.transform.localRotation = Quaternion.Euler((Vector3)originalRotation);

                if (position != null)
                    particlesInstance.transform.localPosition = (Vector3)position;
                else // If no position was passed then use the original prefab position
                    particlesInstance.transform.localPosition = originalPosition;

                ParticleMeta meta = new ParticleMeta();

                meta.id = _serialID;
                meta.autoDestroy = autoDestroy;

                _serialID++;

                _particles.Add(meta, particlesInstance);

                // TODO Implement Scale
                return particlesInstance;
            }
            catch (NullReferenceException e) {

                Debug.LogError(e.Message+"\n"+e.StackTrace);
                return null;

            }

        }

    }

}
