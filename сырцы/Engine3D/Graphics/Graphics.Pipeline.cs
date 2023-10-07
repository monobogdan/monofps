using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine3D
{
    public struct MeshSequence
    {
        public int From;
        public int To;
        public float Speed;
    }

    public delegate void MeshAnimatorEvent();

    public sealed class MeshAnimator
    {
        private Dictionary<string, MeshSequence> sequences;
        private Dictionary<string, MeshAnimatorEvent> events;

        public string CurrentSequence
        {
            get;
            set;
        }

        public int Frame
        {
            get;
            set;
        }

        public bool IsPlaying
        {
            get;
            set;
        }

        public float Time
        {
            get;
            private set;
        }

        public MeshAnimator()
        {
            sequences = new Dictionary<string, MeshSequence>();
            events = new Dictionary<string, MeshAnimatorEvent>();
        }

        public void AddSequence(MeshSequence seq, string name)
        {
            if (!sequences.ContainsKey(name))
                sequences.Add(name, seq);
        }

        public void AttachEventToSequence(string name, MeshAnimatorEvent ev)
        {
            if (ev != null && !events.ContainsKey(name))
                events.Add(name, ev);
        }

        public void Play(string name)
        {
            if (sequences.ContainsKey(name))
            {
                CurrentSequence = name;

                Frame = sequences[name].From;
                IsPlaying = true;
                Time = 0;
            }
        }

        public void Stop()
        {
            CurrentSequence = null;
            IsPlaying = false;
            Time = 0;
        }

        public void Update()
        {
            if (CurrentSequence != null && IsPlaying)
            {
                Time += sequences[CurrentSequence].Speed;

                if (Time >= 1.0f)
                {
                    Frame++;

                    if (Frame == sequences[CurrentSequence].To)
                    {
                        if (events.ContainsKey(CurrentSequence))
                            events[CurrentSequence]();

                        Stop();
                    }

                    Time = 0;
                }
            }
        }
    }
}
