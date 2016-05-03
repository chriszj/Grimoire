using UnityEngine;
using System.Collections;

using GLIB.Core;
using System;

namespace GLIB.Audio
{

    public class SoundModule : BackModule<SoundModule> {

        int _maxChannels = 6;
        public int MaxChannels {
            get {
                return _maxChannels;
            }
            set {

                
                _maxChannels = value;

                
            }
        }

        int _numChannels;
        public int NumChannels {
            get {
                return _numChannels;
            }
            set {
                _numChannels = value;
            }
        }

        protected override void ProcessInitialization()
        {
            
        }

        protected override void ProcessUpdate()
        {
            
        }

        protected override void ProcessTermination()
        {
            
        }

    }

}
