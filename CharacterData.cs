using RAGE;
using System;
using System.Collections.Generic;
using System.Text;

namespace Client
{
    class CharacterData : Events.Script
    {
        private bool gender;
        private float[] face;
        private int parent1;
        private int parent2;
        private int parent3;
        private int skin1;
        private int skin2;
        private int skin3;
        private float shape;
        private float tone;
        private float modifier;

        public float Shape
        {
            get { return shape;  }
            set { shape = value; }
        }

        public float Tone
        {
            get { return tone; }
            set { tone = value; }
        }

        public float Modifier
        {
            get { return modifier; }
            set { modifier = value; }
        }
        public bool Gender
        {
            get { return gender; }
            set { gender = value; }
        }
        public float[] Face
        {
            get { return face; }
            set
            {
                for (int i = 0; i < 19; i++)
                {
                    face[i] = value[i];
                }
            }
        }

        public int[] Parents
        {
            get {
                int[] parents = { parent1, parent2, parent3 };
                return parents;
                }
            set
            {
                parent1 = value[0];
                parent2 = value[1];
                parent3 = value[2];
            }
        }
        public int[] ParentSkins
        {
            get
            {
                int[] skins = { skin1,skin2,skin3 };
                return skins;
            }
            set
            {
                skin1 = value[0];
                skin2 = value[1];
                skin3 = value[2];
            }
        }
    }
}
